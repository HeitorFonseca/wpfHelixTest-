using HelixToolkit.Wpf;
using Microsoft.Win32;
using OpenTK;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
        bool lockVar = false;

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

           var result = interpolate(sensorsDataList[0].x, sensorsDataList[0].y, sensorsDataList[1].x,
                        sensorsDataList[1].y, (sensorsDataList[0].x + sensorsDataList[1].x) / 2);
        }

        private void StartBtnClick(object sender, RoutedEventArgs e)
        {
            this.viewPort3d.Children.Remove(device3D);

            Point mousePos = PointToScreen(Mouse.GetPosition(sender as Button));

            var sphereSize = 5;
            /* keep these values low, the higher the values the more detailed the sphere which may impact your rendering perfomance.*/
            var phi = 12;
            var theta = 12;


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

        static void WhenMessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);
        }

        static double interpolate(double x0, double y0, double x1, double y1, double x)
        {
            return y0 * (x - x1) / (x0 - x1) + y1 * (x - x0) / (x1 - x0);
        }
    }
}
