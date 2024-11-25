using Dal;
using DalApi;
using DO;
using System.Linq.Expressions;

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
    private enum ConfigSubmenu
    {
        Exit,
        AdvanceClockByMinute,
        AdvanceClockByHour,
        AdvanceClockByDay,
        AdvanceClockByMonth,
        AdvanceClockByYear,
        DisplayClock,
        ChangeClockOrRiskRange,
        DisplayConfigVar,
        Reset
    }
    private static void Create(string choice)
    {

    }
    private static void ConfigSubmenuu() {

        Console.WriteLine("Config Menu:");
        foreach (MainMenu option in Enum.GetValues(typeof(ConfigSubmenu)))
        {
            Console.WriteLine($"{(int)option}. {option}");
        }
        Console.Write("Select an option: ");
        if (!Enum.TryParse(Console.ReadLine(), out ConfigSubmenu userInput)) throw new FormatException("Invalid choice");
        {
            while (userInput is not ConfigSubmenu.Exit){ 
            switch (userInput)
                {
                    case ConfigSubmenu.AdvanceClockByMinute:

                        s_dalConfig.Clock = s_dalConfig.Clock.AddMinutes(1);
                        break;
                    case ConfigSubmenu.AdvanceClockByHour:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddHours(1);
                        break;
                    case ConfigSubmenu.AdvanceClockByDay:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddDays(1);
                        break;
                    case ConfigSubmenu.AdvanceClockByMonth:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddMonths(1);
                        break;
                    case ConfigSubmenu.AdvanceClockByYear:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddYears(1);
                        break;
                    case ConfigSubmenu.DisplayClock:
                        Console.WriteLine(s_dalConfig.Clock);
                        break;
                    case ConfigSubmenu.ChangeClockOrRiskRange:
                        Console.WriteLine($"RiskRange : {s_dalConfig.GetRiskRange()}");
                        break;
                    case ConfigSubmenu.DisplayConfigVar:
                        Console.Write("הזן ערך חדש עבור RiskRange (בפורמט שעות:דקות:שניות): ");
                        string riskRangeInput = Console.ReadLine();
                        if (!TimeSpan.TryParse(riskRangeInput, out TimeSpan newRiskRange)) throw new FormatException("Invalid choice");
                        {
                            s_dalConfig.SetRiskRange(newRiskRange);
                            Console.WriteLine($"RiskRange update to: {s_dalConfig.GetRiskRange()}");
                        }
                        break;
                        break;
                    case ConfigSubmenu.Reset:
                        s_dalConfig.Reset();
                        break;

                }
        };

        };
    }
    private static void EntityMenu(string choice)
    {
        Console.WriteLine("Enter a number");
        foreach (SubMenu option in Enum.GetValues(typeof(SubMenu)))
        {
            Console.WriteLine($"{(int)option}. {option}");
        }
        if (!Enum.TryParse(Console.ReadLine(), out SubMenu subChoice)) throw new FormatException("Invalid choice");
        while (subChoice is not SubMenu.Exit)
        {
            switch (subChoice)
            {
                case SubMenu.Create:
                    Create(choice);
                    break;
                case SubMenu.Read:
                    Console.WriteLine("Enter Your ID");
                    int YourId;
                    if (!Enum.TryParse(Console.ReadLine(), out YourId)) throw new FormatException("Id is invalid!");
                    Read(choice, yourId);
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
        try
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
                        string sChoice = userInput.ToString();
                        EntityMenu(sChoice);
                        break;
                    case MainMenu.InitializeData:
                        Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
                        break;
                    case MainMenu.DisplayAllData:
                        {
                            Console.WriteLine(s_dalVolunteer.ReadAll());
                            Console.WriteLine(s_dalCall.ReadAll());
                            Console.WriteLine(s_dalAssignment.ReadAll());
                        }
                        break;
                    case MainMenu.ConfigSubmenu:
                       
                            ConfigSubmenuu();
                       
                        break;
                    case MainMenu.ResetDatabase:
                       
                            s_dalConfig.Reset(); //stage 1
                            s_dalVolunteer.DeleteAll(); //stage 1
                            s_dalCall.DeleteAll(); //stage 1
                            s_dalAssignment.DeleteAll(); //stage 1
                 
                        break;
                }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}


