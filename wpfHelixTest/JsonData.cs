using InteractiveDataDisplay.WPF;
using LiveCharts;
using LiveCharts.Wpf;
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
        public string viewer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<List<string>> values { get; set; }

    }

    public class Values
    {
        /// <summary>
        /// 
        /// </summary>
        public string sensorid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string parameter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
    }

    public class OpticalSensor
    {


        public string Status { get; set; }
        public string Sensorid { get; set; }
        public List<double> Values { get; set; }
        public List<double> Ts { get; set; }

        public LineSeries LnSerie { get; set; }

        public OpticalSensor()
        {   
            Values = new List<double>();
            Ts = new List<double>();
            LnSerie = new LineSeries{ Values = new ChartValues<DateModel>() };
        }

        //public void SetVelocity(double x, double y)
        //{
        //    this.Velocity = 10 * (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
        //}

        //public void SetDirection(double x, double y)
        //{
        //    this.Direction = 180 + ((Math.Atan2(x, -y)) * 180 / Math.PI);
        //}

    }
}
