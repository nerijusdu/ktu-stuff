using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet.Numerics.LinearAlgebra;

namespace Pvz1
{
    public class Lab3
    {
        private readonly Form1 form;

        public Lab3(Form1 form)
        {
            this.form = form;
        }

        double F(double x) => Math.Log10(x) / (Math.Sin(2 * x) + 1.5);

        public void Uzd11()
        {
            var arrayOfX = new List<double>();

            form.ClearForm();
//            form.PreparareForm(1,12,-10,12);
            form.PreparareForm(0,15,0,75);
            var Fx = form.chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;
            var x = 2d;
//            while (x <= 10)
//            {
//                Fx.Points.AddXY(x, F(x));
//                x = x + 0.01;
//            }
            Fx.BorderWidth = 3;

            var X1X2 = form.chart1.Series.Add("x");
            X1X2.MarkerSize = 6;
            X1X2.ChartType = SeriesChartType.Point;
            X1X2.Color = Color.Yellow;

            var intervals = new List<double>();

            for (var i = 0d; i < 7; i+=0.5)
            {
                var ii = (int)Math.Floor(i);
                X1X2.Points.AddXY(stuff[ii, 0], stuff[ii, 1]);
                arrayOfX.Add(stuff[ii, 1]);
                intervals.Add(i);
            }
            //            for (var i = 2d; i <= 10; i = i + 0.5)
            //            {
            //                arrayOfX.Add(F(i));
            //                intervals.Add(i);
            //                X1X2.Points.AddXY(i, F(i));
            //            }
            form.richTextBox1.AppendText(intervals.Count.ToString());

            var busima = new double[intervals.Count, intervals.Count];
            for (int i = 0; i < intervals.Count; i++)
            {
                busima[i, 0] = 1;
                for (int j = 1; j < intervals.Count; j++)
                {
                    var temp = 1d;

                    for (var k = 0; k < j; k++)
                    {
                        temp *= intervals[i] - intervals[k];
                    }

                    busima[i, j] = temp;
                }
            }

            var m = Matrix<double>.Build.DenseOfArray(busima);
            var y = Vector<double>.Build.DenseOfArray(arrayOfX.ToArray());
            var a = m.Inverse() * y;

            // Niutonas
            var Fx_niut = form.chart1.Series.Add("F(x)-niut");
            Fx_niut.ChartType = SeriesChartType.Line;
            Fx_niut.Color = Color.Red;
            Fx_niut.MarkerSize = 7;

            x = 3;
            while (x <= 14)
            {
                var temp = 0d;
                for (int j = 0; j < intervals.Count; j++)
                {
                    var temp1 = 1d;

                    for (int i = 0; i < j; i++)
                    {
                        temp1 = temp1 * (x - intervals[i]);
                    }
                    temp = temp + a[j] * temp1;
                }

                Fx_niut.Points.AddXY(x, temp);
                x = x + 0.01;
            }

            form.richTextBox1.AppendText("\nMatrica m:\n");
            form.richTextBox1.AppendText(m.ToString());

            form.richTextBox1.AppendText("\nVektorius y:\n");
            form.richTextBox1.AppendText(y.ToString());

            form.richTextBox1.AppendText("\nVektorius a:\n");
            form.richTextBox1.AppendText(a.ToString());
        }

        public void Uzd12()
        {
            var arrayOfX = new List<double>();

            form.ClearForm();
            form.PreparareForm(1, 12, -5, 5);
            var Fx = form.chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;
            var x = 2d;
            while (x <= 10)
            {
                Fx.Points.AddXY(x, F(x));
                x = x + 0.01;
            }
            Fx.BorderWidth = 3;

            var X1X2 = form.chart1.Series.Add("x");
            X1X2.MarkerSize = 6;
            X1X2.ChartType = SeriesChartType.Point;
            X1X2.Color = Color.Yellow;

            var aa = 2d;
            var b = 10d;
            var intervals = new List<double>();
            for (var i = 0d; i < 9; i = i + 0.5)
            {
                var temp = (b - aa) / 2.0 * Math.Cos(Math.PI * (2 * i + 1) / (2.0 * 9.0)) + (b + aa) / 2;

                intervals.Add(temp);
                arrayOfX.Add(F(temp));
                X1X2.Points.AddXY(temp, F(temp));
            }

            var busima = new double[intervals.Count, intervals.Count];
            for (int i = 0; i < intervals.Count; i++)
            {
                busima[i, 0] = 1;
                for (int j = 1; j < intervals.Count; j++)
                {
                    var temp = 1d;

                    for (var k = 0; k < j; k++)
                    {
                        temp *= intervals[i] - intervals[k];
                    }

                    busima[i, j] = temp;
                }
            }

            var m = Matrix<double>.Build.DenseOfArray(busima);
            var y = Vector<double>.Build.DenseOfArray(arrayOfX.ToArray());
            var a = m.Inverse() * y;

            // Ciobysevo
            var Fx_ciob = form.chart1.Series.Add("F(x)-ciob");
            Fx_ciob.ChartType = SeriesChartType.Line;
            Fx_ciob.Color = Color.Red;
            Fx_ciob.MarkerSize = 7;

            x = 2;
            while (x <= 10)
            {
                var temp = 0d;
                for (int j = 0; j < intervals.Count; j++)
                {
                    var temp1 = 1d;

                    for (int i = 0; i < j; i++)
                    {
                        temp1 *= x - intervals[i];
                    }
                    temp = temp + a[j] * temp1;
                }

                Fx_ciob.Points.AddXY(x, temp);
                x = x + 0.01;
            }

            form.richTextBox1.AppendText("\nMatrica m:\n");
            form.richTextBox1.AppendText(m.ToString());

            form.richTextBox1.AppendText("\nVektorius y:\n");
            form.richTextBox1.AppendText(y.ToString());

            form.richTextBox1.AppendText("\nVektorius a:\n");
            form.richTextBox1.AppendText(a.ToString());
        }

