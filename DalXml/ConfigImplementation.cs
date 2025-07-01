using DalApi;
using System.Runtime.CompilerServices;

namespace Dal;

/// Implementation of the IConfig interface for managing system configuration settings.
internal class ConfigImplementation : IConfig
{
    /// Gets or sets the system clock.
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    /// Returns the configured risk range duration as a TimeSpan.
    public TimeSpan GetRiskRange()
    {
        return Config.RiskRange;
    }

    /// Sets the risk range duration to the specified TimeSpan.
    public void SetRiskRange(TimeSpan value)
    {
        Config.RiskRange = value;
    }
    /// Resets all configuration settings to their default values.
    public void Reset()
    {

        Config.Reset();
    }
}
