using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Accord.Math;
using System.Diagnostics;
using Accord.Math.Decompositions;

namespace wpfHelixTest
{
    static class interpolat
    {
        static void test()
        {

            double[] x = { 1, 2, 3 };

            double[] y = { 4, 5, 6 };

            Tuple<double[,], double[,]> M = Matrix.MeshGrid<double>(x, y);

            

            double[,] X = M.Item1;
            double[,] Y = M.Item2;
        }

        public static double interp2(IList<double> x, IList<double> y, double[,] f, double xi, double yi)
        {
            if ((x.Count != f.GetLength(0)) || (y.Count != f.GetLength(1)))
            {
                throw new ArgumentException("Error in interp2: arrays x, y and f dimensions do not agree!");
            }
            int currentXIndex = 1;
            int currentYIndex = 1;

            //if ((xi < x[0]) || (yi < y[0]) || (xi > x[x.Count - 1]) || (yi > y[y.Count - 1])) return double.NaN;
            // changed this to clip to bounds (DC - 7/26/09)
            if ((xi <= x[0]) && (yi <= y[0])) return f[currentXIndex, currentYIndex];
            else if ((xi >= x[x.Count - 1]) && (yi >= y[y.Count - 1])) return f[x.Count - 1, y.Count - 1];
            else if (xi <= x[0]) return Vts.Common.Math.Interpolation.interp1(y, f, yi, 1, 0);
            else if (xi > x[x.Count - 1]) return Vts.Common.Math.Interpolation.interp1(y, f, yi, 1, x.Count - 1);
            else if (yi <= y[0]) return Vts.Common.Math.Interpolation.interp1(x, f, xi, 2, 0);
            else if (yi > y[y.Count - 1]) return Vts.Common.Math.Interpolation.interp1(x, f, xi, 2, y.Count - 1);
            else
            {
                // increment the index until you pass the desired interpolation point
                while (x[currentXIndex] < xi) currentXIndex++;
                while (y[currentYIndex] < yi) currentYIndex++;

                // then do the interp between x[currentXIndex-1] and xi[currentXIndex] and
                //                            y[currentYIndex-1] and yi[currentYIndex]
                double t = (xi - x[currentXIndex - 1]) / (x[currentXIndex] - x[currentXIndex - 1]);
                double u = (yi - y[currentYIndex - 1]) / (y[currentYIndex] - y[currentYIndex - 1]);
                return (1 - t) * (1 - u) * f[currentXIndex - 1, currentYIndex - 1] +
                             t * (1 - u) * f[currentXIndex, currentYIndex - 1] +
                                   t * u * f[currentXIndex, currentYIndex] +
                             (1 - t) * u * f[currentXIndex - 1, currentYIndex];
            }
        }

    }
}
