using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using Kopilka.BusinessLogic;
using Kopilka.BusinessLogic.ViewModels;

namespace Kopilka.FinanceManager.Views.Pages
{
    public partial class ChartView : UserControl
    {
        public ChartView()
        {
            InitializeComponent();
            this.DataContextChanged += ChartView_DataContextChanged;
        }

        private void ChartView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ChartViewModel vm)
            {
                vm.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(ChartViewModel.ChartPoints))
                    {
                        DrawChart(vm.ChartPoints);
                    }
                };
                if (vm.ChartPoints?.Count > 0)
                {
                    DrawChart(vm.ChartPoints);
                }
            }
        }

        public void DrawChart(List<ChartPoint> points)
        {
            ChartCanvas.Children.Clear();
            if (points == null || points.Count == 0) return;

            double width = ChartCanvas.ActualWidth;
            double height = ChartCanvas.ActualHeight;

            // Если размеры еще не определены, подождем загрузки
            if (width == 0 || height == 0)
            {
                this.Loaded += (s, e) => DrawChart(points);
                return;
            }

            double barWidth = (width / points.Count) * 0.8;
            double maxAmount = 0;
            foreach (var p in points) maxAmount = Math.Max(maxAmount, (double)Math.Abs(p.Amount));
            if (maxAmount == 0) maxAmount = 1;

            for (int i = 0; i < points.Count; i++)
            {
                double barHeight = ((double)Math.Abs(points[i].Amount) / maxAmount) * (height / 2);
                var rect = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight,
                    Fill = points[i].Amount >= 0 ? Brushes.Green : Brushes.Red,
                    ToolTip = $"{points[i].Date:dd.MM.yyyy}: {points[i].Amount}"
                };

                Canvas.SetLeft(rect, i * (width / points.Count) + (width / points.Count - barWidth) / 2);
                if (points[i].Amount >= 0)
                    Canvas.SetBottom(rect, height / 2);
                else
                    Canvas.SetTop(rect, height / 2);

                ChartCanvas.Children.Add(rect);
            }

            var line = new Line
            {
                X1 = 0, Y1 = height / 2,
                X2 = width, Y2 = height / 2,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            ChartCanvas.Children.Add(line);
        }
    }
}
