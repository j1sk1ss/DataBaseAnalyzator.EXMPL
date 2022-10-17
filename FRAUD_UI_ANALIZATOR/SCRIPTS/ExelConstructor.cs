using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using OfficeOpenXml;

namespace FRAUD_UI_ANALIZATOR.SCRIPTS
{ public static class ExelConstructor
    { private static void ExcelWrite(ExcelPackage excelPackage, Dictionary<string, TransactiondData> data,
            IList<string> patterns, FileDialog path)
        { var fraudSheet = excelPackage.Workbook.Worksheets.Add("Транзакции");
            fraudSheet.Cells[1, 1].Value = "Транзакции";
            fraudSheet.Cells["A1:B1"].Merge = true;
            fraudSheet.Cells[2, 1].Value = "№ паттерна";
            fraudSheet.Cells[2, 2].Value = "Номера транзакций, попадающих под паттерн";
            var countOfPatterns = patterns.Count;
            for (var i = 3; i < countOfPatterns + 3; i++)
            { fraudSheet.Cells[i, 1].Value = i - 2;
                fraudSheet.Cells[i, 2].Value = "[" + patterns[i - 3].Replace(" ", ", ")[4..] + "]"; }
            var infoSheet = excelPackage.Workbook.Worksheets.Add("Фрод паттерны");
            infoSheet.Cells[1, 1].Value = "Выявленные фрод паттерны";
            infoSheet.Cells[2, 1].Value = "№ паттерна"; 
            infoSheet.Cells[2, 3].Value = "Описание";
            infoSheet.Cells["A1:D1"].Merge = true;
            for (var j = 2; j < countOfPatterns + 3; j++)
            { infoSheet.Cells[$"A{j}:B{j}"].Merge = true;
                if (j + 1 < countOfPatterns + 3) infoSheet.Cells[j + 1, 1].Value = j - 1;
                infoSheet.Cells[$"C{j}:D{j}"].Merge = true; }
        }
        public static void SaveToExcel(Dictionary<string, TransactiondData> transactionsData, IList<string> excel)
        { var folderBrowser = new OpenFileDialog
            { ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                Filter = "txt files (*.txt)|*.txt|excel files (*.xlsx)|*.xlsx",
                FileName = "Report" };
            if (folderBrowser.ShowDialog() != true) return;
            if (folderBrowser.FilterIndex == 2)
            { try { using var excelPackage = new ExcelPackage();
                    ExcelWrite(excelPackage, transactionsData, excel, folderBrowser);
                     excelPackage.SaveAs(folderBrowser.FileName);
                } catch (Exception e)
                { MessageBox.Show($"Error with: {e}", "Error with analysing!", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw; }
            }else { File.WriteAllLines(folderBrowser.FileName,excel); }
        }
    }
}