using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneProject.Models
{
    public class ErrorLogModel
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }
}
