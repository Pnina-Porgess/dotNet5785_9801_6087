using DalApi;
using DO;

namespace DalTest;

public static class Initialization
{
    private static IVolunteer? s_dalVolunteer; //stage 1
    private static ICall? s_dalCall; //stage 1
    private static IAssignment? s_dalAssignment; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();
    //...
    private static void createVolunteers()
    {
<<<<<<< HEAD
        string [] name = {"Sari","Pnina","Shira","Chaya","Yosi","Beni","Tamar","Eli","Moshe","Chana","Ari","Chaim","Shani","Yonatan","David"}
        string[] email = { "Sari@gmail.com" };
=======
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
    "050-321-4571", "052-897-6524", "054-123-8745", "053-549-1235", "055-789-6521",
    "050-987-1234", "054-112-3345", "053-785-9513", "052-312-6578", "055-654-9872",
    "053-789-2134", "050-125-6789", "052-456-7890", "054-654-3210", "055-123-7894"
};
        string[] addresses = {
    "Jerusalem", "Tel Aviv", "Haifa", "Beer Sheva", "Ashdod",
    "Netanya", "Rishon LeZion", "Petah Tikva", "Holon", "Bnei Brak",
    "Rehovot", "Bat Yam", "Herzliya", "Hadera", "Eilat"
};
        string[] passwords = {
    "Sari45G92h","Pnina87Z56j", "Shira32X19b", "Chaya54L72k", "Yosi18M91q", "Beni39P65a",
    "Tamar75V21n", "Eli26W83f", "Moshe90R47z", "Chana63S29m", "Ari42D78c", "Chaim56H14p", "Shani81J67y",
   "Yonatan97K50x", "David34Q85v"
};

        // Array of coordinates for each city with latitude and longitude
        (double Latitude, double Longitude)[] coordinates = {
    (31.7683, 35.2137), // Jerusalem
    (32.0853, 34.7818), // Tel Aviv
    (32.7940, 34.9896), // Haifa
    (31.2518, 34.7913), // Beer Sheva
    (31.8044, 34.6553), // Ashdod
    (32.3215, 34.8532), // Netanya
    (31.9706, 34.7925), // Rishon LeZion
    (32.0840, 34.8878), // Petah Tikva
    (32.0158, 34.7874), // Holon
    (32.0809, 34.8333), // Bnei Brak
    (31.8948, 34.8093), // Rehovot
    (32.0236, 34.7502), // Bat Yam
    (32.1663, 34.8436), // Herzliya
    (32.4340, 34.9196), // Hadera
    (29.5581, 34.9482)  // Eilat
};

