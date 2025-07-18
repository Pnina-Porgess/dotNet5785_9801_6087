﻿using DalApi;
using DO;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using System.Security.Cryptography;


namespace DalTest;

public static class Initialization
{
   //private static IVolunteer? s_dalVolunteer; //stage 1
   // private static ICall? s_dalCall; //stage 1
   // private static IAssignment? s_dalAssignment; //stage 1
   // private static IConfig? s_dalConfig; //stage 1
    private static IDal? s_dal; //stage 2
    private static readonly Random s_rand = new();
    private const int MIN_ID = 200000000;
    private const int MAX_ID = 400000000;
    internal static string EncryptPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes!);
    }
    /// <summary>
    /// The function creates 15 volunteers
    /// </summary>

    private static void createVolunteers()
    {
 
        string[] names = {
    "Sari", "Pnina", "Shira", "Chaya", "Yosi",
    "Beni", "Tamar", "Eli", "Moshe", "Chana",
    "Ari", "Chaim", "Shani", "Yonatan", "David"
};

        string[] emails = {
    "Sari@gmail.com", "Pnina@gmail.com", "Shira@gmail.com", "Chaya@gmail.com", "Yosi@gmail.com",
    "Beni@gmail.com", "Tamar@gmail.com", "Eli@gmail.com", "Moshe@gmail.com", "Chana@gmail.com",
    "Ari@gmail.com", "Chaim@gmail.com", "Shani@gmail.com", "Yonatan@gmail.com", "David@gmail.com"
};

        string[] phones = {
    "0503214571", "0528976524", "0541238745", "0535491235", "0557896521",
    "0509871234", "0541123345", "0537859513", "0523126578", "0556549872",
    "0537892134", "0501256789", "0524567890", "0546543210", "0551237894"
};

        //I used GPT to create this array
        string[] addresses = {
    "Jerusalem, King George St.", "Tel Aviv, Rothschild Blvd.", "Haifa, Carmel Beach", "Beer Sheva, Ben Gurion Blvd.", 
    "Ashdod, HaShalom St.", "Netanya, Herzl St.", "Rishon LeZion, Neot Afeka St.", "Petah Tikva, HaSharon St.", "Holon, Jabotinsky St.", 
    "Bnei Brak, Rabbi Akiva St.", "Rehovot, Herzl St.", "Bat Yam, Ben Gurion Blvd.","Herzliya, Sokolov St.","Hadera, HaSharon Blvd.","Eilat, HaTmarim St." 
};
        //I used GPT to create this array
        string[] passwords = {
    "Sari45G92h","Pnina87Z56j", "Shira32X19b", "Chaya54L72k", "Yosi18M91q", "Beni39P65a",
    "Tamar75V21n", "Eli26W83f", "Moshe90R47z", "Chana63S29m", "Ari42D78c", "Chaim56H14p", "Shani81J67y",
   "Yonatan97K50x", "David34Q85v"
};
        int[] id =
        {
            328216783,328227442,328263249,328265640,214786725,214942823,328276332,328279138
            ,328279427,328301940,328304944,214751885,214606170,328308646,328332317
        };
        //I used GPT to create this array
        (double Latitude, double Longitude)[] coordinates = {
    (31.7683, 35.2137),(32.0853, 34.7818), (32.7940, 34.9896), (31.2518, 34.7913), (31.8044, 34.6553), 
    (32.3215, 34.8532), (31.9706, 34.7925), (32.0840, 34.8878), (32.0158, 34.7874),  (32.0809, 34.8333), 
    (31.8948, 34.8093), (32.0236, 34.7502), (32.1663, 34.8436), (32.4340, 34.9196),(29.5581, 34.9482) 
};

        s_dal!.Volunteer.Create(new Volunteer(329236087, "Shlomo", "05321234565", "Shlomo@gmail.com", Role.Manager, true, DistanceType.AerialDistance, s_rand.Next(1000,4000), EncryptPassword("1234@Aaa"), "Tel Aviv", 32.0853, 34.7818));
        for (int i = 0; i < 15; i++)

        {
       
    
            string name = names[i];
            string email = emails[i];
            string phone = phones[i];
            string password = passwords[i];
            double Latitude = coordinates[i].Latitude;
            double Longitude = coordinates[i].Longitude;
            double MaximumDistance = s_rand.Next(1000, 4000);
            s_dal!.Volunteer.Create(new Volunteer(id[i], name, phone, email, Role.Volunteer, true, DistanceType.AerialDistance, MaximumDistance, EncryptPassword(password), addresses[i], Longitude, Latitude));

        }
    }

    /// <summary>
    /// The function creates 50 calls
    /// </summary>

    private static void createCalls()
    {
       // I used GPT to create this array
        string[] issues = {
    "Flat tire", "Engine failure", "Battery is dead", "Ran out of fuel", "Locked keys inside the car", "Brake system malfunction",
    "Overheated engine", "Warning light is on","Strange noise from the engine", "Headlights are not working", "Power steering failure","Transmission problem",
    "Exhaust system damage","Car won’t start","Tire tread is worn out","Leaking oil", "Windshield wipers are broken","Clutch is slipping",
    "Car stuck in gear","AC is not working","Heater is not functioning","Radiator is leaking", "Alternator failure","Starter motor failure",
    "Car is vibrating while driving","Brake pads are worn out","Broken timing belt","Suspension problem", "Check engine light is blinking","Fuel pump failure",
    "Spark plugs need replacement","Catalytic converter issue","Car pulling to one side while driving","Broken fan belt", "Electric window not working",
    "Central locking system failure","Flat spare tire","Key fob is not functioning","Damaged side mirror","Tail lights are not working","Hood won’t close",
    "Trunk won’t open","Excessive smoke from exhaust","Battery terminals are corroded","Parking brake is stuck","Fuel gauge is not accurate",
    "Airbag warning light is on","Steering wheel is stiff","Horn is not working","Dashboard lights flickering"
     };
        //I used GPT to create this array
        double[] longitudes = {
    34.8697, 34.8951, 35.2105, 34.8048, 34.8708,
    34.9482, 35.4732, 34.7746, 34.9640, 34.7915,
    35.3035, 35.5312, 34.6499, 34.5743, 34.8043,
    34.8500, 35.0738, 34.8447, 34.8111, 35.2137,
    34.8925, 34.9537, 34.8703, 34.7640, 35.5683,
    35.0063, 35.4960, 35.5773, 35.0345, 35.1047,
    34.7443, 34.8248, 34.9065, 34.8878, 35.2854,
    34.9197, 34.8019, 34.8383, 35.3047, 35.7496,
    35.5833, 35.2940, 35.5683, 35.0944, 34.7383,
    35.4500, 34.7872, 35.0247,34.3233,35.4545,32.0835,32.0722
};
        //I used GPT to create this array
        double[] latitudes = {
    32.3488, 31.9575, 31.7781, 32.1250, 32.0004,
    29.5580, 31.5590, 32.0628, 32.7980, 31.2525,
    32.6996, 32.7922, 31.8014, 31.6693, 31.9648,
    32.3329, 32.9234, 32.1663, 31.8941, 31.2586,
    32.5113, 32.5726, 32.1847, 31.6096, 33.2082,
    31.8996, 32.9646, 33.2790, 31.0707, 32.6672,
    32.0171, 32.0684, 32.1782, 32.0840, 32.6082,
    32.4340, 30.6100, 32.0853, 32.9170, 33.0622,
    32.8333, 31.7768, 33.2082, 33.0044, 31.8775,
    31.8667, 32.0158, 32.4746,31.2334,32.4554,34.8547,34.7934
};
        //I used GPT to create this array
        string[] addresses = {
    "Highway 2, Central Israel","Highway 6, Central Israel", "Begin Boulevard, Jerusalem","Glilot Interchange, Tel Aviv",
    "HaTmarim, Eilat", "Ein Gedi, Dead Sea","Rothschild Blvd, Tel Aviv","Carmel Beach, Haifa","Soroka Hospital, Beer Sheva",
    "El-Rama, Nazareth", "Kinneret, Tiberias","Port Road, Ashdod","Herzl, Ashkelon","Jabotinsky, Rishon Lezion","Ben Gurion, Netanya",
    "Shderot HaMeginim, Acre (Akko)","Sokolov, Herzliya","Weizmann Institute, Rehovot","Haatzmaut, Arad","Jabotinsky, Caesarea",
    "HaMeyasdim, Zikhron Ya'akov", "Ahuza, Raanana", "Jabotinsky, Kiryat Gat","Golan, Kiryat Shmona","Ze'ev Jabotinsky, Modiin", "HaRabbanim, Safed (Tzfat)",
    "HaGalil, Metula", "HaTomer, Dimona", "HaGilboa, Yokneam", "Ben Gurion, Bat Yam", "Jabotinsky, Ramat Gan",  "Herzl, Kfar Saba",
    "Rothschild Blvd, Petah Tikva", "Shderot HaMeginim, Afula",  "HaBanim, Hadera","Eilat, Mitzpe Ramon","Rabbi Akiva, Bnei Brak","HaMeyasdim, Karmiel",
    "Givat Yoav, Golan Heights","Ginosar, Sea of Galilee","HaNegev, Maale Adumim", "HaTamar, Qiryat Shemona","Sderot HaGalil, Nahariya", "Hachalutzim, Yavne",
    "Ein Esariya, Jericho Area","Sokolov, Holon","Harish Boulevard, Harish", "Bar Ilan, Bnei Brak","Eilat, Machtesh Rimon","Geha Highway","Ayalon Highway",
};




        for (int i = 0; i <50 ; i++)
        {
            TypeOfReading typeOfReading= (TypeOfReading)s_rand.Next(0,Enum.GetValues(typeof(TypeOfReading)).Length);
            DateTime now = s_dal!.Config.Clock; // התאריך הנוכחי לפי השעון המוגדר
            DateTime TimeOfOpen = now.AddDays(-s_rand.Next(0, 30)).AddHours(-s_rand.Next(0, 24)); // נפתח ב-0 עד 30 ימים אחורה
            DateTime MaxTimeToFinish = TimeOfOpen.AddDays(s_rand.Next(1, 31)).AddHours(s_rand.Next(0, 24));
            double Longitude = longitudes[i];
            double Latitude = latitudes[i];
            string? Adress= addresses[i];
            string? Description="";
            s_dal!.Call.Create(new Call(0,typeOfReading, Description, Adress, Longitude, Latitude, TimeOfOpen, MaxTimeToFinish));
        }

    }

    /// <summary>
    /// The function creates 50 assignments
    /// </summary>
    private static void createAssignments()
    {
        List<Volunteer>? volunteersList = s_dal!.Volunteer.ReadAll().ToList();
        List<Call>? callsList = s_dal!.Call.ReadAll().ToList();

        Dictionary<int, int> volunteerAssignmentCount = volunteersList.ToDictionary(v => v.Id, v => 0);
        HashSet<int> volunteersWithActiveAssignment = new();

        int callIndex = 0;

        foreach (var volunteer in volunteersList)
        {
            int assignmentsForThisVolunteer = s_rand.Next(2, 5); // 2 to 4 assignments per volunteer
            for (int j = 0; j < assignmentsForThisVolunteer && callIndex < callsList.Count; j++)
            {
                Call call = callsList[callIndex++];
                bool makeActive = j == 0 && !volunteersWithActiveAssignment.Contains(volunteer.Id); // one active per volunteer

                if (makeActive)
                {
                    volunteersWithActiveAssignment.Add(volunteer.Id);
                    s_dal.Assignment!.Create(new Assignment(
                        0,
                        call.Id,
                        volunteer.Id,
                        null, // TypeOfEndTime
                        DateTime.Now,
                        null // EndTime
                    ));
                }
                else
                {
                    DateTime start = call.TimeOfOpen.AddMinutes(s_rand.Next(30, 120));
                    DateTime end = start.AddHours(2);
                    TypeOfEndTime type = (TypeOfEndTime)s_rand.Next(Enum.GetValues(typeof(TypeOfEndTime)).Length - 1);
                    s_dal.Assignment!.Create(new Assignment(
                        0,
                        call.Id,
                        volunteer.Id,
                        type,
                        start,
                        end
                    ));
                }

                volunteerAssignmentCount[volunteer.Id]++;
            }
        }
    }



    /// <summary>
    /// Initializes the DAL (Data Access Layer) and resets the database. 
    /// It then creates sample data for Volunteers, Calls, and Assignments.
    /// </summary>
    /// <param name="dal">The DAL instance to be used for database operations.</param>
    /// <exception cref="NullReferenceException">Thrown if the provided DAL object is null.</exception>
    //public static void Do(IDal dal) //stage 2
    public static void Do() //stage 4
    {
        //s_dalStudent = dalStudent ?? throw new NullReferenceException("DAL object can not be null!"); // stage 1
        //s_dalCourse = dalCourse ?? throw new NullReferenceException("DAL object can not be null!"); // stage 1
        //s_dalLink = dalStudentInCourse ?? throw new NullReferenceException("DAL object can not be null!"); // stage 1
        //s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1
        //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2
        s_dal = DalApi.Factory.Get; //stage 4
        Console.WriteLine("Reset Configuration values and List values...");
        //s_dalConfig.Reset(); //stage 1
        //s_dalStudent.DeleteAll(); //stage 1
        //...
        s_dal.ResetDB();//stage 2

        createVolunteers();
        createCalls();
        createAssignments();
    }



}

