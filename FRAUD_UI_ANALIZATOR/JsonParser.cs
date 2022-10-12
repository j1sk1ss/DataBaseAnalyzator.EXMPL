using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace FRAUD_UI_ANALIZATOR
{
    public class JsonParser
    {
        public List<string> KeyList = new List<string>();
        public Dictionary<string, TransactiondData> StartParse(string path)
        {
            var datalist = JsonConvert.DeserializeObject
                <Dictionary<string, Dictionary<string, TransactiondData>>>(File.ReadAllText(path));
            if (datalist == null) return null;
            var trn = datalist["transactions"];
            KeyList = new List<string>(trn.Keys);
            return trn;
        }
    }
    public class TransactiondData
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        [JsonProperty("card")]
        public string Card { get; set; }
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("account_valid_to")]
        public DateTime AccountValidTo { get; set; }
        [JsonProperty("client")]
        public string Client { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("patronymic")]
        public string Patronymic { get; set; }
        [JsonProperty("date_of_birth")]
        public DateTime DateOfBirth { get; set; }
        [JsonProperty("passport")]
        public string Passport { get; set; }
        [JsonProperty("passport_valid_to")]
        public DateTime PassportValidTo { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("oper_type")]
        public string OperType { get; set; }
        [JsonProperty("amount")]
        public double Amount { get; set; }
        [JsonProperty("oper_result")]
        public string OperResult { get; set; }
        [JsonProperty("terminal")]
        public string Terminal { get; set; }
        [JsonProperty("terminal_type")]
        public string TerminalType { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
    }
}