using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
public class Tester
{
    private APITesterLogs logger = APITesterLogs.GetInstance();
    private ConfigReader configReader = new();
    private ConfigData? configData;
    private object[] testData = new object[3];
    private HttpRequestBuilder? request = null;
    public IEnumerable<Object[]> GetTestData()
    {
        string testConfigPath = configData.TestConfigsPath;

        foreach (TestConfigData data in configReader.ProcessDirectory(testConfigPath, configData.TestGroups))
        {
            request = new();

            request.SetMethod(data.Method).
                    SetUrl($"{data.Url}/{data.Endpoint}").
                    SetHeaders(data.Headers).
                    SetContent(data.Content).
                    SetQuery(data.Query);

            testData[0] = request;
            testData[1] = data.Expected;

            testData[2] =
            data.LogDirectory == null ? configData.LogDirectory : data.LogDirectory;
            logger._logger.LogTrace($"Tester data is ready!");

            yield return testData;
        }
    }
    public Tester()
    {
        string content = String.Empty;
        try
        {
            using (StreamReader sr = new StreamReader("config.json"))
            {
                content = sr.ReadToEnd();
            }
            configData = JsonConvert.DeserializeObject<ConfigData>(content);
            configData.TestGroups = configData.TestGroups.Where(x => x.Ignore.Equals("False")).ToArray();
        }
        catch (Exception ex)
        {
            logger._logger.LogError($"Error: {ex.Message}");
            Environment.Exit(1);
        }
        logger._logger.LogTrace($"Tester is ready!");
    }
}