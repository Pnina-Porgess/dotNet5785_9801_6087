
using DalApi;

using DO;
using Microsoft.VisualBasic;
using System;
using Dal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DalTest
{
    internal class Program
    {
        //private static IStudent? s_dalStudent = new StudentImplementation(); //stage 1
        //private static ICourse? s_dalCourse = new CourseImlementation(); //stage 1
        //private static ILink? s_dalLink = new LinkImplementation(); //stage 1
        //private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1
       //  private static readonly IDal s_dal = new Dal.DalList(); //stage 2
        static readonly IDal s_dal = new Dal.DalXml();//stage3

        ///enum for The possibilities of the main menu  
        public enum MainMenu
        {
            ExitMainMenu,
            VolunteerSubmenu,
            CallSubmenu,
            AssignmentSubmenu,
            InitializeData,
            DisplayAllData,
            ConfigSubmenu,
            ResetDatabase
        }


        /// enum for the submenu options (for each entity)

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
        /// enum for the submenu options (for the config)
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
        /// A function that creates a volunteer
        private static Volunteer CreateVolunteer(int id)
        {
           
                Console.Write("Enter your name");
                string name = Console.ReadLine()!;
                Console.Write("Enter your email");
                string email = Console.ReadLine()!;
                Console.Write("Enter your phone");
                string phone = Console.ReadLine()!;
                Console.Write("Enter your password");
                string password = Console.ReadLine()!;
                Console.Write("Enter (0 for Manager, 1 for Volunteer, etc.): ");
                if (!Enum.TryParse(Console.ReadLine(), out Role role)) throw new InvalidFormatException("Invalid format");
                Console.Write("Is Active? (true/false): ");
                bool active = bool.Parse(Console.ReadLine()!);
                Console.Write("Enter your address");
                string address = Console.ReadLine()!;
                Console.Write("Enter your Maximum Distance");
                if (!double.TryParse(Console.ReadLine(), out double maximumDistance)) throw new InvalidFormatException("Invalid format");
                Console.Write("Enter your Latitude");
                if (!double.TryParse(Console.ReadLine(), out double latitude)) throw new InvalidFormatException("Invalid format");
                Console.Write("Enter your Longitude");
                if (!double.TryParse(Console.ReadLine(), out double longitude)) throw new InvalidFormatException("Invalid format");
                Console.Write("Enter Distance Type (0 for Aerial Distance, 1 for Walking Distance, 2 for Driving Distance, etc.): ");
                DistanceType distanceType = (DistanceType)int.Parse(Console.ReadLine()!);
                return new Volunteer(id, name, email, phone, role, active, distanceType, maximumDistance, password, address, longitude, latitude);
        }
        /// A function that creates a call
        private static Call CreateCall(int id)
       
            {
                Console.Write("Enter Call Type (0 for Type1, 1 for Type2, etc.): ");
                if (!Enum.TryParse(Console.ReadLine(), out TypeOfReading typeOfReading)) throw new FormatException("Invalid choice");
                Console.Write("Enter Description of the problem");
                string description = Console.ReadLine()!;
                Console.Write("Enter your address");
                string address = Console.ReadLine()!;
                Console.Write("Enter your Latitude");
                if (!double.TryParse(Console.ReadLine(), out double latitude)) throw new InvalidFormatException("Invalid format");
                Console.Write("Enter your Longitude");
                if (!double.TryParse(Console.ReadLine(), out double longitude)) throw new InvalidFormatException("Invalid format");
                Console.Write("Enter Opening Time (YYYY-MM-DD HH:MM): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime openingTime)) throw new InvalidFormatException("Invalid format");
                Console.Write("Enter Max Time Finish Calling (YYYY-MM-DD HH:MM): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime maxClosing)) throw new InvalidFormatException("Invalid format");
                return new Call(id, typeOfReading, description, address, longitude, latitude, openingTime, maxClosing);
            
        }
        /// A function that creates a assignment
        private static Assignment CreateAssignment(int id)
        {
                Console.Write("Enter Call ID: ");
                if (!int.TryParse(Console.ReadLine(), out int callId)) throw new InvalidFormatException("Invalid format");
                Console.Write("Enter Volunteer ID: ");
                if (!int.TryParse(Console.ReadLine(), out int volunteerId)) throw new InvalidFormatException("Invalid format");
                Console.Write("Enter Type Of End Time: 0 for treated, 1 for Self Cancellation, 2 for CancelingAnAdministrator, 4 for CancellationHasExpired ");
                if (!Enum.TryParse(Console.ReadLine(), out TypeOfEndTime typeOfEndTime)) throw new FormatException("Invalid choice");
                Console.Write("Enter Ending Time of Treatment (YYYY-MM-DD HH:MM): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime endTime)) throw new InvalidFormatException("Invalid format");
                return new Assignment(id, callId, volunteerId, typeOfEndTime, endTime);
        
          
        }
        /// A function that creates an entity
        private static void Create(string choice)
        {
            try
            {
                
                switch (choice)
                {
                    case "VolunteerSubmenu":
                        Console.WriteLine("Enter your details");
                        Console.Write("Enter ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new InvalidFormatException("Invalid format");
                        Volunteer vol = CreateVolunteer(yourId);
                        s_dal.Volunteer.Create(vol);
                        break;
                    case "CallSubmenu":
                        Call call = CreateCall(0);
                        s_dal!.Call.Create(call);
                        break;
                    case "AssignmentSubmenu":
                        Assignment ass = CreateAssignment(0);
                        s_dal!.Assignment.Create(ass);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create function: {ex.Message}");
            }
        }
        ///A function that updates an entity
        private static void Update(string choice)
        {
            try
            {
                Console.WriteLine("Enter your details");
                Console.Write("Enter ID: ");
                if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new InvalidFormatException("Invalid format");
                switch (choice)
                {
                    case "VolunteerSubmenu":
                        Volunteer vol = CreateVolunteer(yourId);
                        s_dal!.Volunteer.Update(vol);
                        break;
                    case "CallSubmenu":
                        Call call = CreateCall(yourId);
                        s_dal!.Call.Update(call);
                        break;
                    case "AssignmentSubmenu":
                        Assignment ass = CreateAssignment(yourId);
                        s_dal!.Assignment.Update(ass);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Update function: {ex.Message}");
            }
        }

        /// A function that calls a certain entity by its ID
        private static void Read(string choice)
        {
            try
            {
                Console.WriteLine("Enter ID: ");
                if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new InvalidFormatException("Invalid format");
                switch (choice)
                {
                    case "VolunteerSubmenu":
                        Console.WriteLine(s_dal!.Volunteer.Read(yourId));
                        break;
                    case "CallSubmenu":
                        Console.WriteLine(s_dal!.Call.Read(yourId));
                        break;
                    case "AssignmentSubmenu":
                        Console.WriteLine(s_dal!.Assignment.Read(yourId));
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Read function: {ex.Message}");
            }
        }

        /// A function that displays all of the selected specific entity

        private static void ReadAll(string choice)
        {
            try
            {
                switch (choice)
                {
                    case "VolunteerSubmenu":
                        foreach (var item in s_dal!.Volunteer.ReadAll())
                            Console.WriteLine(item);
                        break;
                    case "CallSubmenu":
                        foreach (var item in s_dal!.Call.ReadAll())
                            Console.WriteLine(item);
                        break;
                    case "AssignmentSubmenu":
                        foreach (var item in s_dal!.Assignment.ReadAll())
                            Console.WriteLine(item);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ReadAll function: {ex.Message}");
            }
        }

        ///A function that deletes according to the ID 

        private static void Delete(string choice)
        {
            try
            {
                Console.WriteLine("Enter ID: ");
                if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new InvalidFormatException("Invalid format");
                switch (choice)
                {
                    case "VolunteerSubmenu":
                        s_dal!.Volunteer.Delete(yourId);
                        break;
                    case "CallSubmenu":
                        s_dal!.Call.Delete(yourId);
                        break;
                    case "AssignmentSubmenu":
                        s_dal!.Assignment.Delete(yourId);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete function: {ex.Message}");
            }
        }
        /// A function that deletes the entire entity
        private static void DeleteAll(string choice)
    {
        switch (choice)
        {
            case "VolunteerSubmenu":
                s_dal!.Volunteer.DeleteAll();
                break;
            case "CallSubmenu":
                s_dal!.Call.DeleteAll();
                break;
            case "AssignmentSubmenu":
                s_dal!.Assignment.DeleteAll();
                break;
        }
    }
        /// A function that displays the menu for each specific entity according to the selected entity
        private static void EntityMenu(string choice)
    {
        foreach (SubMenu option in Enum.GetValues(typeof(SubMenu)))
        {
            Console.WriteLine($"{(int)option}. {option}");
              
            }
        Console.Write("Select an option: ");
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
                    Update(choice);
                    break;
                case SubMenu.Exit:
                    return;
                default:
                    Console.WriteLine("Your choise is not valid, please enter agiam");
                    break;
            }
            foreach (SubMenu option in Enum.GetValues(typeof(SubMenu)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }
            Console.Write("Select an option: ");
            if (!Enum.TryParse(Console.ReadLine(), out  subChoice)) throw new InvalidFormatException("Invalid choice");
        }
    }
        /// A function that creates the submenu for the Config
        private static void ConfigSubmenuu()
        {
            try
            {
                Console.WriteLine("Config Menu:");
                foreach (ConfigSubmenu option in Enum.GetValues(typeof(ConfigSubmenu)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }
                Console.Write("Select an option: ");
                if (!Enum.TryParse(Console.ReadLine(), out ConfigSubmenu userInput)) throw new FormatException("Invalid choice");

                while (userInput is not ConfigSubmenu.Exit)
                {
                    switch (userInput)
                    {
                        case ConfigSubmenu.AdvanceClockByMinute:
                            s_dal!.Config.Clock = s_dal!.Config.Clock.AddMinutes(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByHour:
                            s_dal!.Config.Clock = s_dal!.Config.Clock.AddHours(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByDay:
                            s_dal!.Config.Clock = s_dal!.Config.Clock.AddDays(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByMonth:
                            s_dal!.Config.Clock = s_dal!.Config.Clock.AddMonths(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByYear:
                            s_dal!.Config.Clock = s_dal!.Config.Clock.AddYears(1);
                            break;
                        case ConfigSubmenu.DisplayClock:
                            Console.WriteLine(s_dal!.Config.Clock);
                            break;
                        case ConfigSubmenu.ChangeClockOrRiskRange:
                            Console.WriteLine("Enter property for change (Clock or Risk range): ");
                            string prop = Console.ReadLine()!;
                            if (prop == "clock")
                            {
                                Console.WriteLine("Enter a new value for clock (YYYY-MM-DD HH:MM):");
                                string clockInput =Console.ReadLine();
                                if (!DateTime.TryParse(clockInput, out DateTime updatClock)) throw new InvalidFormatException("Invalid format");
                                s_dal!.Config.Clock = updatClock;
                                Console.WriteLine($"RiskRange update to: {s_dal!.Config.Clock}");
                                break;
                            }
                            else
                            {
                                Console.Write("Enter a new value for RiskRange (in format (HH:MM:SS): ");
                                string riskRangeInput = Console.ReadLine()!;
                                if (!TimeSpan.TryParse(riskRangeInput, out TimeSpan newRiskRange)) throw new InvalidFormatException("Invalid format");
                                s_dal!.Config.SetRiskRange(newRiskRange);
                                Console.WriteLine($"RiskRange update to: {s_dal!.Config.GetRiskRange()}");
                            }
                                break;

                        case ConfigSubmenu.DisplayConfigVar:
                            Console.WriteLine("Enter property for display (Clock or Risk range): ");
                            string _prop = Console.ReadLine()!;
                            if (_prop == "Clock")
                                Console.WriteLine(s_dal!.Config.Clock);
                            else
                                Console.WriteLine($"RiskRange: {s_dal!.Config.GetRiskRange()}");
                            break;

                        case ConfigSubmenu.Reset:
                            s_dal!.Config.Reset();
                            break;
                    case ConfigSubmenu.Exit:
                        return;
                    }

                    Console.WriteLine("Config Menu:");
                    foreach (ConfigSubmenu option in Enum.GetValues(typeof(ConfigSubmenu)))
                    {
                        Console.WriteLine($"{(int)option}. {option}");
                    }
                    Console.Write("Select an option: ");
                    if (!Enum.TryParse(Console.ReadLine(), out  userInput)) throw new FormatException("Invalid choice");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ConfigSubmenu function: {ex.Message}");
            }
        }
        ///The main function that manages the entire program
        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Main Menu:");
                foreach (MainMenu option in Enum.GetValues(typeof(MainMenu)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }
                Console.Write("Select an option: ");
                if (!Enum.TryParse(Console.ReadLine(), out MainMenu userInput)) throw new FormatException("Invalid choice");

                while (userInput is not MainMenu.ExitMainMenu)
                {
                    try
                    {
                        switch (userInput)
                        {
                            case MainMenu.AssignmentSubmenu:
                            case MainMenu.VolunteerSubmenu:
                            case MainMenu.CallSubmenu:
                                string sChoice = userInput.ToString();
                                EntityMenu(sChoice);
                                break;
                            case MainMenu.InitializeData:
                                //Initialization.Do(s_dalStudent, s_dalCourse, s_dalLink, s_dalConfig); //stage 1
                                Initialization.Do(s_dal); //stage 2

                                break;
                            case MainMenu.DisplayAllData:
                                ReadAll("VolunteerSubmenu");
                                ReadAll("CallSubmenu");
                                ReadAll("AssignmentSubmenu");
                                break;
                            case MainMenu.ConfigSubmenu:
                                ConfigSubmenuu();
                                break;
                            case MainMenu.ResetDatabase:
                                s_dal!.Config.Reset(); //stage 1
                                s_dal!.Volunteer!.DeleteAll(); //stage 1
                                s_dal!.Call!.DeleteAll(); //stage 1
                                s_dal!.Assignment!.DeleteAll(); //stage 1
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in main menu operation: {ex.Message}");
                    }

                    Console.WriteLine("Main Menu:");
                    foreach (MainMenu option in Enum.GetValues(typeof(MainMenu)))
                    {
                        Console.WriteLine($"{(int)option}. {option}");
                    }
                    Console.Write("Select an option: ");
                    if (!Enum.TryParse(Console.ReadLine(), out userInput)) throw new FormatException("Invalid choice");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Main function: {ex.Message}");
            }
        }
    }
}