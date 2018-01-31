using HelixToolkit.Wpf;
using InteractiveDataDisplay.WPF;
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
using System.Windows.Media.Media3D;


namespace wpfHelixTest
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private int WINDOW_X_SIZE = 10;
        private List<Color> colors = new List<Color> {  Color.FromRgb( 140, 140, 140), Color.FromRgb(255, 0, 0), Color.FromRgb(255, 255, 0), Color.FromRgb(255, 0, 255), Color.FromRgb(0, 0, 255),
                                                        Color.FromRgb(0, 255, 255), Color.FromRgb(0, 255, 0),  Color.FromRgb( 0, 0, 0) };

        private Dictionary<string, OpticalSensor> opticalSensorsDic = new Dictionary<string, OpticalSensor>();
        private Dictionary<string, LineGraph> graphs = new Dictionary<string, LineGraph>();

        List<SensorsData> sensorsDataList = new List<SensorsData>();

        private const string MODEL_PATH = @"C:\Users\heitor.araujo\Documents\wokspace\GM\TestRhinoGrasshopper\objRhinofile2.obj";

        ModelVisual3D device3D;
        Model3DGroup groupModel = new Model3DGroup();        

        public MainWindow()
        {
            InitializeComponent();

            this.device3D = new ModelVisual3D();

            ProtocolData proc = new ProtocolData("localhost", 5672, "userTest", "userTest", "hello");
            //proc.HostName = "localhost";
            //proc.Username = "userTest";
            //proc.Password = "userTest";
            //proc.Port = 5672;
            //proc.QueueName = "hello";

            proc.Connect();
            proc.ReadEvnt(WhenMessageReceived);

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

                //Load the 3D model file
                device = import.Load(model);
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
                this.viewPort3d.Children.Add(device3D);
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

                meshBuilder.AddBox(new Point3D(sensor.x, sensor.y, sensor.deltaZ), 10, 10, 0.005);
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

        public void UpdateGraph(JsonData data)
        {
            OpticalSensor optSensor;

            if (opticalSensorsDic.ContainsKey(data.sensorid))
            {
                optSensor = opticalSensorsDic[data.sensorid];
            }
            else
            {
                optSensor = new OpticalSensor();
                
                opticalSensorsDic[data.sensorid] = optSensor;

                optSensor.lineGraph.Stroke = new SolidColorBrush(colors.First());
                optSensor.lineGraph.Description = String.Format("Sensor {0}", data.sensorid);
                
                //optSensor.lineGraph.FlowDirection = FlowDirection.LeftToRight;
                    //PlotBase.FlowDirectionProperty;
                linesGraph.Children.Add(optSensor.lineGraph);

                colors.Remove(colors.First());
            }

            //if (optSensor.X.Count > 10)
            //{
            //    optSensor.X.Remove(optSensor.X.First());
            //    optSensor.Y.Remove(optSensor.Y.First());
            //}

            optSensor.X.Add(data.value[0]);
            optSensor.Y.Add(data.value[1]);

            if (optSensor.X.Count > WINDOW_X_SIZE)
            {
                double[] minMax = MinValue(optSensor.X.Count - WINDOW_X_SIZE);
                plotter.PlotHeight = (minMax[1] - minMax[0]) + 5;
                plotter.PlotOriginY = minMax[0] - 5;
                plotter.PlotOriginX = optSensor.X[optSensor.X.Count - WINDOW_X_SIZE];

                //plotter.IsAutoFitEnabled = true;
            }
            else
            {
                double[] minMax = MinValue(0);

                plotter.PlotHeight = (minMax[1] - minMax[0]) + 5;
                plotter.PlotOriginY = minMax[0] - 5;
                plotter.PlotOriginX = optSensor.X.First();
            }

            optSensor.lineGraph.Plot(optSensor.X, optSensor.Y);

        }         

        public double[] MinValue(int c)
        {
    
            double[] ret = new double[2];
            ret[0] = 1000;
            ret[1] =  -1000;

            foreach (OpticalSensor s in opticalSensorsDic.Values)
            {
                for (int i = c; i < s.Y.Count; i++)
                {
                    if (s.Y[i] < ret[0])
                    {
                        ret[0] = s.Y[i];
                    }

                    if (s.Y[i] > ret[1])
                    {
                        ret[1] = s.Y[i];
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

            //x[counter++] = counter;

            //var lg = new LineGraph();
            //linegraph.Children.Add(lg);
            //lg.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, (byte)(40), 0));
            //lg.Description = String.Format("Data series {0}", 1);
            //lg.StrokeThickness = 2;

            //linesGraph.Plot(x, x.Select(v => Math.Sin(v + 1 / 10.0)).ToArray());
            //lg1.Plot(x, y);
            
            //Thread.Sleep(500);            
        }
    }
}
