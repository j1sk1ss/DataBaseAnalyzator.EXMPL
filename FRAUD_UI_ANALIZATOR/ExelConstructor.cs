using System.Collections.Generic;
using OfficeOpenXml;

namespace FRAUD_UI_ANALIZATOR
{
    public class ExelConstructor
    {
        public static List<string> InitList(Dictionary<string, TransactiondData> data, List<string> keys)
        {
            var patternsGetter = new List<string>();
            return patternsGetter;
        }
        public static void ExcelWrite(ExcelPackage excelPackage, Dictionary<string, TransactiondData> data,
            IList<string> patterns)
        {
            var fraudSheet = excelPackage.Workbook.Worksheets.Add("Транзакции");
            fraudSheet.Cells[1, 1].Value = "Транзакции";
            fraudSheet.Cells["A1:B1"].Merge = true;
            fraudSheet.Cells[2, 1].Value = "№ паттерна";
            fraudSheet.Cells[2, 2].Value = "Номера транзакций, попадающих под паттерн";
            var countOfPatterns = patterns.Count;
            for (var i = 3; i < countOfPatterns + 3; i++)
            {
                fraudSheet.Cells[i, 1].Value = i - 2;
                fraudSheet.Cells[i, 2].Value = "[" + patterns[i - 3].Replace(" ", ", ") + "]";
            }
            var infoSheet = excelPackage.Workbook.Worksheets.Add("Фрод паттерны");
            infoSheet.Cells[1, 1].Value = "Выявленные фрод паттерны";
            infoSheet.Cells[2, 1].Value = "№ паттерна";
            infoSheet.Cells[2, 3].Value = "Описание";
            infoSheet.Cells["A1:D1"].Merge = true;
            for (var j = 2; j < countOfPatterns + 3; j++)
            {
                infoSheet.Cells[$"A{j}:B{j}"].Merge = true;
                if (j + 1 < countOfPatterns + 3) infoSheet.Cells[j + 1, 1].Value = j - 1;
                infoSheet.Cells[$"C{j}:D{j}"].Merge = true;
            }
        }
    }
}