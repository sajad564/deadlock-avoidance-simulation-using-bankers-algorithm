using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deadLockAvoidance
{
    public static class Extensions
    {
        public static void ShowChart(string[] chartHeaders, string[][] data)
        {
            for (int i = 0; i < chartHeaders.Length; i++)
            {
                StringBuilder builer = new StringBuilder();
                builer.Append(chartHeaders[i]);
                builer.Append(new string(' ', 10));
                Console.Write(builer.ToString());
                if (i == chartHeaders.Length - 1)
                    Console.WriteLine();
            }
            for (int i = 0; i < data.GetLength(0); i++)
            {

                for (int j = 0; j < data[i].Length; j++)
                {
                    StringBuilder builer = new StringBuilder();
                    builer.Append(data[i][j]);
                    builer.Append(new string(' ', 20));
                    Console.Write(builer.ToString());
                }
                Console.WriteLine();
            }
        }
        public static Dictionary<int, T> ConvertArrayToDic<T>(this T[] arr)
        {
            return arr.Select((value, index) => new { value, index }).ToDictionary(pair => pair.index, pair => pair.value);
        }
        public static bool IsNull<T, V>(this KeyValuePair<T, V> pair)
        {
            return pair.Equals(new KeyValuePair<T, V>());
        }
        public static void Add(this int[,] matrix, int[] arr, int rowPosition)
        {
            if (matrix.GetLength(1) != arr.Length)
                throw new InvalidOperationException("can not attach such a vector to current matrix as a new row");
            for (int i = 0; i < arr.Length; i++)
            {
                matrix[rowPosition, i] = arr[i];
            }
        }
        public static void WriteMatrixToConsole(this int[,] matrix)
        {
            Console.WriteLine('[');
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                Console.Write('[');
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(j == matrix.GetLength(1) - 1 ? matrix[i, j] : $"{matrix[i, j]},");
                }
                Console.WriteLine(i == matrix.GetLength(0) - 1 ? ']' : "],");
            }
            Console.WriteLine(']');
        }
        public static void WriteArrayToConsole(this int[] input)
        {
            Console.Write("[");
            for (int i = 0; i <= input.Length - 1; i++)
            {
                if (i == input.Length - 1)
                {
                    Console.Write(input[i]);
                    continue;
                }
                Console.Write($"{input[i]},");
            }
            Console.WriteLine("]");
        }
        public static int[,] Subtract(int[,] operand1, int[,] operand2)
        {
            if (operand1.GetLength(0) != operand2.GetLength(0) || operand1.GetLength(1) != operand2.GetLength(1))
            {
                throw new InvalidOperationException("can not compare two or more array with different dimmension!");
            }
            var result = new int[operand1.GetLength(0), operand1.GetLength(1)];
            for (var i = 0; i < operand1.GetLength(0); i++)
            {
                for (var j = 0; j < operand1.GetLength(1); j++)
                    result[i, j] = operand1[i, j] - operand2[i, j];
            }
            return result;
        }
        public static int[] Sum(int[] input1, int[] input2)
        {
            var result = new int[input1.Length];
            for (int i = 0; i < input1.Length; i++)
                result[i] = input1[i] + input2[i];
            return result;
        }
        public static T[] GetRow<T>(this T[,] matrix, int row)
        {
            var rowLength = matrix.GetLength(1);
            var rowVector = new T[rowLength];

            for (var i = 0; i < rowLength; i++)
                rowVector[i] = matrix[row, i];

            return rowVector;
        }
        public static bool IsLessThanEqual(this int[] input1, int[] input2)
        {
            for (int i = 0; i < input1.Length; i++)
            {
                if (input1[i] > input2[i])
                    return false;
            }
            return true;
        }
    }
}
