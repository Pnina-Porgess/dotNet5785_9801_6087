namespace DalApi;
public interface IConfig
{
    DateTime Clock { get; set; }

    TimeSpan GetRiskRange();
    void SetRiskRange(TimeSpan value);
    void Reset();
}
