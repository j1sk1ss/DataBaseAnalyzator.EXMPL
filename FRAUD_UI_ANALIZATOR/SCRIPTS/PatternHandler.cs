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
        { List<string> glAr = new();
            var strArray = new int[streaks];
            for (var i = 0; i < streaks; i += 1)
            { glAr.Add((string)action.Invoke(typeof(PatternGetter), new object[] { data, keys, (i * step) + startStreak }));
                strArray[i] = glAr[i].Split(" ").Length; }
            if (!ExtendedData.ContainsKey(action.Name)) ExtendedData.Add(action.Name, glAr);
            else { ExtendedData[action.Name] = glAr; }
            return strArray; }
        public static int[] GenerateFewPatternScalesTwoParam(MethodInfo action,int startStreak, int streaks, int step, TimeSpan duration, Dictionary<string, TransactiondData> data, List<string> keys)
        { List<string> glAr = new();
            var strArray = new int[streaks];
            for (var i = 0; i < streaks; i += 1)
            { glAr.Add((string)action.Invoke(typeof(PatternGetter), new object[] { data, keys, (i * step) + startStreak, duration }));
                strArray[i] = glAr[i].Split(" ").Length; }
            if (!ExtendedData.ContainsKey(action.Name)) ExtendedData.Add(action.Name, glAr);
            else { ExtendedData[action.Name] = glAr; }
            return strArray; }
        public static string PatternMultiply(List<string> patterns)
        { var answer = "";
            foreach (var line in patterns.Select(t1 => t1.Split(" ")))
            { if (line.Length <= 2)
                { MessageBox.Show("Некоторые паттерны не выявили подозрительные транзакции из-за чего учитываться не будут!", "Предупреждение!",
                        MessageBoxButton.OK);
                    continue; }
                answer = line.Aggregate(answer, (current1, t) => patterns.TakeWhile(t2 => t2.Contains(t)).
                    Where((t2, k) => k == patterns.Count - 1).Aggregate(current1, (current, t2) => current + (t + " "))); }
            return answer; }
        protected static DateTime[] AddTime(IReadOnlyList<DateTime> ar, DateTime elem)
        { var array = new DateTime[ar.Count + 1];
            for (var j = 0; j < ar.Count; j++) array[j] = ar[j];
            array[^1] = elem;
            return array; }
    }
}