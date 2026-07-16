public interface IConfiguration
{
    void LoadConfiguration();
}

public interface IParse
{
   Dictionary<string, string> ParseSettings(); 
}

public class ApplicationSettingsManager : IConfiguration
{
    private readonly IParse _parser;

    public ApplicationSettingsManager(IParse parser)
    {
        _parser = parser;
    }

    public void LoadConfiguration()
    {
        var settings = _parser.ParseSettings();
    }
}

public class JsonConfigParser : IParse
{
    public Dictionary<string, string> ParseSettings()
    {
        return new Dictionary<string, string> { { "Theme", "Dark" } };
    }
}