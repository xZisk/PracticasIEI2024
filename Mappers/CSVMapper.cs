using IEIPracticas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEIPracticas.Mappers
{
    public static class CSVMapper
    {
        public static Monumento? CSVMonumentoToMonumento(CSVMonumento csvMonumento)
        {
            if (csvMonumento == null)
            {
                Console.WriteLine("Error: El objeto CSVMonumento no puede ser nulo.");
                return null;
            }

            try
            {
                // Validar errores específicos
                if (string.IsNullOrWhiteSpace(csvMonumento.DENOMINACION))
                {
                    Console.WriteLine("Error: Monumento sin nombre.");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(csvMonumento.UTMESTE) || string.IsNullOrWhiteSpace(csvMonumento.UTMNORTE))
                {
                    Console.WriteLine($"Error: Monumento '{csvMonumento.DENOMINACION}' sin coordenadas válidas (UTM Este o Norte).");
                    return null;
                }

                double longitud = ConvertToDouble(csvMonumento.UTMESTE);
                double latitud = ConvertToDouble(csvMonumento.UTMNORTE);

                if (longitud == 0.0 || latitud == 0.0)
                {
                    Console.WriteLine($"Error: Monumento '{csvMonumento.DENOMINACION}' tiene coordenadas inválidas.");
                    return null;
                }

                /*
                int codigoPostal = 0; // Placeholder hasta obtenerlo por API
                if (codigoPostal != 0 && (codigoPostal < 1000 || codigoPostal > 52999))
                {
                    Console.WriteLine($"Error: Monumento '{csvMonumento.DENOMINACION}' tiene un código postal fuera del rango válido: {codigoPostal}.");
                    return null;
                } 
                */

                // Crear el objeto Monumento si pasa todas las validaciones
                var monumento = new Monumento
                {
                    IdMonumento = 0, // Placeholder
                    Nombre = csvMonumento.DENOMINACION,
                    Tipo = MapTipo(csvMonumento.CATEGORIA, csvMonumento.DENOMINACION),
                    Descripcion = MapTipo(csvMonumento.CATEGORIA, csvMonumento.DENOMINACION).ToString(),
                    Direccion = "Generar mediante API", // Placeholder
                    CodigoPostal = 00000, // Placeholder
                    Longitud = longitud,
                    Latitud = latitud,
                    IdLocalidad = 0 // Placeholder
                };

                return monumento;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al mapear el monumento '{csvMonumento.DENOMINACION}': {ex.Message}");
                return null;
            }
        }

        private static Tipo MapTipo(string categoria, string denominacion)
        {
            if (categoria == "Zona arqueológica")
                return Tipo.Yacimiento; // 1

            if (categoria == "Individual (mueble)" || categoria == "Zona paleontológica" ||
                categoria == "Fondo de museo (primera)" || string.IsNullOrEmpty(categoria))
                return Tipo.Otro; // 7

            if (categoria == "Monumento" && (denominacion.Contains("Iglesia") || denominacion.Contains("Ermita")))
                return Tipo.Iglesia; // 2

            if (categoria == "Monumento" && (denominacion.Contains("Monasterio") || denominacion.Contains("Convento")))
                return Tipo.Monasterio; // 3

            if (categoria == "Monumento" &&
                (denominacion.Contains("Castillo") || denominacion.Contains("Castell") || denominacion.Contains("Castellet") ||
                 denominacion.Contains("Torre") || denominacion.Contains("Torreta") || denominacion.Contains("Muralla") ||
                 denominacion.Contains("Amurallado") || denominacion.Contains("Amurallada")))
                return Tipo.Castillo; // 4

            if (categoria == "Monumento" && (denominacion.Contains("Puente") || denominacion.Contains("Pont")))
                return Tipo.Puente; // 6


            return Tipo.Edificio; // 5
        }

        private static double ConvertToDouble(string value)
        {
            if (double.TryParse(value, out var result))
                return result;

            return 0.0; // Valor por defecto
        }
    }
}
