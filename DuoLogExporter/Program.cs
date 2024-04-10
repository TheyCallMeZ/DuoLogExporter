using Microsoft.Extensions.Configuration;
using System.IO;

namespace DuoLogExporter;


internal class Program
{
    //set us up to read from config and to make rest calls
    private static IConfigurationRoot? _config;
    private static readonly HttpClient _http = new();
    
    private static readonly string? DuoEndPoint = Environment.GetEnvironmentVariable("duoEndPoint");
    private static readonly string? DuoClientKey = Environment.GetEnvironmentVariable("duoClientKey");
    private static readonly string? DuoClientSecret = Environment.GetEnvironmentVariable("duoClientSecret");
    
    //Default the output path to the same directory as the executable
    private static string? outputPath = Directory.GetCurrentDirectory();
    

// string myConfigValue = configuration["yourKey"];
    private static void Main(string[] args)
    {
        // Add the following code to your Main method:
        _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        outputPath = string.IsNullOrWhiteSpace(_config.GetSection("OutputPath").Value) ? Directory.GetCurrentDirectory() : _config.GetSection("OutputPath").Value; 
        
        var program = new Program();
        Console.WriteLine("Starting Up, Fetching Credentials....");
        if (DuoEndPoint == null || DuoClientKey == null || DuoClientSecret == null)
        {
            Console.WriteLine("Missing required credentials. Exiting...");
            Environment.Exit(1);
        }
        
        Console.WriteLine($"Outputting to: {outputPath}");
        
        Console.WriteLine(program.HttpCall());
    }

    public string HttpCall()
    {
        return "success";
    }
}