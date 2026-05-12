using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using Kopilka.BusinessLogic;
using Kopilka.BusinessLogic.ViewModels;
using System.Linq;

namespace Kopilka.FinanceManager.Views.Pages
{
    public partial class ChartView : UserControl
    {
        public ChartView()
        {
            InitializeComponent();
            this.DataContextChanged += ChartView_DataContextChanged;
            this.SizeChanged += (s, e) => { if (DataContext is ChartViewModel vm) DrawChart(vm.ChartPoints); };
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

            if (width <= 0 || height <= 0) return;

            double barWidth = (width / points.Count) * 0.7;
            double maxAmount = points.Max(p => (double)Math.Abs(p.Amount));
            if (maxAmount == 0) maxAmount = 1;

            double centerY = height / 2;

            for (int i = 0; i < points.Count; i++)
            {
                double val = (double)points[i].Amount;
                double barHeight = (Math.Abs(val) / maxAmount) * (height / 2.2);

                var rect = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight,
                    Fill = val >= 0 ? Brushes.Green : Brushes.Red,
                    ToolTip = $"{points[i].Date:dd.MM.yyyy}: {points[i].Amount:N2}"
                };

                Canvas.SetLeft(rect, i * (width / points.Count) + (width / points.Count - barWidth) / 2);

                if (val >= 0)
                    Canvas.SetBottom(rect, height - centerY);
                else
                    Canvas.SetTop(rect, centerY);

                ChartCanvas.Children.Add(rect);

                // Добавим даты снизу
                if (points.Count < 15 || i % (points.Count / 10 + 1) == 0)
                {
                    var text = new TextBlock
                    {
                        Text = points[i].Date.ToString("dd.MM"),
                        FontSize = 10,
                        Foreground = Brushes.Gray
                    };
                    Canvas.SetLeft(text, i * (width / points.Count));
                    Canvas.SetBottom(text, 5);
                    ChartCanvas.Children.Add(text);
                }
            }

            var line = new Line
            {
                X1 = 0, Y1 = centerY,
                X2 = width, Y2 = centerY,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            ChartCanvas.Children.Add(line);
        }
    }
}
