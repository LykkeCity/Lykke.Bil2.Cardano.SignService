using Lykke.Bil2.Sdk.SignService;
using Lykke.Bil2.Sdk.SignService.Settings;

namespace Lykke.Bil2.Cardano.SignService.Settings
{
    /// <summary>
    /// Specific blockchain settings
    /// </summary>
    public class AppSettings : BaseSignServiceSettings
    {
        public string Network { get; set; }
    }
}