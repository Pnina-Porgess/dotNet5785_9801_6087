using DalApi;
using System.Runtime.CompilerServices;

namespace Dal;

/// Implementation of the IConfig interface for managing system configuration settings.
internal class ConfigImplementation : IConfig
{
    /// Gets or sets the system clock.
    public DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => Config.Clock;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set => Config.Clock = value;
    }

    /// Returns the configured risk range duration as a TimeSpan.
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public TimeSpan GetRiskRange()
    {
        return Config.RiskRange;
    }

    /// Sets the risk range duration to the specified TimeSpan.
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void SetRiskRange(TimeSpan value)
    {
        Config.RiskRange = value;
    }
    /// Resets all configuration settings to their default values.
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Reset()
    {

        Config.Reset();
    }
}
