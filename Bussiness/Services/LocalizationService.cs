using Microsoft.Extensions.Localization;

namespace Bussiness.Services;

public interface ILocalizationService
{
    string GetLocalizedString(string key);
    string GetLocalizedString(string key, params object[] arguments);
}

public class LocalizationService : ILocalizationService
{
    private readonly IStringLocalizer<LocalizationService> _localizer;

    public LocalizationService(IStringLocalizer<LocalizationService> localizer)
    {
        _localizer = localizer;
    }

    public string GetLocalizedString(string key)
    {
        return _localizer[key].Value;
    }

    public string GetLocalizedString(string key, params object[] arguments)
    {
        return string.Format(_localizer[key].Value, arguments);
    }
}
