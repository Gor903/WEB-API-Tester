
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class ConfigData
{
    public string TestConfigsPath { get; set; }
    public string LogDirectory { get; set; }
    public GroupData[] TestGroups { get; set; }
}
class GroupData
{
    public string Url { get; set; }
    public string Group { get; set; }
    public string Ignore { get; set; }
    public string[] Files { get; set; }
    public string LogDirectory { get; set; }

}

class TestConfigData
{
    public string Url { get; set; }
    public string Method { get; set; }
    public string Endpoint { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public Dictionary<string, string> Content { get; set; }
    public Dictionary<string, string> Query { get; set; }
    public string LogDirectory { get; set; }

    public JToken Expected { get; set; }
}