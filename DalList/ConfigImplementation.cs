using DalApi;
using System.Runtime.CompilerServices;

namespace Dal;

internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan GetRiskRange()
    {
        return Config.RiskRange;
    }
    public void SetRiskRange(TimeSpan value)
    {
        Config.RiskRange = value;
    }
    public void Reset()
    {
        Config.Reset();
    }
 

}
