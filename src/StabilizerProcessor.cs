using System;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using Microsoft.Quantum.Simulation.Common;
using Microsoft.Quantum.Simulation.Core;

// This C# project is based on a Python implementation by @Strilanc here: 
// https://github.com/Strilanc/python-chp-stabilizer-simulator

// TODO Next:
// - public virtual void Assert(IQArray<Pauli> bases, IQArray<Qubit> qubits, Result result, string msg);
// - public virtual void AssertProb(IQArray<Pauli> bases, IQArray<Qubit> qubits, double probabilityOfZero, string msg, double tol);
namespace chp
{
    public class StabilizerProcessor : QuantumProcessorBase
    {
        private bool[,] Table;
        private int NQubits;
        public StabilizerProcessor(int nQubits = 1)
        {
            NQubits = nQubits;
            // By default, this array is full of false
            Table = new bool[2 * NQubits, 2 * NQubits + 1];
            Table.SetDiagonal(true);
        }

        //////////////////////////////////////////////////////////////////////
        // Internal
        //////////////////////////////////////////////////////////////////////
        private int _x(int idxCol) => idxCol;

        private int _z(int idxCol) => NQubits + idxCol;

        private int _r => 2 * NQubits;

        private void Hadamard(int target)
        {
            // This takes care of mapping X->Z and Z->X
            Table.SwapColumns(_x(target), _z(target));

            // Now need to handle the phase, this represents the fact that HYH = -Y
            foreach (var idxRow in Enumerable.Range(0, Table.GetLength(0)))
            {
                Table[idxRow, _r] ^= Table[idxRow, _x(target)] && Table[idxRow, _z(target)];
            }
        }

        private void Phase(int target) 
        {
            // Add global phase, this represents the fact that HYH = -Y
            foreach (var idxRow in Enumerable.Range(0, Table.GetLength(0)))
            {
                Table[idxRow, _r] ^= Table[idxRow, _x(target)] && Table[idxRow, _z(target)];
            }

            // 
            foreach (var idxRow in Enumerable.Range(0, Table.GetLength(0)))
            {
                Table[idxRow, _z(target)] ^= Table[idxRow, _x(target)];
            }

        }

        private void Cnot(int control, int target)
        {
            foreach (var idxRow in Enumerable.Range(0, Table.GetLength(0)))
            {
                Table[idxRow, _r] ^= Table[idxRow, _x(control)] & 
                    Table[idxRow, _z(target)] &
                    (Table[idxRow, _x(target)] ^ Table[idxRow, _z(control)] ^ true);
            }

            foreach (var idxRow in Enumerable.Range(0, Table.GetLength(0)))
            {
                Table[idxRow, _x(target)] ^= Table[idxRow, _x(control)];
            }

            foreach (var idxRow in Enumerable.Range(0, Table.GetLength(0)))
            {
                Table[idxRow, _z(control)] ^= Table[idxRow, _z(target)];
            }
        }

        private Result MeasureByIndex(int idx)
        {
            // Non-Deterministic Case
            if (Table.Column(idx).Skip(NQubits).Any(b => b))
            {
                var result = (new System.Random()).Next(2) == 1;
                var collisions = Table.Column(idx).IndicesWhere(b => b).ToList();
                var idxFirst = NQubits + Table.Column(idx).Skip(NQubits).IndicesWhere(b => b).First();
                
                foreach (var idxCollision in collisions.Where(idxCollision => idxCollision != idxFirst))
                {
                    Table.SetToRowSum(idxCollision, idxFirst);
                }

                foreach (var idxColumn in Enumerable.Range(0, Table.GetLength(1)))
                {
                    Table[idxFirst - NQubits, idxColumn] = Table[idxFirst, idxColumn]; 
                    Table[idxFirst, idxColumn] = false;              
                }
                Table[idxFirst, _z(idx)] = true;
                Table[idxFirst, _r] = result;
                return result ? Result.One : Result.Zero;

            }
            // Deterministic Case
            else
            {
                var vector = new bool[2 * NQubits + 1];
                foreach (var idxDestabilizer in Enumerable.Range(0, NQubits))
                {
                    if (Table[idxDestabilizer, _x(idx)])
                    {
                        vector.SetToRowSum(Table, idxDestabilizer + NQubits);
                    }                    
                }
                return vector[^1] ? Result.One : Result.Zero;
            }
            
        }

        //////////////////////////////////////////////////////////////////////
        // Overrides - Supported
        //////////////////////////////////////////////////////////////////////
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

        public override void S(Qubit qubit)
        {
            Phase(qubit.Id);
        }

        public override void SAdjoint(Qubit qubit)
        {
            Phase(qubit.Id);
            Phase(qubit.Id);
            Phase(qubit.Id);
        }