        private readonly double[,] stuff =
        {
            {8, 37},
            {4, 19.5},
            {11, 52},
            {4, 22},
            {3, 16.5},
            {6.5, 32.8},
            {14, 72}
        };

        readonly double[] temperature = { 0.64822, -3.9455, 7.99805, 11.983, 16.7858, 21.1055, 23.2656, 22.9801, 18.4952, 11.5009, 7.61598, -0.5816 };

        public void Uzd21()
        {
            form.ClearForm();
            form.PreparareForm(0, 13, -15, 25);
            var Fx = form.chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;

            var x = 1d;
            var id = 0;
            while (x <= 12)
            {
                Fx.Points.AddXY(x, temperature[id]);
                x++;
                id++;
            }

            Fx.BorderWidth = 3;

            var X1X2 = form.chart1.Series.Add("x");
            X1X2.MarkerSize = 6;
            X1X2.ChartType = SeriesChartType.Point;
            X1X2.Color = Color.Yellow;

            id = 0;

            List<double> intervals = new List<double>();
            for (int i = 1; i <= 12; i++)
            {
                intervals.Add(i);
                X1X2.Points.AddXY(i, temperature[id]);
                id++;
            }

            double[,] busima = new double[intervals.Count, intervals.Count];

            for (int i = 0; i < intervals.Count; i++)
            {
                busima[i, 0] = 1;
                for (int j = 1; j < intervals.Count; j++)
                {
                    var temp = 1d;

                    for (var k = 0; k < j; k++)
                    {
                        temp *= intervals[i] - intervals[k];
                    }

                    busima[i, j] = temp;
                }
            }

            Matrix<double> m = Matrix<double>.Build.DenseOfArray(busima);
            Vector<double> y = Vector<double>.Build.DenseOfArray(temperature.ToArray());
            Vector<double> a = m.Inverse() * y;

            var Fx_niut = form.chart1.Series.Add("F(x)-niut");
            Fx_niut.ChartType = SeriesChartType.Line;
            Fx_niut.Color = Color.Red;
            Fx_niut.MarkerSize = 7;

            x = 1;
            while (x <= 12)
            {
                var temp = 0d;
                for (int j = 0; j < intervals.Count; j++)
                {
                    var temp1 = 1d;

                    for (int i = 0; i < j; i++)
                    {
                        temp1 = temp1 * (x - intervals[i]);
                    }
                    temp = temp + a[j] * temp1;
                }

                Fx_niut.Points.AddXY(x, temp);
                x = x + 0.01;
            }

            form.richTextBox1.AppendText("\nMatrica m:\n");
            form.richTextBox1.AppendText(m.ToString());

            form.richTextBox1.AppendText("\nVektorius y:\n");
            form.richTextBox1.AppendText(y.ToString());

            form.richTextBox1.AppendText("\nVektorius a:\n");
            form.richTextBox1.AppendText(a.ToString());
        }

        public void Uzd22()
        {
            form.ClearForm();
            form.PreparareForm(-0, 13, -15, 30);
            var Fx = form.chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;

            var x1 = 1d;
            var id = 0;
            while (x1 <= 12)
            {
                Fx.Points.AddXY(x1, temperature[id]);
                x1++;
                id++;
            }

            var GS = form.chart1.Series.Add("Globalus splainas");
            GS.ChartType = SeriesChartType.Line;
            GS.Points.Clear();
            var x = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            var dF = Isvestines(x, temperature);
            var ve = new double[12];
            ve[0] = 0;
            ve[11] = 0;
            for (var i = 0; i < 12 - 2; i++)
            {
                ve[i + 1] = dF[i];
            }

            dF = Vector<double>.Build.DenseOfArray(ve);
            for (var i = 0; i < 11; i++)
            {
                for (var j = x[i]; j <= x[i + 1]; j = j + 0.1)
                {
                    var s = j - x[i];
                    var d = x[i + 1] - x[i];

                    var y = dF[i] * (Math.Pow(s, 2) / 2) - 
                            dF[i] * (Math.Pow(s, 3) / (6 * d)) + 
                            dF[i + 1] * (Math.Pow(s, 3) / (6 * d)) + s * 
                            ((temperature[i + 1] - temperature[i]) / d) - s * 
                            (dF[i] * (d / 3)) - s * 
                            (dF[i + 1] * (d / 6)) + 
                            temperature[i];
                    GS.Points.AddXY(j, y);
                }
            }

            var X1X2 = form.chart1.Series.Add("x");
            X1X2.MarkerStyle = MarkerStyle.Circle;
            X1X2.MarkerSize = 8;
            X1X2.ChartType = SeriesChartType.Point;
            X1X2.Points.Clear();
            for (int i = 0; i < 12; i++)
            {
                X1X2.Points.AddXY(x[i], temperature[i]);
            }
        }

