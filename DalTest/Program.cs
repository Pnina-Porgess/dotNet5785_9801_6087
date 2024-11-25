using Dal;
using DalApi;
using DO;

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
    private static void Create(string choice)
    {

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
        case MainMenu.ExitMainMenu:
      //      break;

                break;

            case MainMenu.AssignmentSubmenu:

                break;

            case MainMenu.VolunteerSubmenu:

                break;

            case MainMenu.CallSubmenu:

            break;
        case MainMenu.InitializeData:
            Console.WriteLine("יציאה מהתפריט הראשי...");

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


