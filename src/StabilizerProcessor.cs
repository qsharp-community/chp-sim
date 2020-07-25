// <copyright file="StabilizerProcessor.cs" company="https://qsharp.community/">
// Copyright (c) Sarah Kaiser. All rights reserved.
// Licensed under the MIT License.
// </copyright>
#nullable enable

// This C# project is based on a Python implementation by @Strilanc here:
// https://github.com/Strilanc/python-chp-stabilizer-simulator
namespace QSharpCommunity.Simulators.Chp
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using Microsoft.Quantum.Simulation.Common;
    using Microsoft.Quantum.Simulation.Core;

    /// <summary>
    /// CHP Simulator class.
    /// </summary>
    public class StabilizerProcessor : QuantumProcessorBase
    {
        private readonly bool[,] table;
        private readonly int nQubits;

        /// <summary>
        /// Current allocated qubit count.
        /// </summary>
        private int allocated = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="StabilizerProcessor"/> class.
        /// </summary>
        /// <param name="nQubits">Amount of qubits to simulate.</param>
        public StabilizerProcessor(int? nQubits = null)
        {
            this.nQubits = nQubits ?? 1024;

            // By default, this array is full of false
            this.table = new bool[2 * this.nQubits, (2 * this.nQubits) + 1];
            this.table.SetDiagonal(true);
        }

        /// <summary>
        /// Gets or sets the simulator used in this run.
        /// </summary>
        internal StabilizerSimulator? Simulator { get; set; }

        private int RIndex => 2 * this.nQubits;

        //////////////////////////////////////////////////////////////////////
        // Public
        //////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Temporary check to give a more readable exception as long as there is no dynamic allocations.
        /// </summary>
        /// <param name="qubits">qubits to allocate.</param>
        public override void OnAllocateQubits(IQArray<Qubit> qubits)
        {
            this.Allocate(qubits);
            base.OnAllocateQubits(qubits);
        }

        /// <summary>
        /// Temporary check to give a more readable exception as long as there is no dynamic allocations.
        /// </summary>
        /// <param name="qubits">qubits to allocate.</param>
        /// <param name="allocatedForBorrowingCount">count of qubits to reserve for borrowing.</param>
        public override void OnBorrowQubits(IQArray<Qubit> qubits, long allocatedForBorrowingCount)
        {
            this.Allocate(qubits);
            base.OnBorrowQubits(qubits, allocatedForBorrowingCount);
        }

        /// <summary>
        /// Temporary check to give a more readable exception as long as there is no dynamic allocations.
        /// </summary>
        /// <param name="qubits">qubits to deallocate.</param>
        public override void OnReleaseQubits(IQArray<Qubit> qubits)
        {
            this.DeAllocate(qubits);
            base.OnReleaseQubits(qubits);
        }

        /// <summary>
        /// Temporary check to give a more readable exception as long as there is no dynamic allocations.
        /// </summary>
        /// <param name="qubits">qubits to deallocate.</param>
        /// <param name="releasedOnReturnCount">count of qubits being released.</param>
        public override void OnReturnQubits(IQArray<Qubit> qubits, long releasedOnReturnCount)
        {
            this.DeAllocate(qubits);
            base.OnReturnQubits(qubits, releasedOnReturnCount);
        }

        /// <inheritdoc/>
        public override void Reset(Qubit qubit)
        {
            if (this.M(qubit) == Result.One)
            {
                this.X(qubit);
            }
        }

        //////////////////////////////////////////////////////////////////////
        // Overrides - Supported
        //////////////////////////////////////////////////////////////////////

        /// <inheritdoc/>
        public override void H(Qubit qubit)
        {
            this.Hadamard(qubit.Id);
        }

        /// <inheritdoc/>
        public override void ControlledH(IQArray<Qubit> controls, Qubit qubit)
        {
            if (controls.Length == 0)
            {
                this.H(qubit);
            }
            else
            {
                throw new UnsupportedOperationException("Controlled H is not a Clifford operation.");
            }
        }

        /// <inheritdoc/>
        public override void S(Qubit qubit)
        {
            this.Phase(qubit.Id);
        }

        /// <inheritdoc/>
        public override void SAdjoint(Qubit qubit)
        {
            this.Phase(qubit.Id);
            this.Phase(qubit.Id);
            this.Phase(qubit.Id);
        }

        /// <inheritdoc/>
        public override void OnDumpMachine<T>(T location)
        {
            this.Simulator?.MaybeDisplayDiagnostic(new StabilizerTableau
            {
                Data = this.table,
            });
            if (location is QVoid)
            {
                System.Console.WriteLine(this.table.MatrixToString(true));
            }
            else if (location is string filename)
            {
                File.WriteAllText(filename, this.table.MatrixToString(true));
            }
            else
            {
                throw new Exception("Not a valid file path.");
            }
        }

        /// <inheritdoc/>
        public override void OnDumpRegister<T>(T location, IQArray<Qubit> qubits)
        {
            this.OnMessage("Only DumpMachine is supported in this simulator.");
        }

        /// <inheritdoc/>
        public override void X(Qubit qubit)
        {
            this.Hadamard(qubit.Id);
            this.Phase(qubit.Id);
            this.Phase(qubit.Id);
            this.Hadamard(qubit.Id);
        }

        /// <inheritdoc/>
        public override void ControlledX(IQArray<Qubit> controls, Qubit qubit)
        {
            if (controls.Length == 1)
            {
                this.Cnot(controls.First().Id, qubit.Id);
            }
            else
            {
                throw new UnsupportedOperationException("Only singular controlled gates are allowed.");
            }
        }

        /// <inheritdoc/>
        public override void Y(Qubit qubit)
        {
            this.SAdjoint(qubit);
            this.X(qubit);
            this.S(qubit);
        }

        /// <inheritdoc/>
        public override void ControlledY(IQArray<Qubit> controls, Qubit qubit)
        {
            if (controls.Length == 1)
            {
                this.SAdjoint(qubit);
                this.ControlledX(controls, qubit);
                this.S(qubit);
            }
            else
            {
                throw new UnsupportedOperationException("Only singular controlled gates are allowed.");
            }
        }

        /// <inheritdoc/>
        public override void Z(Qubit qubit)
        {
            this.Phase(qubit.Id);
            this.Phase(qubit.Id);
        }

        /// <inheritdoc/>
        public override void ControlledZ(IQArray<Qubit> controls, Qubit qubit)
        {
            if (controls.Length == 1)
            {
                this.Hadamard(qubit.Id);
                this.Cnot(controls.First().Id, qubit.Id);
                this.Hadamard(qubit.Id);
            }
            else
            {
                throw new UnsupportedOperationException("Only singular controlled gates are allowed.");
            }
        }

        /// <inheritdoc/>
        public override void SWAP(Qubit qubit1, Qubit qubit2)
        {
            this.Cnot(qubit1.Id, qubit2.Id);
            this.Cnot(qubit2.Id, qubit1.Id);
            this.Cnot(qubit1.Id, qubit2.Id);
        }

        /// <inheritdoc/>
        public override void Assert(IQArray<Pauli> bases, IQArray<Qubit> qubits, Result result, string msg) =>
            this.AssertProb(bases, qubits, result == Result.One ? 0 : 1, msg, 1e-10);

        /// <inheritdoc/>
        public override void AssertProb(IQArray<Pauli> bases, IQArray<Qubit> qubits, double probabilityOfZero, string msg, double tol)
        {
            bool shouldBeDeterministic;
            Result? expectedResult;

            // Is the probability 0?
            if (Math.Abs(probabilityOfZero - 0) < tol)
            {
                shouldBeDeterministic = true;
                expectedResult = Result.One;
            }
            else if (Math.Abs(probabilityOfZero - 0.5) < tol)
            {
                shouldBeDeterministic = false;
                expectedResult = null;
            }
            else if (Math.Abs(probabilityOfZero - 1) < tol)
            {
                shouldBeDeterministic = true;
                expectedResult = Result.Zero;
            }
            else
            {
                throw new ExecutionFailException(msg);
            }

            this.Simulator?.MaybeDisplayDiagnostic(
                new DebugMessage
                {
                    Message = $"shouldBeDeterministic = {shouldBeDeterministic}, expectedResult = {expectedResult}",
                });

            if (!bases.TryGetSingleZ(out var idx))
            {
                var aux = this.Simulator!.QubitManager?.Allocate();
                if (aux == null)
                {
                    throw new NullReferenceException("Qubit manager was null.");
                }

                try
                {
                    this.WriteToScratch(bases, qubits, aux);
                    this.AssertProb(
                        new QArray<Pauli>(new[] { Pauli.PauliZ }),
                        new QArray<Qubit>(new[] { aux }),
                        probabilityOfZero,
                        msg,
                        tol);
                    this.WriteToScratch(
                        new QArray<Pauli>(bases.Reverse()),
                        new QArray<Qubit>(qubits.Reverse()),
                        aux);
                }
                finally
                {
                    this.Simulator!.QubitManager?.Release(aux);
                }
            }
            else
            {
                var isDeterministic = this.IsMeasurementDetermined(qubits[idx].Id, out var result);
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

        public override Result M(Qubit qubit) => this.MeasureByIndex(qubit.Id);

        /// <inheritdoc/>
        public override Result Measure(IQArray<Pauli> bases, IQArray<Qubit> qubits)
        {
            if (!bases.TryGetSingleZ(out var idx))
            {
                // throw new UnsupportedOperationException("Not yet implemented.");
                var aux = this.Simulator!.QubitManager?.Allocate();
                if (aux == null)
                {
                    throw new NullReferenceException("Qubit manager was null.");
                }

                try
                {
                    this.WriteToScratch(bases, qubits, aux);
                    return this.MeasureByIndex(aux.Id);
                }
                finally
                {
                    this.Simulator!.QubitManager?.Release(aux);
                }
            }

            return this.MeasureByIndex(qubits[idx].Id);
        }

        //////////////////////////////////////////////////////////////////////
        // Overrides - Unsupported
        //////////////////////////////////////////////////////////////////////

        /// <inheritdoc/>
        public override void ControlledExp(IQArray<Qubit> controls, IQArray<Pauli> paulis, double theta, IQArray<Qubit> qubits) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void ControlledExpFrac(IQArray<Qubit> controls, IQArray<Pauli> paulis, long numerator, long power, IQArray<Qubit> qubits) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void ControlledR(IQArray<Qubit> controls, Pauli axis, double theta, Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void ControlledR1(IQArray<Qubit> controls, double theta, Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void ControlledR1Frac(IQArray<Qubit> controls, long numerator, long power, Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void ControlledRFrac(IQArray<Qubit> controls, Pauli axis, long numerator, long power, Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void ControlledS(IQArray<Qubit> controls, Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void ControlledSAdjoint(IQArray<Qubit> controls, Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void ControlledSWAP(IQArray<Qubit> controls, Qubit qubit1, Qubit qubit2) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void ControlledT(IQArray<Qubit> controls, Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void ControlledTAdjoint(IQArray<Qubit> controls, Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void Exp(IQArray<Pauli> paulis, double theta, IQArray<Qubit> qubits) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void ExpFrac(IQArray<Pauli> paulis, long numerator, long power, IQArray<Qubit> qubits) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void R(Pauli axis, double theta, Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void R1(double theta, Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void R1Frac(long numerator, long power, Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void RFrac(Pauli axis, long numerator, long power, Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void T(Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        /// <inheritdoc/>
        public override void TAdjoint(Qubit qubit) =>
            throw new UnsupportedOperationException("This operation is not supported in the CHP Stabilizer formalism.");

        //////////////////////////////////////////////////////////////////////
        // Internal
        //////////////////////////////////////////////////////////////////////

        private int X(int idxCol) => idxCol;

        private int Z(int idxCol) => this.nQubits + idxCol;

        private void Hadamard(int target)
        {
            // This takes care of mapping X->Z and Z->X
            this.table.SwapColumns(this.X(target), this.Z(target));

            // Now need to handle the phase, this represents the fact that HYH = -Y
            foreach (var idxRow in Enumerable.Range(0, this.table.GetLength(0)))
            {
                this.table[idxRow, this.RIndex] ^= this.table[idxRow, this.X(target)] && this.table[idxRow, this.Z(target)];
            }
        }

        private void Phase(int target)
        {
            // Add global phase, this represents the fact that SYS^{+} = -Y
            foreach (var idxRow in Enumerable.Range(0, this.table.GetLength(0)))
            {
                this.table[idxRow, this.RIndex] ^= this.table[idxRow, this.X(target)] && this.table[idxRow, this.Z(target)];
            }

            foreach (var idxRow in Enumerable.Range(0, this.table.GetLength(0)))
            {
                this.table[idxRow, this.Z(target)] ^= this.table[idxRow, this.X(target)];
            }
        }

        private void Cnot(int control, int target)
        {
            foreach (var idxRow in Enumerable.Range(0, this.table.GetLength(0)))
            {
                this.table[idxRow, this.RIndex] ^= this.table[idxRow, this.X(control)] &
                    this.table[idxRow, this.Z(target)] &
                    (this.table[idxRow, this.X(target)] ^ this.table[idxRow, this.Z(control)] ^ true);
            }

            foreach (var idxRow in Enumerable.Range(0, this.table.GetLength(0)))
            {
                this.table[idxRow, this.X(target)] ^= this.table[idxRow, this.X(control)];
            }

            foreach (var idxRow in Enumerable.Range(0, this.table.GetLength(0)))
            {
                this.table[idxRow, this.Z(control)] ^= this.table[idxRow, this.Z(target)];
            }
        }

        private Result MeasureByIndex(int idx)
        {
            // Deterministic Case
            if (this.IsMeasurementDetermined(idx, out var result))
            {
                return result;
            }

            // Non-Deterministic Case
            else
            {
                var collisions = this.table.Column(idx).IndicesWhere(b => b).ToList();
                var idxFirst = this.nQubits + this.table.Column(idx).Skip(this.nQubits).IndicesWhere(b => b).First();

                foreach (var idxCollision in collisions.Where(idxCollision => idxCollision != idxFirst))
                {
                    this.table.SetToRowSum(idxCollision, idxFirst);
                }

                foreach (var idxColumn in Enumerable.Range(0, this.table.GetLength(1)))
                {
                    this.table[idxFirst - this.nQubits, idxColumn] = this.table[idxFirst, idxColumn];
                    this.table[idxFirst, idxColumn] = false;
                }

                this.table[idxFirst, this.Z(idx)] = true;
                this.table[idxFirst, this.RIndex] = result == Result.One;
                return result;
            }
        }

        private bool IsMeasurementDetermined(int idx, [NotNullWhen(true)] out Result result)
        {
            var isDetermined = !this.table.Column(idx).Skip(this.nQubits).Any(b => b);
            if (isDetermined)
            {
                var vector = new bool[(2 * this.nQubits) + 1];
                foreach (var idxDestabilizer in Enumerable.Range(0, this.nQubits))
                {
                    if (this.table[idxDestabilizer, this.X(idx)])
                    {
                        vector.SetToRowSum(this.table, idxDestabilizer + this.nQubits);
                    }
                }

                result = vector[^1] ? Result.One : Result.Zero;
            }
            else
            {
                result = new System.Random().Next(2) == 1 ? Result.One : Result.Zero;
            }

            this.Simulator?.MaybeDisplayDiagnostic(new DebugMessage
            {
                Message = $"IsResultDetermined({idx}, out {result}) = {isDetermined}",
            });
            return isDetermined;
        }

        private void Allocate(IQArray<Qubit> qubits)
        {
            this.allocated += qubits.Count;
            if (this.allocated > this.nQubits)
            {
                throw new UnsupportedOperationException($"Simulator supports a max of {this.nQubits} qubits. Total requested {this.allocated}");
            }
        }

        private void DeAllocate(IQArray<Qubit> qubits)
        {
            this.allocated -= qubits.Count;
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
                        this.H(qubit);
                        this.ControlledX(new QArray<Qubit>(new[] { qubit }), aux);
                        this.H(qubit);
                        break;

                    case Pauli.PauliY:
                        this.H(qubit);
                        this.S(qubit);
                        this.ControlledX(new QArray<Qubit>(new[] { qubit }), aux);
                        this.SAdjoint(qubit);
                        this.H(qubit);
                        break;

                    // Pauli.PauliZ:
                    default:
                        this.ControlledX(new QArray<Qubit>(new[] { qubit }), aux);
                        break;
                }
            }
        }
     }
}
