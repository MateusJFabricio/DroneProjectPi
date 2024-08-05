using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneProject.ModelContext.Serial.FlyController
{
    public class FlyControllerSerial
    {
        public int BaudRate { get; set; }
        public string Port { get; set; }
        public bool Connected { get; set; }

        public FlyControllerSerial()
        {

        }

        public void Connect()
        {

        }

        public void Connect(int baudRate) { }
        public void Disconnect() { }

    }
}
