namespace DuoLogExporter;

public class DuoAuthenticationLogs
{
    public Authlogs[] authlogs { get; set; }
    public Metadata metadata { get; set; }
}

public class Authlogs
{
    public Access_device access_device { get; set; }
    public Adaptive_trust_assessments adaptive_trust_assessments { get; set; }
    public string alias { get; set; }
    public Application application { get; set; }
    public Auth_device auth_device { get; set; }
    public string email { get; set; }
    public string event_type { get; set; }
    public string factor { get; set; }
    public string isotimestamp { get; set; }
    public object ood_software { get; set; }
    public string reason { get; set; }
    public string result { get; set; }
    public int timestamp { get; set; }
    public string trusted_endpoint_status { get; set; }
    public string txid { get; set; }
    public User user { get; set; }
}

public class Access_device
{
    public string browser { get; set; }
    public string browser_version { get; set; }
    public string epkey { get; set; }
    public string flash_version { get; set; }
    public object hostname { get; set; }
    public string ip { get; set; }
    public string is_encryption_enabled { get; set; }
    public string is_firewall_enabled { get; set; }
    public string is_password_set { get; set; }
    public string java_version { get; set; }
    public Location location { get; set; }
    public string os { get; set; }
    public string os_version { get; set; }
    public object[] security_agents { get; set; }
}

public class Location
{
    public string city { get; set; }
    public string country { get; set; }
    public string state { get; set; }
}

public class Adaptive_trust_assessments
{
    public More_secure_auth more_secure_auth { get; set; }
    public Remember_me remember_me { get; set; }
}

public class More_secure_auth
{
    public string[]? detected_attack_detectors { get; set; }
    public string features_version { get; set; }
    public string model_version { get; set; }
    public bool policy_enabled { get; set; }
    public string reason { get; set; }
    public string trust_level { get; set; }
}

public class Remember_me
{
    public string features_version { get; set; }
    public string model_version { get; set; }
    public bool policy_enabled { get; set; }
    public string reason { get; set; }
    public string trust_level { get; set; }
}

public class Application
{
    public string key { get; set; }
    public string name { get; set; }
}

public class Auth_device
{
    public string ip { get; set; }
    public string key { get; set; }
    public Location location { get; set; }
    public string name { get; set; }
}

public class User
{
    public string[] groups { get; set; }
    public string key { get; set; }
    public string name { get; set; }
}

public class Metadata
{
    public string[] next_offset { get; set; }
    public int total_objects { get; set; }
}