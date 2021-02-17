using System;
using System.Linq;
using MathNet;
using MathNet.Numerics.Differentiation;
using MathNet.Numerics.LinearAlgebra;

namespace Pvz1
{
    public class Lab2
    {
        private readonly Form1 form;

        public Lab2(Form1 form)
        {
            this.form = form;
        }
        
        private int maxIteraciju = 1000;
        private double eps = 1e-12;

        public void GausoZeidelioMetodas()
        {
            Matrix<double> M = Matrix<double>.Build.DenseOfArray(new double[,] {
                { 9,  3, -1,  2},
                { 3, 11, -2, -2},
                {-1, -2,  6, -1},
                { 2,  2, -1,  9}
            });
            Vector<double> B = Vector<double>.Build.DenseOfArray(new double[]
            {
                65, 27, -23, 39
            });

            Vector<double> alpha = Vector<double>.Build.DenseOfArray(new double[]
            {
                1, 1, 1, 1
            });
            int n = 4;

            var atld = Matrix<double>.Build
                .DenseOfDiagonalVector(M.Diagonal().DivideByThis(1))
                .Multiply(M)
                .Subtract(Matrix<double>.Build.DenseOfDiagonalVector(alpha));
            
            var btld = Matrix<double>.Build
                .DenseOfDiagonalVector(M.Diagonal().DivideByThis(1))
                .Multiply(B);

            var x = Vector<double>.Build.DenseOfArray(new double[] { 0, 0, 0, 0 });
            var x1 = x.Clone();

            for (int i = 0; i < maxIteraciju; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    x1[j] = (btld[j] - atld.Row(j) * x1) / alpha[j];
                }

                form.richTextBox1.AppendText(x1.ToString());
                var tikslumas = (x1 - x).Norm(2) / (x.Norm(2) + x1.Norm(2));

                if (tikslumas < eps)
                {
                    return;    
                }

                x = x1.Clone();
            }

            /* Matlab result
            x =
                6.1361
                0.8621
               -2.0991
                2.5449
 
            */
        }

        public void NiutonoMetodas(bool isF2)
        {
            //Gynimas
            double F1_1(double[] p) => 2 * p[0] + 2 * p[1] - 20;
            double F1_2(double[] p) => p[0] * p[1] - 24.821596d;

            //double F1_1(double[] p) => 10 * p[0] / (p[1] * p[1] + 1) + p[0] * p[0] - p[1] * p[1];
            //double F1_2(double[] p) => p[0] * p[0] + 2 * p[1] * p[1] - 32;
            double F2_1(double[] p) => p[0] + 4 * p[1] + p[2] - 22;
            double F2_2(double[] p) => p[1] * p[2] * 2 * p[2] - 18;
            double F2_3(double[] p) => -1 * p[1] * p[1] + 2 * Math.Pow(p[3], 3) - 3 * p[0] * p[3] + 335;
            double F2_4(double[] p) => 2 * p[2] - 12 * p[1] + 2 * p[3] + 58;

            double a = 1;
            var x = !isF2
                ? new[] {1d, -1d}
                : new[] {1d, 1d, -1d, -1d};

            for (int i = 0; i < 500; i++)
            {
                form.richTextBox1.AppendText($"Iteracija: {i + 1}\n");

                var final = x.ToArray();
                var func = !isF2
                    ? new[] {F1_1(x), F1_2(x)}
                    : new[] {F2_1(x), F2_2(x), F2_3(x), F2_4(x)};

                // Jakobo matrica
                var jacMatrix = !isF2
                    ? new NumericalJacobian()
                        .Evaluate(new Func<double[], double>[] {F1_1, F1_2}, x)
                        .ToMatrix()
                    : new NumericalJacobian()
                        .Evaluate(new Func<double[], double>[] {F2_1, F2_2, F2_3, F2_4}, x)
                        .ToMatrix();


                var deltaX = jacMatrix.Solve(func.ToVector());
                x = (final.ToVector() - a * deltaX).ToArray();

                if (!isF2)
                {
                    form.richTextBox1.AppendText($"x:{x[0],20}, y:{x[1],20}\n");
                }
                else
                {
                    form.richTextBox1.AppendText($"x1:{x[0],20}, x2:{x[1],20}, x3:{x[2],20}, x4:{x[3],20}\n");
                }

                if ((x.ToVector() - final.ToVector()).Norm(2) < 1e-8)
                    break;
            }
        }
    }

    static class MatrixHelper
    {
        public static Vector<double> ToVector(this double[] array)
        {
            return Vector<double>.Build.DenseOfArray(array);
        }

        public static Matrix<double> ToMatrix(this double[,] array)
        {
            return Matrix<double>.Build.DenseOfArray(array);
        }
    }
}
