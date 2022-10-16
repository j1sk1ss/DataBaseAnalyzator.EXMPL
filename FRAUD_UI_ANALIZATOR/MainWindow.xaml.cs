using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FRAUD_UI_ANALIZATOR.SCRIPTS;
using LiveCharts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Microsoft.Win32;
using OfficeOpenXml;
namespace FRAUD_UI_ANALIZATOR
{ public partial class MainWindow
    {         private readonly List<string> _excel = new();
        public MainWindow()
        { InitializeComponent(); }
        private readonly JsonParser _jsonParser = new();
        private Dictionary<string, TransactiondData> _transactionsData = new();
        private void LoadJson(object sender, RoutedEventArgs e)
        { try
            { var childhood = new OpenFileDialog
                { Filter = "JSON Files (*.json)|*.json",
                    FilterIndex = 1,
                    Multiselect = true };
                if (childhood.ShowDialog() != true) return;
                var fileName = childhood.FileName;
                DbPathLabel.Content = System.IO.Path.GetFileName(fileName);
                _transactionsData = _jsonParser.StartParse(fileName); }
            catch (Exception exception) { DbPathLabel.Content = "";
                MessageBox.Show($"Error with: {exception}", "Error with Parsing!", MessageBoxButton.OK, MessageBoxImage.Error); } }

        [SuppressMessage("ReSharper.DPA", "DPA0003: Excessive memory allocations in LOH", MessageId = "type: System.String")]
        private void PatternGet(object sender, RoutedEventArgs routedEventArgs)
        { if (_transactionsData.Count < 1)
            { MessageBox.Show("Load Json before start!", "Pattern getting error!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return; }
            _excel.Clear();
            PatternInit(_excel);
            Chart.Series.Clear();
            SaveButton.Visibility = Visibility.Visible;
            foreach (var t in _excel)
                Chart.Series.Add(new PieSeries
                { Title = $"{t.Split(" ")[0]}",
                    Values = new ChartValues<int> { t.Split(" ").Length - 1 } });
            DataContext = this; }

        private void SaveExcel(object sender, RoutedEventArgs routedEventArgs)
        {
            ExelConstructor.SaveToExcel(_transactionsData, _excel);
        }
        private const string Path = "pack://application:,,,";
        private void ValueChanger(object sender, RoutedEventArgs routedEventArgs)
        { var obj = sender as FrameworkElement;
            Dictionary<string, Image> checkers = new()
            { {"b_1", StrangeTime},
                {"b_2", SmallTransaction},
                {"b_3", BigTransaction},
                {"b_4", NotValidPassport},
                {"b_5", NotValidAccount},
                {"b_6", DurationPattern},
                {"b_7", DifferentCities},
                {"b_8", TooManyCards},
                {"b_9", TooManyPos},
                {"b_10", TooManyPassports},
                {"b_11", Older},
                {"b_12", CancelledStreak},
                {"b_13", ManyTransactions}, 
                {"b_14", ManyTerminalsInTime} };
            if (obj == null) return;
            var img = checkers[$"{obj.Name}"];
            img.Source = img.Source.ToString() == $"{Path}/IMG/patterninactive_button.png" ?
                new BitmapImage(new Uri(@"/IMG/pattern_button.png", UriKind.Relative)) : 
                new BitmapImage(new Uri(@"/IMG/patterninactive_button.png", UriKind.Relative)); }
        private void TransactionType(object sender, RoutedEventArgs routedEventArgs) {
            Type.Source = Type.Source.ToString() == $"{Path}/IMG/unchecked_checkbox.png" ?
                new BitmapImage(new Uri(@"/IMG/checked_checkbox_1.png", UriKind.Relative)) : 
                new BitmapImage(new Uri(@"/IMG/unchecked_checkbox.png", UriKind.Relative)); }
        private void OpenCharts(object sender, RoutedEventArgs routedEventArgs)
        {   if (_excel.Count >= 1) SaveButton.Visibility = Visibility.Hidden;
            foreach (var obj in _buttons)
            { obj.Source =
                new BitmapImage(new Uri($@"IMG/Graph_inactive_tab.png", UriKind.Relative)); }
            Tabs.Source = new BitmapImage(new Uri(@"/IMG/tabs2.png", UriKind.Relative));
            Charts.Visibility = Visibility.Visible;
            MoreAboutChart.Visibility = Visibility.Hidden;
            SavedCharts.Visibility = Visibility.Visible; }
        private void OpenMenu(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_excel.Count >= 1) SaveButton.Visibility = Visibility.Visible;
            Tabs.Source = new BitmapImage(new Uri(@"/IMG/tabs.png", UriKind.Relative));
            Charts.Visibility = Visibility.Hidden;
            MoreAboutChart.Visibility = Visibility.Hidden;
            SavedCharts.Visibility = Visibility.Hidden; }
        private void PatternInit(ICollection<string> lst) 
        { try
            { if (StrangeTime.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("GTP " + PatternGetter.GetTimePattern(_transactionsData, _jsonParser.KeyList, 
                        TimeSpan.Parse(EndTimeTp.Text), TimeSpan.Parse(StartTimeTp.Text)));
                if (SmallTransaction.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("SAP " + PatternGetter.GetSmallAmountPattern(_transactionsData, _jsonParser.KeyList, 
                        int.Parse(SmallAmount.Text)));
                if (BigTransaction.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("BAP " + PatternGetter.GetBigAmountPattern(_transactionsData, _jsonParser.KeyList, 
                        int.Parse(BigAmount.Text)));
                if (NotValidPassport.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("PVP " + PatternGetter.GetPassportValidPattern(_transactionsData, _jsonParser.KeyList, 
                        int.Parse(PassportDays.Text)));
                if (NotValidAccount.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("AVP " + PatternGetter.GetAccountValidPattern(_transactionsData, _jsonParser.KeyList, 
                        int.Parse(AccountDays.Text)));
                if (DurationPattern.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("TDP " + PatternGetter.GetTimeDurationPattern(_transactionsData, _jsonParser.KeyList, 
                        int.Parse(DurationStreak.Text), TimeSpan.Parse(DurationInterval.Text)));
                if (DifferentCities.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("DCP " + PatternGetter.GetDifferentCityPattern(_transactionsData, _jsonParser.KeyList, 
                        int.Parse(CitiesCount.Text)));
                if (TooManyCards.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("MCP " + PatternGetter.GetMultiCardPattern(_transactionsData, _jsonParser.KeyList, 
                        int.Parse(CardCount.Text)));
                if (TooManyPos.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("MPP " + PatternGetter.GetMultiPosPatter(_transactionsData, _jsonParser.KeyList, 
                        int.Parse(PosCount.Text)));
                if (TooManyPassports.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("MPC " + PatternGetter.GetMultiPassportAccount(_transactionsData, _jsonParser.KeyList, 
                        int.Parse(PassportCount.Text)));
                if (Older.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("GOP " + PatternGetter.GetOldersPattern(_transactionsData, _jsonParser.KeyList, 
                        int.Parse(Age.Text), Type.Source.ToString() == $"{Path}/IMG/unchecked_checkbox.png"));
                if (CancelledStreak.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("CSP " + PatternGetter.GetCancelledStreakPattern(_transactionsData, _jsonParser.KeyList, 
                        int.Parse(StreakCount.Text))); 
                if (ManyTransactions.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("MTP " + PatternGetter.GetManyTransactionsPattern(_transactionsData, _jsonParser.KeyList, 
                        int.Parse(DurationStreak.Text), TimeSpan.Parse(TimeTransaction.Text)));
                if (ManyTerminalsInTime.Source.ToString() == $"{Path}/IMG/pattern_button.png") 
                    lst.Add("MTP " + PatternGetter.GetManyTransactionsPattern(_transactionsData, _jsonParser.KeyList, 
                        int.Parse(TransactionsTimeCount.Text), TimeSpan.Parse(TimeTransactionTime.Text)));
            }
            catch (Exception e) {
                MessageBox.Show($"Error with: {e}", "Pattern getting error!", MessageBoxButton.OK); }
        }
        private void InformationAboutOnePPattern(object sender, ChartPoint chartPoint)
        {
            if (SaveButton.Visibility == Visibility.Visible) SaveButton.Visibility = Visibility.Hidden;
            if (_cartesianCharts.Keys.Contains(chartPoint.SeriesView.Title))
            { MoreAboutChart.Visibility = Visibility.Visible;
                GetSaved(chartPoint.SeriesView.Title);
                return; }
            const string str = "SAP . BAP . PVP . AVP . DCP . MCP . MPP . MPC . CSP";
            if (!str.Contains(chartPoint.SeriesView.Title)) return;
            Dictionary<string, string> patternsByName = new()
            { {"SAP", "GetSmallAmountPattern"},
                {"BAP", "GetBigAmountPattern"},
                {"PVP", "GetPassportValidPattern"},
                {"AVP", "GetAccountValidPattern"},
                {"DCP", "GetDifferentCityPattern"},
                {"MCP", "GetMultiCardPattern"},
                {"MPP", "GetMultiPosPatter"},
                {"MPC", "GetMultiPassportAccount"},
                {"CSP", "GetCancelledStreakPattern"} };
            try
            { var array = PatternHandler.GenerateFewPatternScales(typeof(PatternGetter).GetMethod(patternsByName[chartPoint.SeriesView.Title]), 
                    int.Parse(StartValue.Text), int.Parse(StreakCount.Text), int.Parse(Step.Text), _transactionsData, _jsonParser.KeyList);
                CartesianChart.Series = new SeriesCollection
                { new LineSeries {
                        Title = chartPoint.SeriesView.Title,
                        Values = array.AsChartValues() }
                };
                AddToCash(chartPoint.SeriesView.Title, CartesianChart);
                DataContext = this; }
            catch (Exception e) {
                MessageBox.Show(e.ToString(), "error", MessageBoxButton.OK, MessageBoxImage.Error);
                return; }
            MoreAboutChart.Visibility = Visibility.Visible; }
        private readonly Dictionary<string, string> _patternsByName = new()
        { {"SAP", "GetSmallAmountPattern"},
            {"BAP", "GetBigAmountPattern"},
            {"PVP", "GetPassportValidPattern"},
            {"AVP", "GetAccountValidPattern"},
            {"DCP", "GetDifferentCityPattern"},
            {"MCP", "GetMultiCardPattern"},
            {"MPP", "GetMultiPosPatter"},
            {"MPC", "GetMultiPassportAccount"},
            {"CSP", "GetCancelledStreakPattern"} };
        private void Regenerate(object sender, RoutedEventArgs routedEventArgs)
        {
            try {
                var array = PatternHandler.GenerateFewPatternScales(typeof(PatternGetter).GetMethod(_patternsByName[CartesianChart.Series[0].Title]), 
                    int.Parse(StartValue.Text), int.Parse(CountStep.Text), int.Parse(Step.Text), _transactionsData, _jsonParser.KeyList);
                CartesianChart.Series = new SeriesCollection {
                    new LineSeries {
                        Title = CartesianChart.Series[0].Title,
                        Values = array.AsChartValues() }
                };
                DataContext = this;
                AddToCash(CartesianChart.Series[0].Title,CartesianChart);
            }
            catch (Exception e) {
                MessageBox.Show(e.ToString(), "error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        private readonly Dictionary<string, IChartValues> _cartesianCharts = new();
        private readonly List<Image> _buttons = new List<Image>();
        private void AddToCash(string name, IChartView cartesianChart)
        { if (!_cartesianCharts.ContainsKey(name)) { _cartesianCharts.Add(name, cartesianChart.Series[0].Values);
                SavedCharts.Children.Add(new Image
                { Name = name,
                        Margin = new Thickness(1200,_cartesianCharts.Count * 60 + 500,3,0),
                            Source = new BitmapImage(new Uri(@"/IMG/Graph_inactive_tab.png", UriKind.Relative)),
                                Cursor = Cursors.Hand });
                SavedCharts.Children[^1].MouseDown += delegate { GetSaved(name); };
                    _buttons.Add((Image)SavedCharts.Children[^1]);
                SavedCharts.Children.Add(new Label
                {
                    Content = name,
                    Margin = new Thickness(1200, _cartesianCharts.Count * 60 + 510, 0, 0),
                    FontSize = 24,
                    FontFamily = new System.Windows.Media.FontFamily("Bahnschrift"),
                    IsHitTestVisible = false
                });
        } else _cartesianCharts[name] = cartesianChart.Series[0].Values; foreach (var obj in _buttons) { 
                if (obj.Source.ToString() == $"{Path}/IMG/Graph_tab.png")
                    obj.Source =
                        new BitmapImage(new Uri($@"IMG/Graph_inactive_tab.png", UriKind.Relative));
                if (obj.Name == name) obj.Source =
                    new BitmapImage(new Uri($@"IMG/Graph_tab.png", UriKind.Relative)); }
        }
        private void GetSaved(string buttonName)
        { try
            { foreach (var obj in _buttons)
                { if (obj.Source.ToString() == $"{Path}/IMG/Graph_tab.png")
                        obj.Source =
                            new BitmapImage(new Uri($@"IMG/Graph_inactive_tab.png", UriKind.Relative));
                    if (obj.Name == buttonName) obj.Source =
                        new BitmapImage(new Uri($@"IMG/Graph_tab.png", UriKind.Relative));
                } if (MoreAboutChart.Visibility != Visibility.Visible) MoreAboutChart.Visibility = Visibility.Visible;
                CartesianChart.Series = new SeriesCollection {
                    new LineSeries
                    { Title = buttonName,
                        Values = _cartesianCharts[buttonName]} 
                };
                DataContext = this; }
            catch (Exception e) {
                MessageBox.Show(e.ToString(), "error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}