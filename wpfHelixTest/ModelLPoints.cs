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
        public Dictionary<Tuple<int, int>, double> dic = new Dictionary<Tuple<int, int>, double>();

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

            //for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            //{
            //    int index0 = newMesh.TriangleIndices[i];
            //    int index1 = newMesh.TriangleIndices[i + 1];
            //    int index2 = newMesh.TriangleIndices[i + 2];

            //    Point3D p0 = newMesh.Positions[index0];
            //    Point3D p1 = newMesh.Positions[index1];
            //    Point3D p2 = newMesh.Positions[index2];

            //    p0.X = Math.Round(p0.X * 10);
            //    p0.Y = Math.Round(p0.Y * 10);
            //    p0.Z = Math.Round(p0.Z * 10);

            //    newMesh.Positions[index0] = p0;

            //    p1.X = Math.Round(p1.X * 10);
            //    p1.Y = Math.Round(p1.Y * 10);
            //    p1.Z = Math.Round(p1.Z * 10);

            //    newMesh.Positions[index1] = p1;

            //    p2.X = Math.Round(p2.X * 10);
            //    p2.Y = Math.Round(p2.Y * 10);
            //    p2.Z = Math.Round(p2.Z * 10);

            //    newMesh.Positions[index2] = p2;
            //}

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
                pointsOfTriangle(p0, p1, p2);
            }
            

            return newMesh;
        }

        void pointsOfTriangle(Point3D p0, Point3D p1, Point3D p2)
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

                    double s = (double)crossProduct(q, vs2) / crossProduct(vs1, vs2);
                    double t = (double)crossProduct(vs1, q) / crossProduct(vs1, vs2);
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

        double crossProduct(Point3D p1, Point3D p2)
        {

            //std::cout << "crossProduct p1.x " << p1.x << " p2.y " << p2.y << " p1.y " << p1.y << " p2.x " << p2.x << std::endl;
            return p1.X * p2.Y - p1.Y * p2.X;
        }

        // If the point already exists, return its index.
        // Otherwise create the point and return its new index.
        private int AddPoint(Point3DCollection points, PointCollection texture_coords, Point3D point)
        {
            // If the point is in the point dictionary,
            // return its saved index.
            if (PointDictionary.ContainsKey(point))
                return PointDictionary[point];

            // We didn't find the point. Create it.
            points.Add(point);
            PointDictionary.Add(point, points.Count - 1);

            // Return the new point's index.
            return points.Count - 1;
        }
    }
}
