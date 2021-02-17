using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace Pvz1
{
    public class Lab4
    {
        private readonly Form1 form;

        public Lab4(Form1 form)
        {
            this.form = form;
        }

        public void Run(double deltaT, bool keepForm = false)
        {
            var m = 5d;
            var v = 80d;
            var s = 5d;
            var k1 = 0.15d;
            var k2 = 0.6d;
            var g = 9.8d;
            var t = 0d;

            form.PreparareForm(0, 10, -10, 55);
            var H = form.X1X2;
            var V = form.Fx;

            V = form.chart1.Series.Add($"V, dT: {deltaT}");
            V.MarkerStyle = MarkerStyle.Circle;
            V.MarkerSize = 5;
            V.ChartType = SeriesChartType.Point;
            V.ChartType = SeriesChartType.Line;

            H = form.chart1.Series.Add($"H, dT: {deltaT}");
            H.MarkerStyle = MarkerStyle.Circle;
            H.MarkerSize = 5;
            H.ChartType = SeriesChartType.Point;
            H.ChartType = SeriesChartType.Line;

            if (!keepForm)
            {
                V.Points.Clear();
                H.Points.Clear();
            }

            V.Points.AddXY(t, v);
            H.Points.AddXY(t, s);

            double F(double _v)
            {
                var k = _v > 0 ? k1 : k2;
                return (m * - g - k * _v * Math.Abs(_v)) / m;
            }

            double G(double _v) => _v;

            while (s > 0)
            {
                t += deltaT;
                s = s + deltaT * G(v);
                v = v + deltaT * F(v);
                H.Points.AddXY(t, s);
                V.Points.AddXY(t, v);


                form.richTextBox1.AppendText($"t:{t}, v:{v}, s:{s} \n");
            }
        }
        public void RunAnalysis()
        {
            var t1 = 0.1d;
            var t2 = 0.15d;
            var t3 = 0.3d;
            Run(t1);
            Run(t2, true);
            Run(t3, true);
        }
    }

}
