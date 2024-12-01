using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEIPracticas.Mappers
{
    public static class JSONMapper
    {

        public static Monumento? JSONMonumentoToMonumento(JSONMonumento jsonMonumento)
        {
            if (jsonMonumento == null)
            {
                Console.WriteLine("Error: El objeto JSONMonumento no puede ser nulo.");
                return null;
            }

            try
            {
                // Validar errores específicos
                if (string.IsNullOrWhiteSpace(jsonMonumento.getDocumentName()))
                {
                    Console.WriteLine("Error: Monumento sin nombre.");
                    return null;
                }

                /*
                if (string.IsNullOrWhiteSpace(csvMonumento.UTMESTE) || string.IsNullOrWhiteSpace(csvMonumento.UTMNORTE))
                {
                    Console.WriteLine($"Error: Monumento '{csvMonumento.DENOMINACION}' sin coordenadas válidas (UTM Este o Norte).");
                    return null;
                }
                */

                if (longitud == 0.0 || latitud == 0.0)
                {
                    Console.WriteLine($"Error: Monumento '{jsonMonumento.getDocumentName()}' tiene coordenadas inválidas.");
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
                    Nombre = jsonMonumento.getDocumentName(),
                    Localidad = jsonMonumento.getPoblacion().getLocalidad(), //No he encontrado
                    Provincia = jsonMonumento.getPoblacion().getTerritory(),
                    Tipo = MapTipo(jsonMonumento.getDocumentName()),
                    Descripcion = jsonMonumento.getDocumentDescription(),
                    Direccion = jsonMonumento.getAddress(),
                    CodigoPostal = jsonMonumento.getPostalCode(),
                    Longitud = jsonMonumento.getCordenadas().getLonwgs84(),
                    Latitud = jsonMonumento.getCordenadas().getLatwgs84(),
                    IdLocalidad = 1 // Placeholder, todas ligadas a Valencia de momento
                };

                return monumento;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al mapear el monumento '{jsonMonumento.getDocumentName()}': {ex.Message}");
                return null;
            }
        }

        private static Tipo MapTipo(string documentName)
        {
            if (documentName.Contains("Yacimiento arqueológico", StringComparison.OrdinalIgnoreCase))
                return Tipo.Yacimiento; // Yacimiento arqueológico

            if (documentName.Contains("Ermita", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Iglesia", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Catedral", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Basílica", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Santuario", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Parroquia", StringComparison.OrdinalIgnoreCase))
                return Tipo.Iglesia; // Iglesia-Ermita

            if (documentName.Contains("Monasterio", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Convento", StringComparison.OrdinalIgnoreCase))
                return Tipo.Monasterio; // Monasterio-Convento

            if (documentName.Contains("Palacio", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Castillo", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Torre", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Fortaleza", StringComparison.OrdinalIgnoreCase))
                return Tipo.Castillo; // Castillo-Fortaleza-Torre

            if (documentName.Contains("Edificio", StringComparison.OrdinalIgnoreCase))
                return Tipo.Edificio; // Edificio singular

            if (documentName.Contains("Puente", StringComparison.OrdinalIgnoreCase))
                return Tipo.Puente; // Puente

            return Tipo.Otros; // Otro
        }

    }
}
