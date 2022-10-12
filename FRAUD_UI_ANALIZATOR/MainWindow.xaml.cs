using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;
using OfficeOpenXml;
using Path = System.IO.Path;

namespace FRAUD_UI_ANALIZATOR
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private readonly JsonParser _jsonParser = new();
        private Dictionary<string, TransactiondData> _transactionsData = new();
        private readonly ExelConstructor _constructor = new();
        private readonly PatternGetter _patternGetter = new();
        private void LoadJson(object sender, RoutedEventArgs e)
        {
            var childhood = new OpenFileDialog
            { Filter = "JSON Files (*.json)|*.json",
                FilterIndex = 1,
                Multiselect = true };
            if (childhood.ShowDialog() != true) return;
            var path = childhood.FileName;
                JsonLoadButton.Content = path;
                _transactionsData = _jsonParser.StartParse(path);
        }
        private void PatternGet(object sender, RoutedEventArgs routedEventArgs)
        {
            var folderBrowser = new OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Report"
            };
            var path = string.Empty;
            if (folderBrowser.ShowDialog() == true)
            {
                path = Path.GetDirectoryName(folderBrowser.FileName);
            }
            var lst = _constructor.InitList(_transactionsData, _jsonParser.KeyList);
                if (TP.IsChecked == true) lst.Add(_patternGetter.GetTimePattern(_transactionsData, _jsonParser.KeyList, 6, 1));
                    if (SAP.IsChecked == true) lst.Add(_patternGetter.GetSmallAmountPattern(_transactionsData, _jsonParser.KeyList, 10000));
                using var excelPackage = new ExcelPackage();
            _constructor.ExcelWrite(excelPackage, _transactionsData, lst);
            excelPackage.SaveAs(path+@"\Report.xlsx");
        }
    }
}