        private Vector<double> Isvestines(double[] x, double[] y)
        {
            var n = x.Length;
            var vekt = new double[n - 2, n - 2];
            var X = Matrix<double>.Build.DenseOfArray(vekt);
            var d = new double[n - 1];

            for (var i = 0; i < n - 1; i++)
            {
                d[i] = x[i + 1] - x[i];
            }

            for (var i = 0; i < n - 2; i++)
            {
                for (var j = 0; j < n - 2; j++)
                {
                    if (i == 0 && j == 0 ||
                        i == n - 2 && j == n - 2 ||
                        j == (i) && i != 0 && i != n - 2)
                    {
                        X[i, j] = (d[i] + d[i + 1]) / 3;
                    }
                    else if (i == 0 && j == 1 ||
                             j == (i + 1) && i != 0 && i != n - 2)
                    {
                        X[i, j] = d[i + 1] / 6;
                    }
                    else if (i == n - 2 && j == n - 3 ||
                             j == i - 1 && i != 0 && i != n - 2)
                    {
                        X[i, j] = d[i] / 6;
                    }
                    else
                    {
                        X[i, j] = 0;
                    }
                }
            }
            var v = new double[n - 2];
            for (var i = 0; i < n - 2; i++)
            {
                v[i] = (y[i + 2] - y[i + 1]) / d[i + 1] - ((y[i + 1] - y[i]) / d[i]);
            }
            var Y = Vector<double>.Build.DenseOfArray(v);

            return X.Solve(Y);
        }

