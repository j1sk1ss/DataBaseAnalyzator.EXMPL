using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FRAUD_UI_ANALIZATOR.SCRIPTS
{
    public class PatternGetter : PatternHandler
    {
        public static string GetTimePattern(Dictionary<string, TransactiondData> data,List<string> keys, int endTime, int startTime)
        { 
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                if (data[$"{keys[i]}"].Date.Hour < endTime && data[$"{keys[i]}"].Date.Hour > startTime)
                    answer += keys[i] + " ";
            return answer; 
        }

        public  string GetSmallAmountPattern(Dictionary<string, TransactiondData> data,List<string> keys, double amount) { 
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                if (data[$"{keys[i]}"].Amount < amount)
                    answer += keys[i] + " ";
            return answer;  
        }
        public  string GetBigAmountPattern(Dictionary<string, TransactiondData> data,List<string> keys, double amount) { 
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                if (data[$"{keys[i]}"].Amount > amount)
                    answer += keys[i] + " ";
            return answer; 
        }
        public  string GetPassportValidPattern(Dictionary<string, TransactiondData> data,List<string> keys) {
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                if (data[$"{keys[i]}"].PassportValidTo < data[$"{keys[i]}"].Date)
                    answer += keys[i] + " ";
            return answer; 
        }
        public  string GetAccountValidPattern(Dictionary<string, TransactiondData> data,List<string> keys) {
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                if (data[$"{keys[i]}"].AccountValidTo < data[$"{keys[i]}"].Date)
                    answer += keys[i] + " ";
            return answer; 
        }
        public  string GetTimeDurationPattern(Dictionary<string, TransactiondData> data,List<string> keys) { 
            var answer = string.Empty; 
            var times = Array.Empty<int>();
            for (var i = 0; i < data.Count; i++) {
                times = data.Where((t, j) => i != j && (data[$"{keys[i]}"].Card == data[$"{keys[j]}"].Card 
                                                        && data[$"{keys[i]}"].Date == data[$"{keys[j]}"].Date)).Aggregate(times,
                    (current, t) => AddInt(current, data[$"{keys[i]}"].Date.Hour));
                if (times.Length == 0) break;
                    var check = new int[times.Length - 1];
                    for (var j = 0; j < check.Length; j++) if (j + 1 < times.Length) check[j] = Math.Abs(times[j + 1] - times[j]);
                    if (check.ToHashSet().Count == 1) answer += keys[i] + " ";
            }
            return answer; }
        public  string GetDifferentCityPattern(Dictionary<string, TransactiondData> data,List<string> keys, int cities) {
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++) {
                var tmp = data.Where((t, j) => i != j && data[$"{keys[i]}"].Card == data
                    [$"{keys[j]}"].Card && data[$"{keys[i]}"].City != data[$"{keys[j]}"].City).Aggregate(string.Empty, 
                    (current, t) => current + keys[i] + " ");
                if (tmp.Split(" ").Length >= cities) answer += keys[i] + " "; } 
            return answer; }
        public  string GetMultiCardPattern(Dictionary<string, TransactiondData> data, List<string> keys) 
        {
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++) {
                    var count = 0;
                    answer += keys[i] + " ";
                    for (var j = 0; j < data.Count; j++) {
                        if (i == j) continue;
                        if (data[$"{keys[i]}"].Passport == data[$"{keys[j]}"].Passport && data[$"{keys[i]}"].Card != data
                                [$"{keys[j]}"].Card) {
                            count++;
                            answer += keys[i] + " "; }
                        if (count > 5) break; }
                    answer += "\n"; }
            return answer; 
        }
        public  string GetMultiPosPatter(Dictionary<string, TransactiondData> data, List<string> keys)
        {
            var answer = string.Empty; 
            var posCount = 0;
            for (var i = 0; i < data.Count; i++) {
                for (var j = 0; j < data.Count; j++) {
                    if (i == j) continue;
                    if (data[$"{keys[i]}"].Address != data[$"{keys[j]}"].Address || data[$"{keys[i]}"].Terminal == data[$"" +
                            $"{keys[j]}"].Terminal ||
                        data[$"{keys[i]}"].TerminalType != "POS" || data[$"{keys[i]}"].Passport == data[$"{keys[j]}"].
                            Passport) continue;
                    posCount++;
                    if (posCount < 5) continue;
                    answer += keys[i] + " ";
                    posCount = 0;
                    break; } 
            }
            return answer; 
        }
        public  string GetMultiPassportCard(Dictionary<string, TransactiondData> data, List<string> keys)
        {
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++) {
                for (var j = 0; j < data.Count; j++) {
                    if (i == j) continue;
                    if (data[$"{keys[i]}"].Card == data[$"{keys[j]}"].Card && data[$"{keys[i]}"].Passport != data[$"{keys[j]}"].
                            Passport) answer += keys[i] + " ";
                    break; } 
            }
            return answer; 
        }
        public  string GetOldersPattern(Dictionary<string, TransactiondData> data, List<string> keys, int age)
        {
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                if (data[$"{keys[i]}"].Date.Year - data[$"{keys[i]}"].DateOfBirth.Year > age)
                    answer += keys[i] + " ";
            return answer; 
        }
        public  string GetYoungerPattern(Dictionary<string, TransactiondData> data, List<string> keys, int age)
        {
            var answer = string.Empty; 
            for (var i = 0; i < data.Count; i++)
                if (data[$"{keys[i]}"].Date.Year - data[$"{keys[i]}"].DateOfBirth.Year < age)
                    answer += keys[i] + " ";
            return answer; 
        }
        public  string GetCancelledPattern(Dictionary<string, TransactiondData> data, List<string> keys, int cancelledLimit)
        { 
            var answer = string.Empty;
            var count = 0;
            for (var i = 0; i < data.Count; i++)
            { if (data[$"{keys[i]}"].OperType == "Успешно") continue;
                    for (var j = 0; j < data.Count; j++)
                        if (data[$"{keys[i]}"].Card == data[$"{keys[j]}"].Card &&
                            data[$"{keys[j]}"].OperResult != "Успешно")
                        {
                            count++;
                            if (count <= cancelledLimit) continue;
                            answer += keys[i] + " ";
                            break;
                        }
                        else if (data[$"{keys[i]}"].Card == data[$"{keys[j]}"].Card && data[$"{keys[j]}"].OperResult == "Успешно") count = 0; }
            return answer;
            
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
    }
}