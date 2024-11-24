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

    }
    private static void createCalls()
    {

    }
    private static void createAssignments()
    {

    }
    private static void createConfigs()
    {

    }

}