        readonly double[] countryX = { 22.878, 22.861, 22.836, 22.82, 22.78, 22.764, 22.753, 22.753, 22.758, 22.76, 22.752, 22.746, 22.724, 22.704, 22.692, 22.667, 22.638, 22.601, 22.563, 22.528, 22.454, 22.424, 22.407, 22.396, 22.383, 22.368, 22.322, 22.309, 22.292, 22.273, 22.262, 22.24, 22.232, 22.215, 22.208, 22.204, 22.201, 22.198, 22.173, 22.17, 22.169, 22.168, 22.167, 22.162, 22.154, 22.149, 22.1, 22.037, 22.008, 21.989, 21.991, 22, 22.002, 21.982, 21.937, 21.919, 21.901, 21.862, 21.856, 21.845, 21.839, 21.828, 21.824, 21.823, 21.826, 21.826, 21.82, 21.812, 21.789, 21.78, 21.775, 21.77, 21.764, 21.744, 21.694, 21.672, 21.633, 21.634, 21.637, 21.645, 21.655, 21.662, 21.671, 21.671, 21.668, 21.649, 21.64, 21.594, 21.59, 21.589, 21.587, 21.588, 21.591, 21.592, 21.583, 21.573, 21.537, 21.516, 21.503, 21.482, 21.477, 21.474, 21.471, 21.47, 21.473, 21.475, 21.478, 21.505, 21.502, 21.484, 21.464, 21.436, 21.425, 21.424, 21.417, 21.396, 21.374, 21.338, 21.316, 21.301, 21.295, 21.291, 21.279, 21.262, 21.248, 21.245, 21.274, 21.281, 21.258, 21.215, 21.196, 21.179, 21.169, 21.165, 21.156, 21.144, 21.135, 21.106, 21.099, 21.051, 21.033, 21.014, 21.007, 20.999, 20.991, 20.981, 20.962, 20.924, 20.907, 20.9, 20.885, 20.875, 20.867, 20.849, 20.84, 20.82, 20.799, 20.787, 20.779, 20.739, 20.735, 20.736, 20.745, 20.745, 20.737, 20.727, 20.718, 20.71, 20.705, 20.704, 20.704, 20.699, 20.684, 20.664, 20.608, 20.6, 20.588, 20.578, 20.549, 20.509, 20.469, 20.444, 20.283, 20.243, 20.188, 20.17, 20.145, 20.138, 20.13, 20.12, 20.115, 20.098, 20.089, 20.063, 20.035, 19.993, 19.929, 19.889, 19.874, 19.79, 19.773, 19.712, 19.69, 19.669, 19.648, 19.59, 19.568, 19.55, 19.525, 19.502, 19.488, 19.489, 19.497, 19.499, 19.473, 19.466, 19.461, 19.454, 19.437, 19.428, 19.417, 19.405, 19.397, 19.389, 19.379, 19.362, 19.325, 19.307, 19.298, 19.287, 19.279, 19.275, 19.263, 19.236, 19.148, 19.126, 19.126, 19.111, 19.088, 19.065, 19.05, 19.049, 19.031, 19.006, 18.982, 18.979, 18.978, 18.979, 18.986, 18.989, 18.988, 18.982, 18.963, 18.901, 18.887, 18.877, 18.865, 18.845, 18.828, 18.822, 18.818, 18.805, 18.795, 18.79, 18.786, 18.776, 18.763, 18.724, 18.674, 18.656, 18.633, 18.629, 18.626, 18.624, 18.622, 18.607, 18.585, 18.573, 18.53, 18.504, 18.482, 18.465, 18.431, 18.412, 18.404, 18.398, 18.391, 18.384, 18.384, 18.367, 18.351, 18.335, 18.318, 18.301, 18.283, 18.261, 18.23, 18.211, 18.129, 18.12, 18.103, 18.092, 18.08, 17.975, 17.906, 17.89, 17.88, 17.876, 17.87, 17.858, 17.858, 17.838, 17.809, 17.687, 17.665, 17.657, 17.655, 17.653, 17.652, 17.647, 17.646, 17.638, 17.591, 17.554, 17.512, 17.427, 17.418, 17.415, 17.412, 17.406, 17.4, 17.389, 17.365, 17.345, 17.342, 17.344, 17.338, 17.327, 17.317, 17.307, 17.305, 17.29, 17.29, 17.297, 17.309, 17.317, 17.308, 17.291, 17.276, 17.269, 17.267, 17.254, 17.249, 17.209, 17.197, 17.116, 17.101, 17.053, 17.041, 17.036, 17.028, 16.975, 16.965, 16.941, 16.903, 16.896, 16.889, 16.879, 16.872, 16.875, 16.865, 16.859, 16.85, 16.838, 16.832, 16.83, 16.828, 16.821, 16.817, 16.812, 16.808, 16.803, 16.776, 16.769, 16.762, 16.755, 16.743, 16.712, 16.694, 16.677, 16.659, 16.638, 16.611, 16.602, 16.594, 16.588, 16.584, 16.578, 16.564, 16.521, 16.515, 16.501, 16.467, 16.43, 16.395, 16.376, 16.372, 16.368, 16.378, 16.397, 16.403, 16.411, 16.405, 16.39, 16.371, 16.366, 16.365, 16.358, 16.357, 16.343, 16.334, 16.326, 16.314, 16.3, 16.298, 16.299, 16.302, 16.311, 16.315, 16.322, 16.328, 16.33, 16.325, 16.311, 16.302, 16.297, 16.282, 16.272, 16.179, 16.135, 16.13, 16.094, 16.11, 16.123, 16.159, 16.171, 16.196, 16.217, 16.224, 16.231, 16.231, 16.23, 16.233, 16.243, 16.253, 16.261, 16.265, 16.275, 16.289, 16.326, 16.367, 16.388, 16.405, 16.41, 16.416, 16.42, 16.425, 16.425, 16.441, 16.467, 16.486, 16.482, 16.468, 16.453, 16.425, 16.437, 16.482, 16.493, 16.497, 16.494, 16.482, 16.462, 16.454, 16.461, 16.482, 16.505, 16.51, 16.497, 16.481, 16.447, 16.434, 16.434, 16.442, 16.442, 16.435, 16.427, 16.419, 16.413, 16.409, 16.425, 16.421, 16.421, 16.452, 16.467, 16.473, 16.469, 16.439, 16.436, 16.432, 16.427, 16.424, 16.425, 16.429, 16.434, 16.436, 16.434, 16.444, 16.457, 16.47, 16.482, 16.589, 16.627, 16.641, 16.637, 16.648, 16.677, 16.688, 16.689, 16.681, 16.668, 16.651, 16.656, 16.648, 16.63, 16.608, 16.575, 16.51, 16.482, 16.426, 16.408, 16.417, 16.432, 16.439, 16.445, 16.45, 16.455, 16.461, 16.473, 16.513, 16.521, 16.527, 16.525, 16.531, 16.568, 16.61, 16.69, 16.702, 16.708, 16.712, 16.719, 16.73, 16.741, 16.797, 16.806, 16.817, 16.837, 16.85, 16.865, 16.903, 16.982, 17.055, 17.075, 17.064, 17.056, 17.051, 17.049, 17.042, 17.041, 17.056, 17.049, 17.04, 17.032, 17.011, 17.004, 17.004, 17.017, 17.051, 17.068, 17.078, 17.083, 17.087, 17.085, 17.08, 17.075, 17.074, 17.077, 17.086, 17.088, 17.091, 17.096, 17.096, 17.086, 17.148, 17.185, 17.221, 17.262, 17.273, 17.338, 17.369, 17.472, 17.482, 17.492, 17.517, 17.527, 17.56, 17.573, 17.583, 17.593, 17.604, 17.619, 17.64, 17.658, 17.666, 17.677, 17.719, 17.742, 17.826, 17.884, 18.113, 18.236, 18.273, 18.348, 18.553, 18.597, 18.634, 18.664, 18.693, 18.717, 18.75, 18.768, 18.79, 18.815, 18.816, 18.778, 18.749, 18.742, 18.745, 18.755, 18.751, 18.745, 18.743, 18.756, 18.765, 18.785, 18.794, 18.821, 18.838, 18.934, 18.982, 18.996, 19.019, 19.039, 19.099, 19.223, 19.233, 19.293, 19.428, 19.482, 19.483, 19.484, 19.483, 19.482, 19.481, 19.481, 19.481, 19.482, 19.494, 19.503, 19.514, 19.531, 19.623, 19.634, 19.644, 19.655, 19.677, 19.687, 19.733, 19.757, 19.774, 19.775, 19.769, 19.767, 19.776, 19.786, 19.822, 19.846, 19.884, 19.905, 19.929, 19.974, 19.997, 20.035, 20.038, 20.078, 20.097, 20.105, 20.113, 20.118, 20.122, 20.135, 20.143, 20.153, 20.171, 20.188, 20.218, 20.229, 20.249, 20.26, 20.272, 20.295, 20.324, 20.349, 20.37, 20.409, 20.421, 20.436, 20.466, 20.468, 20.482, 20.483, 20.483, 20.483, 20.482, 20.48, 20.481, 20.482, 20.511, 20.573, 20.784, 20.8, 20.816, 20.845, 20.86, 20.891, 20.946, 20.981, 21.007, 21.036, 21.064, 21.084, 21.109, 21.187, 21.22, 21.238, 21.25, 21.262, 21.277, 21.288, 21.294, 21.302, 21.322, 21.339, 21.373, 21.425, 21.439, 21.473, 21.491, 21.499, 21.506, 21.515, 21.522, 21.538, 21.575, 21.592, 21.6, 21.613, 21.622, 21.678, 21.701, 21.728, 21.759, 21.789, 21.841, 21.884, 21.915, 21.929, 21.982, 22, 22.018, 22.078, 22.096, 22.114, 22.133, 22.157, 22.158, 22.159, 22.169, 22.202, 22.236, 22.272, 22.257, 22.257, 22.284, 22.291, 22.299, 22.299, 22.296, 22.297, 22.308, 22.357, 22.364, 22.371, 22.379, 22.386, 22.39, 22.395, 22.399, 22.418, 22.434, 22.45, 22.469, 22.473, 22.477, 22.481, 22.556, 22.569, 22.583, 22.6, 22.605, 22.608, 22.621, 22.693, 22.712, 22.729, 22.746, 22.762, 22.766, 22.801, 22.831, 22.844, 22.855, 22.862, 22.858, 22.851, 22.849, 22.832, 22.841, 22.878 };
        readonly double[] countryY = { 47.947, 47.934, 47.902, 47.892, 47.882, 47.875, 47.861, 47.853, 47.846, 47.839, 47.828, 47.825, 47.823, 47.817, 47.811, 47.789, 47.772, 47.761, 47.757, 47.761, 47.787, 47.783, 47.743, 47.736, 47.732, 47.731, 47.736, 47.735, 47.731, 47.724, 47.716, 47.693, 47.688, 47.68, 47.674, 47.666, 47.648, 47.639, 47.615, 47.609, 47.601, 47.595, 47.594, 47.586, 47.582, 47.579, 47.571, 47.539, 47.517, 47.493, 47.462, 47.427, 47.394, 47.366, 47.357, 47.35, 47.336, 47.297, 47.286, 47.25, 47.241, 47.226, 47.215, 47.203, 47.194, 47.185, 47.173, 47.165, 47.15, 47.141, 47.132, 47.114, 47.105, 47.092, 47.069, 47.055, 47.023, 47.019, 47.014, 47.011, 47.01, 47.006, 46.994, 46.993, 46.992, 46.943, 46.936, 46.91, 46.909, 46.906, 46.894, 46.882, 46.872, 46.861, 46.848, 46.842, 46.835, 46.822, 46.805, 46.765, 46.76, 46.755, 46.749, 46.743, 46.74, 46.738, 46.736, 46.723, 46.704, 46.685, 46.677, 46.674, 46.662, 46.658, 46.645, 46.626, 46.618, 46.62, 46.617, 46.604, 46.585, 46.547, 46.528, 46.513, 46.497, 46.477, 46.438, 46.416, 46.404, 46.403, 46.398, 46.384, 46.363, 46.318, 46.299, 46.284, 46.279, 46.279, 46.276, 46.236, 46.231, 46.243, 46.249, 46.252, 46.252, 46.249, 46.248, 46.26, 46.262, 46.261, 46.255, 46.254, 46.257, 46.268, 46.271, 46.272, 46.268, 46.263, 46.26, 46.237, 46.232, 46.223, 46.2, 46.192, 46.187, 46.188, 46.19, 46.188, 46.181, 46.169, 46.166, 46.156, 46.145, 46.138, 46.129, 46.13, 46.133, 46.138, 46.156, 46.168, 46.174, 46.147, 46.144, 46.108, 46.14, 46.146, 46.137, 46.136, 46.139, 46.149, 46.152, 46.155, 46.154, 46.145, 46.143, 46.159, 46.164, 46.157, 46.153, 46.129, 46.132, 46.159, 46.168, 46.173, 46.174, 46.166, 46.166, 46.164, 46.156, 46.146, 46.134, 46.126, 46.117, 46.109, 46.099, 46.092, 46.084, 46.078, 46.068, 46.066, 46.064, 46.06, 46.052, 46.042, 46.034, 46.03, 46.029, 46.027, 46.022, 46.016, 46.004, 45.992, 45.981, 45.978, 45.984, 45.993, 45.993, 46.013, 46.019, 46.012, 45.993, 45.963, 45.96, 45.963, 45.951, 45.947, 45.944, 45.94, 45.931, 45.927, 45.924, 45.922, 45.928, 45.931, 45.93, 45.922, 45.918, 45.914, 45.906, 45.906, 45.905, 45.914, 45.903, 45.894, 45.887, 45.883, 45.884, 45.898, 45.91, 45.908, 45.892, 45.887, 45.875, 45.869, 45.868, 45.857, 45.827, 45.817, 45.791, 45.784, 45.791, 45.784, 45.754, 45.743, 45.742, 45.741, 45.742, 45.743, 45.743, 45.758, 45.758, 45.752, 45.747, 45.752, 45.765, 45.765, 45.781, 45.785, 45.785, 45.783, 45.775, 45.772, 45.772, 45.792, 45.792, 45.792, 45.789, 45.781, 45.773, 45.772, 45.776, 45.8, 45.814, 45.841, 45.842, 45.845, 45.852, 45.857, 45.869, 45.884, 45.892, 45.901, 45.936, 45.948, 45.954, 45.957, 45.953, 45.95, 45.947, 45.944, 45.96, 45.963, 45.957, 45.956, 45.959, 45.968, 45.985, 45.972, 45.974, 45.98, 45.981, 45.985, 45.991, 45.993, 45.997, 45.998, 46.006, 46.008, 46.012, 46.029, 46.039, 46.071, 46.08, 46.117, 46.121, 46.123, 46.128, 46.153, 46.162, 46.173, 46.18, 46.211, 46.219, 46.251, 46.282, 46.286, 46.292, 46.312, 46.327, 46.343, 46.359, 46.365, 46.373, 46.382, 46.382, 46.377, 46.374, 46.378, 46.382, 46.385, 46.387, 46.389, 46.393, 46.396, 46.382, 46.382, 46.4, 46.413, 46.43, 46.449, 46.464, 46.474, 46.478, 46.482, 46.483, 46.48, 46.471, 46.471, 46.48, 46.499, 46.502, 46.545, 46.565, 46.604, 46.619, 46.629, 46.636, 46.643, 46.653, 46.659, 46.663, 46.668, 46.687, 46.694, 46.695, 46.696, 46.697, 46.699, 46.716, 46.714, 46.722, 46.733, 46.743, 46.772, 46.776, 46.78, 46.787, 46.798, 46.802, 46.813, 46.825, 46.834, 46.839, 46.84, 46.843, 46.847, 46.86, 46.864, 46.858, 46.856, 46.857, 46.863, 46.868, 46.876, 46.91, 46.919, 46.931, 46.937, 46.941, 46.948, 46.954, 46.96, 46.966, 46.972, 46.974, 46.978, 46.993, 47.004, 47.006, 47, 47.004, 47.002, 46.993, 46.99, 46.989, 46.99, 46.993, 46.993, 46.995, 46.995, 46.999, 47.009, 47.018, 47.022, 47.024, 47.032, 47.044, 47.049, 47.055, 47.06, 47.064, 47.068, 47.082, 47.096, 47.105, 47.126, 47.138, 47.15, 47.151, 47.14, 47.146, 47.151, 47.168, 47.177, 47.184, 47.184, 47.184, 47.187, 47.204, 47.226, 47.243, 47.243, 47.255, 47.263, 47.277, 47.293, 47.337, 47.339, 47.34, 47.341, 47.345, 47.351, 47.354, 47.356, 47.359, 47.397, 47.41, 47.412, 47.406, 47.392, 47.426, 47.446, 47.453, 47.493, 47.502, 47.51, 47.523, 47.538, 47.551, 47.56, 47.567, 47.586, 47.606, 47.622, 47.629, 47.625, 47.643, 47.639, 47.654, 47.661, 47.669, 47.685, 47.69, 47.686, 47.682, 47.682, 47.685, 47.692, 47.706, 47.712, 47.72, 47.733, 47.743, 47.754, 47.751, 47.73, 47.724, 47.715, 47.704, 47.694, 47.686, 47.681, 47.675, 47.677, 47.684, 47.705, 47.713, 47.687, 47.682, 47.695, 47.702, 47.708, 47.713, 47.721, 47.731, 47.764, 47.784, 47.801, 47.812, 47.819, 47.837, 47.841, 47.848, 47.852, 47.863, 47.868, 47.873, 47.882, 47.892, 47.905, 47.923, 47.925, 47.926, 47.928, 47.932, 47.935, 47.939, 47.941, 47.949, 47.956, 47.962, 47.97, 48.005, 48.02, 48.015, 48.007, 48.005, 47.999, 47.981, 47.889, 47.883, 47.88, 47.876, 47.872, 47.838, 47.83, 47.83, 47.833, 47.834, 47.829, 47.819, 47.807, 47.797, 47.789, 47.774, 47.765, 47.75, 47.753, 47.762, 47.754, 47.756, 47.777, 47.793, 47.791, 47.78, 47.776, 47.778, 47.788, 47.814, 47.822, 47.826, 47.832, 47.833, 47.851, 47.871, 47.889, 47.911, 47.952, 47.963, 47.967, 47.971, 47.982, 47.985, 47.988, 47.993, 48.03, 48.04, 48.054, 48.062, 48.066, 48.065, 48.065, 48.071, 48.061, 48.062, 48.088, 48.086, 48.111, 48.116, 48.122, 48.127, 48.133, 48.133, 48.134, 48.134, 48.135, 48.151, 48.189, 48.204, 48.211, 48.227, 48.227, 48.225, 48.218, 48.2, 48.197, 48.203, 48.2, 48.186, 48.176, 48.167, 48.159, 48.15, 48.149, 48.158, 48.153, 48.13, 48.124, 48.13, 48.158, 48.168, 48.176, 48.177, 48.194, 48.198, 48.203, 48.212, 48.23, 48.237, 48.247, 48.248, 48.245, 48.244, 48.249, 48.268, 48.271, 48.264, 48.256, 48.252, 48.26, 48.28, 48.305, 48.334, 48.414, 48.429, 48.442, 48.464, 48.465, 48.479, 48.482, 48.486, 48.489, 48.493, 48.51, 48.519, 48.526, 48.534, 48.537, 48.569, 48.569, 48.564, 48.546, 48.543, 48.541, 48.519, 48.517, 48.518, 48.515, 48.506, 48.493, 48.489, 48.514, 48.519, 48.513, 48.506, 48.503, 48.509, 48.52, 48.531, 48.54, 48.548, 48.55, 48.55, 48.561, 48.558, 48.545, 48.54, 48.535, 48.526, 48.507, 48.5, 48.495, 48.496, 48.493, 48.482, 48.44, 48.43, 48.372, 48.354, 48.341, 48.334, 48.336, 48.353, 48.357, 48.369, 48.373, 48.375, 48.379, 48.38, 48.376, 48.379, 48.389, 48.405, 48.402, 48.402, 48.402, 48.41, 48.418, 48.415, 48.403, 48.373, 48.357, 48.358, 48.358, 48.349, 48.339, 48.328, 48.314, 48.294, 48.243, 48.239, 48.237, 48.239, 48.243, 48.244, 48.245, 48.244, 48.239, 48.237, 48.238, 48.244, 48.244, 48.244, 48.243, 48.177, 48.157, 48.125, 48.101, 48.097, 48.097, 48.102, 48.102, 48.106, 48.113, 48.116, 48.109, 48.105, 48.091, 48.072, 48.061, 48.047, 48.028, 48.018, 48.009, 47.993, 47.979, 47.967, 47.947 };

