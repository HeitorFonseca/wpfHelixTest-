using HelixToolkit.Wpf;
using InteractiveDataDisplay.WPF;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using OpenTK;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;


namespace wpfHelixTest
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private int WINDOW_X_SIZE = 10;
        private List<Color> colors = new List<Color> {  Color.FromRgb( 140, 140, 140), Color.FromRgb(255, 0, 0), Color.FromRgb(0, 255, 255),
                                                        Color.FromRgb(255, 255, 0), Color.FromRgb(255, 0, 255), Color.FromRgb(0, 0, 255),
                                                        Color.FromRgb(0, 255, 0),  Color.FromRgb( 0, 0, 0) };

        private Dictionary<string, OpticalSensor> opticalSensorsDic = new Dictionary<string, OpticalSensor>();

        private string HostName { get; set; }
        private int Port { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }


        ProtocolData proc = new ProtocolData("localhost", 5672, "userTest", "userTest", "hello");        
        List<SensorsData> sensorsDataList = new List<SensorsData>();

        public Func<double, string> Formatter { get; set; }

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels = new List<string>();

        ModelVisual3D device3D;
        Model3DGroup groupModel = new Model3DGroup();

        MeshGeometry3D modelMesh = new MeshGeometry3D();
        MeshGeometry3D rectangleMesh = new MeshGeometry3D();

        private const double xmin = -592.5;
        private const double xmax = 592.5;

        private const double ymin = -210;
        private const double ymax = 210;

        private const double texture_xscale = (xmax - xmin);
        private const double texture_yscale = (ymax - ymin);

        public MainWindow()
        {
            InitializeComponent();

            DefineLights();

            this.device3D = new ModelVisual3D();

            //proc.HostName = "localhost";
            //proc.Username = "userTest";
            //proc.Password = "userTest";
            //proc.Port = 5672;
            //proc.QueueName = "hello";

            proc.Connect();
            proc.ReadEvnt(WhenMessageReceived);

            var dayConfig = Mappers.Xy<DateModel>()
                .X(dayModel => (double)dayModel.DateTime.Ticks / TimeSpan.FromHours(1).Ticks)
                .Y(dayModel => dayModel.Value);

            SeriesCollection = new SeriesCollection(dayConfig);

            Formatter = value => new System.DateTime((long)(value * TimeSpan.FromHours(1).Ticks)).ToString("t");

            DataContext = this;
        
        }

        // Define the lights.
        private void DefineLights()
        {
            AmbientLight ambient_light = new AmbientLight(Colors.Gray);
            DirectionalLight directional_light =
                new DirectionalLight(Colors.Gray, new Vector3D(-1.0, -3.0, -2.0));
            groupModel.Children.Add(ambient_light);
            groupModel.Children.Add(directional_light);
        }

        /// <summary>
        /// Display 3D Model
        /// </summary>
        /// <param name="model">Path to the Model file</param>
        /// <returns>3D Model Content</returns>
        private Model3D Display3d(string model)
        {
            Model3D device = null;
            try
            {
                //Adding a gesture here
                viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);

                //Import 3D model file
                ModelImporter import = new ModelImporter();

                Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Black));
                import.DefaultMaterial = material;

                //Load the 3D model file
                device = import.Load(model);

                int countVertices = 0;

                Action<GeometryModel3D, Transform3D> nameAction = ((geometryModel, transform) =>
                {
                    modelMesh = (MeshGeometry3D)geometryModel.Geometry;
                    countVertices += modelMesh.Positions.Count;
                });
                
                device.Traverse(nameAction);
            }            
            catch (Exception e)
            {
                // Handle exception in case can not find the 3D model file
                MessageBox.Show("Exception Error : " + e.StackTrace);
            }
                        
            return device;
        }

        private void LoadStlBntClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "STL (*.stl)|*.stl|IGES (*.igs;*.iges)|*.igs;*.iges|STEP (*.stp;*.step)|*.stp;*.step|All Files(*.*)|*.*";

            if (dlg.ShowDialog() == true)
            {
                this.viewPort3d.Children.Remove(device3D);

                string filePath = dlg.FileName;

                groupModel.Children.Add(Display3d(filePath));

                device3D.Content = groupModel;

                // Add to view port
                //this.viewPort3d.Children.Add(device3D);
                //this.viewPort3d.ZoomExtents();


            }
        }

        private void LoadSensorsBtnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text (*.txt)|*.txt|All Files(*.*)|*.*";

            if (dlg.ShowDialog() == true)
            {
                string filePath = dlg.FileName;

                string[] lines = System.IO.File.ReadAllLines(filePath);

                int counter = 1;

                Vector3 X = new Vector3();

                foreach (string line in lines)
                {
                    string[] data = line.Split(' ');

                    Vector3 aux = new Vector3(float.Parse(data[0], CultureInfo.InvariantCulture.NumberFormat),
                                              float.Parse(data[1], CultureInfo.InvariantCulture.NumberFormat),
                                              float.Parse(data[2], CultureInfo.InvariantCulture.NumberFormat));

                    
                    SensorsData s = new SensorsData("Sensor " + counter++, Convert.ToDouble(data[0]), Convert.ToDouble(data[1]), Convert.ToDouble(data[2]), Convert.ToDouble(data[3]));

                    SensorsInfoDataGrid.Items.Add(s);
                    sensorsDataList.Add(s);
                }       
            }

           var result = Interpolate(sensorsDataList[0].x, sensorsDataList[0].y, sensorsDataList[1].x,
                        sensorsDataList[1].y, (sensorsDataList[0].x + sensorsDataList[1].x) / 2);
        }

        private void StartBtnClick(object sender, RoutedEventArgs e)
        {
            this.viewPort3d.Children.Remove(device3D);

            Point mousePos = PointToScreen(Mouse.GetPosition(sender as Button));

            //var sphereSize = 5;
            /* keep these values low, the higher the values the more detailed the sphere which may impact your rendering perfomance.*/
            //var phi = 12;
            //var theta = 12;


            foreach (SensorsData sensor in sensorsDataList)
            {
                MeshBuilder meshBuilder = new MeshBuilder();

                meshBuilder.AddBox(new Point3D(sensor.x, sensor.y, sensor.deltaZ), 20, 20, 0.005);
                //meshBuilder.AddCylinder(new Point3D(sensor.x+100, sensor.y+100, sensor.deltaZ), new Point3D(sensor.x+100, sensor.y+100, sensor.deltaZ + 10), 10, 12);
                //meshBuilder.AddSphere(new Point3D(sensor.x, sensor.y, sensor.z), sphereSize, theta, phi);
                GeometryModel3D sphereModel;

                if (sensor.deltaZ >= 2)
                {
                    sphereModel = new GeometryModel3D(meshBuilder.ToMesh(), MaterialHelper.CreateMaterial(Brushes.Red, null, null, 1, 0));
                }
                else if (sensor.deltaZ < 2 && sensor.deltaZ > -2)
                {
                    sphereModel = new GeometryModel3D(meshBuilder.ToMesh(), MaterialHelper.CreateMaterial(Brushes.Red));
                }
                else
                {
                    sphereModel = new GeometryModel3D(meshBuilder.ToMesh(), MaterialHelper.CreateMaterial(Brushes.LawnGreen, null, null, 1, 0));
                }

                groupModel.Children.Add(sphereModel);
            }

            device3D.Content = groupModel; 
            
            this.viewPort3d.Children.Add(device3D);
        }

        public void WhenMessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);

            JsonData dyn = JsonConvert.DeserializeObject<JsonData>(message);

            this.Dispatcher.Invoke((Action)(() =>
            {       
                UpdateGraph(dyn);
            }));
     
        }

        public void UpdateGraph(JsonData jsonData)
        {
            
            foreach (List<string> data in jsonData.values)
            {
                OpticalSensor optSensor;

                string sensorId = data[0];
                long timestamp = Convert.ToInt64(data[1]);
                DateTimeOffset dateTimeOffset = DateTimeOffset.Now; //DateTimeOffset.FromUnixTimeSeconds(timestamp);
               
                double value = Convert.ToDouble(data[2]);
                string parameter = data[3];
                string status = data[3];

                if (opticalSensorsDic.ContainsKey(sensorId))
                {
                    optSensor = opticalSensorsDic[sensorId];
                }
                else
                {
                    optSensor = new OpticalSensor();

                    opticalSensorsDic[sensorId] = optSensor;                
                    optSensor.LnSerie.Title = sensorId;
                    
                    this.SeriesCollection.Add(optSensor.LnSerie);

                    colors.Remove(colors.First());
                }

                optSensor.Values.Add(value);
                optSensor.Ts.Add(dateTimeOffset.Millisecond);
               
                int currentW = (optSensor.Values.Count >= WINDOW_X_SIZE ? optSensor.Values.Count - WINDOW_X_SIZE : 0);

                double[] minMax = MinMaxValue(currentW);

                DateModel d = new DateModel { Value = value, DateTime = dateTimeOffset.DateTime } ;

                optSensor.LnSerie.Values.Add(d);               

            }
        }         

        public double[] MinMaxValue(int c)
        {
    
            double[] ret = new double[2];
            ret[0] = 1000;
            ret[1] = -1000;

            foreach (OpticalSensor s in opticalSensorsDic.Values)
            {
                for (int i = c; i < s.Ts.Count; i++)
                {
                    if (s.Ts[i] < ret[0])
                    {
                        ret[0] = s.Ts[i];
                    }

                    if (s.Ts[i] > ret[1])
                    {
                        ret[1] = s.Ts[i];
                    }
                }
            }

            return ret;
        }

        static double Interpolate(double x0, double y0, double x1, double y1, double x)
        {
            return y0 * (x - x1) / (x0 - x1) + y1 * (x - x0) / (x1 - x0);
        }

        private void PlayBtnClick(object sender, RoutedEventArgs e)
        {

            ConnectionDialog connectionDiag = new ConnectionDialog(proc);

            connectionDiag.ShowDialog();

            // User clicked OK
            if (connectionDiag.DialogResult.HasValue && connectionDiag.DialogResult.Value)
            {
                this.HostName = connectionDiag.HostnameBox.Text;
                this.Port = Convert.ToInt32(connectionDiag.PortBox.Text);
                this.Username = connectionDiag.UsernameBox.Text;
                this.Password = connectionDiag.PasswordBox.Password;
            }         
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            proc.Disconnect();
        }

        private void ExportCsvBtnClick(object sender, RoutedEventArgs e)
        {
            List<double> X = new List<double>() { -572.000, -552.500, -100.000, 0.000, 100.000, 537.500, 572.500 };
            List<double> Y = new List<double>() { -175.500, -175.000, -155.500, -155.000, 124.000, 124.500, 129.500 };


            double[,] matrix = { { -0.153, -0.153, -0.153, -0.153, -0.153, -0.153, -0.153 },
                                 { -0.091, -0.091, -0.091, -0.091, -0.091, -0.091, -0.091 },
                                 { 0.010, 0.010, 0.010, 0.010, 0.010, 0.010, 0.010 },
                                 { 0.009, 0.009, 0.009, 0.009, 0.009, 0.009, 0.009, },
                                 { 0.012, 0.012, 0.012, 0.012, 0.012, 0.012, 0.012 },
                                 { 0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000 },
                                 { -0.074 , -0.074 , -0.074 , -0.074 ,  -0.074 , -0.074 , -0.074 }
                               };
            
            double valkue = interpolat.interp2(X, Y, matrix, 384.500, 100);

            //interpolat inter = new interpolat();

            //Vts.Common.Math.Interpolation.interp2()

            //defineModel();

            modelForIntersection();
        }

        private void defineModel()
        {
            // Make a mesh to hold the surface.
            MeshGeometry3D mesh = new MeshGeometry3D();

            List<double> Xs = new List<double>();
            List<double> Ys = new List<double>();
            List<double> Zs = new List<double>();

            double dx = 15;
            double dy = 15;

            foreach (SensorsData data in sensorsDataList)
            {
                Xs.Add(data.x);
                Xs.Add(data.x);
                Xs.Add(data.x);

                Point3D p00 = new Point3D(data.x, data.y, data.deltaZ);
                Point3D p10 = new Point3D(data.x + dx, data.y, data.deltaZ);
                Point3D p01 = new Point3D(data.x, data.z + dy, data.deltaZ);
                Point3D p11 = new Point3D(data.x + dx, data.y + dy, data.deltaZ);

                // Add the triangles.
                AddTriangle(mesh, p00, p01, p11);
                AddTriangle(mesh, p00, p11, p10);
            }

            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Red));


            GeometryModel3D surface_model = new GeometryModel3D(mesh, material);

            groupModel.Children.Add(surface_model);

            device3D.Content = groupModel;

            // Add to view port
            this.viewPort3d.Children.Add(device3D);
            this.viewPort3d.ZoomExtents();
        }

        // Add a triangle to the indicated mesh.
        // If the triangle's points already exist, reuse them.
        private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Get the points' indices.
            int index1 = AddPoint(mesh.Positions, mesh.TextureCoordinates, point1);
            int index2 = AddPoint(mesh.Positions, mesh.TextureCoordinates, point2);
            int index3 = AddPoint(mesh.Positions, mesh.TextureCoordinates, point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index2);
            mesh.TriangleIndices.Add(index3);
        }

        // A dictionary to hold points for fast lookup.
        private Dictionary<Point3D, int> PointDictionary =
            new Dictionary<Point3D, int>();

        // If the point already exists, return its index.
        // Otherwise create the point and return its new index.
        private int AddPoint(Point3DCollection points,
            PointCollection texture_coords, Point3D point)
        {
            // If the point is in the point dictionary,
            // return its saved index.
            if (PointDictionary.ContainsKey(point))
                return PointDictionary[point];

            // We didn't find the point. Create it.
            points.Add(point);
            PointDictionary.Add(point, points.Count - 1);

            // Set the point's texture coordinates.
            texture_coords.Add(
                new Point(
                    (point.X - xmin) * texture_xscale,
                    (point.Y - ymin) * texture_yscale));

            // Return the new point's index.
            return points.Count - 1;
        }

        private void modelForIntersection()
        {
            const int xwidth = 1185;
            const int ywidth = 420;
            const double dx = 2*(xmax - xmin) / xwidth;
            const double dy = 2*(ymax - ymin) / ywidth;
            double[,] values = new double[xwidth, ywidth];          

            // Make a mesh to hold the surface.
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Make the surface's points and triangles.
            for (double x = xmin; x <= xmax - dx; x += dx)
            {
                for (double y = ymin; y <= ymax - dy; y += dx)
                {
                    // Make points at the corners of the surface
                    // over (x, z) - (x + dx, z + dz).
                    Point3D p00 = new Point3D(x, y, 0);
                    Point3D p10 = new Point3D(x + dx, y, 0);
                    Point3D p01 = new Point3D(x, y+dy, 0);
                    Point3D p11 = new Point3D(x + dx, y+dy, 0);

                    // Add the triangles.
                    AddTriangle(mesh, p00, p01, p11);
                    AddTriangle(mesh, p00, p11, p10);
                }
            }

            //rectangleMesh = mesh.Clone();
            rectangleMesh = mesh;

            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Red));

            GeometryModel3D surface_model = new GeometryModel3D(mesh, material);

            surface_model.BackMaterial = material;

            groupModel.Children.Add(surface_model);

            device3D.Content = groupModel;

            // Add to view port
            this.viewPort3d.Children.Add(device3D);
            this.viewPort3d.ZoomExtents();
        }

        private void ExportTxtBtnClick(object sender, RoutedEventArgs e)
        {

            //Point3DCollection modelMeshPositions = modelMesh.Positions;
            //Point3DCollection recMeshPositions = rectangleMesh.Positions;

            //Point3DCollection intersection = new Point3DCollection();

            //MeshBuilder meshb = new MeshBuilder();

            //foreach(Point3D modelPoint in modelMeshPositions)
            //{
            //    foreach(Point3D recPoint in recMeshPositions)
            //    {

            //        if ((Math.Round(modelPoint.X) == Math.Round(recPoint.X)) && (Math.Round(modelPoint.Y) == Math.Round(recPoint.Y)))
            //        {
            //            intersection.Add(modelPoint);
            //            meshb.Positions.Add(modelPoint);
            //        }
            //    }
            //}

            MeshGeometry3D newmesh = new MeshGeometry3D();

            ModelLPoints lp = new ModelLPoints();

            newmesh = lp.BuildDic(modelMesh);

            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Red));

            GeometryModel3D surface_model = new GeometryModel3D(newmesh, material);

            groupModel.Children.Clear();
            this.viewPort3d.Children.Remove(device3D);
            surface_model.BackMaterial = material;

            groupModel.Children.Add(surface_model);

            device3D.Content = groupModel;

            // Add to view port
            this.viewPort3d.Children.Add(device3D);
            this.viewPort3d.ZoomExtents();

            lp.FillSensorDataDictionary(sensorsDataList);

        }
    }
}
