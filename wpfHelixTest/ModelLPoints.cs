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
        public MeshBuilder modelMeshBuilder = new MeshBuilder(true, true);

        private Dictionary<Point3D, int> PointDictionary = new Dictionary<Point3D, int>();

        public SortedDictionary<Tuple<int, int>, double> dic = new SortedDictionary<Tuple<int, int>, double>();

        private int offsetX { get; set; }
        private int offsetY { get; set; }

        private double averageValue {get; set; }

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

                int index0 = newMesh.TriangleIndices[i];
                int index1 = newMesh.TriangleIndices[i + 1];
                int index2 = newMesh.TriangleIndices[i + 2];
                             
                Point3D p0 = newMesh.Positions[index0];
                Point3D p1 = newMesh.Positions[index1];
                Point3D p2 = newMesh.Positions[index2];

                
                // Add the rest of the points of the triangle in dictionary
                PointsOfTriangle(p0, p1, p2, newMesh, i);
            }

            this.offsetX = Convert.ToInt32(newMesh.Bounds.X);
            this.offsetY = Convert.ToInt32(newMesh.Bounds.Y);

            return newMesh;
        }

        void PointsOfTriangle(Point3D p0, Point3D p1, Point3D p2, MeshGeometry3D newMesh, int i)
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
                        //Tuple<int, int> key = new Tuple<int, int>(x - this.offsetX), y - this.offsetY);
                        Tuple<int, int> key = new Tuple<int, int>(x, y);

                        if (!dic.ContainsKey(key))
                        {
                            dic.Add(key, Math.Floor(q.Z));
                            
                            modelMeshBuilder.Positions.Add(new Point3D(x, y, q.Z));
                            modelMeshBuilder.Normals.Add(newMesh.Normals[i]);
                            modelMeshBuilder.TextureCoordinates.Add(newMesh.TextureCoordinates[i]);
                        }
                    }
                }
            }
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

                //Tuple<int, int> key = new Tuple<int, int>(x - this.offsetX, y - this.offsetY);
                Tuple<int, int> key = new Tuple<int, int>(x, y);

                newDictionary.Add(key, sd.deltaZ);

                dic[key] = sd.deltaZ;
            }

            PreProcessing(newDictionary);

            //while (dic.Count > 0)
            //{
            //    int index = dic.Count / 2;

            //    KeyValuePair<Tuple<int,int>, double> v = dic.ElementAt(index);

            //    List<KeyValuePair<Tuple<int, int>, double>> keyPoints = GetNeighboringPoints(v.Key.Item1, v.Key.Item2, newDictionary);

            //    double value = EuclideanWeightedAverage(keyPoints, newDictionary);

            //    newDictionary.Add(v.Key, value);

            //    dic.Remove(v.Key);
            //}
        }

        public void PreProcessing(Dictionary<Tuple<int, int>, double> newDictionary)
        {

            Dictionary<Tuple<int, int>, double> sortDic = new Dictionary<Tuple<int, int>, double>();
            Dictionary<Tuple<int, int>, double> ndic = new Dictionary<Tuple<int, int>, double>();

            foreach (KeyValuePair<Tuple<int, int>, double> v in dic)
            {
                int nx = v.Key.Item1;
                int ny = v.Key.Item2;


                //foreach(KeyValuePair<Tuple<int, int>, double> sensor in newDictionary)
                //{
                //    int x = sensor.Key.Item1;
                //    int y = sensor.Key.Item2;

                //    double euclideanDistance = Math.Sqrt(Math.Pow(nx - x, 2) + Math.Pow(ny - y, 2));

                //    sortDic.Add(sensor.Key, euclideanDistance);
                //}

                //// Order by values.
                //IEnumerable<KeyValuePair<Tuple<int, int>, double>> items = from pair in sortDic
                //                                                           orderby pair.Value ascending
                //                                                           select pair;

                //ndic[v.Key] = (newDictionary[items.ElementAt(0).Key] + newDictionary[items.ElementAt(1).Key]) / 2;

                List<Tuple<int, int>> neighbors = GetNeighboringPoints(nx, ny, newDictionary);

                ndic[v.Key] = (newDictionary[neighbors.ElementAt(0)] + newDictionary[neighbors.ElementAt(1)]) / 2;

                //sortDic.Clear();
            }

            foreach (KeyValuePair<Tuple<int, int>, double> sd in newDictionary)
            {
                int x = Convert.ToInt32((sd.Key.Item1));
                int y = Convert.ToInt32((sd.Key.Item2));

                //Tuple<int, int> key = new Tuple<int, int>(x - this.offsetX, y - this.offsetY);
                Tuple<int, int> key = new Tuple<int, int>(x, y);
                ndic[key] = sd.Value;
            }

            StartInterpolation(ndic, newDictionary);

        }

        public void StartInterpolation(Dictionary<Tuple<int, int>, double> ndic, Dictionary<Tuple<int, int>, double> sensorDictionary)
        {
            this.averageValue = 0.006;

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\heitor.araujo\source\repos\wpfHelixTest\interpolation.txt", true))
            {
                foreach (KeyValuePair<Tuple<int, int>, double> item in ndic)
                {
                    if (sensorDictionary.ContainsKey(item.Key))
                    {
                        continue;
                    }

                    int x = item.Key.Item1;
                    int y = item.Key.Item2;

                    List<Tuple<int, int>> neighbors = GetNeighboringPoints(x, y, sensorDictionary);

                    Tuple<int, int> q11 = neighbors[0];
                    Tuple<int, int> q22 = neighbors[1];

                    Tuple<int, int> q12 = new Tuple<int, int>(neighbors[0].Item1, neighbors[1].Item2);
                    Tuple<int, int> q21 = new Tuple<int, int>(neighbors[1].Item1, neighbors[0].Item2);

                    double q12Value, q21Value;

                    if (!ndic.ContainsKey(q12))
                    {
                        q12Value = averageValue;
                    }
                    else
                    {
                        q12Value = ndic[q12];
                    }

                    if (!ndic.ContainsKey(q21))
                    {
                        q21Value = averageValue;
                    }
                    else
                    {
                        q21Value = ndic[q21];
                    }

                    double ret = BilinearInterpolation(ndic[q11], q12Value, q21Value, ndic[q22], neighbors[0].Item1, neighbors[1].Item1, neighbors[0].Item2, neighbors[1].Item2, x, y);

                    string r = "" + x + " " + y + " " + ret;

                    file.WriteLine(r);
                }
            }            
        }

        public List<Tuple<int, int>> GetNeighboringPoints(int x, int y, Dictionary<Tuple<int, int>, double> newDictionary)
        {

            List<Tuple<int, int>> ret = new List<Tuple<int, int>>();

            double min1 = double.MaxValue, min2 = double.MaxValue;
            ret.Add(newDictionary.ElementAt(0).Key);
            ret.Add(newDictionary.ElementAt(1).Key);

            foreach (KeyValuePair<Tuple<int, int>, double> v in newDictionary)
            {
                int nx = v.Key.Item1;
                int ny = v.Key.Item2;

                double euclideanDistance = Math.Sqrt(Math.Pow(nx-x, 2) + Math.Pow(ny-y, 2));

                if (euclideanDistance < min1)
                {
                    min2 = min1;
                    min1 = euclideanDistance;

                    ret[1] = ret[0];
                    ret[0] = v.Key;
                    
                }
                else if (euclideanDistance < min2)
                {
                    min2 = euclideanDistance;
                    ret[1] = v.Key;
                }
            }

            return ret;
        }        

        //public double EuclideanWeightedAverage(List<KeyValuePair<Tuple<int, int>, double>> keys, Dictionary<Tuple<int, int>, double> nDic)
        //{

        //    double w1 = 1/keys.ElementAt(0).Value;
        //    double w2 = 1/keys.ElementAt(1).Value;
        //    double w3 = 1/keys.ElementAt(2).Value;
        //    double w4 = 1/keys.ElementAt(3).Value;

        //    double num = (w1 * nDic[keys.ElementAt(0).Key] + w2 * nDic[keys.ElementAt(1).Key] + w3 * nDic[keys.ElementAt(2).Key] + w4* nDic[keys.ElementAt(3).Key]);
        //    double den = w1 + w2 + w3 + w4;

        //    return num / den;
        //}

        public double BilinearInterpolation(double q11, double q12, double q21, double q22, float x1, float x2, float y1, float y2, float x, float y)
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

        double CrossProduct(Point3D p1, Point3D p2)
        {

            //std::cout << "crossProduct p1.x " << p1.x << " p2.y " << p2.y << " p1.y " << p1.y << " p2.x " << p2.x << std::endl;
            return p1.X * p2.Y - p1.Y * p2.X;
        }

    }
}

