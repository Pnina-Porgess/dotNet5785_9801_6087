using Dal;
using DalApi;
using DO;
using System;

namespace DalTest;

internal class Program
{
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
    private static ICall? s_dalCall = new CallImplementation(); //stage 1
    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation();
    public enum MainMenu
    {
        ExitMainMenu,
        AssignmentSubmenu,
        VolunteerSubmenu,
        CallSubmenu,
        InitializeData,
        DisplayAllData,
        ConfigSubmenu,
        ResetDatabase
    }
    public enum SubMenu
    {
        Exit,
        Create,
        Read,
        ReadAll,
        UpDate,
        Delete,
        DeleteAll
    }
    private static Volunteer CreateVolunteer(int id)
    {
        Console.Write("Enter your name");
        string name=Console.ReadLine();
        Console.Write("Enter your email");
        string email = Console.ReadLine();
        Console.Write("Enter your phone");
        string phone = Console.ReadLine();
        Console.Write("Enter your password");
        string password = Console.ReadLine();
        Console.Write("Enter  (1 for Manager, 2 for Volunteer, etc.): ");
        Role Role = (Role)int.Parse(Console.ReadLine()!);
        Console.Write("Is Active? (true/false): ");
        bool Active = bool.Parse(Console.ReadLine()!);
        Console.Write("Enter your address");
        string address = Console.ReadLine();
        Console.Write("Enter your Maximum Distance");
        double MaximumDistance = double.Parse(Console.ReadLine()!);
        Console.Write("Enter your Latitude");
        double Latitude = double.Parse(Console.ReadLine()!);
        Console.Write("Enter your Longitude");
        double Longitude = double.Parse(Console.ReadLine()!);
        Console.Write("Enter Distance Type  (1 for driving Distance, 2 for walking Distance,3 for Aerial Distance, etc.): ");
        DistanceType DistanceType = (DistanceType)int.Parse(Console.ReadLine()!);
        return new Volunteer(id, name, email, phone, Role, Active, DistanceType, MaximumDistance, password, address, Longitude, Latitude);
    }
    private static Call CreateCall(int id)
    {

        Console.Write("Enter Call Type (1 for Type1, 2 for Type2, etc.): ");
        TypeOfReading typeOfReading = (TypeOfReading)int.Parse(Console.ReadLine()!);
        Console.Write("Enter Description of the problem");
        string description= Console.ReadLine();
        Console.Write("Enter your address");
        string address = Console.ReadLine();
        Console.Write("Enter your Latitude");
        double Latitude = double.Parse(Console.ReadLine()!);
        Console.Write("Enter your Longitude");
        double Longitude = double.Parse(Console.ReadLine()!);
        Console.Write("Enter Opening Time (YYYY-MM-DD HH:MM): ");
        DateTime OpeningTime = DateTime.Parse(Console.ReadLine());
        Console.Write("Enter Max Time Finish Calling (YYYY-MM-DD HH:MM): ");
        DateTime MaxClosing = DateTime.Parse(Console.ReadLine());
        return new Call(id, typeOfReading, description, address, Longitude, Latitude, OpeningTime, MaxClosing);
    }

    private static Assignment CreateAssignment(int id)
    {
        Console.Write("Enter Volunteer ID: ");
        int CallId = int.Parse(Console.ReadLine()!);
        Console.Write("Enter Volunteer ID: ");
        int volunteerId = int.Parse(Console.ReadLine()!);
        Console.Write("Enter Type Of End Time : 1 for treated, 2 for Self Cancellation,3 for CancelingAnAdministrator,4 for CancellationHasExpired ");
        TypeOfEndTime typeOfEndTime= (TypeOfEndTime)int.Parse(Console.ReadLine()!);
        Console.Write("Enter Ending Time of Treatment ( YYYY-MM-DD HH:MM): ");
        DateTime EndTime = DateTime.Parse(Console.ReadLine()!);
        return new Assignment(id, CallId, volunteerId, typeOfEndTime, EndTime);
    }
        private static void Create(string choice)
    {
        Console.WriteLine("Enter your details");
        Console.Write("Enter ID: ");
        int yourId = int.Parse(Console.ReadLine()!);
        switch (choice)
        {
            case "VolunteerSubmenu":
                Volunteer Vol = CreateVolunteer(yourId);
                s_dalVolunteer.Create(Vol);
                break;
            case "CallSubmenu":
                Call Call = CreateCall(yourId);
                s_dalCall.Create(Call);
                break;
            case "AssignmentSubmenu":
                Assignment Ass = CreateAssignment(yourId);
                s_dalAssignment.Create(Ass);
                break;
        }
    }

