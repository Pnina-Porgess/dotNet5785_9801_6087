using DalApi;
namespace Dal;
/// <summary>
/// A class that will inherit and implement the IDal interface by initializing the subinterfaces in the access classes that we implemented with XML.
/// </summary>
sealed public class DalXml : IDal
{
    public IAssignment Assignment { get; } = new AssignmentImplementation();
   
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
   
    public ICall Call { get; } = new CallImplementation();
   
    public IConfig Config { get; } = new ConfigImplementation();
    /// <summary>
    /// A method that resets all data lists including configuration data.
    /// </summary>
    public void ResetDB()
    {
        Assignment.DeleteAll();
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Config.Reset();
    }

}

