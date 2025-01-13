using Microsoft.Extensions.Logging;

class APITesterLogs
{
    public readonly ILogger _logger;
    private static APITesterLogs? _instance;
    private APITesterLogs(ILogger logger)
    {
        this._logger = logger;
    }
    public static APITesterLogs GetInstance(LogLevel level=LogLevel.Information)
    {
        if (_instance != null)
        {
            return _instance;
        }
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole().SetMinimumLevel(level);
        });

        ILogger<APITesterLogs> logger = loggerFactory.CreateLogger<APITesterLogs>();

        _instance = new APITesterLogs(logger);
        return _instance;
    }
}

