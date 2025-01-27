
namespace DalApi;
    /// Represents the Data Access Layer (DAL) interface, 
    /// providing access to various entity-specific interfaces and database management.
    public interface IDal
    {
    IAssignment Assignment { get; } /// Provides access to assignment-related data and operations.                                     
    IVolunteer Volunteer { get; }/// Provides access to volunteer-related data and operations
                              
    ICall Call { get; }   /// Provides access to call-related data and operations.
                         
    IConfig Config { get; } /// Provides access to configuration-related data and operations.
                          
    void ResetDB();  /// Resets the database, including all entities and configuration data.
}
