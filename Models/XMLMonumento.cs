using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEIPracticas.Models
{
    internal class XMLMonumento
    {
        public string documentName { get; set; }
        public string documentDescription { get; set; }
        public string address { get; set; }
        public string postalCode { get; set; }
        public string latitudelongitude { get; set; }
        public string latwgs84 { get; set; }
        public string lonwgs84 { get; set; }
        public string municipality { get; set; }
    }
}
