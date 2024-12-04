using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IEIPracticas.Models
{
    public class XMLMonumento
    {
        public string nombre { get; set; }  
        public string tipoMonumento { get; set; }  
        public string calle { get; set; } 
        public string codigoPostal { get; set; }  
        public Municipality poblacion { get; set; }
        public Desc Descripcion { get; set; }  
        public Coords coordenadas { get; set; }
    }

    public class Municipality
    {
        public string localidad { get; set; }
        public string provincia { get; set; }
    }

    public class Desc
    {
        [JsonPropertyName("#cdata-section")]
        public string CDataSection { get; set; }  
    }
    public class Coords
    {
        public string latitud { get; set; }
        public string longitud { get; set; }
    }

}
