using System;
using System.Collections.Generic;
using System.Windows;
using FRAUD_UI_ANALIZATOR.SCRIPTS;
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
        private void LoadJson(object sender, RoutedEventArgs e)
        { try
            { var childhood = new OpenFileDialog
                { Filter = "JSON Files (*.json)|*.json",
                    FilterIndex = 1,
                    Multiselect = true };
                if (childhood.ShowDialog() != true) return;
                var path = childhood.FileName;
                DbPathLabel.Content = Path.GetFileName(path);
                _transactionsData = _jsonParser.StartParse(path); }
            catch (Exception exception) {
                DbPathLabel.Content = "";
                MessageBox.Show($"Error with: {exception}", "Error with Parsing!", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        private void PatternGet(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_transactionsData.Count < 1)
            {
                MessageBox.Show("Load Json before start!", "Pattern getting error!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            var folderBrowser = new OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Report"
            };
                var path = string.Empty;
                    if (folderBrowser.ShowDialog() == true) {
                        path = Path.GetDirectoryName(folderBrowser.FileName); }
                try {
                    var lst = ExelConstructor.InitList(_transactionsData, _jsonParser.KeyList);
                    PatternInit(lst);
                    using var excelPackage = new ExcelPackage();
                    ExelConstructor.ExcelWrite(excelPackage, _transactionsData, lst);
                    excelPackage.SaveAs(path + @"\Report.xlsx"); }
                catch (Exception e)
                { MessageBox.Show($"Error with: {e}", "Error with Parsing!", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw;
                }
        }

        private void PatternInit(ICollection<string> lst)
        {
            //if (Tp.IsChecked == true) lst.Add(PatternGetter.GetTimePattern(_transactionsData, _jsonParser.KeyList, 6, 1));
        }
    }
}