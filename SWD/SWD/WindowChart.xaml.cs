using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SWD
{
    /// <summary>
    /// Interaction logic for WindowChart.xaml
    /// </summary>
    public partial class WindowChart : Window
    {
        int iBrush = -1;

        SolidColorBrush[] kolory = new SolidColorBrush[]
        {
            new SolidColorBrush(Color.FromRgb(255,0,0)),
            new SolidColorBrush(Color.FromRgb(0,255,0)),
            new SolidColorBrush(Color.FromRgb(0,0,255)),
            new SolidColorBrush(Color.FromRgb(255,255,0)),
            new SolidColorBrush(Color.FromRgb(255,0,255)),
            new SolidColorBrush(Color.FromRgb(0,255,255)),
            new SolidColorBrush(Color.FromRgb(255,255,255)),
            new SolidColorBrush(Color.FromRgb(0,0,0)),
            new SolidColorBrush(Color.FromRgb(100,0,0)),
            new SolidColorBrush(Color.FromRgb(0,100,0)),
            new SolidColorBrush(Color.FromRgb(0,0,100)),
            new SolidColorBrush(Color.FromRgb(100,100,0)),
            new SolidColorBrush(Color.FromRgb(100,0,100)),
            new SolidColorBrush(Color.FromRgb(0,100,100)),
            new SolidColorBrush(Color.FromRgb(100,100,100)),
            new SolidColorBrush(Color.FromRgb(0,0,0)),
        };


        public WindowChart(Dictionary<double, double> slownik)
        {
            InitializeComponent();
        }

        public WindowChart()
        {
            InitializeComponent();
        }

        private SolidColorBrush getColorBrush()
        {
            iBrush++;
            if (iBrush == 16)
                iBrush = 0;
            return kolory[iBrush];
        }

        public void addSeries(Dictionary<double, double> slownik)
        {
            ScatterSeries series = new ScatterSeries();
            series.Name = "nazwa";
            series.IndependentValuePath = "Key";
            series.DependentValuePath = "Value";
            series.ItemsSource = slownik;

            Style s = new Style(typeof(ScatterDataPoint), chart.Palette[chart.Series.Count % chart.Palette.Count]["DataPointStyle"] as Style);
            s.Setters.Add(new Setter(OpacityProperty, 100.0));
            s.Setters.Add(new Setter(BackgroundProperty, getColorBrush()));

            series.DataPointStyle = s;

            chart.Series.Add(series);
        }
    }
}
