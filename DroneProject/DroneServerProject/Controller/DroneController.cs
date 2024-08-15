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
        public List<DroneModel> Drones { get; set; } = new List<DroneModel>();
        public DroneController() 
        {
            
        }
        public void NewDrone(DroneModel drone)
        {
            Drones.Add(drone);
        }
    }
}
