using System;
using System.IO;

public static class ExtractorJson
{
    public static string LoadJsonAsString(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"El archivo JSON no existe en la ruta: {filePath}");
        }
        return File.ReadAllText(filePath);
    }
}
