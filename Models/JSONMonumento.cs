using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEIPracticas.Models
{
    public class JSONMonumento
    {
        public string documentName { get; set; }
        public string postalCode { get; set; }
        public string documentDescription { get; set; }
        public string locality { get; set; }
        public string address { get; set; } 
        public string territory { get; set; }
        public string latwgs84 { get; set; }
        public string lonwgs84 { get; set; }
    }
}
