using Dal;
using DalApi;
using DO;

namespace DalTest
{
    internal class Program
    {
        public static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
        public static ICall? s_dalCall = new CallImplementation(); //stage 1
        public static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
        public static IConfig? s_dalConfig = new ConfigImplementation();

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
                if (!Enum.TryParse(Console.ReadLine(), out Role role)) throw new FormatException("Invalid choice");
                Console.Write("Is Active? (true/false): ");
                bool active = bool.Parse(Console.ReadLine()!);
                Console.Write("Enter your address");
                string address = Console.ReadLine()!;
                Console.Write("Enter your Maximum Distance");
                if (!double.TryParse(Console.ReadLine(), out double maximumDistance)) throw new FormatException("Invalid choice");
                Console.Write("Enter your Latitude");
                if (!double.TryParse(Console.ReadLine(), out double latitude)) throw new FormatException("Invalid choice");
                Console.Write("Enter your Longitude");
                if (!double.TryParse(Console.ReadLine(), out double longitude)) throw new FormatException("Invalid choice");
                Console.Write("Enter Distance Type (0 for Aerial Distance, 1 for Walking Distance, 2 for Driving Distance, etc.): ");
                DistanceType distanceType = (DistanceType)int.Parse(Console.ReadLine()!);
                return new Volunteer(id, name, email, phone, role, active, distanceType, maximumDistance, password, address, longitude, latitude);
        }

        private static Call CreateCall(int id)
       
