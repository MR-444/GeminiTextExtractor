using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Check if the correct number of arguments are provided
if (args.Length != 1)
{
    Console.WriteLine("Usage: <program> <inputFileName>");
    return;
}

// Get the input file name from the command-line arguments
var inputFileName = args[0];
var inputFilePath = Path.Combine(Environment.CurrentDirectory, inputFileName);

// Construct the output file name
var outputFileName = Path.GetFileNameWithoutExtension(inputFileName) + "_extract.txt";
var outputFilePath = Path.Combine(Environment.CurrentDirectory, outputFileName);

try
{
    // Read the JSON file
    var jsonContent = File.ReadAllText(inputFilePath);

    // Parse the JSON content
    var jsonObject = JObject.Parse(jsonContent);

    // Extract the texts with "role": "model"
    var modelTexts = ExtractModelTexts(jsonObject);

    // Save the extracted texts to a new file
    File.WriteAllLines(outputFilePath, modelTexts);

    Console.WriteLine("Extracted texts have been saved to " + outputFilePath);
}
catch (FileNotFoundException ex)
{
    Console.WriteLine("File not found: " + ex.Message);
}
catch (JsonException ex)
{
    Console.WriteLine("Error parsing JSON: " + ex.Message);
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred: " + ex.Message);
}

List<string> ExtractModelTexts(JObject jsonObject)
{
    var chunks = jsonObject["chunkedPrompt"]?["chunks"];
    if (chunks == null) return new List<string>();

    return chunks
        .Where(chunk => chunk["role"]?.ToString() == "model")
        .Select(chunk => chunk["text"]?.ToString())
        .Where(text => !string.IsNullOrEmpty(text))
        .ToList();
}