using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Quantum.Simulation.Core;

// This C# project is based on a Python implementation by @Strilanc here: 
// https://github.com/Strilanc/python-chp-stabilizer-simulator
namespace chp
{

    internal static class Extensions 
    {
        internal static void SetDiagonal<T>(this T[,] matrix, T value)
        {
            // TODO: better safety checking for the num rows
            foreach (var idxRow in Enumerable.Range(0, matrix.GetLength(0)))
            {
                matrix[idxRow, idxRow] = value;
            }
        }

        internal static void SwapColumns(this bool[,] matrix, int idx1, int idx2)
        {
            foreach (var idxRow in Enumerable.Range(0, matrix.GetLength(0)))
            {
                matrix[idxRow, idx1] ^= matrix[idxRow, idx2];
                matrix[idxRow, idx2] ^= matrix[idxRow, idx1];
                matrix[idxRow, idx1] ^= matrix[idxRow, idx2];
            }
        }

        internal static IEnumerable<T> Row<T>(this T[,] matrix, int idxRow)
        {
            foreach (var idxColumn in Enumerable.Range(0, matrix.GetLength(1)))
            {
                yield return matrix[idxRow, idxColumn];
            }
        }

        internal static IEnumerable<T> Column<T>(this T[,] matrix, int idxColumn)
        {
            foreach (var idxRow in Enumerable.Range(0, matrix.GetLength(0)))
            {
                yield return matrix[idxRow, idxColumn];
            }
        }

        internal static string RowToString(this bool[] vector)
        {
            var (xs, zs, r) = vector.SplitRow();
            return (r ? "-" : "+") + string.Join("",
                Enumerable.Zip(xs, zs, (x, z) => 
                    (x, z) switch
                    {
                        (false, false) => "I",
                        (true, false) => "X",
                        (false, true) => "Z",
                        (true, true) => "Y"
                    }
                )
            );
        }

        internal static string RowToString(this bool[,] matrix, int idx) => matrix.Row(idx).ToArray().RowToString();

        internal static string MatrixToString(this bool[,] matrix, bool showDestabilizers = false) =>
            "<" + string.Join(", ", Enumerable.Range(matrix.GetLength(0)/2, matrix.GetLength(0)/2).Select(idx => matrix.RowToString(idx))) + ">" +
            (showDestabilizers ?
            "| >" + string.Join(", ", Enumerable.Range(0, matrix.GetLength(0)/2).Select(idx => matrix.RowToString(idx))) + "<" :
            ">");


        internal static (bool[], bool[], bool) SplitRow(this IEnumerable<bool> row)
        {
            var vector = row.ToArray();
            var nQubits = (vector.Length - 1) / 2;
            return (vector[0..nQubits], vector[nQubits..^1], vector[^1]);
        }

        internal static bool PhaseProduct(this IEnumerable<bool> row1, IEnumerable<bool> row2)
        {
            int g(bool x1, bool z1, bool x2, bool z2) =>
                (x1, z1) switch
                {
                    (false, false) => 0,
                    (true, true) => (z2 ? 1 : 0) - (x2 ? 1 : 0),
                    (true, false) => (z2 ? 1 : 0) * (2 * (x2 ? 1 : 0) - 1),
                    (false, true) =>(x2 ? 1 : 0) * (2 * (z2 ? 1 : 0) - 1)
                };
            

            var (xs1, zs1, r1) = row1.SplitRow();
            var (xs2, zs2, r2) = row2.SplitRow();
            var acc = 0;

            foreach (var idxColumn in Enumerable.Range(0, xs1.Length))
            {
                acc += g(xs1[idxColumn], zs1[idxColumn], xs2[idxColumn], zs2[idxColumn]);
            }

            return ((r1 ? 2 : 0) + (r2 ? 2 : 0) + acc) % 4 == 2;
        }

        internal static void SetToRowSum(this bool[,] matrix, int idxTarget, int idxSource)
        {
            foreach (var idxColumn in Enumerable.Range(0, matrix.GetLength(1) - 1))
            {
                matrix[idxTarget, idxColumn] ^= matrix[idxSource, idxColumn];
            }

            matrix[idxTarget, matrix.GetLength(1) - 1] = matrix.Row(idxTarget).PhaseProduct(matrix.Row(idxSource));
        }

        internal static void SetToRowSum(this bool[] vector, bool[,] matrix, int idxSource)
        {
            foreach (var idxColumn in Enumerable.Range(0, matrix.GetLength(1) - 1))
            {
                vector[idxColumn] ^= matrix[idxSource, idxColumn];
            }

            vector[matrix.GetLength(1) - 1] = vector.PhaseProduct(matrix.Row(idxSource));
        }

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