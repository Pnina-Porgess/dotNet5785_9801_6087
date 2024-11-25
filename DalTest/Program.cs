using Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic.FileIO;

namespace DalTest
{
    internal class Program
    {
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
        private static ICall? s_dalCall = new CallImplementation(); //stage 1
        private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
        private static IConfig? s_dalConfig = new ConfigImplementation();


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
    private static void EntityMenu(string choice)
    {
        Console.WriteLine("Enter a number");
        foreach (SubMenu option in Enum.GetValues(typeof(SubMenu)))
        {
            Console.WriteLine($"{(int)option}. {option}");
        }
        if(!Enum.TryParse(Console.ReadLine(), out SubMenu subChoice)) throw new FormatException("BirthDate is invalid!"); 
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
                    if (!Enum.TryParse(Console.ReadLine(), out  YourId)) throw new FormatException("Id is invalid!");
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

        

    }

    

