using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfHelixTest
{
    public class SensorsData
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double x { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public double y { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double z { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double deltaZ { get; set; }

        public SensorsData(string name, double x, double y, double z, double deltaZ)
        {
            this.Name = name;
            this.x = x;
            this.y = y;
            this.z = z;
            this.deltaZ = deltaZ;
        }
    }
}
