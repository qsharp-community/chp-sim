using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using Microsoft.Quantum.Simulation.Common;
using Microsoft.Quantum.Simulation.Core;

// This C# project is based on a Python implementation by @Strilanc here: 
// https://github.com/Strilanc/python-chp-stabilizer-simulator
namespace chp
{
    public class StabilizerProcessor : QuantumProcessorBase
    {
        private bool[,] Table;
        private int NQubits;
        public StabilizerProcessor(int nQubits = 2)
        {
            NQubits = nQubits;
            // By default, this array is full of false
            Table = new bool[2 * nQubits + 1, 2 * nQubits + 1];
            Table.SetDiagonal(true);

        }

        private int _x(int idxCol) => idxCol;
        private int _z(int idxCol) => NQubits + idxCol;
        private int _r => 2 * NQubits;

        private void Hadamard(int target)
        {
            System.Console.WriteLine("Before swap");
            // This takes care of mapping X->Z and Z->X
            Table.SwapColumns(_x(target), _z(target));
            System.Console.WriteLine("after swap");
            // Now need to handle the phase, this represents the fact that HYH = -Y
            foreach (var idxRow in Enumerable.Range(0, Table.GetLength(0)))
            {
                Table[idxRow, _r] ^= Table[idxRow, _x(target)] && Table[idxRow, _z(target)];
            }

            // DELETEMEPLZ
            System.Console.WriteLine(Table.MatrixToString());

        }

       
        public override void H(Qubit qubit)
        {
            Hadamard(qubit.Id);
        }

        public override void ControlledH(IQArray<Qubit> controls, Qubit qubit)
        {
            if (controls.Length == 0)
            {
                H(qubit);
            }
            else
            {
                throw new UnsupportedOperationException("Controlled H is not a Clifford operation.");
            }
        }
    }
}
