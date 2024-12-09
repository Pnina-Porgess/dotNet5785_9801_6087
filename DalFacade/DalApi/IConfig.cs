namespace DalApi;
/// <summary>
/// The configuration entity (config) is a static entity. No instances/objects will be created for this entity. Fields of this entity are like "system variables".
/// </summary>
public interface IConfig
{
    DateTime Clock { get; set; }

    TimeSpan GetRiskRange();
    void SetRiskRange(TimeSpan value);
    void Reset();
}
