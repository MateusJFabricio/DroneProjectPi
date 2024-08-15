using DroneServerProject.RDC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneServerProject.Models
{
    public class DroneModel
    {
        public bool Mode { get; set; }
        public string Name { get; set; }
        public RDCManualControl ManualControl { get; set; }
        public RDCTelemetry Telemetry { get; set; }
    }
}
