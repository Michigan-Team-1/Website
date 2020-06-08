using Microsoft.Extensions.Configuration;

namespace Team1.Infrastructure
{
    public class AppSettings
    {
        public AppSettings(IConfiguration configuration)
        {
            JWTSettings = configuration.GetSection(nameof(JWTSettings)).Get<JWTSettings>();
            SupportedCultures = configuration.GetSection(nameof(SupportedCultures)).Get<SupportedCultures>();
        }

        public JWTSettings JWTSettings { get; }
        public SupportedCultures SupportedCultures { get; }

        public const int PasswordMinimumLength = 12;
    }
}
