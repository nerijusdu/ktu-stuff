using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace Pvz1
{
    public partial class Form1 : Form
    {
        List<Timer> Timerlist = new List<Timer>();
        Lab2 lab2;
        Lab3 lab3;
        Lab4 lab4;

        public Form1()
        {
            InitializeComponent();
            Initialize();
            FillIntervals();
            comboBox1.SelectedIndex = 0;
            lab2 = new Lab2(this);
            lab3 = new Lab3(this);
            lab4 = new Lab4(this);
        }

        // ---------------------------------------------- PUSIAUKIRTOS METODAS ----------------------------------------------

        double x1, x2, xtemp; // izoliacijos intervalo pradžia ir galas, vidurio taškas
        int N = 1000; // maksimalus iteracijų skaičius
        int iii; // iteracijos numeris
        private double stepSize = 0.1;

        public Series Fx, X1X2, XMid; // naudojama atvaizduoti f-jai, šaknų rėžiams ir vidiniams taškams


        /// <summary>
        /// Sprendžiama lygtis F(x) = 0
        /// </summary>
        /// <param name="x">funkcijos argumentas</param>
        /// <returns></returns>
        private double F(double x)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    return - 0.95 * Math.Pow(x, 4)
                               + 10.19 * Math.Pow(x, 3)
                               - 37.83 * Math.Pow(x, 2)
                               + 55.58 * x
                               - 24.49;
                case 1: return Math.Sin(x) * (x * x - 1) * (x + 3) - 0.9;
                case 2: return 50 * Math.Pow(Math.E, -x * 3 / 2)
                               + (19.6 / x * Math.Pow(Math.E, -x * 3 / 2) - 1)
                               - 14;
            }

            return 0;
        }
        
        private double dF(double x)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    return - 3.8 * Math.Pow(x, 3)
                           + 30.57 * Math.Pow(x, 2)
                           - 75.66 * x
                           + 55.58;
                case 1:
                    return Math.Sin(x) * (x * x - 1)
                           + Math.Cos(x) * (x * x - 1) * (x + 3)
                           + 2 * x * Math.Sin(x) * (x + 3);
                case 2:
                    return (Math.Pow(Math.E, -x * 3 / 2) * (98 * Math.Pow(Math.E, x * 3 / 2)) - 375 * x * x - 147 * x - 98) / 5 * x * x;
            }

            return 0;
        }

        private Queue<Tuple<double, double>> int1 = new Queue<Tuple<double, double>>();
        private Queue<Tuple<double, double>> int2 = new Queue<Tuple<double, double>>();
        private Queue<Tuple<double, double>> int3 = new Queue<Tuple<double, double>>();

        private Queue<Tuple<double, double>> intervals
        {
            get
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0: return int1;
                    case 1: return int2;
                    case 2: return int3;
                    default: return null;
                }
            }
        }

        private void FillIntervals()
        {
            int1.Clear();
            int2.Clear();
            int3.Clear();

            comboBox1.SelectedIndex = 0;
            x1 = 0.01;
            x2 = 10;
            var xTemp = x1;
            for (x1 += stepSize; x1 < x2; x1 += stepSize)
            {
                if (Math.Sign(F(x1)) != Math.Sign(F(xTemp)))
                {
                    int1.Enqueue(new Tuple<double, double>(xTemp, x1));
                }

                xTemp = x1;
            }

            comboBox1.SelectedIndex = 1;
            x1 = 0;
            x2 = 5;
            xTemp = x1;
            for (x1 += stepSize; x1 < x2; x1 += stepSize)
            {
                if (Math.Sign(F(x1)) != Math.Sign(F(xTemp)))
                {
                    int2.Enqueue(new Tuple<double, double>(xTemp, x1));
                }

                xTemp = x1;
            }

            comboBox1.SelectedIndex = 2;
            x1 = 0.5;
            x2 = 10;
            xTemp = x1;
            for (x1 += stepSize; x1 < x2; x1 += stepSize)
            {
                if (Math.Sign(F(x1)) != Math.Sign(F(xTemp)))
                {
                    int3.Enqueue(new Tuple<double, double>(xTemp, x1));
                }

                xTemp = x1;
            }
        }

        // Mygtukas "Pusiaukirtos metodas" - ieškoma šaknies, ir vizualizuojamas paieškos procesas
        private void button3_Click(object sender, EventArgs e)
        {
            ClearForm(); // išvalomi programos duomenys
            PreparareForm((float)-3.25329, (float)59.505263, (float)-2.0, (float)2.0);
            x1 = 2; // izoliacijos intervalo pradžia
            x2 = 5; // izoliacijos intervalo galas
            iii = 0; // iteraciju skaičius
            richTextBox1.AppendText("Iteracija         x            F(x)        x1          x2          F(x1)         F(x2)       \n");
            // Nubraižoma f-ja, kuriai ieskome saknies
            Fx = chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;
            double x = 0;
            for (int i = 0; i < 50; i++)
            {
                Fx.Points.AddXY(x, F(x));  x = x + (2 * Math.PI) /50;
            }
            Fx.BorderWidth = 3;
            
            X1X2 = chart1.Series.Add("X1X2");
            X1X2.MarkerStyle = MarkerStyle.Circle;
            X1X2.MarkerSize = 8;
            X1X2.ChartType = SeriesChartType.Point;
            X1X2.ChartType = SeriesChartType.Line;


            XMid = chart1.Series.Add("XMid");
            XMid.MarkerStyle = MarkerStyle.Circle;
            X1X2.ChartType = SeriesChartType.Point;
            X1X2.ChartType = SeriesChartType.Line;
            XMid.MarkerSize = 8;

            timer2.Enabled = true;
            timer2.Interval = 500; // timer2 intervalas milisekundemis
            timer2.Start();           
        }
        

        /// <summary>
        /// timer2 iteracijoje atliekami veiksmai
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            xtemp = (x1 + x2) / 2; // apskaiciuojamas vidurinis taskas


            if (Math.Abs(F(xtemp)) > 1e-6 & iii <= N)
            // tikrinama salyga, ar funkcijos absoliuti reiksme daugiau uz nustatyta (norima) 
            // tiksluma ir nevirsytas maksimalus iteraciju skaicius
            {
                X1X2.Points.Clear();
                XMid.Points.Clear();

                X1X2.Points.AddXY(x1, 0);
                X1X2.Points.AddXY(x2, 0);
                XMid.Points.AddXY(xtemp, 0);

                richTextBox1.AppendText(String.Format(" {0,6:d}   {1,12:f7}  {2,12:f7} {3,12:f7} {4,12:f7} {5,12:f7} {6,12:f7}\n",
                    iii, xtemp, F(xtemp), x1, x2, F(x1), F(x2)));
                if (Math.Sign((double)F(x1)) != Math.Sign((double)F(xtemp)))
                {
                    x2 = xtemp;
                }
                else
                {
                    x1 = xtemp;
                }
                iii = iii + 1;

            }
            else
            // skaiciavimai stabdomi
            {
                richTextBox1.AppendText(String.Format(" {0,6:d}   {1,12:f7}  {2,12:f7} {3,12:f7} {4,12:f7} {5,12:f7} {6,12:f7}\n",
                    iii, xtemp, F(xtemp), x1, x2, F(x1), F(x2)));
                richTextBox1.AppendText("Skaičiavimai baigti");
                timer2.Stop();
            }
        }


        // ---------------------------------------------- PARAMETRINĖS FUNKCIJOS ----------------------------------------------

        List<PointF> data = new List<PointF>(); 
        Series S1;

        /// <summary>
        /// Parametrinis interpoliavimas
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            ClearForm(); // išvalomi programos duomenys
            PreparareForm(-10, 10, -10, 10);
            data.Clear();
            // apskaičiuojamos funkcijos reikšmės
            for (double i = 0; i < 2 * Math.PI; i += 0.05)
            {
                //float x = i / 50f * (float)(Math.Sin(2*i / 10f));
                //float y = i / 50f * (float)(Math.Sin(i / 10f));
                var x = (float) (Math.Sin(i) * (Math.Exp(Math.Cos(i)) - 2 * Math.Cos(4 * i) + Math.Pow(Math.Sin(i / 12), 5)));
                var y = (float) (Math.Cos(i) * (Math.Exp(Math.Cos(i)) - 2 * Math.Cos(4 * i) + Math.Pow(Math.Sin(i / 12), 5)));
                data.Add(new PointF(x, y));
            }
            S1 = chart1.Series.Add("S1");
            S1.BorderWidth = 9;
            S1.ChartType = SeriesChartType.Line;
          
            timer3.Enabled = true;
            timer3.Interval = 15;
            timer3.Start();
        }


        private void timer3_Tick(object sender, EventArgs e)
        {
            Series S1 = chart1.Series[0];
            int pointsSoFar = S1.Points.Count;
            if (pointsSoFar < data.Count)
            {
                S1.Points.AddXY(data[pointsSoFar].X, data[pointsSoFar].Y);
            }
            else
            {
                timer1.Stop();
            }
        }

        // ---------------------------------------------- Stygu metodas ----------------------------------------------

        private void button6_Click(object sender, EventArgs e)
        {
            ClearForm();

            double x = 0;
            int i = 0;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    PreparareForm(0, 10, -5, 5);
                    break;
                case 1:
                    PreparareForm(0, 10, -20, 20);
                    break;
                case 2:
                    PreparareForm(0, 10, -20, 20);
                    x = 0.01;
                    i = 1;
                    break;
            }

            richTextBox1.AppendText("Iteracija         x            F(x)        x1          x2          F(x1)         F(x2)       \n");

            Fx = chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;
            for (; i < 50; i++)
            {
                Fx.Points.AddXY(x, F(x)); x = x + (2 * Math.PI) / 50;
            }
            Fx.BorderWidth = 3;

            X1X2 = chart1.Series.Add("X1X2");
            X1X2.MarkerStyle = MarkerStyle.Circle;
            X1X2.MarkerSize = 8;
            X1X2.ChartType = SeriesChartType.Point;
            X1X2.ChartType = SeriesChartType.Line;


            XMid = chart1.Series.Add("XMid");
            XMid.MarkerStyle = MarkerStyle.Circle;
            X1X2.ChartType = SeriesChartType.Point;
            X1X2.ChartType = SeriesChartType.Line;
            XMid.MarkerSize = 8;

            var thing = intervals.Dequeue();
            x1 = thing.Item1;
            x2 = thing.Item2;

            timer4.Enabled = true;
            timer4.Interval = 500;
            timer4.Start();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            var cof = Math.Abs(F(x1) / F(x2));
            xtemp = (x1 + cof * x2) / (1 + cof);

            if (Math.Abs(F(xtemp)) > 1e-6 & iii <= N)
            {
                X1X2.Points.Clear();
                XMid.Points.Clear();

                X1X2.Points.AddXY(x1, F(x1));
                X1X2.Points.AddXY(x2, F(x2));
                XMid.Points.AddXY(xtemp, 0);

                richTextBox1.AppendText($" {iii,6:d}   {xtemp,12:f7}  {F(xtemp),12:f7} {x1,12:f7} {x2,12:f7} {F(x1),12:f7} {F(x2),12:f7}\n");

                if (Math.Sign(F(x1)) != Math.Sign(F(xtemp)))
                {
                    x2 = xtemp;
                }
                else
                {
                    x1 = xtemp;
                }
                iii = iii + 1;

            }
            else
            {
                richTextBox1.AppendText($" {iii,6:d}   {xtemp,12:f7}  {F(xtemp),12:f7} {x1,12:f7} {x2,12:f7} {F(x1),12:f7} {F(x2),12:f7}\n");
                richTextBox1.AppendText("Skaičiavimai baigti");
                if (intervals.Any())
                {
                    var thing = intervals.Dequeue();
                    x1 = thing.Item1;
                    x2 = thing.Item2;
                }
                else
                {
                    timer4.Stop();
                }
            }
        }

        // ---------------------------------------------- intervalu skenavimo metodas -----------------------------------
        private void button7_Click(object sender, EventArgs e)
        {
            ClearForm();

            double x = 0;
            int i = 0;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    PreparareForm(0, 10, -5, 5);
                    break;
                case 1:
                    PreparareForm(0, 10, -20, 20);
                    break;
                case 2:
                    PreparareForm(0, 10, -20, 20);
                    x = 0.01;
                    i = 1;
                    break;
            }

            richTextBox1.AppendText("Iteracija         x            F(x)        x1          x2          F(x1)         F(x2)       \n");

            Fx = chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;
            for (; i < 50; i++)
            {
                Fx.Points.AddXY(x, F(x)); x = x + (2 * Math.PI) / 50;
            }
            Fx.BorderWidth = 3;

            X1X2 = chart1.Series.Add("X1X2");
            X1X2.MarkerStyle = MarkerStyle.Circle;
            X1X2.MarkerSize = 8;
            X1X2.ChartType = SeriesChartType.Point;
            X1X2.ChartType = SeriesChartType.Line;


            XMid = chart1.Series.Add("XMid");
            XMid.MarkerStyle = MarkerStyle.Circle;
            X1X2.ChartType = SeriesChartType.Point;
            X1X2.ChartType = SeriesChartType.Line;
            XMid.MarkerSize = 8;
            
            var thing = intervals.Dequeue();
            x1 = thing.Item1;
            x2 = thing.Item2;

            timer5.Enabled = true;
            timer5.Interval = 50;
            timer5.Start();
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            xtemp = x1 + stepSize;

            if (Math.Abs(F(xtemp)) > 1e-6 & iii <= N)
            {
                X1X2.Points.Clear();

                X1X2.Points.AddXY(x1, 0);
                X1X2.Points.AddXY(xtemp, 0);

                richTextBox1.AppendText($" {iii,6:d}   {xtemp,12:f7}  {F(xtemp),12:f7} {x1,12:f7} {x2,12:f7} {F(x1),12:f7} {F(x2),12:f7}\n");

                if (Math.Sign(F(x1)) != Math.Sign(F(xtemp)))
                {
                    stepSize /= 10;
                }
                else
                {
                    x1 += stepSize;
                }


                iii = iii + 1;
            }
            else
            {
                richTextBox1.AppendText($" {iii,6:d}   {xtemp,12:f7}  {F(xtemp),12:f7} {x1,12:f7} {x2,12:f7} {F(x1),12:f7} {F(x2),12:f7}\n");
                richTextBox1.AppendText("Skaičiavimai baigti");
                if (intervals.Any())
                {
                    var thing = intervals.Dequeue();
                    x1 = thing.Item1;
                    x2 = thing.Item2;
                    stepSize = 0.1;
                }
                else
                {
                    timer5.Stop();
                }
            }
        }

        // ---------------------------------------------- Liestiniu metodas ----------------------------------------------
        
        private void button8_Click(object sender, EventArgs e)
        {
            ClearForm();

            double x = 0;
            int i = 0;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    PreparareForm(0, 10, -5, 5);
                    x1 = 1;
                    break;
                case 1:
                    PreparareForm(0, 10, -20, 20);
                    x1 = 1;
                    break;
                case 2:
                    PreparareForm(0, 10, -20, 20);
                    x1 = 1.5;
                    x = 0.01;
                    i = 1;
                    break;
            }

            richTextBox1.AppendText("Iteracija         x            F(x)        x1          x2          F(x1)         F(x2)       \n");

            Fx = chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;
            for (; i < 50; i++)
            {
                Fx.Points.AddXY(x, F(x)); x = x + (2 * Math.PI) / 50;
            }
            Fx.BorderWidth = 3;

            X1X2 = chart1.Series.Add("X1X2");
            X1X2.MarkerStyle = MarkerStyle.Circle;
            X1X2.MarkerSize = 8;
            X1X2.ChartType = SeriesChartType.Point;
            X1X2.ChartType = SeriesChartType.Line;


            XMid = chart1.Series.Add("XMid");
            XMid.MarkerStyle = MarkerStyle.Circle;
            X1X2.ChartType = SeriesChartType.Point;
            X1X2.ChartType = SeriesChartType.Line;
            XMid.MarkerSize = 8;

            timer6.Enabled = true;
            timer6.Interval = 500;
            timer6.Start();
        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            xtemp = -F(x1) / dF(x1);
            x1 += xtemp;
            xtemp = x1;

            if (Math.Abs(F(x1)) > 1e-6 & iii <= N)
            {
                X1X2.Points.Clear();
                XMid.Points.Clear();
                
                XMid.Points.AddXY(x1, 0);

                richTextBox1.AppendText($" {iii,6:d}   {xtemp,12:f7}  {F(xtemp),12:f7} {x1,12:f7} {x2,12:f7} {F(x1),12:f7} {F(x2),12:f7}\n");
                
                iii = iii + 1;
            }
            else
            {
                richTextBox1.AppendText($" {iii,6:d}   {xtemp,12:f7}  {F(xtemp),12:f7} {x1,12:f7} {x2,12:f7} {F(x1),12:f7} {F(x2),12:f7}\n");
                richTextBox1.AppendText("Skaičiavimai baigti");
                timer6.Stop();
            }
        }

        // ------------------------------------------------ 3 UZD --------------------------------------------------------

        private void button10_Click(object sender, EventArgs e)
        {
            lab3.Uzd11();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            lab3.Uzd12();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            lab3.Uzd21();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            lab3.Uzd22();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            lab3.Uzd3();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            lab3.Uzd4();
        }

        // ------------------------------------------------ 2 UZD --------------------------------------------------------

        private void button11_Click(object sender, EventArgs e)
        {
            lab2.NiutonoMetodas(false);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            lab2.NiutonoMetodas(true);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            lab2.GausoZeidelioMetodas();
        }

        // ------------------------------------------------ 4 UZD --------------------------------------------------------

        private void button13_Click(object sender, EventArgs e)
        {
            var deltaT = 0.1d;
            //lab4.Run(deltaT);
            lab4.RunAnalysis();
        }

        // ---------------------------------------------- TIESINĖ ALGEBRA ----------------------------------------------

        /// <summary>
        /// Tiesine algebra (naudojama MathNet)
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            ClearForm();

            double[,] x = { { 1, 2, 3 }, { 3, 4, 5 }, { 6, 5, 8 } };
            // iš masyvo sugeneruoja matricą, is matricos išskiria eilutę - suformuoja vektorių
            Matrix<double> m = Matrix<double>.Build.DenseOfArray(x);
            Vector<double> v = m.Row(1);
            richTextBox1.AppendText("\nMatrica m:\n");
            richTextBox1.AppendText(m.ToString());

            richTextBox1.AppendText("\nVektorius v:\n");
            richTextBox1.AppendText(v.ToString());

            richTextBox1.AppendText("\ntranspose(m):\n");
            richTextBox1.AppendText(m.Transpose().ToString());

            Matrix<double> vm = v.ToRowMatrix();
            richTextBox1.AppendText("\nvm = v' - toRowMatrix()\n");
            richTextBox1.AppendText(vm.ToString());

            Vector<double> v1 = m * v;
            richTextBox1.AppendText("\nv1 = m * v\n");
            richTextBox1.AppendText(v1.ToString());
            richTextBox1.AppendText("\nmin(v1)\n");
            richTextBox1.AppendText(v1.Min().ToString());

            Matrix<double> m1 = m.Inverse();
            richTextBox1.AppendText("\ninverse(m)\n");
            richTextBox1.AppendText(m1.ToString());

            richTextBox1.AppendText("\ndet(m)\n");
            richTextBox1.AppendText(m.Determinant().ToString());

            // you must add reference to assembly system.Numerics
            Evd<double> eigenv = m.Evd();
            richTextBox1.AppendText("\neigenvalues(m)\n");
            richTextBox1.AppendText(eigenv.EigenValues.ToString());
            
            LU<double> LUanswer = m.LU();
            richTextBox1.AppendText("\nMatricos M LU skaida\n");
            richTextBox1.AppendText("\nMatrica L:\n");
            richTextBox1.AppendText(LUanswer.L.ToString());
            richTextBox1.AppendText("\nMatrica U:\n");
            richTextBox1.AppendText(LUanswer.U.ToString());
            
            QR<double> QRanswer = m.QR();
            richTextBox1.AppendText("\nMatricos M QR skaida\n");
            richTextBox1.AppendText("\nMatrica Q:\n");
            richTextBox1.AppendText(QRanswer.Q.ToString());
            richTextBox1.AppendText("\nMatrica R:\n");
            richTextBox1.AppendText(QRanswer.R.ToString());

            Vector<double> v3 = m.Solve(v);
            richTextBox1.AppendText("\nm*v3 = v sprendziama QR metodu\n");
            richTextBox1.AppendText(v3.ToString());
            richTextBox1.AppendText("Patikrinimas\n");
            richTextBox1.AppendText((m * v3 - v).ToString());
            
        }
        

        // ---------------------------------------------- KITI METODAI ----------------------------------------------

        /// <summary>
        /// Uždaroma programa
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        /// <summary>
        /// Išvalomas grafikas ir consolė
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        

        public void ClearForm()
        {
            foreach (var timer in Timerlist)
            {
                timer.Stop();
            }

            var s = comboBox1.SelectedIndex;
            FillIntervals();
            comboBox1.SelectedIndex = s;

            stepSize = 0.1;
            richTextBox1.Clear();
            chart1.Series.Clear();
        }

    }
}
