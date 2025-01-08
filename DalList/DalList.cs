using DalApi;
namespace Dal;
/// Implements the IDal interface using a list-based data storage approach.
/// Provides access to entity-specific implementations and manages data operations.
sealed internal class DalList : IDal
{
    public static IDal Instance { get; } = new DalList();
    private DalList() { }
    public IAssignment Assignment { get; } = new AssignmentImplementation();/// Provides access to assignment-related data operations.
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();   /// Provides access to volunteer-related data operations.
    public ICall Call { get; } = new CallImplementation();    /// Provides access to call-related data operations.
    public IConfig Config { get; } = new ConfigImplementation();  /// Provides access to configuration-related data operations.
  

    public void ResetDB()/// Resets the database by clearing all entity lists and resetting configuration data.
    {
        Assignment.DeleteAll();
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Config.Reset();
    }

}
