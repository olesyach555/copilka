using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using Kopilka.BusinessLogic;

namespace Kopilka.FinanceManager.Views.Pages
{
    public partial class ChartView : UserControl
    {
        public ChartView()
        {
            InitializeComponent();
        }

        // В реальном приложении данные приходили бы через Binding,
        // и мы бы рисовали при изменении данных.
        // Здесь - упрощенный пример рисования.
        public void DrawChart(List<ChartPoint> points)
        {
            ChartCanvas.Children.Clear();
            if (points == null || points.Count == 0) return;

            double width = ChartCanvas.ActualWidth;
            double height = ChartCanvas.ActualHeight;
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
                    Fill = points[i].Amount >= 0 ? Brushes.Green : Brushes.Red
                };

                Canvas.SetLeft(rect, i * (width / points.Count) + (width / points.Count - barWidth) / 2);
                if (points[i].Amount >= 0)
                    Canvas.SetBottom(rect, height / 2);
                else
                    Canvas.SetTop(rect, height / 2);

                ChartCanvas.Children.Add(rect);
            }

            // Осевая линия
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