        public void Uzd3()
        {
            var taskuSk = 100;
            
            var atstumai = new double[taskuSk];
            var ve = new double[taskuSk];
            var x = new double[taskuSk];
            var y = new double[taskuSk];
            var t = new double[countryX.Length];
            atstumai[0] = 0;
            x[0] = countryX[0];
            y[0] = countryY[0];
            t[0] = 0;

            form.ClearForm(); // išvalomi programos duomenys
            form.PreparareForm(16, 24, 45, 49);
            var S1 = form.chart1.Series.Add("Splainas");
            S1.Color = Color.Red;
            S1.BorderWidth = 3;
            S1.Points.Clear();
            S1.ChartType = SeriesChartType.Line;
            var taskai = form.chart1.Series.Add("taskai");
            taskai.Color = Color.Yellow;
            taskai.BorderWidth = 5;
            taskai.Points.Clear();
            taskai.ChartType = SeriesChartType.Point;

            var Fx = form.chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;
            Fx.Points.Clear();
            Fx.Color = Color.Black;

            for (int i = 1; i < countryX.Length; i++)
            {
                t[i] = t[i - 1] + Math.Sqrt(Math.Pow(countryX[i] - countryX[i - 1], 2) + Math.Pow(countryY[i] - countryY[i - 1], 2));
            }
            for (int i = 0; i < countryX.Length; i++)
            {
                Fx.Points.AddXY(countryX[i], countryY[i]);
            }

            var deltaT = t[countryX.Length - 1] / (taskuSk - 1);

            for (int i = 1; i < taskuSk; i++)
            {
                int j = 0;
                while (j != countryX.Length && t[j] < i * deltaT)
                {
                    j++;
                }
                atstumai[i] = t[j - 1];
                x[i] = countryX[j - 1];
                y[i] = countryY[j - 1];
            }
            for (int i = 0; i < taskuSk; i++)
            {
                taskai.Points.AddXY(x[i], y[i]);
            }

            var x_isvestines = Isvestines(atstumai, x);
            var y_isvestines = Isvestines(atstumai, y);
            ve[0] = 0;
            ve[taskuSk - 1] = 0;
            for (int i = 0; i < taskuSk - 2; i++)
            {
                ve[i + 1] = x_isvestines[i];
            }
            x_isvestines = Vector<double>.Build.DenseOfArray(ve);
            ve[0] = 0;
            ve[taskuSk - 1] = 0;
            for (int i = 0; i < taskuSk - 2; i++)
            {
                ve[i + 1] = y_isvestines[i];
            }
            y_isvestines = Vector<double>.Build.DenseOfArray(ve);

            for (int i = 0; i < taskuSk - 1; i++)
            {
                for (double j = atstumai[i]; j < atstumai[i + 1]; j = j + 0.1)
                {
                    var s = j - atstumai[i];
                    var d = atstumai[i + 1] - atstumai[i];

                    var nx = x_isvestines[i] * (Math.Pow(s, 2) / 2) -
                             x_isvestines[i] * (Math.Pow(s, 3) / (6 * d)) +
                             x_isvestines[i + 1] * (Math.Pow(s, 3) / (6 * d)) + s *
                             ((x[i + 1] - x[i]) / d) - s *
                             (x_isvestines[i] * (d / 3)) - s *
                             (x_isvestines[i + 1] * (d / 6)) + 
                             x[i];

                    var ny = y_isvestines[i] * (Math.Pow(s, 2) / 2) - 
                             y_isvestines[i] * (Math.Pow(s, 3) / (6 * d)) + 
                             y_isvestines[i + 1] * (Math.Pow(s, 3) / (6 * d)) + s * 
                             ((y[i + 1] - y[i]) / d) - s * 
                             (y_isvestines[i] * (d / 3)) - s * 
                             (y_isvestines[i + 1] * (d / 6)) + 
                             y[i];

                    S1.Points.AddXY(nx, ny);
                }
            }
            S1.Points.AddXY(x[0], y[0]);
        }

