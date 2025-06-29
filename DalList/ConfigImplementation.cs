using DalApi;
using System.Runtime.CompilerServices;

namespace Dal;

internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => Config.Clock;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set => Config.Clock = value;
    }
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public TimeSpan GetRiskRange()
    {
        return Config.RiskRange;
    }
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void SetRiskRange(TimeSpan value)
    {
        Config.RiskRange = value;
    }
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Reset()
    {
        Config.Reset();
    }
 

}
