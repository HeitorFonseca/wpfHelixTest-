using InteractiveDataDisplay.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfHelixTest
{
    public class JsonData
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sensorid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string user { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<double> value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ts { get; set; }

    }

    public class OpticalSensor
    {

        public double Velocity { get; set; }
        public double Direction { get; set; }
        public string Status { get; set; }

        public List<double> X { get; set; }
        public List<double> Y { get; set; }

        public LineGraph lineGraph { get; set; }

        public OpticalSensor()
        {
            lineGraph = new LineGraph();
            X = new List<double>();
            Y = new List<double>();
        }

        public void SetVelocity(double x, double y)
        {
            this.Velocity = 10 * (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
        }

        public void SetDirection(double x, double y)
        {
            this.Direction = 180 + ((Math.Atan2(x, -y)) * 180 / Math.PI); 
        }

    }
}
