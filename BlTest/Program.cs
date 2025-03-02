using BlApi;
using BO;

namespace BlTest;

internal class Program
{
    // Enums for menu choices
    private enum AdminAction
    {
        Logout,
        ManageVolunteers,
        ManageCalls,
        ViewStatistics
    }

    private enum VolunteerAction
    {
        Logout,
        ViewProfile,
        UpdateVolunteerDetails,
        CompleteCallTreatment,
        CancelCallTreatment,
        GetClosedCallsByVolunteer,
        GetOpenCallsForVolunteer
    }

    private enum VolunteerManagementAction
    {
        Back,
        ViewAll,
        Add,
        Update,
        Delete
    }

    private enum CallManagementAction
    {
        Back,
        ViewAll,
        Add,
        Update,
        Delete
    }
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private static int s_currentUserId = 0;
    private static Role s_currentUserRole;

    private static void Main(string[] args)
    {
        while (true)
        {
            try
            {
                // Initial login screen
                Console.Clear();
                Console.WriteLine("Welcome to Volunteer Management System");
                Console.WriteLine("====================================");
                Console.WriteLine("Please login to continue");
                Console.Write("Username: ");
                string? username = Console.ReadLine();
                Console.Write("Password: ");
                string? password = Console.ReadLine();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Username and password cannot be empty!");
                    continue;
                }

                // Perform login
                s_currentUserRole = s_bl.Volunteer.Login(username, password);

                // After successful login, show appropriate menu based on role
                switch (s_currentUserRole)
                {
                    case Role.Manager:
                        AdminMainMenu();
                        break;
                    case Role.Volunteer:
                        VolunteerMainMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid role assigned!");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Additional Info: {ex.InnerException.Message}");
            }
           

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private static void AdminMainMenu()
    {
        AdminAction choice;
        do
        {
            Console.Clear();
            Console.WriteLine("\nAdmin Main Menu");
            Console.WriteLine("===============");
            Console.WriteLine("0: Logout");
            Console.WriteLine("1: Manage Volunteers");
            Console.WriteLine("2: Manage Calls");
            Console.WriteLine("3: View Statistics");

            choice = (AdminAction)GetEnumValue(typeof(AdminAction));

            switch (choice)
            {
                case AdminAction.Logout:
                    break;
                case AdminAction.ManageVolunteers:
                    ManageVolunteersMenu();
                    break;
                case AdminAction.ManageCalls:
                    ManageCallsMenu();
                    break;
                //case AdminAction.ViewStatistics:
                //    ViewStatistics();
                //    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        } while (choice != AdminAction.Logout);
    }

    private static void VolunteerMainMenu()
    {
        VolunteerAction choice;
        do
        {
            Console.Clear();
            Console.WriteLine("\nVolunteer Main Menu");
            Console.WriteLine("===================");
            Console.WriteLine("0: Logout");
            Console.WriteLine("1: View My Profile");
            Console.WriteLine("2: Update My Details ");
            Console.WriteLine("3: View My Active Calls");
            Console.WriteLine("4: View My Closed Calls");

            choice = (VolunteerAction)GetEnumValue(typeof(VolunteerAction));

            switch (choice)
            {
                case VolunteerAction.Logout:
                    break;
                case VolunteerAction.ViewProfile:
                    Console.Write("Enter volunteer ID number: ");
                    int number = int.TryParse(Console.ReadLine()??"", out int n) ? n : 0;
                    var volunteerDetails = s_bl.Volunteer.GetVolunteerDetails(number);
                    Console.WriteLine(volunteerDetails);
                    break;
                case VolunteerAction.UpdateVolunteerDetails:
                    Console.Write("Enter volunteer ID to update: ");
                    int updateId = int.TryParse(Console.ReadLine() ?? "", out int u) ? u: 0;
                    var existingVolunteer = s_bl.Volunteer.GetVolunteerDetails(updateId);
                    Console.Write($"Enter new FullName (current: {existingVolunteer.FullName}, press Enter to keep current): ");
                    string updatedName = Console.ReadLine() ?? existingVolunteer.FullName;
                    existingVolunteer.FullName = updatedName;

                    Console.Write($"Enter new PhoneNumber (current: {existingVolunteer.Phone}, press Enter to keep current): ");
                    string updatedPhoneNumber = Console.ReadLine() ?? existingVolunteer.Phone;
                    existingVolunteer.Phone = updatedPhoneNumber;

                    Console.Write($"Enter new Email (current: {existingVolunteer.Email}, press Enter to keep current): ");
                    string updatedEmail = Console.ReadLine() ?? existingVolunteer.Email;
                    existingVolunteer.Email = updatedEmail;

                    Console.Write($"Enter new Address (current: {existingVolunteer.CurrentAddress}, press Enter to keep current): ");
                    string updatedAddress = Console.ReadLine() ?? existingVolunteer.CurrentAddress!;
                    existingVolunteer.CurrentAddress = updatedAddress;
                    s_bl.Volunteer.UpdateVolunteerDetails(updateId, existingVolunteer);
                    Console.WriteLine("Volunteer updated successfully");
                    break;
                case VolunteerAction.CompleteCallTreatment:
                    Console.Write($"Enter volunteer ID to update assigment");
                    int UpdateId = int.TryParse(Console.ReadLine() ?? "", out int i) ? i : 0;
                    Console.Write($"Enter assigment ID to Complete the assigment");
                    int AssigmentId = int.TryParse(Console.ReadLine() ?? "", out int a) ? a : 0;
                    s_bl.Call.CompleteCallTreatment(UpdateId, AssigmentId);
                    break;
                case VolunteerAction.CancelCallTreatment:
                    Console.Write($"Enter volunteer ID to update assigment");
                    int VolunteerId = int.TryParse(Console.ReadLine() ?? "", out int v) ? v : 0;
                    Console.Write($"Enter assigment ID to Complete the assigment");
                    int AssigmentCencelId = int.TryParse(Console.ReadLine() ?? "", out int c) ? c : 0;
                    s_bl.Call.CancelCallTreatment(VolunteerId, AssigmentCencelId);
                    break;
                case VolunteerAction.GetClosedCallsByVolunteer:
                    Console.Write("Enter volunteer ID: ");
                    if (int.TryParse(Console.ReadLine(), out int volId))
                    {
                        Console.WriteLine("Filter by call type? (1: FlatTire, 2: DeadBattery, 3: EngineFailure , 0: None):");
                        TypeOfReading? filterType = null;
                        if (int.TryParse(Console.ReadLine(), out int typeFilter) && typeFilter > 0 && typeFilter <= 3)
                            filterType = (TypeOfReading)typeFilter;
                        Console.WriteLine("Sort by call type? (0: Status, 1: OpeningTime, 2: MaxEndTime , 3: Address):");
                        CallField? CallFieldType = null;
                        if (int.TryParse(Console.ReadLine(), out int CallField) && CallField > 0 && CallField <= 3)
                            CallFieldType = (CallField)CallField;
                        var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(volId, filterType, CallFieldType);
                        foreach (var closedCall in closedCalls)
                            Console.WriteLine(closedCall);
                    }
                    break;
                //case VolunteerAction.GetOpenCallsForVolunteer:
                //    Console.Write("Enter volunteer ID: ");
                //    if (int.TryParse(Console.ReadLine(), out int volunteerId))
                //    {
                //        Console.WriteLine("Filter by call type? (1: Urgent, 2: Medium_Urgency, 3: General_Assistance, 4: Non_Urgent, 0: None):");
                //        var closedCalls = s_bl.Call.GetOpenCallsForVolunteer(volId, filterType, CallFieldType);
                //        foreach (var closedCall in closedCalls)
                //            Console.WriteLine(closedCall);
                //    }

                //        break;
            }
        } while (choice != VolunteerAction.Logout);
    }



    private static void ManageVolunteersMenu()
    {
        VolunteerManagementAction choice;
        do
        {
            Console.WriteLine("\nManage Volunteers Menu");
            Console.WriteLine("=====================");
            Console.WriteLine("0: Back to Main Menu");
            Console.WriteLine("1: View All Volunteers");
            Console.WriteLine("2: Add New Volunteer");
            Console.WriteLine("3: Update Volunteer");
            Console.WriteLine("4: Delete Volunteer");

            choice = (VolunteerManagementAction)GetEnumValue(typeof(VolunteerManagementAction));

            switch (choice)
            {
                case VolunteerManagementAction.Back:
                    break;
                case VolunteerManagementAction.ViewAll:
                    s_bl.Volunteer.GetVolunteerDetails();
                    break;
                case VolunteerManagementAction.Add:
                    s_bl.Volunteer.AddVolunteer(updateId, existingVolunteer);
                    break;
                case VolunteerManagementAction.Update:
                    Console.Write("Enter volunteer ID to update: ");
                    int updateId = int.TryParse(Console.ReadLine() ?? "", out int u) ? u : 0;
                    var existingVolunteer = s_bl.Volunteer.GetVolunteerDetails(updateId);
                    Console.Write($"Enter new FullName (current: {existingVolunteer.FullName}, press Enter to keep current): ");
                    string updatedName = Console.ReadLine() ?? existingVolunteer.FullName;
                    existingVolunteer.FullName = updatedName;
                    s_bl.Volunteer.UpdateVolunteerDetails(updateId, existingVolunteer);
                    break;
                case VolunteerManagementAction.Delete:
                    s_bl.Volunteer.DeleteVolunteer(updateId, existingVolunteer);
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
        } while (choice != VolunteerManagementAction.Back);
    }

    private static void ManageCallsMenu()
    {
        //CallManagementAction choice;
        //do
        //{
        //    Console.WriteLine("\nManage Calls Menu");
        //    Console.WriteLine("=================");
        //    Console.WriteLine("0: Back to Main Menu");
        //    Console.WriteLine("1: View All Calls");
        //    Console.WriteLine("2: Add New Call");
        //    Console.WriteLine("3: Update Call");
        //    Console.WriteLine("4: Delete Call");

        //    choice = (CallManagementAction)GetEnumValue(typeof(CallManagementAction));

        //    switch (choice)
        //    {
        //        case CallManagementAction.Back:
        //            break;
        //        case CallManagementAction.ViewAll:
        //            ViewAllCalls();
        //            break;
        //        case CallManagementAction.Add:
        //            AddCall();
        //            break;
        //        case CallManagementAction.Update:
        //            UpdateCall();
        //            break;
        //        case CallManagementAction.Delete:
        //            DeleteCall();
        //            break;
        //        default:
        //            Console.WriteLine("Invalid choice");
        //            break;
        //    }
        //} while (choice != CallManagementAction.Back);
    }

    // Helper method to get enum value with input validation
    private static int GetEnumValue(Type enumType)
    {
        while (true)
        {
            Console.Write("\nEnter your choice: ");
            if (int.TryParse(Console.ReadLine(), out int choice) &&
                Enum.IsDefined(enumType, choice))
            {
                return choice;
            }
            Console.WriteLine("Invalid input, please try again");
        }
    }

 
}