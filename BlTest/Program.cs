using BlApi;
using BO;
using DO;
using System;

namespace BlTest;

internal class Program
{
   
    private enum MainMenu
    {
      LogOut,
      ManageVolunteers,
      ManageCalls,
      Admin
    }
    private enum VolunteerMenu
    {
        Logout,
        AddVolunteer,
        DeleteVolunteer,
        Login,
        GetVolunteerDetails,
        UpdateVolunteerDetails,
        GetVolunteersList
    }
    
    enum AdminFunctions
    {
        Logout,
        GetClock,
        ForwardClock,
        GetRiskRange,
        SetRiskRange,
        ResetDB,
        InitializeDB
    }
    private enum CallMenu
    {
        Back,
        AddCall,
        UpdateCall,
        DeleteCall,
        GetCallDetails,
        GetCalls,
        GetCallCountsByStatus,
        CancelCallTreatment,
        CompleteCallTreatment,
        GetClosedCallsByVolunteer,
        GetOpenCallsForVolunteer,
        SelectCallForTreatment
    }
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private static int s_currentUserId = 0;
    private static BO.Role s_currentUserRole;

    private static void Main(string[] args)
    {
        MainMenu choice;
        while (true)
        {
            try
            {
                // Initial login screen
               
                Console.WriteLine("Welcome to Volunteer Management System");
                Console.WriteLine("====================================");
                Console.WriteLine("Please press 0 to log out, 1 for the volunteer menu, 2 for the readings menu, 3 for the administrator menu.");
                choice = (MainMenu)GetEnumValue(typeof(MainMenu));
                // After successful login, show appropriate menu based on role
                switch (choice)
                {
                    case MainMenu.LogOut:
                        break;
                    case MainMenu.ManageVolunteers:
                        VolunteerMainMenu();
                        break;
                    case MainMenu.ManageCalls:
                        CallMainMenu();
                        break;
                    case MainMenu.Admin:
                        AdminMainMenu();
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
        }
    }

   

    private static void VolunteerMainMenu()
    {
        VolunteerMenu choice;
        do
        {
            Console.WriteLine("\nVolunteer Main Menu");
            Console.WriteLine("===================");
            Console.WriteLine("0:Logout");
            Console.WriteLine("1:Add Volunteer");
            Console.WriteLine("2:Delete Volunteer ");
            Console.WriteLine("3:Login");
            Console.WriteLine("4:Get Volunteer Details");
            Console.WriteLine("5:Update Volunteer");
            Console.WriteLine("6:Get Volunteers List");
            choice = (VolunteerMenu)GetEnumValue(typeof(VolunteerMenu));

            switch (choice)
            {
                case VolunteerMenu.Logout:
                    break;
                case VolunteerMenu.AddVolunteer:
                    AddVolunteer();
                    break;
                case VolunteerMenu.DeleteVolunteer:
                    DeleteVolunteer();
                    break;
                case VolunteerMenu.Login:
                    Login();
                    break;
                case VolunteerMenu.GetVolunteerDetails:
                    GetVolunteerDetails();
                    break;
                case VolunteerMenu.UpdateVolunteerDetails:
                    UpdateVolunteerDetails();
                    break;
                case VolunteerMenu.GetVolunteersList:
                    GetVolunteersList();
                    break;
            }
        } while (choice != VolunteerMenu.Logout);
    }
    private static void CallMainMenu()
    {
        CallMenu choice;
        do
        {
            Console.WriteLine("\nManage Calls Menu");
            Console.WriteLine("=================");
            Console.WriteLine("0: Back to Main Menu");
            Console.WriteLine("1: Add New Call");
            Console.WriteLine("2: Update Call");
            Console.WriteLine("3: delete Call");
            Console.WriteLine("4: Get Call Details");
            Console.WriteLine("5: Get Calls");
            Console.WriteLine("6: Get Call Counts By Status");
            Console.WriteLine("7: Cancel Call Treatment");
            Console.WriteLine("8: Complete Call Treatment");
            Console.WriteLine("9: Get Closed Calls By Volunteer");
            Console.WriteLine("10: Get Open Calls For Volunteer");
            Console.WriteLine("11: Select Call For Treatment");

            choice = (CallMenu)GetEnumValue(typeof(CallMenu));

            switch (choice)
            {
                case CallMenu.AddCall:
                    AddCall();
                    break;
                case CallMenu.DeleteCall:
                    DeleteCall();
                    break;
                case CallMenu.UpdateCall:
                    UpdateCall();
                    break;
                case CallMenu.GetCallDetails:
                    GetCallDetails();
                    break;
                case CallMenu.GetCalls:
                    GetCalls();
                    break;
                case CallMenu.GetCallCountsByStatus:
                    GetCallCountsByStatus();
                    break;
                case CallMenu.CancelCallTreatment:
                    CancelCallTreatment();
                    break;
                case CallMenu.CompleteCallTreatment:
                    CompleteCallTreatment();
                    break;
                case CallMenu.GetClosedCallsByVolunteer:
                    GetClosedCallsByVolunteer();
                    break;
                case CallMenu.GetOpenCallsForVolunteer:
                    GetOpenCallsForVolunteer();
                    break;
                case CallMenu.SelectCallForTreatment:
                    SelectCallForTreatment();
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        } while (choice != CallMenu.Back);
    }
    private static void AdminMainMenu()
    {
        AdminFunctions choice;

        do
        {
            Console.WriteLine("\nAdmin Main Menu");
            Console.WriteLine("===============");
            Console.WriteLine("0: Back to Main Menu");
            Console.WriteLine("1: Get Current Clock");
            Console.WriteLine("2: Forward Clock");
            Console.WriteLine("3: Get Risk Range");
            Console.WriteLine("4: Set Risk Range");
            Console.WriteLine("5: Reset Database");
            Console.WriteLine("6: Initialize Database");

            choice = (AdminFunctions)GetEnumValue(typeof(AdminFunctions));

            switch (choice)
            {
                case AdminFunctions.Logout:
                    break;

                case AdminFunctions.GetClock:
                    Console.WriteLine($"Current Clock: {s_bl.Admin.GetClock()}");
                    break;

                case AdminFunctions.ForwardClock:
                    Console.WriteLine("Choose time unit to forward (0: Minute, 1: Hour, 2: Day, 3: Month, 4: Year):");
                    if (Enum.TryParse(Console.ReadLine(), out BO.TimeUnit unit))
                    {
                        s_bl.Admin.ForwardClock(unit);
                        Console.WriteLine("Clock forwarded successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case AdminFunctions.GetRiskRange:
                    Console.WriteLine($"Risk Range: {s_bl.Admin.GetRiskRange()}");
                    break;

                case AdminFunctions.SetRiskRange:
                    Console.Write("Enter new risk range in hours: ");
                    if (double.TryParse(Console.ReadLine(), out double hours))
                    {
                        s_bl.Admin.SetRiskRange(TimeSpan.FromHours(hours));
                        Console.WriteLine("Risk range updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;

                case AdminFunctions.ResetDB:
                    s_bl.Admin.ResetDB();
                    Console.WriteLine("Database reset successfully.");
                    break;

                case AdminFunctions.InitializeDB:
                    s_bl.Admin.InitializeDB();
                    Console.WriteLine("Database initialized successfully.");
                    break;

                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }


        } while (choice != AdminFunctions.Logout);
    }

    private static void AddVolunteer()
    {
        Console.Write("Enter ID to add volunteer: ");
        int id = int.TryParse(Console.ReadLine() ?? "", out int u) ? u : 0;
        Console.Write("Enter new FullName: ");
        string name = Console.ReadLine()!;
        Console.Write("Enter new PhoneNumber: ");
        string phone = Console.ReadLine()!;
        Console.Write("Enter new Email: ");
        string email = Console.ReadLine()!;
        Console.Write("Enter new password: ");
        string password = Console.ReadLine()!;
        Console.Write("Enter new Address: ");
        string address = Console.ReadLine()!;
        Console.Write("Enter max Distance: ");
        int maxDistance = int.TryParse(Console.ReadLine() ?? "", out int d) ? d : 0;
        Console.Write("Enter Distance Type (1: Air, 2: Walking, 3: Driving): ");
        BO.DistanceType distanceType = (BO.DistanceType)int.Parse(Console.ReadLine()!);
        Console.Write("Enter role (0: Manager, 1: Volunteer): ");
        BO.Role role = (BO.Role)int.Parse(Console.ReadLine()!);

        BO.Volunteer newVolunteer = new BO.Volunteer
        {
            Id = id,
            FullName = name,
            Phone = phone,
            Email = email,
            Password = password,
            CurrentAddress = address,
            Role = role,
            IsActive = true,
            MaxDistance = maxDistance,
            DistanceType = distanceType,
            TotalCanceledCalls = 0,
            TotalHandledCalls = 0,
            TotalExpiredCalls = 0
        };

        s_bl.Volunteer.AddVolunteer(newVolunteer);
        Console.WriteLine("Volunteer added successfully");
    }

    private static void DeleteVolunteer()
    {
        Console.Write("Enter volunteer ID to delete: ");
        int volunteerId = int.TryParse(Console.ReadLine() ?? "", out int v) ? v : 0;
        s_bl.Volunteer.DeleteVolunteer(volunteerId);
        Console.WriteLine("Volunteer deleted successfully");
    }

    private static void Login()
    {
        while (true)
        {
            Console.Write("Username: ");
            string? username = Console.ReadLine();
            Console.Write("Password: ");
            string? password = Console.ReadLine();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Username and password cannot be empty!");
                continue;
            }
            Console.Write(s_bl.Volunteer.Login(username, password));
            break;
        }
    }

    private static void GetVolunteerDetails()
    {
        Console.Write("Enter volunteer ID number: ");
        int id = int.TryParse(Console.ReadLine() ?? "", out int n) ? n : 0;
        var volunteerDetails = s_bl.Volunteer.GetVolunteerDetails(id);
        Console.WriteLine(volunteerDetails);
    }

    private static void UpdateVolunteerDetails()
    {
        int updateId;
        string? Password;
        while (true)
        {
            Console.Write("id: ");
            updateId = int.TryParse(Console.ReadLine() ?? "", out int na) ? na : 0;
            Console.Write("Username: ");
            string? username = Console.ReadLine();
            Console.Write("Password: ");
            Password = Console.ReadLine();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(Password))
            {
                Console.WriteLine("Username and password cannot be empty!");
                continue;
            }
            Console.Write(s_bl.Volunteer.Login(username, Password));
            break;
        }
        var existingVolunteer = s_bl.Volunteer.GetVolunteerDetails(updateId);

        Console.Write($"Enter new FullName (current: {existingVolunteer.FullName}, press Enter to keep current): ");
        string input = Console.ReadLine()!;
        existingVolunteer.FullName = string.IsNullOrEmpty(input) ? existingVolunteer.FullName : input;

        Console.Write($"Enter new PhoneNumber (current: {existingVolunteer.Phone}, press Enter to keep current): ");
        input = Console.ReadLine()!;
        existingVolunteer.Phone = string.IsNullOrEmpty(input) ? existingVolunteer.Phone : input;

        Console.Write("Enter new Password (press Enter to keep current): ");
        input = Console.ReadLine()!;
        existingVolunteer.Password = string.IsNullOrEmpty(input) ? Password : input;

        Console.Write($"Enter new Email (current: {existingVolunteer.Email}, press Enter to keep current): ");
        input = Console.ReadLine()!;
        existingVolunteer.Email = string.IsNullOrEmpty(input) ? existingVolunteer.Email : input;

        Console.Write($"Enter new Address (current: {existingVolunteer.CurrentAddress}, press Enter to keep current): ");
        input = Console.ReadLine()!;
        existingVolunteer.CurrentAddress = string.IsNullOrEmpty(input) ? existingVolunteer.CurrentAddress : input;

        s_bl.Volunteer.UpdateVolunteerDetails(updateId, existingVolunteer);
        Console.WriteLine("Volunteer updated successfully");
    }

    private static void GetVolunteersList()
    {
        Console.Write("Enter true to show active, false for inactive, or any other value to show all: ");
        bool? isActive = bool.TryParse(Console.ReadLine() ?? "", out bool g) ? g : (bool?)null;
        Console.Write("Enter sorting option (0: FullName, 1: TotalHandledCalls, 2: TotalCanceledCalls, 3: TotalExpiredCalls): ");
        BO.VolunteerSortBy sortBy = (BO.VolunteerSortBy)int.Parse(Console.ReadLine()!);

        var volunteersList = s_bl.Volunteer.GetVolunteersList(isActive, sortBy);
        foreach (var volunteer in volunteersList)
        {
            Console.WriteLine(volunteer);
        }
    }

    private static void AddCall()
    {
        Console.Write($"Enter new type 1 for Regular, 2 for Emergency, 3 for HighPriority: ");
        CallType newType = (CallType)int.Parse(Console.ReadLine()!);
        Console.Write($"Enter new Description: ");
        string? Description = Console.ReadLine() ?? null;
        Console.Write($"Enter new Address: ");
        string Address = Console.ReadLine()!;
        Console.Write($"Enter new OpeningTime: ");
        DateTime OpeningTime = DateTime.Parse(Console.ReadLine()!);
        Console.Write($"Enter new MaxEndTime : ");
        DateTime? MaxEndTime = DateTime.Parse(Console.ReadLine()!);
        Console.Write($"Enter new   0 for Open,1 for InProgress,2 for Closed,3 for Expired,4 for OpenAtRisk,5 for InProgressAtRisk: ");
        CallStatus Status = (CallStatus)int.Parse(Console.ReadLine()!);
        BO.Call addCall = new BO.Call { Type = newType, Description = Description, Address = Address, OpeningTime = OpeningTime, MaxEndTime = MaxEndTime, Status = Status };
        s_bl.Call.AddCall(addCall);
        Console.WriteLine("Add call successfully");

    }

    private static void DeleteCall()
    {
        Console.Write("Enter Call ID to delete: ");
        int CallId = int.TryParse(Console.ReadLine() ?? "", out int v) ? v : 0;
        s_bl.Call.DeleteCall(CallId);
        Console.WriteLine("Delete call successfully");
    }

    private static void UpdateCall()
    {

        Console.Write("Enter call ID to update: ");
        int updateId = int.Parse(Console.ReadLine()!);
        var existingCall = s_bl.Call.GetCallDetails(updateId);

        Console.WriteLine("\nUpdating Call Details:");
        Console.WriteLine("=======================");


        Console.Write($"Enter new type (1 - Regular, 2 - Emergency, 3 - HighPriority), current: {existingCall.Type} (press Enter to keep current): ");
        string input = Console.ReadLine()!;
        existingCall.Type = (string.IsNullOrEmpty(input)) ? existingCall.Type : (CallType)int.Parse(input);


        Console.Write($"Enter new Description (current: {existingCall.Description}, press Enter to keep current): ");
        input = Console.ReadLine()!;
         existingCall.Description =(string.IsNullOrEmpty(input))? existingCall.Description: input;

        Console.Write($"Enter new Address (current: {existingCall.Address}, press Enter to keep current): ");
        input = Console.ReadLine()!;
        existingCall.Address =(string.IsNullOrEmpty(input))? existingCall.Address: input;

        Console.Write($"Enter new max end time (current: {existingCall.MaxEndTime}, press Enter to keep current): ");
        input = Console.ReadLine()!;
        if (DateTime.TryParse(input, out DateTime updatedMaxEndTime))
            existingCall.MaxEndTime = updatedMaxEndTime;

        Console.Write($"Enter new status (0 - Open, 1 - InProgress, 2 - Closed, 3 - Expired, 4 - OpenAtRisk, 5 - InProgressAtRisk), current: {existingCall.Status} (press Enter to keep current): ");
        input = Console.ReadLine()!;
         existingCall.Status =(string.IsNullOrEmpty(input)) ? existingCall.Status:(CallStatus)int.Parse(input);

        s_bl.Call.UpdateCall(existingCall);

        Console.WriteLine("Call updated successfully.");
    }
    private static void GetCalls()
    { // Step 1: Ask for filter field
        Console.WriteLine("Choose a field to filter by:");
        foreach (var field in Enum.GetValues(typeof(BO.CallField)))
        {
            Console.WriteLine($"{(int)field}: {field}");
        }

        // קבלת הבחירה מהמשתמש
        BO.CallField? filterField = null;
        object? filterValue = null;
        if (int.TryParse(Console.ReadLine(), out int filterFieldInput) && Enum.IsDefined(typeof(BO.CallField), filterFieldInput))
        {
            filterField = (BO.CallField)filterFieldInput;

            // Step 2: Get the value based on selected filter field
            switch (filterField)
            {
                case BO.CallField.CallType:
                    Console.WriteLine("Choose call type:");
                    foreach (var type in Enum.GetValues(typeof(BO.CallType)))
                    {
                        Console.WriteLine($"{(int)type}: {type}");
                    }
                    if (int.TryParse(Console.ReadLine(), out int typeInput) && Enum.IsDefined(typeof(BO.CallType), typeInput))
                    {
                        filterValue = (BO.CallType)typeInput;
                    }
                    break;

                case BO.CallField.CallStatus:
                    Console.WriteLine("Choose call status:");
                    foreach (var status in Enum.GetValues(typeof(BO.CallStatus)))
                    {
                        Console.WriteLine($"{(int)status}: {status}");
                    }
                    if (int.TryParse(Console.ReadLine(), out int statusInput) && Enum.IsDefined(typeof(BO.CallStatus), statusInput))
                    {
                        filterValue = (BO.CallStatus)statusInput;
                    }
                    break;

                case BO.CallField.AssignmentId:
                    Console.WriteLine("Enter assignment ID:");
                    if (int.TryParse(Console.ReadLine(), out int assignmentId))
                    {
                        filterValue = assignmentId;
                    }
                    break;

                case BO.CallField.CallId:
                    Console.WriteLine("Enter call ID:");
                    if (int.TryParse(Console.ReadLine(), out int callId))
                    {
                        filterValue = callId;
                    }
                    break;

                case BO.CallField.OpeningTime:
                    Console.WriteLine("Enter opening time (yyyy-MM-dd):");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime openingTime))
                    {
                        filterValue = openingTime;
                    }
                    break;

                case BO.CallField.TotalAssignments:

                    break;
                case BO.CallField.RemainingTime:

                    break;
                case BO.CallField.CompletionTime:

                    break;
                case BO.CallField.LastVolunteerName:

                    break;
            }
        }

        // Step 3: Ask for sorting field
        Console.WriteLine("Choose a field to sort by:");
        foreach (var field in Enum.GetValues(typeof(BO.CallField)))
        {
            Console.WriteLine($"{(int)field}:{field}");
        }
        BO.CallField? sortField = null;
        if (int.TryParse(Console.ReadLine(), out int sortFieldInput) && Enum.IsDefined(typeof(BO.CallField), sortFieldInput))
        {
            sortField = (BO.CallField)sortFieldInput;
        }
        // Step 4: Perform filtering and sorting
        try
        {
            var calls = s_bl.Call.GetCalls(filterField, filterValue, sortField);

            if (!calls.Any())
            {
                Console.WriteLine("No calls to display."); // No results found
                return;
            }
            // Step 5: Display results
            Console.WriteLine("\nFiltered and Sorted Calls:");
            foreach (var Call in calls)
            {
                Console.WriteLine("\n------------------------------");
                Console.WriteLine(Call.ToString());
                Console.WriteLine("------------------------------");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void GetCallDetails()
    {
        Console.Write("Enter call ID to get details: ");
        int CallIdToGetDetaails = int.TryParse(Console.ReadLine() ?? "", out int d) ? d : 0;
        Console.Write(s_bl.Call.GetCallDetails(CallIdToGetDetaails));
    }

    private static void GetCallCountsByStatus()
    {
        string[] statusNames = Enum.GetNames(typeof(CallStatus));
        int[] counts = s_bl.Call.GetCallCountsByStatus();
        for (int j = 0; j < counts.Length; j++)
            Console.WriteLine($"{statusNames[j]}: {counts[j]}");
    }

    private static void CancelCallTreatment()
    {
        Console.Write($"Enter volunteer ID to update assignment: ");
        int VolunteerId = int.TryParse(Console.ReadLine() ?? "", out int D) ? D : 0;
        Console.Write($"Enter assignment ID to complete the assignment: ");
        int AssigmentCencelId = int.TryParse(Console.ReadLine() ?? "", out int c) ? c : 0;
        s_bl.Call.CancelCallTreatment(VolunteerId, AssigmentCencelId);
    }

    private static void CompleteCallTreatment()
    {
        Console.Write($"Enter volunteer ID to update assignment: ");
        int UpdateId = int.TryParse(Console.ReadLine() ?? "", out int i) ? i : 0;
        Console.Write($"Enter assignment ID to complete the assignment: ");
        int AssigmentId = int.TryParse(Console.ReadLine() ?? "", out int a) ? a : 0;
        s_bl.Call.CompleteCallTreatment(UpdateId, AssigmentId);
    }

    private static void GetClosedCallsByVolunteer()
    {
        Console.Write("Enter volunteer ID: ");
        if (int.TryParse(Console.ReadLine(), out int volId))
        {
            Console.WriteLine("Filter by call type? (1: FlatTire, 2: DeadBattery, 3: EngineFailure , 0: None):");
            BO.TypeOfReading? filterType = null;
            if (int.TryParse(Console.ReadLine(), out int typeFilter) && typeFilter > 0 && typeFilter <= 3)
                filterType = (BO.TypeOfReading)typeFilter;
            Console.WriteLine("Sort by call type? (0: Status, 1: OpeningTime, 2: MaxEndTime , 3: Address):");
            BO.ClosedCallField? CallFieldType = null;
            if (int.TryParse(Console.ReadLine(), out int CallField) && CallField > 0 && CallField <= 3)
                CallFieldType = (BO.ClosedCallField)CallField;
            var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(volId, filterType, CallFieldType);
            foreach (var closedCall in closedCalls)
                Console.WriteLine(closedCall);
        }
    }

    private static void GetOpenCallsForVolunteer()
    {
        Console.Write("Enter volunteer ID: ");
        if (int.TryParse(Console.ReadLine(), out int volunteerId))
        {
            Console.WriteLine("Filter by call type? (1: Urgent, 2: Medium_Urgency, 3: General_Assistance, 4: Non_Urgent, 0: None):");
            BO.CallStatus? CallFilter = null;
            if (int.TryParse(Console.ReadLine(), out int CallField) && CallField > 0 && CallField <= 3)
                CallFilter = (BO.CallStatus)CallField;
            Console.WriteLine("Sort by call type? (0: Status, 1: OpeningTime, 2: MaxEndTime , 3: Address):");
            BO.OpenCallField? CallFieldType = null;
            if (int.TryParse(Console.ReadLine(), out CallField) && CallField > 0 && CallField <= 3)
                CallFieldType = (BO.OpenCallField)CallField;
            var closedCalls = s_bl.Call.GetOpenCallsForVolunteer(volunteerId, CallFilter, CallFieldType);
            foreach (var closedCall in closedCalls)
                Console.WriteLine(closedCall);
        }
    }

    private static void SelectCallForTreatment()
    {
        Console.Write("To create a connection between volunteer and reading: ");
        Console.Write("Enter volunteer ID: ");
        int.TryParse(Console.ReadLine(), out int volunteer);
        Console.Write("Enter call ID: ");
        int.TryParse(Console.ReadLine(), out int call);
        s_bl.Call.SelectCallForTreatment(volunteer, call);
        Console.WriteLine("The call was successfully assigned.");
    }

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