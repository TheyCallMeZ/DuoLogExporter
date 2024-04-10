using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using Duo;
using Newtonsoft.Json;

namespace DuoLogExporter;


internal class Program
{
    //set us up to read from config and to make rest calls
    private static IConfigurationRoot? _config;
    private static readonly HttpClient _http = new();
    private static readonly string? DuoEndPoint = Environment.GetEnvironmentVariable("duoEndPoint");
    private static readonly string? DuoClientKey = Environment.GetEnvironmentVariable("duoClientKey");
    private static readonly string? DuoClientSecret = Environment.GetEnvironmentVariable("duoClientSecret");
    
    private DuoApi duoClient = new Duo.DuoApi(DuoClientKey, DuoClientSecret, DuoEndPoint);
    
    //Default the output path to the same directory as the executable
    private static string? outputPath = Directory.GetCurrentDirectory();
    

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
        
        DateTime today = DateTime.Today; 
        DateTimeOffset startOfLastMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
        DateTimeOffset endOfLastMonth = startOfLastMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
        
        program.Get_Log_Data(1000, startOfLastMonth, endOfLastMonth);
    }

    public void Get_Authentication_Attemps()
    {
        Console.WriteLine("Fetching Authentication Attempts from Duo");
        
        var parameters = new Dictionary<string, string>();
        var r = duoClient.JSONApiCall<Dictionary<string, object>>(
            "GET", "/admin/v1/info/authentication_attempts", parameters);
        var attempts = r["authentication_attempts"] as Dictionary<string, object>;
        foreach (KeyValuePair<string, object> info in attempts)
        {
            var s = String.Format("{0} authentication(s) ended with {1}.",
                info.Value,
                info.Key);
            System.Console.WriteLine(s);
        }
    }

    public void Get_Log_Data(int limit, DateTimeOffset mintime, DateTimeOffset maxtime)
    {
        var parameters = new Dictionary<string, string>();
        parameters.Add("limit", limit.ToString());
        parameters.Add("mintime", mintime.ToUnixTimeMilliseconds().ToString());
        parameters.Add("maxtime", maxtime.ToUnixTimeMilliseconds().ToString());
        
        var r = duoClient.JSONApiCall<Dictionary<string, object>>(
            "GET", "/admin/v2/logs/authentication", parameters);

       // var log = JsonConvert.DeserializeObject<DuoAuthenticationLogs>(r["response"]);
       var txt = JsonConvert.SerializeObject(r);
       File.WriteAllText(Path.Combine(outputPath, $"[{DateTime.Today:yyyy-M-d}]-duo_logs.json"), txt);
       Console.WriteLine("Json Written");

       var csv = new StringBuilder();

       csv.AppendLine("Timestamp (UTC),User,Integration,Factor,Result,Reason,Enrollment,2FA Device Name,2FA Device Key,2FA Device IP,2FA Device Country,2FA Device State,2FA Device City,Access Device IP,Access Device Country,Access Device State,Access Device City,Access Device Hostname,Alias,Email,Out Of Date Software,Access Device OS,Access Device OS Version,Access Device Browser,Access Device Browser Version,Access Device Java Version,Access Device Flash Version,Trusted Endpoint Status,Risk-based factor selection detected attacks,Risk-based factor selection trust level,Risk-based factor selection reason,Risk-based factor selection policy enabled,Risk-based remember me trust level,Risk-based remember me reason,Risk-based remember me policy enabled,Risk-based factor selection step-up status");

       var logs = JsonConvert.DeserializeObject<DuoAuthenticationLogs>(txt);
       
       foreach (var log in logs?.authlogs )
       {
               var detectedAttacks =  string.Join(',',
                   log.adaptive_trust_assessments.more_secure_auth.detected_attack_detectors ?? []);
           
               csv.AppendLine(
                   $"{log.isotimestamp},{log.user.name},{log.application.name},{log.factor},{log.result},{log.reason},{"enrollment?"},{log.auth_device.name},{log.auth_device.key},{log.auth_device.ip},{log.auth_device.location.country},{log.auth_device.location.state},{log.auth_device.location.city},{log.access_device.ip},{log.access_device.location.country},{log.access_device.location.state},{log.access_device.location.city},{log.access_device.hostname},{log.alias},{log.email},{log.ood_software},{log.access_device.os},{log.access_device.os_version},{log.access_device.browser},{log.access_device.browser_version},{log.access_device.java_version},{log.access_device.flash_version},{log.trusted_endpoint_status},{detectedAttacks},{log.adaptive_trust_assessments.more_secure_auth.trust_level},{log.adaptive_trust_assessments.more_secure_auth.reason},{log.adaptive_trust_assessments.more_secure_auth.policy_enabled},{log.adaptive_trust_assessments.remember_me.trust_level},{log.adaptive_trust_assessments.remember_me.reason},{log.adaptive_trust_assessments.remember_me.policy_enabled},{"riskBasedFactorSelectionStepUpStatus?"}");
       }
       
       File.WriteAllText(Path.Combine(outputPath, $"[{DateTime.Today:yyyy-M-d}]-duo_logs.csv"), csv.ToString());
       Console.WriteLine("CSV Written");
       
    }
}