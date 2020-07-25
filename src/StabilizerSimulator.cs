// Copyright (c) Sarah Kaiser. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Quantum.Simulation.Common;
using Microsoft.Quantum.Simulation.QuantumProcessor;

namespace QSharpCommunity.Simulators.Chp
{
    public class StabilizerSimulator : QuantumProcessorDispatcher
    {
        public StabilizerSimulator() : base(new StabilizerProcessor())
        {
            (this.QuantumProcessor as StabilizerProcessor).Simulator = this;
        }
    }
}
