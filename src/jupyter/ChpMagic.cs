// Copyright (c) Sarah Kaiser. All rights reserved.
// Licensed under the MIT License.
// Adapted from Toffoli magic command in the IQSharp project here: 
// https://github.com/microsoft/iqsharp/blob/master/src/Kernel/Magic/ToffoliMagic.cs

using System;
using System.Threading.Tasks;
using Microsoft.Jupyter.Core;
using Microsoft.Quantum.IQSharp;
using Microsoft.Quantum.IQSharp.Common;
using Microsoft.Quantum.IQSharp.Jupyter;
using Microsoft.Quantum.Simulation.Simulators;

namespace QSharpCommunity.Simulators.Chp
{
    /// <summary>
    /// Runs a given function or operation on the ChpSimulator target machine.
    /// </summary>
    public class ChpMagic : AbstractMagic
    {
        private const string ParameterNameOperationName = "__operationName__";
        private const int DefaultNQubits = 1024;
        private IConfigurationSource configurationSource;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ChpMagic(ISymbolResolver resolver, IConfigurationSource configurationSource) : base(
            "chp",
            new Documentation
            {
                Summary = "Runs a given function or operation on the ToffoliSimulator target machine.",
                Description = @"
                    This magic command allows executing a given function or operation on the ToffoliSimulator, 
                    which performs a simulation of the given function or operation in which the state is always
                    a simple product state in the computational basis, and prints the resulting return value.

                    See the [ToffoliSimulator user guide](https://docs.microsoft.com/quantum/user-guide/machines/toffoli-simulator) to learn more.

                    #### Required parameters

                    - Q# operation or function name. This must be the first parameter, and must be a valid Q# operation
                    or function name that has been defined either in the notebook or in a Q# file in the same folder.
                    - Arguments for the Q# operation or function must also be specified as `key=value` pairs.
                ",
                Examples = new []
                {
                    @"
                        Use the ToffoliSimulator to simulate a Q# operation
                        defined as `operation MyOperation() : Result`:
                        ```
                        In []: %toffoli MyOperation
                        Out[]: <return value of the operation>
                        ```
                    ",
                    @"
                        Use the ToffoliSimulator to simulate a Q# operation
                        defined as `operation MyOperation(a : Int, b : Int) : Result`:
                        ```
                        In []: %toffoli MyOperation a=5 b=10
                        Out[]: <return value of the operation>
                        ```
                    ",
                }
            })
        {
            this.SymbolResolver = resolver;
            this.configurationSource = configurationSource;
        }

        /// <summary>
        /// ISumbolResolver used to find the function/operation to simulate.
        /// </summary>
        public ISymbolResolver SymbolResolver { get; }

        /// <inheritdoc />
        public override ExecutionResult Run(string input, IChannel channel) =>
            RunAsync(input, channel).Result;

        /// <summary>
        /// Simulates a function/operation using the ChpSimulator as target machine.
        /// It expects a single input: the name of the function/operation to simulate.
        /// </summary>
        public async Task<ExecutionResult> RunAsync(string input, IChannel channel)
        {
            var inputParameters = ParseInputParameters(input, firstParameterInferredName: ParameterNameOperationName);

            var name = inputParameters.DecodeParameter<string>(ParameterNameOperationName);
            var symbol = SymbolResolver.Resolve(name) as dynamic; // FIXME: should be as IQSharpSymbol.
            if (symbol == null) throw new InvalidOperationException($"Invalid operation name: {name}");

            //TODO: File bug for the following to be public:
            // https://github.com/microsoft/iqsharp/blob/9fa7d4da4ec0401bf5803e40fce5b37e716c3574/src/Jupyter/ConfigurationSource.cs#L35
            var nQubits =
                configurationSource.Configuration.TryGetValue("chp.nQubits", out var token)
                ? token.ToObject<int>()
                : DefaultNQubits;

            var debug =
                configurationSource.Configuration.TryGetValue("chp.debug", out var tokenDebug)
                ? tokenDebug.ToObject<bool>()
                : false;

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

            var operationInfo = ((OperationInfo)symbol.Operation);
            var value = await operationInfo.RunAsync(qsim, inputParameters);

            return value.ToExecutionResult();
        }
    }
}