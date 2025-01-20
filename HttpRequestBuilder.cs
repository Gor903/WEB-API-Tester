using Newtonsoft.Json;
using System.Reflection.PortableExecutable;
using System.Text;

public class HttpRequestBuilder
{
    public HttpMethod _method { get; private set; }
    public string _url { get; private set; }
    public Dictionary<string, object> _content { get; private set; }
    public Dictionary<string, string> _headers { get; private set; } 
    public Dictionary<string, string> _query { get; private set; }

    public HttpRequestBuilder()
    {
        _headers = new Dictionary<string, string>();
        _query = new Dictionary<string, string>();
        _content = new Dictionary<string, object>();
        _url = String.Empty;
        _method = HttpMethod.Get;
    }
    public override string ToString()
    {
        return $"{{" +
            $"\n\tHttpMethod: {_method}" +
            $"\n\tUrl: {_url}" +
            $"\n\tContent: {{\n\t\t{string.Join("\n\t\t", _content.Select(pair => $"{pair.Key}: {pair.Value}"))}\n\t}}" +
            $"\n\tHeaders: {{\n\t\t{string.Join("\n\t\t", _headers.Select(pair => $"{pair.Key}: {pair.Value}"))}\n\t}}" +
            $"\n\tQuery: {{\n\t\t{string.Join("\n\t\t", _query.Select(pair => $"{pair.Key}: {pair.Value}"))}\n\t}}" +
            $"\n}}";
    }
    public HttpRequestBuilder SetMethod(string method)
    {
        switch (method)
        {
            case "GET": _method = HttpMethod.Get; break;
            case "POST": _method = HttpMethod.Post; break;
            case "PUT": _method = HttpMethod.Put; break;
            case "DELETE": _method = HttpMethod.Delete; break;
            case "PATCH": _method = HttpMethod.Patch; break;
        }
        return this;
    }
    public HttpRequestBuilder SetUrl(string url)
    {
        _url = url;
        return this;
    }
    public HttpRequestBuilder SetQuery(Dictionary<string, string> query)
    {
        if (query == null) return this;
        _query = _query.Concat(query)
                       .GroupBy(pair => pair.Key)
                       .ToDictionary(group => group.Key, group => group.Last().Value);
        return this;
    }

    public HttpRequestBuilder SetContent(Dictionary<string, object> content)
    {
        if (content == null) return this; 
        _content = _content.Concat(content)
                       .GroupBy(pair => pair.Key)
                       .ToDictionary(group => group.Key, group => group.Last().Value);
        return this;
    }

    public HttpRequestBuilder SetHeaders(Dictionary<string, string> headers)
    {
        if (headers == null) return this;
        _headers = _headers.Concat(headers)
                       .GroupBy(pair => pair.Key)
                       .ToDictionary(group => group.Key, group => group.Last().Value);
        return this;
    }

    public HttpRequestMessage Build()
    {
        string jsonString = JsonConvert.SerializeObject(_content);
        HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
        if (_query.Keys.Count > 0)
        {
            _url += $"?{string.Join("&", _query.Select(
                pair => $"" +
                $"{Uri.EscapeDataString(pair.Key)}=" +
                $"{Uri.EscapeDataString(pair.Value)}"
            ))}";
        }
        var request = new HttpRequestMessage(_method, _url)
        {
            Content = content
        };

        if (_headers != null)
        {
            foreach (var header in _headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }
        
        return request;
    }
}

