// <copyright file="Extensions.cs" company="https://qsharp.community/">
// Copyright (c) Sarah Kaiser. Licensed under the MIT License.
// </copyright>

// This C# project is based on a Python implementation by @Strilanc here:
// https://github.com/Strilanc/python-chp-stabilizer-simulator
namespace QSharpCommunity.Simulators.Chp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Quantum.Simulation.Core;

    /// <summary>
    /// Extension methods for the simulators.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Set the diagonal line of the matix to the value.
        /// </summary>
        /// <typeparam name="T">Matrix type.</typeparam>
        /// <param name="matrix">The matrix.</param>
        /// <param name="value">The value to set it at.</param>
        internal static void SetDiagonal<T>(this T[,] matrix, T value)
        {
            // TODO: better safety checking for the num rows
            foreach (var idxRow in Enumerable.Range(0, matrix.GetLength(0)))
            {
                matrix[idxRow, idxRow] = value;
            }
        }

        /// <summary>
        /// Swapped the collumns.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="idx1">Collum to swap with idx2.</param>
        /// <param name="idx2">Collum to swap with idx1.</param>
        internal static void SwapColumns(this bool[,] matrix, int idx1, int idx2)
        {
            foreach (var idxRow in Enumerable.Range(0, matrix.GetLength(0)))
            {
                matrix[idxRow, idx1] ^= matrix[idxRow, idx2];
                matrix[idxRow, idx2] ^= matrix[idxRow, idx1];
                matrix[idxRow, idx1] ^= matrix[idxRow, idx2];
            }
        }

        /// <summary>
        /// Returns the row of the matrix.
        /// </summary>
        /// <typeparam name="T">the matrix type.</typeparam>
        /// <param name="matrix">The matrix.</param>
        /// <param name="idxRow">The row that needs to be returned.</param>
        /// <returns>The row highlighted by idxRow.</returns>
        internal static IEnumerable<T> Row<T>(this T[,] matrix, int idxRow)
        {
            foreach (var idxColumn in Enumerable.Range(0, matrix.GetLength(1)))
            {
                yield return matrix[idxRow, idxColumn];
            }
        }

        /// <summary>
        /// Returns the column of the matrix.
        /// </summary>
        /// <typeparam name="T">the matrix type.</typeparam>
        /// <param name="matrix">The matrix.</param>
        /// <param name="idxColumn">The column that needs to be returned.</param>
        /// <returns>The row highlighted by idxColumn.</returns>
        internal static IEnumerable<T> Column<T>(this T[,] matrix, int idxColumn)
        {
            foreach (var idxRow in Enumerable.Range(0, matrix.GetLength(0)))
            {
                yield return matrix[idxRow, idxColumn];
            }
        }

        /// <summary>
        /// Represents the Row including the phase.
        /// </summary>
        /// <param name="vector">Row represented as a vector.</param>
        /// <returns>The rendered row including the phase.</returns>
        internal static string RowToString(this bool[] vector)
        {
            var (xs, zs, r) = vector.SplitRow();
            return (r ? "-" : "+") + string.Join(
                string.Empty,
                Enumerable.Zip(xs, zs, (x, z) =>
                    (x, z) switch
                    {
                        (false, false) => "I",
                        (true, false) => "X",
                        (false, true) => "Z",
                        (true, true) => "Y",
                    }));
        }

        /// <summary>
        /// Renders the row to a text string.
        /// </summary>
        /// <param name="matrix">The Matrix.</param>
        /// <param name="idx">Row index.</param>
        /// <returns>A rendered string of the row.</returns>
        internal static string RowToString(this bool[,] matrix, int idx) => matrix.Row(idx).ToArray().RowToString();

        /// <summary>
        /// Renders the row to a latex usable text string.
        /// </summary>
        /// <param name="matrix">The Matrix.</param>
        /// <param name="idx">Row index.</param>
        /// <returns>A rendered string of the row.</returns>
        internal static string RowToLatex(this bool[,] matrix, int idx) =>
            string.Join(" & ", matrix.Row(idx).ToArray().Select(val => val ? 1 : 0));

        /// <summary>
        /// Renders the matrix to a text string.
        /// </summary>
        /// <param name="matrix">The Matrix.</param>
        /// <param name="showDestabilizers">Include the stabilizers in the string.</param>
        /// <returns>A rendered string of the table.</returns>
        internal static string MatrixToString(this bool[,] matrix, bool showDestabilizers = false) =>
            "<" + string.Join(", ", Enumerable.Range(matrix.GetLength(0) / 2, matrix.GetLength(0) / 2).Select(idx => matrix.RowToString(idx))) + ">" +
            (showDestabilizers ?
            "| >" + string.Join(", ", Enumerable.Range(0, matrix.GetLength(0) / 2).Select(idx => matrix.RowToString(idx))) + "<" :
            ">");

        /// <summary>
        /// Renders the matrixto a latex usable text string.
        /// </summary>
        /// <param name="matrix">The Matrix.</param>
        /// <param name="showDestabilizers">Include the stabilizers in the string.</param>
        /// <returns>A rendered string of the table.</returns>
        internal static string MatrixToLatexString(this bool[,] matrix, bool showDestabilizers = false) =>
            (
                showDestabilizers
                ? string.Join(
                    @" \\", Enumerable.Range(0, matrix.GetLength(0) / 2).Select(idx => matrix.RowToLatex(idx))) + @" \\ \hline"
                : string.Empty) +
            string.Join(@" \\", Enumerable.Range(matrix.GetLength(0) / 2, matrix.GetLength(0) / 2).Select(idx => matrix.RowToLatex(idx)));

        /// <summary>
        /// Split the row into; stabalizers, destabilzers, and phase.
        /// </summary>
        /// <param name="row">Boolean representation of the row.</param>
        /// <returns>3-Element Tuple with seperated stabalizers, destabilzers, and phase.</returns>
        internal static (bool[] Stabalizers, bool[] DeStabilzers, bool Phase) SplitRow(this IEnumerable<bool> row)
        {
            var vector = row.ToArray();
            var nQubits = (vector.Length - 1) / 2;
            return (vector[0..nQubits], vector[nQubits..^1], vector[^1]);
        }

        /// <summary>
        /// Calculate the product of the phase.
        /// </summary>
        /// <param name="row1">Left vector.</param>
        /// <param name="row2">Right vector.</param>
        /// <returns>PhaseProduct.</returns>
        internal static bool PhaseProduct(this IEnumerable<bool> row1, IEnumerable<bool> row2)
        {
            static int G(bool x1, bool z1, bool x2, bool z2) =>
                (x1, z1) switch
                {
                    (false, false) => 0,
                    (true, true) => (z2 ? 1 : 0) - (x2 ? 1 : 0),
                    (true, false) => (z2 ? 1 : 0) * ((2 * (x2 ? 1 : 0)) - 1),
                    (false, true) => (x2 ? 1 : 0) * ((2 * (z2 ? 1 : 0)) - 1),
                };

            var (xs1, zs1, r1) = row1.SplitRow();
            var (xs2, zs2, r2) = row2.SplitRow();
            var acc = 0;

            foreach (var idxColumn in Enumerable.Range(0, xs1.Length))
            {
                acc += G(xs1[idxColumn], zs1[idxColumn], xs2[idxColumn], zs2[idxColumn]);
            }

            return ((r1 ? 2 : 0) + (r2 ? 2 : 0) + acc) % 4 == 2;
        }

        /// <summary>
        /// Set the row sum for the Matrix.
        /// </summary>
        /// <param name="matrix">The Matrix.</param>
        /// <param name="idxTarget">Index of vector to.</param>
        /// <param name="idxSource">Index of vector from.</param>
        internal static void SetToRowSum(this bool[,] matrix, int idxTarget, int idxSource)
        {
            foreach (var idxColumn in Enumerable.Range(0, matrix.GetLength(1) - 1))
            {
                matrix[idxTarget, idxColumn] ^= matrix[idxSource, idxColumn];
            }

            matrix[idxTarget, matrix.GetLength(1) - 1] = matrix.Row(idxTarget).PhaseProduct(matrix.Row(idxSource));
        }

        /// <summary>
        /// Set the row sum for the Matrix.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="matrix">The Matrix.</param>
        /// <param name="idxSource">Index of vector from.</param>
        internal static void SetToRowSum(this bool[] vector, bool[,] matrix, int idxSource)
        {
            foreach (var idxColumn in Enumerable.Range(0, matrix.GetLength(1) - 1))
            {
                vector[idxColumn] ^= matrix[idxSource, idxColumn];
            }

            vector[matrix.GetLength(1) - 1] = vector.PhaseProduct(matrix.Row(idxSource));
        }

        /// <summary>
        /// Search the matrix.
        /// </summary>
        /// <typeparam name="T">Type of the matrix.</typeparam>
        /// <param name="source">Source list to check.</param>
        /// <param name="predicate">Predicate to check.</param>
        /// <returns>Indices of where predicate is true.</returns>
        internal static IEnumerable<int> IndicesWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (var item in source.Select((element, idx) => (element, idx)))
            {
                if (predicate(item.element))
                {
                    yield return item.idx;
                }
            }
        }

        /// <summary>
        /// Try to find the PauliZ that it can be measured on (if there isn't a measurement on X or Y.).
        /// </summary>
        /// <param name="paulis">The Pauli axis being measured on.</param>
        /// <param name="idx">The index of the PauliX. -1 otherwise.</param>
        /// <returns>If it can be measured on a single PauliZ.</returns>
        internal static bool TryGetSingleZ(this IEnumerable<Pauli> paulis, out int idx)
        {
            if (paulis.Any(basis => basis == Pauli.PauliX || basis == Pauli.PauliY))
            {
                idx = -1;
                return false;
            }

            var idxs = paulis
                .Select((pauli, idx) => (pauli, idx))
                .Where(item => item.pauli == Pauli.PauliZ)
                .Select(item => item.idx);

            if (idxs.Count() != 1)
            {
                idx = -1;
                return false;
            }
            else
            {
                idx = idxs.Single();
                return true;
            }
        }
    }
}