    private static void Update(string choice)
    {
        Console.WriteLine("Enter your details");
        Console.Write("Enter ID: ");
        int yourId = int.Parse(Console.ReadLine()!);
        switch (choice)
        {
            case "VolunteerSubmenu":
                Volunteer Vol = CreateVolunteer(yourId);
                s_dalVolunteer.Update(Vol);
                break;
            case "CallSubmenu":
                Call Call = CreateCall(yourId);
                s_dalCall.Update(Call);
                break;
            case "AssignmentSubmenu":
                Assignment Ass = CreateAssignment(yourId);
                s_dalAssignment.Update(Ass);
                break;
        }
    }

    private static void Read(string choice)
    {
        Console.WriteLine("Enter ID: ");
        int yourId = int.Parse(Console.ReadLine()!);
        switch (choice)
        {
            case "VolunteerSubmenu":
                s_dalVolunteer.Read(yourId);
                break;
            case "CallSubmenu":
                s_dalCall.Delete(yourId);
                break;
            case "AssignmentSubmenu":
                s_dalAssignment.Delete(yourId);
                break;
        }
    }
    private static void ReadAll(string choice)
    {
    
        switch (choice)
        {
            case "VolunteerSubmenu":
              Console.Write( s_dalVolunteer.ReadAll());
                break;
            case "CallSubmenu":
                Console.Write(s_dalCall.ReadAll());
                break;
            case "AssignmentSubmenu":
                Console.Write(s_dalAssignment.ReadAll());
                break;
        }
    }
    private static void Delete(string choice)
    {
        Console.WriteLine("Enter ID: ");
        int yourId = int.Parse(Console.ReadLine()!);
        switch (choice)
        {
            case "VolunteerSubmenu":
                s_dalVolunteer.Delete(yourId);
                break;
            case "CallSubmenu":
                s_dalCall.Delete(yourId);
                break;
            case "AssignmentSubmenu":
                s_dalAssignment.Delete(yourId);
                break;
        }
    }

    private static void DeleteAll(string choice)
    {
        switch (choice)
        {
            case "VolunteerSubmenu":
                s_dalVolunteer.DeleteAll();
                break;
            case "CallSubmenu":
                s_dalCall.DeleteAll();
                break;
            case "AssignmentSubmenu":
                s_dalAssignment.DeleteAll();
                break;
        }
    }
    private static void EntityMenu(string choice)
    {
        Console.WriteLine("Enter a number");
        foreach (SubMenu option in Enum.GetValues(typeof(SubMenu)))
        {
            Console.WriteLine($"{(int)option}. {option}");
        }
        if (!Enum.TryParse(Console.ReadLine(), out SubMenu subChoice)) throw new FormatException("BirthDate is invalid!");
        while (subChoice is not SubMenu.Exit)
        {
            switch (subChoice)
            {
                case SubMenu.Create:
                    Create(choice);
                    break;
                case SubMenu.Read:
                    Console.WriteLine("Enter Your ID");

                    Read(choice);
                    break;
                case SubMenu.ReadAll:
                    ReadAll(choice);
                    break;
                case SubMenu.Delete:
                    Delete(choice);
                    break;
                case SubMenu.DeleteAll:
                    DeleteAll(choice);
                    break;
                case SubMenu.UpDate:
                    UpDate(choice);
                    break;
                case SubMenu.Exit:
                    return;
                default:
                    Console.WriteLine("Your choise is not valid, please enter agiam");
                    break;
            }
            Console.WriteLine("Enter a number");
            Enum.TryParse(Console.ReadLine(), out subChoice);
        }
    }
    static void main(string[] args)
{   
    Console.WriteLine("Main Menu:");
    foreach (MainMenu option in Enum.GetValues(typeof(MainMenu)))
    {
        Console.WriteLine($"{(int)option}. {option}");
    }
    Console.Write("Select an option: ");
    //int userInput;
    if (!Enum.TryParse(Console.ReadLine(), out MainMenu userInput)) throw new FormatException("Invalid choice");
        while (userInput is not MainMenu.ExitMainMenu)
        switch (userInput)
    {
            case MainMenu.AssignmentSubmenu:
            case MainMenu.VolunteerSubmenu:
            case MainMenu.CallSubmenu:
              
            break;
            case MainMenu.InitializeData:
          
                break;
            case MainMenu.DisplayAllData:
                break;
            case MainMenu.ConfigSubmenu:
            break;
        case MainMenu.ResetDatabase:
 
break;
        }
    }
}


