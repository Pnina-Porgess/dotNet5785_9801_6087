

namespace DalApi;
public interface IDal
{
    IAssignment Assignment { get; }
    IVolunteer Volunteer { get; }
    ICall call { get; }
    IConfig Config { get; }

    void ResetDB();
}