        public void Uzd4()
        {
            var series = new Series[4];
            form.ClearForm();
            form.PreparareForm(-0, 13, -10, 25);

            var Fx = form.chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;
            Fx.Points.Clear();

            var x = new double[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var dF = Isvestines(x, temperature);
            double[] ve = new double[12];
            ve[0] = 0;
            ve[11] = 0;
            for (int i = 0; i < 12 - 2; i++)
            {
                ve[i + 1] = dF[i];
            }
            dF = Vector<double>.Build.DenseOfArray(ve);

            for (int i = 0; i < 11; i++)
            {
                for (double j = x[i]; j <= x[i + 1]; j = j + 0.1)
                {
                    var s = j - x[i];
                    var d = x[i + 1] - x[i];
                    var y = dF[i] * (Math.Pow(s, 2) / 2) - 
                              dF[i] * (Math.Pow(s, 3) / (6 * d)) + 
                              dF[i + 1] * (Math.Pow(s, 3) / (6 * d)) + s * 
                              ((temperature[i + 1] - temperature[i]) / d) - s * 
                              (dF[i] * (d / 3)) - s * 
                              (dF[i + 1] * (d / 6)) + 
                              temperature[i];

                    Fx.Points.AddXY(j, y);
                }
            }

            var X1X2 = form.chart1.Series.Add("Taskai");
            X1X2.MarkerStyle = MarkerStyle.Circle;
            X1X2.MarkerSize = 8;
            X1X2.ChartType = SeriesChartType.Point;
            X1X2.Points.Clear();

            for (int i = 0; i < 11; i++)
            {
                X1X2.Points.AddXY(x[i], temperature[i]);
            }

            var a = Vector<double>.Build.DenseOfArray(temperature);
            var v = Vector<double>.Build.DenseOfArray(temperature);
            var colors = new[]{ Color.Green, Color.Red, Color.Violet, Color.Teal, Color.Purple };

            double F2(Vector<double> _a, double _x)
            {
                var sum = 0d;
                for (int i = 0; i < _a.Count; i++)
                {
                    sum = sum + _a[i] * Math.Pow(_x, i);
                }
                return sum;
            }

            for (int i = 0; i < 4; i++)
            {
                series[i] = form.chart1.Series.Add("Eilė: " + (i + 2));
                series[i].ChartType = SeriesChartType.Line;
                series[i].Color = colors[i];

                series[i].Points.Clear();
                a = Aproksimavimas(x, 12, i + 2, v);
                for (var j = 1d; j < 12; j = j + 0.1)
                {
                    series[i].Points.AddXY(j, F2(a, j));
                }
            }
        }

        private Vector<double> Aproksimavimas(double[] taskai, int taskuSk, int eile, Vector<double> v)
        {
            var vekt = new double[taskuSk, eile];
            for (var i = 0; i < taskuSk; i++)
            {
                for (var j = 0; j < eile; j++)
                {
                    vekt[i, j] = Math.Pow(taskai[i], j);
                }
            }
            // iš masyvo sugeneruoja matricą, is matricos išskiria eilutę - suformuoja vektorių
            var m = Matrix<double>.Build.DenseOfArray(vekt);
            var a = Vector<double>.Build.DenseOfArray(taskai);
            var mt = m.Transpose();
            a = (mt * m).Solve(mt * v);
            form.richTextBox1.AppendText(m.ToString());
            form.richTextBox1.AppendText(v.ToString());
            form.richTextBox1.AppendText(a.ToString());
            for (var i = 0; i < eile; i++)
            {
                form.richTextBox1.AppendText("(" + a[i] + ")" + "x" + "^" + i + "+");
            }
            form.richTextBox1.AppendText("\n");
            return a;
        }
    }
}
