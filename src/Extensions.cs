using System.Collections.Generic;
using System.Linq;

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

        internal static string RowToString<T>(this T[,] matrix, int idx) =>
            "[" + string.Join(", ", matrix.Row(idx).Select(element => element.ToString())) + "]";
        
        internal static string MatrixToString<T>(this T[,] matrix) =>
        "[\n\t" + string.Join("\n\t", Enumerable.Range(0, matrix.GetLength(0)).Select(idx => matrix.RowToString(idx))) + "\n]";
    }

}