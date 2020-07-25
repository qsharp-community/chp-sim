// <copyright file="StabilizerSimulator.cs" company="https://qsharp.community/">
// Copyright (c) Sarah Kaiser. All rights reserved.
// Licensed under the MIT License.
// </copyright>
// This C# project is based on a Python implementation by @Strilanc here:
// https://github.com/Strilanc/python-chp-stabilizer-simulator

namespace QSharpCommunity.Simulators.Chp
{
    using System;
    using Microsoft.Quantum.Simulation.Common;
    using Microsoft.Quantum.Simulation.QuantumProcessor;

    public class StabilizerSimulator : QuantumProcessorDispatcher
    {
        public StabilizerSimulator(int? nQubits = null)
            : base(new StabilizerProcessor(nQubits))
        {
            (this.QuantumProcessor as StabilizerProcessor).Simulator = this;
        }

        internal new void MaybeDisplayDiagnostic(object displayable) =>
            base.MaybeDisplayDiagnostic(displayable);
    }
}
