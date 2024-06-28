using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GeminiTextExtractor;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        if (args is not [var inputFileName])
        {
            Console.WriteLine("Usage: <program> <inputFileName>");
            return;
        }

        var inputFilePath = Path.GetFullPath(inputFileName);
        var outputFilePath = Path.ChangeExtension(inputFilePath, "_extract.txt");

        if (File.Exists(inputFilePath) is false)
        {
            Console.WriteLine($"Input file not found: {inputFilePath}");
            return;
        }

        try
        {
            var jsonContent = await File.ReadAllTextAsync(inputFilePath);
            var jsonObject = JObject.Parse(jsonContent);
            var modelTexts = ExtractTexts(jsonObject, "model");

            await File.WriteAllTextAsync(outputFilePath, 
                string.Join(Environment.NewLine + Environment.NewLine, modelTexts));

            Console.WriteLine($"Extracted texts have been saved to {outputFilePath}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"File not found: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static IEnumerable<string> ExtractTexts(JObject jsonObject, string role) =>
        jsonObject["chunkedPrompt"]?["chunks"]?
            .Where(chunk => chunk["role"]?.ToString() == role)
            .Select(chunk => chunk["text"]?.ToString())
            .Where(text => !string.IsNullOrEmpty(text))
            .Select(text => text!)
            .ToList() ?? [];
}