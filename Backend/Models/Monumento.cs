using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IEIPracticas.Models
{
    public class Monumento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMonumento { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public int CodigoPostal { get; set; }
        public double Longitud { get; set; }
        public double Latitud { get; set; }
        public string Descripcion { get; set; }
        public Tipo Tipo { get; set; }
        public int IdLocalidad { get; set; }
        public string Fuente { get; set; }
    }
}
