namespace IEIPracticas.Models
{
    public class Monumento
    {
        public int IdMonumento { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public int CodigoPostal { get; set; }
        public double Longitud { get; set; }
        public double Latitud { get; set; }
        public string Descripcion { get; set; }
        public Tipo Tipo { get; set; }
        public int IdLocalidad { get; set; }
    }
}
