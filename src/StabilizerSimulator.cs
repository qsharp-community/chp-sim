using System;
using Microsoft.Quantum.Simulation.Common;
using Microsoft.Quantum.Simulation.QuantumProcessor;

// This C# project is based on a Python implementation by @Strilanc here: 
// https://github.com/Strilanc/python-chp-stabilizer-simulator
namespace chp
{
    public class StabilizerSimulator : QuantumProcessorDispatcher
    {
        public StabilizerSimulator() : base(new StabilizerProcessor())
        {
            System.Console.WriteLine("HI I AM A SIMULATOR");
        }
    }
}
