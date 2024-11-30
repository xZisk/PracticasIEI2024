using System;
using System.Globalization;
using IEIPracticas.Models;

namespace IEIPracticas
{
    public static class Mapper
    {
        public static Monumento XMLMonumentoToMonumento(XMLMonumento xm)
        {
            if (xm == null)
            {
                Console.WriteLine("Error: El objeto XMLMonumento no puede ser nulo.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(xm.nombre))  
            {
                Console.WriteLine("Error: Monumento sin nombre.");
                return null;
            }

            if (!double.TryParse(xm.coordenadas.latitud, NumberStyles.Any, CultureInfo.InvariantCulture, out double latitud) || latitud < -90 || latitud > 90)
            {
                Console.WriteLine($"Error: Monumento '{xm.nombre}' tiene una latitud inválida.");
                return null;
            }

            if (!double.TryParse(xm.coordenadas.longitud, NumberStyles.Any, CultureInfo.InvariantCulture, out double longitud) || longitud < -180 || longitud > 180)
            {
                Console.WriteLine($"Error: Monumento '{xm.nombre}' tiene una longitud inválida.");
                return null;
            }

            if (!int.TryParse(xm.codigoPostal, out int codigoPostal))
            {
                Console.WriteLine($"Error: Monumento '{xm.nombre}' tiene un código postal inválido.");
                return null;  
            }

            Monumento monumento = new Monumento
            {
                Nombre = xm.nombre,
                Direccion = xm.calle,
                CodigoPostal = codigoPostal,
                Tipo = MapTipo(xm.TipoMonumento, xm.nombre),  
                Latitud = latitud, //no funciona con double
                Longitud = longitud,  //no funciona con double
                Descripcion = xm.Descripcion?.CDataSection,  
                IdLocalidad = 1  
            };

            return monumento;
        }


        private static Tipo MapTipo(string tipoMonumento, string nombre)
        {
            if (string.IsNullOrWhiteSpace(tipoMonumento))
            {
                return Tipo.Otros;
            }

            if (tipoMonumento.Contains("Yacimientos arqueológicos", StringComparison.OrdinalIgnoreCase))
            {
                return Tipo.Yacimiento; // 1
            }

            if (tipoMonumento.Contains("Monasterios", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Monasterio", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Convento", StringComparison.OrdinalIgnoreCase))
            {
                return Tipo.Monasterio; // 3
            }

            if (tipoMonumento.Contains("Iglesias", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Iglesia", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Ermita", StringComparison.OrdinalIgnoreCase))
            {
                return Tipo.Iglesia; // 2
            }

            if (tipoMonumento.Contains("Castillos", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Castillo", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Torre", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Muralla", StringComparison.OrdinalIgnoreCase))
            {
                return Tipo.Castillo; // 4
            }

            if (tipoMonumento.Contains("Puentes", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Puente", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Pont", StringComparison.OrdinalIgnoreCase))
            {
                return Tipo.Puente; // 6
            }

            return Tipo.Edificio; // 5
        }

    }
}
