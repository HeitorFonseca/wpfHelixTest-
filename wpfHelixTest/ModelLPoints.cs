using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace wpfHelixTest
{
    public class ModelLPoints
    {
        private Dictionary<Point3D, int> PointDictionary = new Dictionary<Point3D, int>();

        public SortedDictionary<Tuple<int, int>, double> dic = new SortedDictionary<Tuple<int, int>, double>();

        public ModelLPoints()
        {

        }

        public MeshGeometry3D BuildDic (MeshGeometry3D mesh)
        {
            // Make a mesh to hold the surface.
            MeshGeometry3D newMesh = new MeshGeometry3D();

            newMesh = mesh.Clone();

            var p = new Point3DCollection();
            var ti = new Int32Collection();

            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                int index0 = newMesh.TriangleIndices[i];
                int index1 = newMesh.TriangleIndices[i + 1];
                int index2 = newMesh.TriangleIndices[i + 2];

                Point3D p0 = newMesh.Positions[index0];
                Point3D p1 = newMesh.Positions[index1];
                Point3D p2 = newMesh.Positions[index2];

                p0.X = Math.Round(p0.X );
                p0.Y = Math.Round(p0.Y );
                p0.Z = Math.Round(p0.Z );

                newMesh.Positions[index0] = p0;

                p1.X = Math.Round(p1.X );
                p1.Y = Math.Round(p1.Y );
                p1.Z = Math.Round(p1.Z );

                newMesh.Positions[index1] = p1;

                p2.X = Math.Round(p2.X );
                p2.Y = Math.Round(p2.Y );
                p2.Z = Math.Round(p2.Z );

                newMesh.Positions[index2] = p2;
            }

            for (int i = 0; i < newMesh.TriangleIndices.Count; i += 3)
            {
                //// Add the 3 points of the triangle in dictionary
                //for (int j = i; j < 3; j++)
                //{
                //    int index = newMesh.TriangleIndices[j];
                //    Point3D point = newMesh.Positions[index];

                //    int newX = Convert.ToInt32(Math.Round(point.X));
                //    int newY = Convert.ToInt32(Math.Round(point.Y));

                //    Tuple<int, int> key = new Tuple<int, int>(newX, newY);
                //    dic.Add(key, Math.Floor(point.Z));
                //}

                int index0 = newMesh.TriangleIndices[i];
                int index1 = newMesh.TriangleIndices[i + 1];
                int index2 = newMesh.TriangleIndices[i + 2];
                             
                Point3D p0 = newMesh.Positions[index0];
                Point3D p1 = newMesh.Positions[index1];
                Point3D p2 = newMesh.Positions[index2];

                // Add the rest of the points of the triangle in dictionary
                PointsOfTriangle(p0, p1, p2);
            }
            

            return newMesh;
        }

        void PointsOfTriangle(Point3D p0, Point3D p1, Point3D p2)
        {
            int maxX = Convert.ToInt32(Math.Max(p0.X, Math.Max(p1.X, p2.X)));
            int minX = Convert.ToInt32(Math.Min(p0.X, Math.Min(p1.X, p2.X)));
            int maxY = Convert.ToInt32(Math.Max(p0.Y, Math.Max(p1.Y, p2.Y)));
            int minY = Convert.ToInt32(Math.Min(p0.Y, Math.Min(p1.Y, p2.Y)));
                                                                                  
            Point3D vs1 = new Point3D(p1.X - p0.X, p1.Y - p0.Y, 0);
            Point3D vs2 = new Point3D(p2.X - p0.X, p2.Y - p0.Y, 0);            

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y<=maxY; y++)
                {
                    Point3D q = new Point3D(x - p0.X, y - p0.Y, 0);

                    double s = (double)CrossProduct(q, vs2) / CrossProduct(vs1, vs2);
                    double t = (double)CrossProduct(vs1, q) / CrossProduct(vs1, vs2);
                    if ((s >= 0) && (t >= 0) && (s + t <= 1))
                    {

                        //int newX = Convert.ToInt32(Math.Round(x));
                        //int newY = Convert.ToInt32(Math.Round(y));

                        Tuple<int, int> key = new Tuple<int, int>(x, y);

                        if(!dic.ContainsKey(key))
                        dic.Add(key, Math.Floor(q.Z));
                    }
                }
            }
        }

        double CrossProduct(Point3D p1, Point3D p2)
        {

            //std::cout << "crossProduct p1.x " << p1.x << " p2.y " << p2.y << " p1.y " << p1.y << " p2.x " << p2.x << std::endl;
            return p1.X * p2.Y - p1.Y * p2.X;
        }

        public void FillSensorDataDictionary(List<SensorsData> sensorsDataList)
        {

            Dictionary<Tuple<int, int>, double> newDictionary = new Dictionary<Tuple<int, int>, double>();


            // Acquire keys and sort them.
            //var list = dic.Keys.ToList();
            //list.Sort();

            foreach (SensorsData sd in sensorsDataList)
            {
                int x = Convert.ToInt32(Math.Round(sd.x));
                int y = Convert.ToInt32(Math.Round(sd.y));

                Tuple<int, int> key = new Tuple<int, int>(x, y);

                newDictionary.Add(key, sd.deltaZ);

                dic.Remove(key);
            }

            while (dic.Count > 0)
            {
                int index = dic.Count / 2;

                KeyValuePair<Tuple<int,int>, double> v = dic.ElementAt(index);

                List<KeyValuePair<Tuple<int, int>, double>> keyPoints = GetNeighboringPoints(v.Key.Item1, v.Key.Item2, newDictionary);

                double value = EuclideanWeightedAverage(keyPoints, newDictionary);

                newDictionary.Add(v.Key, value);

                dic.Remove(v.Key);
            }
        }

        public List<KeyValuePair<Tuple<int, int>, double>> GetNeighboringPoints(int x, int y, Dictionary<Tuple<int, int>, double> newDictionary)
        {

            List<KeyValuePair<Tuple<int, int>, double>> ret = new List<KeyValuePair<Tuple<int, int>, double>>();

            SortedDictionary<Tuple<int, int>, double> sortDic = new SortedDictionary<Tuple<int, int>, double>();          

            foreach (KeyValuePair<Tuple<int, int>, double> v in newDictionary)
            {
                int nx = v.Key.Item1;
                int ny = v.Key.Item2;

                double euclideanDistance = Math.Sqrt(Math.Pow(nx-x, 2) + Math.Pow(ny-y, 2));

                sortDic.Add(v.Key, euclideanDistance);               
            }

            // Order by values.
            // ... Use LINQ to specify sorting by value.
            IEnumerable<KeyValuePair<Tuple<int, int>, double>> items = from pair in sortDic
                                                                        orderby pair.Value ascending
                                                                        select pair;

            ret.Add(items.ElementAt(0));
            ret.Add(items.ElementAt(1));
            ret.Add(items.ElementAt(2));
            ret.Add(items.ElementAt(3));

            return ret;
        }

        public double EuclideanWeightedAverage(List<KeyValuePair<Tuple<int, int>, double>> keys, Dictionary<Tuple<int, int>, double> nDic)
        {

            double w1 = 1/keys.ElementAt(0).Value;
            double w2 = 1/keys.ElementAt(1).Value;
            double w3 = 1/keys.ElementAt(2).Value;
            double w4 = 1/keys.ElementAt(3).Value;

            double num = (w1 * nDic[keys.ElementAt(0).Key] + w2 * nDic[keys.ElementAt(1).Key] + w3 * nDic[keys.ElementAt(2).Key] + w4* nDic[keys.ElementAt(3).Key]);
            double den = w1 + w2 + w3 + w4;

            return num / den;
        }

        public double BilinearInterpolation(float q11, float q12, float q21, float q22, float x1, float x2, float y1, float y2, float x, float y)
        {
            float x2x1, y2y1, x2x, y2y, yy1, xx1;
            x2x1 = x2 - x1;
            y2y1 = y2 - y1;
            x2x = x2 - x;
            y2y = y2 - y;
            yy1 = y - y1;
            xx1 = x - x1;
            return 1.0 / (x2x1 * y2y1) * (q11 * x2x * y2y + q21 * xx1 * y2y + q12 * x2x * yy1 + q22 * xx1 * yy1);
        }
    }
}
