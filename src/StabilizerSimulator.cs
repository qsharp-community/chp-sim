// <copyright file="StabilizerSimulator.cs" company="https://qsharp.community/">
// Copyright (c) Sarah Kaiser. Licensed under the MIT License.
// </copyright>
// This C# project is based on a Python implementation by @Strilanc here:
// https://github.com/Strilanc/python-chp-stabilizer-simulator

namespace QSharpCommunity.Simulators.Chp
{
    using System;
    using Microsoft.Quantum.Simulation.Common;
    using Microsoft.Quantum.Simulation.QuantumProcessor;

    /// <summary>
    /// CHP Simulator.
    /// </summary>
    public class StabilizerSimulator : QuantumProcessorDispatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StabilizerSimulator"/> class.
        /// </summary>
        /// <param name="nQubits">Qubits to use for simulation.</param>
        public StabilizerSimulator(int? nQubits = null)
            : base(new StabilizerProcessor(nQubits))
        {
            (this.QuantumProcessor as StabilizerProcessor).Simulator = this;
        }

        /// <summary>
        /// If diagnostic is enabled, show diagnostic information.
        /// </summary>
        /// <param name="displayable">Diagnostic displayable information.</param>
        internal new void MaybeDisplayDiagnostic(object displayable) =>
            base.MaybeDisplayDiagnostic(displayable);
    }
}
