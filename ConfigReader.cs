using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
class ConfigReader
{
    APITesterLogs logger = APITesterLogs.GetInstance();
    public IEnumerable<TestConfigData> ProcessDirectory(string path, GroupData[] groupData)
    {
        foreach (GroupData item in groupData)
        {
            logger._logger.LogTrace($"Start: {item.Group}");
            foreach (string file in item.Files)
            {
                TestConfigData temp = ProcessFile(Path.Combine(path, item.Group, file + ".json"));
                if (temp == null)
                {
                    continue;
                }
                temp.Url = item.Url;
                temp.LogDirectory = item.LogDirectory;
                yield return temp;
            }
        }
    }
    private TestConfigData ProcessFile(string path)
    {
        logger._logger.LogTrace($"Read: {path}");
        string content = String.Empty;
        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                content = sr.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<TestConfigData>(content);
        }
        catch (Exception ex)
        {
            logger._logger.LogError($"Error while reading: {ex.Message}");
            return null;
        }

    }
}