            {
                Console.Write("Enter Call Type (0 for Type1, 1 for Type2, etc.): ");
                if (!Enum.TryParse(Console.ReadLine(), out TypeOfReading typeOfReading)) throw new FormatException("Invalid choice");
                Console.Write("Enter Description of the problem");
                string description = Console.ReadLine()!;
                Console.Write("Enter your address");
                string address = Console.ReadLine()!;
                Console.Write("Enter your Latitude");
                if (!double.TryParse(Console.ReadLine(), out double latitude)) throw new FormatException("Invalid choice");
                Console.Write("Enter your Longitude");
                if (!double.TryParse(Console.ReadLine(), out double longitude)) throw new FormatException("Invalid choice");
                Console.Write("Enter Opening Time (YYYY-MM-DD HH:MM): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime openingTime)) throw new FormatException("Invalid choice");
                Console.Write("Enter Max Time Finish Calling (YYYY-MM-DD HH:MM): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime maxClosing)) throw new FormatException("Invalid choice");
                return new Call(id, typeOfReading, description, address, longitude, latitude, openingTime, maxClosing);
            
        }

        private static Assignment CreateAssignment(int id)
        {
                Console.Write("Enter Call ID: ");
                if (!int.TryParse(Console.ReadLine(), out int callId)) throw new FormatException("Invalid choice");
                Console.Write("Enter Volunteer ID: ");
                if (!int.TryParse(Console.ReadLine(), out int volunteerId)) throw new FormatException("Invalid choice");
                Console.Write("Enter Type Of End Time: 0 for treated, 1 for Self Cancellation, 2 for CancelingAnAdministrator, 4 for CancellationHasExpired ");
                if (!Enum.TryParse(Console.ReadLine(), out TypeOfEndTime typeOfEndTime)) throw new FormatException("Invalid choice");
                Console.Write("Enter Ending Time of Treatment (YYYY-MM-DD HH:MM): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime endTime)) throw new FormatException("Invalid choice");
                return new Assignment(id, callId, volunteerId, typeOfEndTime, endTime);
        
          
        }

        private static void Create(string choice)
        {
            try
            {
                Console.WriteLine("Enter your details");
                Console.Write("Enter ID: ");
                if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new FormatException("Invalid choice");
                switch (choice)
                {
                    case "VolunteerSubmenu":
                        Volunteer vol = CreateVolunteer(yourId);
                        s_dalVolunteer!.Create(vol);
                        break;
                    case "CallSubmenu":
                        Call call = CreateCall(yourId);
                        s_dalCall!.Create(call);
                        break;
                    case "AssignmentSubmenu":
                        Assignment ass = CreateAssignment(yourId);
                        s_dalAssignment!.Create(ass);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create function: {ex.Message}");
            }
        }

        private static void Update(string choice)
        {
            try
            {
                Console.WriteLine("Enter your details");
                Console.Write("Enter ID: ");
                if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new FormatException("Invalid choice");
                switch (choice)
                {
                    case "VolunteerSubmenu":
                        Volunteer vol = CreateVolunteer(yourId);
                        s_dalVolunteer!.Update(vol);
                        break;
                    case "CallSubmenu":
                        Call call = CreateCall(yourId);
                        s_dalCall!.Update(call);
                        break;
                    case "AssignmentSubmenu":
                        Assignment ass = CreateAssignment(yourId);
                        s_dalAssignment!.Update(ass);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Update function: {ex.Message}");
            }
        }

        private static void Read(string choice)
        {
            try
            {
                Console.WriteLine("Enter ID: ");
                if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new FormatException("Invalid choice");
                switch (choice)
                {
                    case "VolunteerSubmenu":
                        Console.WriteLine(s_dalVolunteer!.Read(yourId));
                        break;
                    case "CallSubmenu":
                        Console.WriteLine(s_dalCall!.Read(yourId));
                        break;
                    case "AssignmentSubmenu":
                        Console.WriteLine(s_dalAssignment!.Read(yourId));
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Read function: {ex.Message}");
            }
        }

        private static void ReadAll(string choice)
        {
            try
            {
                switch (choice)
                {
                    case "VolunteerSubmenu":
                        foreach (var item in s_dalVolunteer!.ReadAll())
                            Console.WriteLine(item);
                        break;
                    case "CallSubmenu":
                        foreach (var item in s_dalCall!.ReadAll())
                            Console.WriteLine(item);
                        break;
                    case "AssignmentSubmenu":
                        foreach (var item in s_dalAssignment!.ReadAll())
                            Console.WriteLine(item);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ReadAll function: {ex.Message}");
            }
        }

        private static void Delete(string choice)
        {
            try
            {
                Console.WriteLine("Enter ID: ");
                if (!int.TryParse(Console.ReadLine(), out int yourId)) throw new FormatException("Invalid choice"); switch (choice)
                {
                    case "VolunteerSubmenu":
                        s_dalVolunteer!.Delete(yourId);
                        break;
                    case "CallSubmenu":
                        s_dalCall!.Delete(yourId);
                        break;
                    case "AssignmentSubmenu":
                        s_dalAssignment!.Delete(yourId);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete function: {ex.Message}");
            }
        }

    private static void DeleteAll(string choice)
    {
        switch (choice)
        {
            case "VolunteerSubmenu":
                s_dalVolunteer!.DeleteAll();
                break;
            case "CallSubmenu":
                s_dalCall!.DeleteAll();
                break;
            case "AssignmentSubmenu":
                s_dalAssignment!.DeleteAll();
                break;
        }
    }
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
            if (!Enum.TryParse(Console.ReadLine(), out  subChoice)) throw new FormatException("Invalid choice");
        }
    }
    
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
                            s_dalConfig!.Clock = s_dalConfig.Clock.AddMinutes(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByHour:
                            s_dalConfig!.Clock = s_dalConfig.Clock.AddHours(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByDay:
                            s_dalConfig!.Clock = s_dalConfig.Clock.AddDays(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByMonth:
                            s_dalConfig!.Clock = s_dalConfig.Clock.AddMonths(1);
                            break;
                        case ConfigSubmenu.AdvanceClockByYear:
                            s_dalConfig!.Clock = s_dalConfig.Clock.AddYears(1);
                            break;
                        case ConfigSubmenu.DisplayClock:
                            Console.WriteLine(s_dalConfig!.Clock);
                            break;
                        case ConfigSubmenu.ChangeClockOrRiskRange:
                            Console.WriteLine("Enter property for change (Clock or Risk range): ");
                            string prop = Console.ReadLine()!;
                            if (prop == "clock")
                            {
                                Console.WriteLine("Enter a new value for clock (YYYY-MM-DD HH:MM):");
                                string clockInput =Console.ReadLine();
                                if (!DateTime.TryParse(clockInput, out DateTime updatClock)) throw new FormatException("Invalid format");
                                s_dalConfig.Clock = updatClock;
                                Console.WriteLine($"RiskRange update to: {s_dalConfig.Clock}");
                                break;
                            }
                            else
                            {
                                Console.Write("Enter a new value for RiskRange (in format (HH:MM:SS): ");
                                string riskRangeInput = Console.ReadLine()!;
                                if (!TimeSpan.TryParse(riskRangeInput, out TimeSpan newRiskRange)) throw new FormatException("Invalid format");
                                s_dalConfig!.SetRiskRange(newRiskRange);
                                Console.WriteLine($"RiskRange update to: {s_dalConfig.GetRiskRange()}");
                            }
                                break;

                        case ConfigSubmenu.DisplayConfigVar:
                            Console.WriteLine("Enter property for display (Clock or Risk range): ");
                            string _prop = Console.ReadLine()!;
                            if (_prop == "Clock")
                                Console.WriteLine(s_dalConfig.Clock);
                            else
                                Console.WriteLine($"RiskRange: {s_dalConfig!.GetRiskRange()}");
                            break;

                        case ConfigSubmenu.Reset:
                            s_dalConfig!.Reset();
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
                                Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
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
                                s_dalConfig!.Reset(); //stage 1
                                s_dalVolunteer!.DeleteAll(); //stage 1
                                s_dalCall!.DeleteAll(); //stage 1
                                s_dalAssignment!.DeleteAll(); //stage 1
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

