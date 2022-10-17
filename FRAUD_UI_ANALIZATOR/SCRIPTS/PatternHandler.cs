using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace FRAUD_UI_ANALIZATOR.SCRIPTS
{
    public class PatternHandler
    {
        public static readonly Dictionary<string, List<string>> ExtendedData = new();
        public static int[] GenerateFewPatternScalesOneParam(MethodInfo action,int startStreak, int streaks, int step, Dictionary<string, TransactiondData> data, List<string> keys)
        {
            List<string> glAr = new();
            var strArray = new int[streaks];
            for (var i = 0; i < streaks; i += 1)
            {
                glAr.Add((string)action.Invoke(typeof(PatternGetter), new object[] { data, keys, (i * step) + startStreak }));
                strArray[i] = glAr[i].Split(" ").Length;
            }
            if (!ExtendedData.ContainsKey(action.Name)) ExtendedData.Add(action.Name, glAr);
            else { ExtendedData[action.Name] = glAr; }
            return strArray;
        }
        public static int[] GenerateFewPatternScalesTwoParam(MethodInfo action,int startStreak, int streaks, int step, TimeSpan duration, Dictionary<string, TransactiondData> data, List<string> keys)
        {
            List<string> glAr = new();
            var strArray = new int[streaks];
            for (var i = 0; i < streaks; i += 1)
            {
                glAr.Add((string)action.Invoke(typeof(PatternGetter), new object[] { data, keys, (i * step) + startStreak, duration }));
                strArray[i] = glAr[i].Split(" ").Length;
            }
            if (!ExtendedData.ContainsKey(action.Name)) ExtendedData.Add(action.Name, glAr);
            else { ExtendedData[action.Name] = glAr; }
            return strArray;
        }
        protected static string PrintArray(IEnumerable<int> array)
        {
            return array.Aggregate("", (current, t) => current + (t + " "));
        }
        protected static string PrintDoubleArray(int[,] array)
        {
            var answer = string.Empty;
            for (var i = 0; i < array.GetLength(0); i++)
            {
                for (var j = 0; j < array.GetLength(1); j++)
                    answer += array[i,j] + " ";
                answer += "\n";
            }

            return answer;
        }
        protected string PatternMultiply(string firstPattern, string secondPattern) 
        {
            var ar1 = firstPattern.Split(" ");
            var ar2 = secondPattern.Split(" ");
            const string answer = "";
            return (ar1.Length > ar2.Length) switch
            { true => ar1.Where(t => ar2.Any(t1 => t == t1)).Aggregate(answer, (current, t) => current + (t + " ")),
                false => ar2.Where(t => ar1.Any(t1 => t == t1)).Aggregate(answer, (current, t) => current + (t + " ")) };
        }
        protected static int[] AddInt(IReadOnlyList<int> ar, int elem)
        { var array = new int[ar.Count + 1];
            for (var j = 0; j < ar.Count; j++) array[j] = ar[j];
            array[^1] = elem;
            return array; }
        protected static DateTime[] AddTime(IReadOnlyList<DateTime> ar, DateTime elem)
        { var array = new DateTime[ar.Count + 1];
            for (var j = 0; j < ar.Count; j++) array[j] = ar[j];
            array[^1] = elem;
            return array; }
        protected static string[] AddString(IReadOnlyList<string> ar, string elem)
        { var array = new string[ar.Count + 1];
            for (var j = 0; j < ar.Count; j++) array[j] = ar[j];
            array[^1] = elem;
            return array; }
    }
}