using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEIPracticas.Models
{
    public class JSONMonumento
    {
        public string identificador { get; set; }
        public string nombre { get; set; }
        public string tipoMonumento { get; set; }
        public string codigoPostal { get; set; }
        public string Descripcion { get; set; }
        public string email { get; set; }
        public string periodoHistorico { get; set; }
        public Poblacion poblacion { get; set; }
        public Coordenadas coordenadas { get; set; }
        public string web { get; set; }
    }
    public class Descripcion
    {
        public string CDataSection { get; set; }
    }

    public class HorariosYTarifas
    {
        public string CDataSection { get; set; }
    }

    public class Poblacion
    {
        public string provincia { get; set; }
        public string municipio { get; set; }
        public string localidad { get; set; }
    }

    public class Coordenadas
    {
        public string latitud { get; set; }
        public string longitud { get; set; }
    }
}