        public override void OnDumpMachine<T>(T location)
        {
            if (location is QVoid)
            {
                System.Console.WriteLine(Table.MatrixToString(true));
            }
            else if (location is string filename)
            {
                File.WriteAllText(filename, Table.MatrixToString(true));  
            }
            else
            {
                throw new Exception("Not a valid file path.");
            }

        }

        public override void OnDumpRegister<T>(T location, IQArray<Qubit> qubits)
        {
            OnMessage("Only DumpMachine is supported in this simulator.");
        }

        public override Result M(Qubit qubit) => MeasureByIndex(qubit.Id);

        public override Result Measure(IQArray<Pauli> bases, IQArray<Qubit> qubits)
        {
            if (bases.Any(basis => basis == Pauli.PauliX || basis == Pauli.PauliY) ||
                bases.Where(basis => basis == Pauli.PauliZ).Count() != 1)
            {
                throw new UnsupportedOperationException("Not yet implemented.");
            }
            var idxQubit = Enumerable.Zip(bases, qubits, (b, q) => (b, q)).Single(item => item.b == Pauli.PauliZ).q.Id;
            return MeasureByIndex(idxQubit);
        }

        public override void X(Qubit qubit)
        {
            Hadamard(qubit.Id);
            Phase(qubit.Id);
            Phase(qubit.Id);
            Hadamard(qubit.Id);
        }

        public override void ControlledX(IQArray<Qubit> controls, Qubit qubit)
        {
            if (controls.Length == 1)
            {
                Cnot(controls.First().Id, qubit.Id);
            }
            else
            {
                throw new UnsupportedOperationException("Only singular controlled gates are allowed.");
            }

        }

        public override void Y(Qubit qubit)
        {
            SAdjoint(qubit);
            X(qubit);
            S(qubit);
        }

        public override void ControlledY(IQArray<Qubit> controls, Qubit qubit)
        {
            if (controls.Length == 1)
            {
                SAdjoint(qubit);
                ControlledX(controls, qubit);
                S(qubit);
            }
            else
            {
                throw new UnsupportedOperationException("Only singular controlled gates are allowed.");
            }

        }

        public override void Z(Qubit qubit)
        {
            Hadamard(qubit.Id);
            Phase(qubit.Id);
            Phase(qubit.Id);
        }

        public override void ControlledZ(IQArray<Qubit> controls, Qubit qubit)
        {
            if (controls.Length == 1)
            {
                Hadamard(qubit.Id);
                Cnot(controls.First().Id, qubit.Id);
                Hadamard(qubit.Id);
            }
            else
            {
                throw new UnsupportedOperationException("Only singular controlled gates are allowed.");
            }

        }

        public override void SWAP(Qubit qubit1, Qubit qubit2)
        {
            Cnot(qubit1.Id, qubit2.Id);
            Cnot(qubit2.Id, qubit1.Id);
            Cnot(qubit1.Id, qubit2.Id);
        }

        public override void Reset(Qubit qubit) {
            if (M(qubit) == Result.One) { X(qubit); }
        }

        //////////////////////////////////////////////////////////////////////
        // Overrides - Unsupported
        //////////////////////////////////////////////////////////////////////
        public override void ControlledExp(IQArray<Qubit> controls, IQArray<Pauli> paulis, double theta, IQArray<Qubit> qubits) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void ControlledExpFrac(IQArray<Qubit> controls, IQArray<Pauli> paulis, long numerator, long power, IQArray<Qubit> qubits) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void ControlledR(IQArray<Qubit> controls, Pauli axis, double theta, Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void ControlledR1(IQArray<Qubit> controls, double theta, Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void ControlledR1Frac(IQArray<Qubit> controls, long numerator, long power, Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void ControlledRFrac(IQArray<Qubit> controls, Pauli axis, long numerator, long power, Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void ControlledS(IQArray<Qubit> controls, Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void ControlledSAdjoint(IQArray<Qubit> controls, Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void ControlledSWAP(IQArray<Qubit> controls, Qubit qubit1, Qubit qubit2) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void ControlledT(IQArray<Qubit> controls, Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void ControlledTAdjoint(IQArray<Qubit> controls, Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void Exp(IQArray<Pauli> paulis, double theta, IQArray<Qubit> qubits) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void ExpFrac(IQArray<Pauli> paulis, long numerator, long power, IQArray<Qubit> qubits) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void R(Pauli axis, double theta, Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void R1(double theta, Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void R1Frac(long numerator, long power, Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void RFrac(Pauli axis, long numerator, long power, Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void T(Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
        public override void TAdjoint(Qubit qubit) => 
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");
            
    }
}
