﻿using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Graphics
{
    public class GraphTest
    {
        public SeriesCollection SeriesCollection { get; private set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public GraphTest(MainWindow mainWindow)
        {
            SeriesCollection = new();
            Labels = ["Maria", "Susan", "Charles", "Frida"];
            Formatter = value => value.ToString("N");
            InitValues();
            mainWindow.DataContext = this;
        }

        private void InitValues()
        {
            SeriesCollection =
            [
                new ColumnSeries
                {
                    Title = "2015",
                    Values = new ChartValues<double> { 10, 50, 39, 50 }
                },
                //adding series will update and animate the chart automatically
                new ColumnSeries
                {
                    Title = "2016",
                    Values = new ChartValues<double> { 11, 56, 42 }
                },
            ];

            // pridani hodnoty do ColumnSeries dodatečně - ručně, konkretně do druheho sloupce
            SeriesCollection[1].Values.Add(48d);
        }   
    }
}