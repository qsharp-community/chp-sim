// <copyright file="ChpMagic.cs" company="https://qsharp.community/">
// Copyright (c) Sarah Kaiser. Licensed under the MIT License.
// </copyright>
// Adapted from Toffoli magic command in the IQSharp project here:
// https://github.com/microsoft/iqsharp/blob/master/src/Kernel/Magic/ToffoliMagic.cs

namespace QSharpCommunity.Simulators.Chp
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Jupyter.Core;
    using Microsoft.Quantum.IQSharp;
    using Microsoft.Quantum.IQSharp.Common;
    using Microsoft.Quantum.IQSharp.Jupyter;
    using Microsoft.Quantum.Simulation.Simulators;

    /// <summary>
    /// Runs a given function or operation on the ChpSimulator target machine.
    /// </summary>
    public class ChpMagic : AbstractMagic
    {
        private const string ParameterNameOperationName = "__operationName__";
        private const int DefaultNQubits = 1024;
        private const string DefaultDumpFormat = "dense-paulis";
        private readonly IConfigurationSource configurationSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChpMagic"/> class.
        /// Default constructor.
        /// </summary>
        /// <param name="resolver">Symbol resolver.</param>
        /// <param name="configurationSource">Source for confirgarion settings.</param>
        public ChpMagic(ISymbolResolver resolver, IConfigurationSource configurationSource)
            : base(
            "chp",
            new Documentation
            {
                Summary = "Runs a given function or operation on the StabilizerSimulator target machine.",
                Description = @"
                    This magic command allows executing a given function or operation on the StabilizerSimulator, 
                    which performs a simulation of the given function or operation in which the state can always be 
                    represented by a stabilizer of the form described by CHP, and prints the resulting return value.

                    See the [StabilizerSimulator user guide](https://github.com/qsharp-community/chp-sim/docs/user-guide.md) to learn more.

                    #### Required parameters

                    - Q# operation or function name. This must be the first parameter, and must be a valid Q# operation
                    or function name that has been defined either in the notebook or in a Q# file in the same folder.
                    - Arguments for the Q# operation or function must also be specified as `key=value` pairs.
                ",
                Examples = new[]
                {
                    @"
                        Use the StabilizerSimulator to simulate a Q# operation
                        defined as `operation MyOperation() : Result`:
                        ```
                        In []: %chp MyOperation
                        Out[]: <return value of the operation>
                        ```
                    ",
                    @"
                        Use the StabilizerSimulator to simulate a Q# operation
                        defined as `operation MyOperation(a : Int, b : Int) : Result`:
                        ```
                        In []: %chp MyOperation a=5 b=10
                        Out[]: <return value of the operation>
                        ```
                    ",
                },
            })
        {
            this.SymbolResolver = resolver;
            this.configurationSource = configurationSource;
        }

        /// <summary>
        /// Gets the ISumbolResolver used to find the function/operation to simulate.
        /// </summary>
        public ISymbolResolver SymbolResolver { get; }

        /// <inheritdoc />
        public override ExecutionResult Run(string input, IChannel channel) =>
            this.RunAsync(input, channel).Result;

        /// <summary>
        /// Simulates a function/operation using the ChpSimulator as target machine.
        /// It expects a single input: the name of the function/operation to simulate.
        /// </summary>
        /// <param name="input">current parameters for the function.</param>
        /// <param name="channel">channel connecting up with Jupyter.</param>
        /// <returns>function result.</returns>
        public async Task<ExecutionResult> RunAsync(string input, IChannel channel)
        {
            var inputParameters = ParseInputParameters(input, firstParameterInferredName: ParameterNameOperationName);

            var name = inputParameters.DecodeParameter<string>(ParameterNameOperationName);
            var symbol = this.SymbolResolver.Resolve(name) as dynamic; // FIXME: should be as IQSharpSymbol.
            if (symbol == null)
            {
                throw new InvalidOperationException($"Invalid operation name: {name}");
            }

            var nQubits =
                this.configurationSource.GetOptionOrDefault<int>("chp.nQubits", DefaultNQubits);

            var debug =
                this.configurationSource.GetOptionOrDefault<bool>("chp.debug", false);

            var qsim = new StabilizerSimulator(nQubits).WithStackTraceDisplay(channel);
            qsim.DisableLogToConsole();
            qsim.OnDisplayableDiagnostic += (displayable) =>
            {
                if (displayable is DebugMessage message)
                {
                    if (debug)
                    {
                        channel.Stderr($"DEBUG: {message.Message}");
                    }
                }
                else
                {
                    channel.Display(displayable);
                }
            };
            qsim.OnLog += channel.Stdout;

            var operationInfo = (OperationInfo)symbol.Operation;
            var value = await operationInfo.RunAsync(qsim, inputParameters);

            return value.ToExecutionResult();
        }
    }
}