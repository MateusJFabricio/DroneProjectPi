using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneProject.Models
{
    public class SerialModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int BaudRate { get; set; }
        public string PortCOM { get; set; } = string.Empty;
    }
}
