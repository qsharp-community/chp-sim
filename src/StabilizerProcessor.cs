// Copyright (c) Sarah Kaiser. All rights reserved.
// Licensed under the MIT License.
#nullable enable

using System;
using System.IO;
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
        private readonly bool[,] table;
        private readonly int nQubits;
        internal StabilizerSimulator? Simulator;

        public StabilizerProcessor(int nQubits = 1024)
        {
            this.nQubits = nQubits;
            // By default, this array is full of false
            table = new bool[2 * this.nQubits, 2 * this.nQubits + 1];
            table.SetDiagonal(true);
        }

        //////////////////////////////////////////////////////////////////////
        // Internal
        //////////////////////////////////////////////////////////////////////
        private int X(int idxCol) => idxCol;

        private int Z(int idxCol) => nQubits + idxCol;

        private int RIndex => 2 * nQubits;

        private void Hadamard(int target)
        {
            // This takes care of mapping X->Z and Z->X
            table.SwapColumns(X(target), Z(target));

            // Now need to handle the phase, this represents the fact that HYH = -Y
            foreach (var idxRow in Enumerable.Range(0, table.GetLength(0)))
            {
                table[idxRow, RIndex] ^= table[idxRow, X(target)] && table[idxRow, Z(target)];
            }
        }

        private void Phase(int target) 
        {
            // Add global phase, this represents the fact that HYH = -Y
            foreach (var idxRow in Enumerable.Range(0, table.GetLength(0)))
            {
                table[idxRow, RIndex] ^= table[idxRow, X(target)] && table[idxRow, Z(target)];
            }

            // 
            foreach (var idxRow in Enumerable.Range(0, table.GetLength(0)))
            {
                table[idxRow, Z(target)] ^= table[idxRow, X(target)];
            }

        }

        private void Cnot(int control, int target)
        {
            foreach (var idxRow in Enumerable.Range(0, table.GetLength(0)))
            {
                table[idxRow, RIndex] ^= table[idxRow, X(control)] & 
                    table[idxRow, Z(target)] &
                    (table[idxRow, X(target)] ^ table[idxRow, Z(control)] ^ true);
            }

            foreach (var idxRow in Enumerable.Range(0, table.GetLength(0)))
            {
                table[idxRow, X(target)] ^= table[idxRow, X(control)];
            }

            foreach (var idxRow in Enumerable.Range(0, table.GetLength(0)))
            {
                table[idxRow, Z(control)] ^= table[idxRow, Z(target)];
            }
        }

        private Result MeasureByIndex(int idx)
        {
            // Deterministic Case
            if (IsMeasurementDetermined(idx, out var result))
            {
                return result;
            }
            // Non-Deterministic Case
            else
            {
                var collisions = table.Column(idx).IndicesWhere(b => b).ToList();
                var idxFirst = nQubits + table.Column(idx).Skip(nQubits).IndicesWhere(b => b).First();
                
                foreach (var idxCollision in collisions.Where(idxCollision => idxCollision != idxFirst))
                {
                    table.SetToRowSum(idxCollision, idxFirst);
                }

                foreach (var idxColumn in Enumerable.Range(0, table.GetLength(1)))
                {
                    table[idxFirst - nQubits, idxColumn] = table[idxFirst, idxColumn]; 
                    table[idxFirst, idxColumn] = false;              
                }
                table[idxFirst, Z(idx)] = true;
                table[idxFirst, RIndex] = result == Result.One;
                return result;
            }
            
        }

        private bool IsMeasurementDetermined(int idx, out Result result)
        {
            var isDetermined = !table.Column(idx).Skip(nQubits).Any(b => b);
            if (isDetermined)
            {
                var vector = new bool[2 * nQubits + 1];
                foreach (var idxDestabilizer in Enumerable.Range(0, nQubits))
                {
                    if (table[idxDestabilizer, X(idx)])
                    {
                        vector.SetToRowSum(table, idxDestabilizer + nQubits);
                    }                    
                }
                result = vector[^1] ? Result.One : Result.Zero;
            }
            else
            {
                result = (new System.Random()).Next(2) == 1 ? Result.One : Result.Zero;
            }

            return isDetermined;
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
                System.Console.WriteLine(table.MatrixToString(true));
            }
            else if (location is string filename)
            {
                File.WriteAllText(filename, table.MatrixToString(true));  
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
            if (!bases.TryGetSingleZ(out var idx))
            {
                // throw new UnsupportedOperationException("Not yet implemented.");
                var aux = this.Simulator!.QubitManager?.Allocate();
                if (aux == null) throw new NullReferenceException("Qubit manager was null.");
                try
                {
                    WriteToScratch(bases, qubits, aux);
                    return MeasureByIndex(aux.Id);
                }
                finally
                {
                    this.Simulator!.QubitManager?.Release(aux);
                }
            }
            return MeasureByIndex(idx);
        }

        private void WriteToScratch(IQArray<Pauli> bases, IQArray<Qubit> qubits, Qubit aux)
        {
            foreach (var (pauli, qubit) in Enumerable.Zip(bases, qubits, (pauli, qubit) => (pauli, qubit)))
            {
                switch (pauli)
                {
                    case Pauli.PauliI:
                        break;

                    case Pauli.PauliX:
                        H(qubit);
                        ControlledX(new QArray<Qubit>(new[] { qubit }), aux);
                        H(qubit);
                        break;

                    case Pauli.PauliY:
                        H(qubit);
                        S(qubit);
                        ControlledX(new QArray<Qubit>(new[] { qubit }), aux);
                        SAdjoint(qubit);
                        H(qubit);
                        break;

                    default:
                        ControlledX(new QArray<Qubit>(new[] { qubit }), aux);
                        break;
                }
            }
        }

        public override void Reset(Qubit qubit) {
            if (M(qubit) == Result.One) { X(qubit); }
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

        //////////////////////////////////////////////////////////////////////
        // Overrides - Unsupported
        //////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Temporary check to give a more readable exception as long as there is no dynamic allocations.
        /// </summary>
        /// <param name="qubits">qubits to allocate</param>
        public override void OnAllocateQubits(IQArray<Qubit> qubits)
        {
            Allocate(qubits);
            base.OnAllocateQubits(qubits);
        }
        /// <summary>
        /// Temporary check to give a more readable exception as long as there is no dynamic allocations.
        /// </summary>
        /// <param name="qubits">qubits to allocate</param>
        public override void OnBorrowQubits(IQArray<Qubit> qubits, long allocatedForBorrowingCount)
        {
            Allocate(qubits);
            base.OnBorrowQubits(qubits, allocatedForBorrowingCount);
        }
        /// <summary>
        /// Temporary check to give a more readable exception as long as there is no dynamic allocations.
        /// </summary>
        /// <param name="qubits">qubits to deallocate</param>
        public override void OnReleaseQubits(IQArray<Qubit> qubits)
        {
            DeAllocate(qubits);
            base.OnReleaseQubits(qubits);
        }
        /// <summary>
        /// Temporary check to give a more readable exception as long as there is no dynamic allocations.
        /// </summary>
        /// <param name="qubits">qubits to deallocate</param>
        public override void OnReturnQubits(IQArray<Qubit> qubits, long releasedOnReturnCount)
        {
            DeAllocate(qubits);
            base.OnReturnQubits(qubits, releasedOnReturnCount);
        }

        private void Allocate(IQArray<Qubit> qubits)
        {
            allocated += qubits.Count;
            if (allocated > nQubits)
            {
                throw new UnsupportedOperationException($"Simulator supports a max of {nQubits} qubits. Total requested {allocated}");
            }
        }
        private void DeAllocate(IQArray<Qubit> qubits)
        {
            allocated -= qubits.Count;
        }
        private int allocated = 0;

        public override void Assert(IQArray<Pauli> bases, IQArray<Qubit> qubits, Result result, string msg) =>
            AssertProb(bases, qubits, result == Result.One ? 0 : 1, msg, 1e-10);
        public override void AssertProb(IQArray<Pauli> bases, IQArray<Qubit> qubits, double probabilityOfZero, string msg, double tol) 
        {
            bool shouldBeDeterministic;
            var expectedResult = Result.Zero;
            // Is the probability 0?
            if (Math.Abs(probabilityOfZero-0)<tol)
            {
                shouldBeDeterministic = true;
                expectedResult = Result.One;
            }
            else if (Math.Abs(probabilityOfZero-0.5)<tol)
            {
                shouldBeDeterministic = false;
            }
            else if (Math.Abs(probabilityOfZero-1)<tol)
            {
                shouldBeDeterministic = true;
                expectedResult = Result.Zero;               
            }
            else
            {
                throw new ExecutionFailException(msg);
            }


            if (!bases.TryGetSingleZ(out var idx))
            {
                var aux = this.Simulator!.QubitManager?.Allocate();
                if (aux == null) throw new NullReferenceException("Qubit manager was null.");
                try
                {
                    WriteToScratch(bases, qubits, aux);
                    AssertProb(
                        new QArray<Pauli>(new[] { Pauli.PauliZ }), 
                        new QArray<Qubit>(new[] { aux }), 
                        probabilityOfZero,
                        msg, 
                        tol
                    );
                    WriteToScratch(
                        new QArray<Pauli>(bases.Reverse()),
                        new QArray<Qubit>(qubits.Reverse()),
                        aux
                    );
                }
                finally
                {
                    this.Simulator!.QubitManager?.Release(aux);
                }
            }
            else
            {
                var isDeterministic = IsMeasurementDetermined(idx, out var result);
                if (isDeterministic == shouldBeDeterministic) 
                {
                    if (!isDeterministic || expectedResult == result)
                    {
                        return;
                    }
                    throw new ExecutionFailException(msg);
                }
            }
            
        }
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
