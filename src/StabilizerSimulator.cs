// Copyright (c) Sarah Kaiser. All rights reserved.
// Licensed under the MIT License.
// This C# project is based on a Python implementation by @Strilanc here: 
// https://github.com/Strilanc/python-chp-stabilizer-simulator

using System;
using Microsoft.Quantum.Simulation.Common;
using Microsoft.Quantum.Simulation.QuantumProcessor;

namespace QSharpCommunity.Simulators.Chp
{
    public class StabilizerSimulator : QuantumProcessorDispatcher
    {
        public StabilizerSimulator(int? nQubits = null) : base(new StabilizerProcessor(nQubits))
        {
            (this.QuantumProcessor as StabilizerProcessor).Simulator = this;
        }

        internal new void MaybeDisplayDiagnostic(object displayable) =>
            base.MaybeDisplayDiagnostic(displayable);
    }
}
