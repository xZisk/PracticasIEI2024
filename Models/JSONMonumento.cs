using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEIPracticas.Models
{
    public class JSONMonumento
    {
        public string identificador { get; set; } //No hay
        public string documentName { get; set; } // Hecho
        public string tipoMonumento { get; set; } // No hay, obtenerlo de documentName
        public string postalCode { get; set; } // Hecho
        public string documentDescription { get; set; } // Hecho
        public string tourismEmail { get; set; } // Hecho
        public string periodoHistorico { get; set; } // No hay
        public Poblacion poblacion { get; set; }
        public Coords coordenadas { get; set; }
        public string friendlyUrl { get; set; } // Ns si friendly o physical
        public string address { get; set; } // Ns si es correcto poner esto
    }


    public class Poblacion
    {
        public string territory { get; set; } // Hecho
        public string municipality { get; set; } //Hecho
        public string localidad { get; set; } // Ns
    }

    public class Coords { 
        public string latwgs84 { get; set; } // Hecho
        public string lonwgs84 { get; set; } // Hecho
    }
}
