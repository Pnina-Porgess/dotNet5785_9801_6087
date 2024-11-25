namespace DalApi;
public interface IConfig
{
    DateTime Clock { get; set; }
    public TimeSpan RiskRange { get; set; }
    void Reset();
}