        for (int i = 0; i < names.Length; i++)
        {

            int id;
            do
                id = s_rand.Next(100000000, 999999999);
            while (s_dalVolunteer!.Read(id) != null);
            string name = names[i];
            string email = emails[i];
            string phone = phones[i];
            string password=passwords[i];
            double Latitude= coordinates[i].Latitude;   
            double Longitude= coordinates[i].Longitude;
            double MaximumDistance = s_rand.Next(5, 50);
            s_dalVolunteer!.Create(new Volunteer(id,name, email, phone, Role.Volunteer, true, DistanceType.AerialDistance, MaximumDistance, password,addresses[i], Longitude, Latitude));

        }

>>>>>>> e5e21c59425a3627fa08db4d1ba5df8b57383fee
    }
    private static void createCalls()
    {
        string[] issues = {
    "Flat tire",
    "Engine failure",
    "Battery is dead",
    "Ran out of fuel",
    "Locked keys inside the car",
    "Brake system malfunction",
    "Overheated engine",
    "Warning light is on",
    "Strange noise from the engine",
    "Headlights are not working",
    "Power steering failure",
    "Transmission problem",
    "Exhaust system damage",
    "Car won’t start",
    "Tire tread is worn out",
    "Leaking oil",
    "Windshield wipers are broken",
    "Clutch is slipping",
    "Car stuck in gear",
    "AC is not working",
    "Heater is not functioning",
    "Radiator is leaking",
    "Alternator failure",
    "Starter motor failure",
    "Car is vibrating while driving",
    "Brake pads are worn out",
    "Broken timing belt",
    "Suspension problem",
    "Check engine light is blinking",
    "Fuel pump failure",
    "Spark plugs need replacement",
    "Catalytic converter issue",
    "Car pulling to one side while driving",
    "Broken fan belt",
    "Electric window not working",
    "Central locking system failure",
    "Flat spare tire",
    "Key fob is not functioning",
    "Damaged side mirror",
    "Tail lights are not working",
    "Hood won’t close",
    "Trunk won’t open",
    "Excessive smoke from exhaust",
    "Battery terminals are corroded",
    "Parking brake is stuck",
    "Fuel gauge is not accurate",
    "Airbag warning light is on",
    "Steering wheel is stiff",
    "Horn is not working",
    "Dashboard lights flickering"
     };

        //string[] locations = {
        //"Highway 2 (Coastal Road)",
        //"Highway 6",
        //"Begin Boulevard in Jerusalem",
        //"Glilot Interchange",
        //"Highway 4",
        //"Rothschild Boulevard in Tel Aviv",
        //"Beit Dagan Junction",
        //"Highway 443",
        //"La Guardia Interchange",
        //"Geha Interchange",
        //"Poleg Industrial Zone in Netanya",
        //"Arava Road (Highway 90)",
        //"Highway 90 near Ein Gedi",
        //"Highway 40 near Mitzpe Ramon",
        //"Highway 1 between Tel Aviv and Jerusalem",
        //"Ra'anana Junction",
        //"Highway 70 in the North",
        //"Highway 85 - Akko to Safed Road",
        //"Jaffa Street in Jerusalem",
        //"Ben Gurion Boulevard in Haifa",
        //"Highway 531",
        //"Dor Alon Gas Station in the Arava",
        //"Highway 232 near Sderot",
        //"Highway 60 in Gush Etzion",
        //"HaShalom Interchange in Tel Aviv",
        //"Jerusalem Boulevard in Jaffa",
        //"Highway 31 near Arad",
        //"Highway 375 near Beit Shemesh",
        //"Sonol Gas Station at Horashim Interchange",
        //"Highway 65 - Wadi Ara Road",
        //"HaHistadrut Boulevard in Haifa",
        //"Herzl Street in Rehovot",
        //"Aluf Sadeh Interchange in Ramat Gan",
        //"Highway 25 near Dimona",
        //"Kiryat Ata Interchange",
        //"Highway 721 in the Carmel region",
        //"Highway 204 near Yeruham",
        //"Highway 75 near Nahalal",
        //"Highway 12 near Eilat",
        //"Morasha Interchange in Ramat HaSharon",
        //"Highway 722 in the Jezreel Valley",
        //"Highway 264 near Lehavim",
        //"HaNevi'im Street in Jerusalem",
        //"Savidor Central Train Station in Tel Aviv",
        //"Highway 5711 near Emek Hefer",
        //"Dizengoff Street in Tel Aviv",
        //"Highway 3 near Kiryat Malakhi",
        //"Highway 38 to Beit Shemesh",
        //"Highway 554 near Tira",
        //"Highway 77 near Golani Junction"
        //};
        //    string[,] locationsWithCoordinates = {
        //{ "32.3488", "34.8697" },
        //{ "31.9575", "34.8951" },
        //{ "31.7781", "35.2105" },
        //{ "32.1250", "34.8048" },
        //{ "32.0004", "34.8708" },
        //{ "29.5580", "34.9482" },
        //{ "31.5590", "35.4732" },
        //{ "32.0628", "34.7746" },
        //{ "32.7980", "34.9640" },
        //{ "31.2525", "34.7915" },
        //{ "32.6996", "35.3035" },
        //{ "32.7922", "35.5312" },
        //{ "31.8014", "34.6499" },
        //{ "31.6693", "34.5743" },
        //{ "31.9648", "34.8043" },
        //{ "32.3329", "34.8500" },
        //{ "32.9234", "35.0738" },
        //{ "32.1663", "34.8447" },
        //{ "31.8941", "34.8111" },
        //{ "31.2586", "35.2137" },
        //{ "32.5113", "34.8925" },
        //{ "32.5726", "34.9537" },
        //{ "32.1847", "34.8703" },
        //{ "31.6096", "34.7640" },
        //{ "33.2082", "35.5683" },
        //{ "31.8996", "35.0063" },
        //{ "32.9646", "35.4960" },
        //{ "33.2790", "35.5773" },
        //{ "31.0707", "35.0345" },
        //{ "32.6672", "35.1047" },
        //{ "32.0171", "34.7443" },
        //{ "32.0684", "34.8248" },
        //{ "32.1782", "34.9065" },
        //{ "32.0840", "34.8878" },
        //{ "32.6082", "35.2854" },
        //{ "32.4340", "34.9197" },
        //{ "30.6100", "34.8019" },
        //{ "32.0853", "34.8383" },
        //{ "32.9170", "35.3047" },
        //{ "33.0622", "35.7496" },
        //{ "32.8333", "35.5833" },
        //{ "31.7768", "35.2940" },
        //{ "33.2082", "35.5683" },
        //{ "33.0044", "35.0944" },
        //{ "31.8775", "34.7383" },
        //{ "31.8667", "35.4500" },
        //{ "32.0158", "34.7872" },
        //{ "32.4746", "35.0247" }
        //};
        //        string[,] locationsWithCoordinates = {
        //    { "Highway 2", "32.3488", "34.8697" },
        //    { "Highway 6", "31.9575", "34.8951" },
        //    { "Begin Boulevard, Jerusalem", "31.7781", "35.2105" },
        //    { "Glilot Interchange", "32.1250", "34.8048" },
        //    { "Ben Gurion Airport", "32.0004", "34.8708" },
        //    { "Eilat", "29.5580", "34.9482" },
        //    { "Dead Sea", "31.5590", "35.4732" },
        //    { "Tel Aviv, Rothschild Blvd", "32.0628", "34.7746" },
        //    { "Haifa, Carmel Beach", "32.7980", "34.9640" },
        //    { "Beer Sheva, Soroka Hospital", "31.2525", "34.7915" },
        //    { "Nazareth", "32.6996", "35.3035" },
        //    { "Tiberias", "32.7922", "35.5312" },
        //    { "Ashdod", "31.8014", "34.6499" },
        //    { "Ashkelon", "31.6693", "34.5743" },
        //    { "Rishon Lezion", "31.9648", "34.8043" },
        //    { "Netanya", "32.3329", "34.8500" },
        //    { "Acre (Akko)", "32.9234", "35.0738" },
        //    { "Herzliya", "32.1663", "34.8447" },
        //    { "Rehovot", "31.8941", "34.8111" },
        //    { "Arad", "31.2586", "35.2137" },
        //    { "Caesarea", "32.5113", "34.8925" },
        //    { "Zikhron Ya'akov", "32.5726", "34.9537" },
        //    { "Raanana", "32.1847", "34.8703" },
        //    { "Kiryat Gat", "31.6096", "34.7640" },
        //    { "Kiryat Shmona", "33.2082", "35.5683" },
        //    { "Modiin", "31.8996", "35.0063" },
        //    { "Safed (Tzfat)", "32.9646", "35.4960" },
        //    { "Metula", "33.2790", "35.5773" },
        //    { "Dimona", "31.0707", "35.0345" },
        //    { "Yokneam", "32.6672", "35.1047" },
        //    { "Bat Yam", "32.0171", "34.7443" },
        //    { "Ramat Gan", "32.0684", "34.8248" },
        //    { "Kfar Saba", "32.1782", "34.9065" },
        //    { "Petah Tikva", "32.0840", "34.8878" },
        //    { "Afula", "32.6082", "35.2854" },
        //    { "Hadera", "32.4340", "34.9197" },
        //    { "Mitzpe Ramon", "30.6100", "34.8019" },
        //    { "Bnei Brak", "32.0853", "34.8383" },
        //    { "Karmiel", "32.9170", "35.3047" },
        //    { "Golan Heights", "33.0622", "35.7496" },
        //    { "Sea of Galilee", "32.8333", "35.5833" },
        //    { "Maale Adumim", "31.7768", "35.2940" },
        //    { "Qiryat Shemona", "33.2082", "35.5683" },
        //    { "Nahariya", "33.0044", "35.0944" },
        //    { "Yavne", "31.8775", "34.7383" },
        //    { "Jericho Area", "31.8667", "35.4500" },
        //    { "Holon", "32.0158", "34.7872" },
        //    { "Harish", "32.4746", "35.0247" }
        //};
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
    35.4500, 34.7872, 35.0247
};

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
    31.8667, 32.0158, 32.4746
};

        string[] addresses = {
    "Highway 2","Highway 6","Begin Boulevard,Jerusalem","Glilot Interchange",
    "Ben Gurion Airport","Eilat","Dead Sea","Tel Aviv, Rothschild Blvd",
    "Haifa, Carmel Beach","Beer Sheva, Soroka Hospital","Nazareth","Tiberias",
    "Ashdod","Ashkelon","Rishon Lezion","Netanya","Acre (Akko)","Herzliya",
    "Rehovot","Arad","Caesarea","Zikhron Ya'akov","Raanana","Kiryat Gat",
    "Kiryat Shmona","Modiin","Safed (Tzfat)","Metula","Dimona", "Yokneam",
    "Bat Yam","Ramat Gan","Kfar Saba","Petah Tikva","Afula","Hadera",
    "Mitzpe Ramon","Bnei Brak","Karmiel","Golan Heights","Sea of Galilee",
    "Maale Adumim","Qiryat Shemona","Nahariya", "Yavne","Jericho Area","Holon","Harish"
};


        for (int i = 0; i < 50; i++)
        {

            TypeOfReading typeOfReading= (TypeOfReading)s_rand.Next(0,Enum.GetValues(typeof(TypeOfReading)).Length);
            DateTime TimeOfOpen = new DateTime(s_dalConfig.Clock.Hour - 2,1,1); //stage 1
            DateTime MaxTimeToFinish = TimeOfOpen.AddDays(s_rand.Next((s_dalConfig.Clock - TimeOfOpen).Days));
            double Longitude = longitudes[i];
            double Latitude = latitudes[i];
            string? Adress= addresses[i];
            string? Description="";
        s_dalCall!.Create(new Call(0,typeOfReading, Description, Adress, Longitude, Latitude, TimeOfOpen, MaxTimeToFinish));
        }
    }
    private static void createAssignments()
    {

    }
    private static void createConfigs()
    {

    }

}

