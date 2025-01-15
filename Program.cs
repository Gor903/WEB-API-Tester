using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Text.Json;


LogLevel level = LogLevel.Information;
int success = 0, fail = 0;

if (args.Length > 0)
{
    string logLevel = args[0];
    switch (logLevel)
    {
        case "trce": level = LogLevel.Trace; break;
        case "dbug": level = LogLevel.Debug; break;
        case "info": level = LogLevel.Information; break;
        case "warn": level = LogLevel.Warning; break;
        case "fail": level = LogLevel.Error; break;
        case "crit": level = LogLevel.Critical; break;
        default: level = LogLevel.Information; break;
    }
}

APITesterLogs logger = APITesterLogs.GetInstance(level);
Dictionary<string, string> data = [];
HttpResponseMessage? response = null;
Validator validator = new();

foreach (var item in Tester.GetTestData())
{
    HttpRequestBuilder request = (HttpRequestBuilder)item[0];
    JToken expected = (JToken)item[1];
    string logDirectory = (string)item[2];

    try
    {
        List<string[]> dynamicFields = GetDynamicFields(expected);
        using (HttpClient client = new HttpClient())
        {
            response = await client.SendAsync(GetRequest(request));
        }
        string actual = await response.Content.ReadAsStringAsync();
        if (!JtokenTryParse(actual, out JToken actualToken))
        {
            throw new Exception(actualToken.ToString());
        }
        logger._logger.LogDebug($"Response received: {actualToken.ToString()}");
        for (int i = 0; i < dynamicFields.Count; i++)
        {
            string temp = validator.FieldValidator(dynamicFields[i][0], dynamicFields[i][1], ref actualToken);
            if (temp != null && !dynamicFields[i][1].Contains('-'))
            {
                data[dynamicFields[i][0][1..]] = temp;
            }
        }
        bool result = JToken.DeepEquals(actualToken, expected);
        if (result)
        {
            logger._logger.LogInformation($"Passed: {request._method} -> {request._url}");
            success++;
        }
        else
        {
            logger._logger.LogWarning($"Fatal: {request._method} -> {request._url}");
            using (StreamWriter sw = new StreamWriter(Path.Combine(logDirectory, "logs.txt"), true))
            {
                sw.WriteLine($"Request: {request}");
                sw.WriteLine($"Expected: {expected}");
                sw.WriteLine($"Actual: {JToken.Parse(actual)}");
                sw.WriteLine($"Data: {{\n\t{string.Join("\n\t", data.Select(pair => $"{pair.Key}: {pair.Value}"))}\n}}");
                sw.WriteLine(new string('=', 100));
            }
            fail++;
        }
    }
    catch (Exception ex)
    {
        logger._logger.LogError($"Error: {ex.Message}");
        using (StreamWriter sw = new StreamWriter(Path.Combine(logDirectory, "error.txt"), true))
        {
            sw.WriteLine(new string('#', 100));
            sw.WriteLine($"Request: {request}");
            sw.WriteLine(new string('#', 100));
        }
    }
}

logger._logger.LogInformation($"Success: {success}. Fail: {fail}. Total: {success + fail}");
HttpRequestMessage GetRequest(HttpRequestBuilder request)
{
    string url = request._url;
    var headers = request._headers;
    var content = request._content;
    var query = request._query;
    foreach (var pair in data)
    {
        if (url.Contains($"*{pair.Key}"))
        {
            url = url.Replace($"*{pair.Key}", pair.Value);
        }
        else if (headers.ContainsKey($"*{pair.Key}"))
        {
            headers.Remove($"*{pair.Key}");
            if (pair.Key == "access_token" || pair.Key == "refresh_token")
            {
                headers["Authorization"] = $"Bearer {pair.Value}";
            }
            else
            {
                headers[pair.Key] = pair.Value;
            }
        }
        else if (content.ContainsKey($"*{pair.Key}"))
        {
            content.Remove($"*{pair.Key}");
            content[pair.Key] = pair.Value;
        }
        else if (query.ContainsKey($"*{pair.Key}"))
        {
            query.Remove($"*{pair.Key}");
            query[pair.Key] = pair.Value;
        }
        logger._logger.LogTrace("Request is ready!");
    }

    return request.SetUrl(url).
            SetContent(content).
            SetHeaders(headers).
            SetQuery(query).
            Build();
}
List<string[]> GetDynamicFields(JToken token)
{
    List<string[]> dynamicFields = new List<string[]>();
    if (token is JObject obj)
    {
        foreach (var item in obj)
        {
            if (item.Key.StartsWith('*'))
            {
                dynamicFields.Add(new string[] { item.Key.ToString(), item.Value.ToString() });
            }
        }
    }
    else if (token is JArray array)
    {
        foreach (var item in array)
        {
            return GetDynamicFields(item);
        }
    }
    return dynamicFields;
}
bool JtokenTryParse(string value, out JToken token)
{
    try
    {
        token = JToken.Parse(value);
        return true;
    }
    catch (JsonException jex)
    {
        string message = $"{{\"Error message\": {jex}}}";
        token = JToken.Parse(message);
        return false;
    }
}