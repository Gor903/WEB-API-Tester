using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
public class Tester
{
    private APITesterLogs logger = APITesterLogs.GetInstance();
    private static Tester instance = new();
    private ConfigReader configReader = new();
    private ConfigData? configData;
    private object[] testData = new object[3];
    HttpRequestBuilder? request = null;
    public static IEnumerable<Object[]> GetTestData()
    {
        Tester tester = GetInstance();
        string testConfigPath = tester.configData.TestConfigsPath;

        foreach (TestConfigData data in tester.configReader.ProcessDirectory(testConfigPath, tester.configData.TestGroups))
        {
            tester.request = new();

            tester.request.SetMethod(data.Method).
                    SetUrl($"{data.Url}/{data.Endpoint}").
                    SetHeaders(data.Headers).
                    SetContent(data.Content).
                    SetQuery(data.Query);

            tester.testData[0] = tester.request;
            tester.testData[1] = data.Expected;

            tester.testData[2] =
                data.LogDirectory == null ? tester.configData.LogDirectory : data.LogDirectory;
            tester.logger._logger.LogTrace($"Tester data is ready!");

            yield return tester.testData;
        }
    }
    private static Tester GetInstance()
    {
        instance = new Tester();
        string content = String.Empty;
        using (StreamReader sr = new StreamReader("C:\\Users\\gbegl\\source\\repos\\WEB-API-Tester\\WEB-API-Tester\\config.json"))
        {
            content = sr.ReadToEnd();
        }
        instance.configData = JsonConvert.DeserializeObject<ConfigData>(content);
        instance.configData.TestGroups = instance.configData.TestGroups.Where(x => x.Ignore.Equals("False")).ToArray();
        instance.logger._logger.LogTrace($"Tester is ready!");
        return instance;
    }
}