using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
namespace FRAUD_UI_ANALIZATOR.SCRIPTS
{
    public class PatternGetter : PatternHandler
    {
         public static string GetTimePattern(Dictionary<string, TransactiondData> data,List<string> keys, TimeSpan endTime, TimeSpan startTime) { 
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                if (data[$"{keys[i]}"].Date.TimeOfDay < endTime && data[$"{keys[i]}"].Date.TimeOfDay > startTime)
                    answer += keys[i] + " ";
            return answer; }
        public static string GetSmallAmountPattern(Dictionary<string, TransactiondData> data,List<string> keys, double amount) { 
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                if (data[$"{keys[i]}"].Amount < amount)
                    answer += keys[i] + " ";
            return answer; }
        public static string GetBigAmountPattern(Dictionary<string, TransactiondData> data,List<string> keys, double amount) { 
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                if (data[$"{keys[i]}"].Amount > amount)
                    answer += keys[i] + " ";
            return answer; }
        public static string GetPassportValidPattern(Dictionary<string, TransactiondData> data,List<string> keys, int days) {
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                if (data[$"{keys[i]}"].PassportValidTo < data[$"{keys[i]}"].Date - new TimeSpan(days))
                    answer += keys[i] + " ";
            return answer; 
        }
        public static string GetAccountValidPattern(Dictionary<string, TransactiondData> data,List<string> keys, int days) {
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                if (data[$"{keys[i]}"].AccountValidTo < data[$"{keys[i]}"].Date - new TimeSpan(days))
                    answer += keys[i] + " ";
            return answer; 
        }
        public static string GetTimeDurationPattern(Dictionary<string, TransactiondData> data,List<string> keys, int copyCount, TimeSpan duration) { 
            var answer = string.Empty;
            for (var i = 0; i < data.Count; i++)
            { var times = Array.Empty<DateTime>();
                for (var j = 0; j < data.Count; j++) if (data[$"{keys[i]}"].Date.Day == data[$"{keys[j]}"].Date.Day
                                                         && data[$"{keys[i]}"].Date.Hour == data[$"{keys[j]}"].Date.Hour
                                                         && data[$"{keys[i]}"].Passport == data[$"{keys[j]}"].Passport) 
                    times = AddTime(times, data[$"{keys[j]}"].Date);
                if (times.Length <= 1) continue;
                var check = new double[times.Length - 1];
                    for (var j = 0; j < check.Length; j++)
                        if (j + 1 < times.Length) check[j] = (times[j + 1] - times[j]).Duration().TotalMinutes;
                    var count = 0;
                    for (var j = 1; j < check.Length; j++)
                    { if (!(Math.Abs(check[j] - check[j - 1]) < duration.Minutes) ) continue;
                        if (++count <= copyCount) continue;
                        answer += keys[i] + " ";
                        break; }
            }
            return answer; }
        public static string GetDifferentCityPattern(Dictionary<string, TransactiondData> data,List<string> keys, int cities) {
            var answer = string.Empty;
            for (var i = 0; i < data.Count; i++) {
                var tmp = data.Where((t, j) => i != j && data[$"{keys[i]}"].Passport == data
                    [$"{keys[j]}"].Passport && data[$"{keys[i]}"].City != data[$"{keys[j]}"].City).Aggregate(string.Empty, 
                    (current, t) => current + keys[i] + " ");
                if (tmp.Split(" ").Length >= cities) answer += keys[i] + " "; } 
            return answer; }
        public static string GetMultiCardPattern(Dictionary<string, TransactiondData> data, List<string> keys, int copyCount) 
        {
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++) {
                var count = 0;
                answer += keys[i] + " ";
                for (var j = 0; j < data.Count; j++) {
                    if (i == j) continue;
                    if (data[$"{keys[i]}"].Passport == data[$"{keys[j]}"].Passport && data[$"{keys[i]}"].Card != data
                            [$"{keys[j]}"].Card) if (++count < copyCount) continue;
                    answer += keys[i] + " ";
                    break; }
            }
            return answer; 
        }
        public static string GetMultiPosPatter(Dictionary<string, TransactiondData> data, List<string> keys, int copyCount)
        { var answer = string.Empty; 
            var posCount = 0;
            for (var i = 0; i < data.Count; i++)
            { if (!data.Where((t, j) => i != j && (data[$"{keys[i]}"].Address ==
                                                   data[$"{keys[j]}"].Address && data[$"{keys[i]}"].Terminal != data[$"" +
                                                       $"{keys[j]}"].Terminal &&
                                                   data[$"{keys[i]}"].TerminalType == "POS" && data[$"{keys[i]}"].Passport
                                                   != data[$"{keys[j]}"].Passport))
                      .Any(t => ++posCount >= copyCount)) continue;
                answer += keys[i] + " ";
                posCount = 0; }
            return answer; 
        }
        public static string GetMultiPassportCard(Dictionary<string, TransactiondData> data, List<string> keys, int copyCount)
        { var answer = string.Empty;
            for (var i = 0; i < data.Count; i++) {
                var count = 0;
                answer = data.Where((t, j) => i != j && (data[$"{keys[i]}"].Card == 
                        data[$"{keys[j]}"].Card && data[$"{keys[i]}"].Passport != data[$"{keys[j]}"].Passport)).
                    Where(t => ++count > copyCount).Aggregate(answer, (current, 
                        t) => current + (keys[i] + " "));
                count = 0; }
            return answer; }
        public static string GetOldersPattern(Dictionary<string, TransactiondData> data, List<string> keys, int age, bool type)
        { var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                switch (type)
                { case false:
                        if (data[$"{keys[i]}"].Date.Year - data[$"{keys[i]}"].DateOfBirth.Year > age && data[$"{keys[i]}"].OperType == "Снятие")
                            answer += keys[i] + " ";
                        break;
                    default:
                        if (data[$"{keys[i]}"].Date.Year - data[$"{keys[i]}"].DateOfBirth.Year > age)
                            answer += keys[i] + " ";
                        break; }
            return answer; }
        public static string GetYoungerPattern(Dictionary<string, TransactiondData> data, List<string> keys, int age)
        { var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                if (data[$"{keys[i]}"].Date.Year - data[$"{keys[i]}"].DateOfBirth.Year < age)
                    answer += keys[i] + " ";
            return answer; }
        public static string GetCancelledStreakPattern(Dictionary<string, TransactiondData> data, List<string> keys, int cancelledLimit)
        { 
            var answer = string.Empty;
            var count = 0;
            for (var i = 0; i < data.Count; i++)
            { if (data[$"{keys[i]}"].OperType == "Успешно") continue;
                for (var j = 0; j < data.Count; j++)
                    if (data[$"{keys[i]}"].Card == data[$"{keys[j]}"].Card &&
                        data[$"{keys[j]}"].OperResult != "Успешно")
                    { count++;
                        if (count <= cancelledLimit) continue;
                        answer += keys[i] + " ";
                        break; }
                    else if (data[$"{keys[i]}"].Card == data[$"{keys[j]}"].Card && data[$"{keys[j]}"].OperResult == "Успешно") count = 0; }
            return answer; }
        [SuppressMessage("ReSharper.DPA", "DPA0003: Excessive memory allocations in LOH", MessageId = "type: System.String")]
        public static string GetManyTransactionsPattern(Dictionary<string, TransactiondData> data, List<string> keys, int time, TimeSpan duration)
        { var answer = string.Empty;
            var count = 0;
            for (var i = 0; i < data.Count; i++) {
                if (data.Where((t, j) => data[$"{keys[i]}"].Passport == 
                        data[$"{keys[j]}"].Passport && (data[$"{keys[i]}"].Date - data[$"{keys[j]}"].Date).
                        Duration() <= duration).Any(t => ++count > time)) {
                    answer += keys[i] + " "; }
                count = 0; }
            return answer; }  
    }
}