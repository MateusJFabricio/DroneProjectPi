using DroneServerProject.Models;
using DroneServerProject.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneServerProject.Controller
{
    public class DroneController
    {
        public DroneSerial DroneSerial { get; set; }
        public DroneController() 
        {
            
        }
    }
}
