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
            double Latitude=s_rand.NextDouble()*(33.0-29.5)+29.5;
            double Longitude= s_rand.NextDouble() * (35.9 - 34.2) + 34.2;
            Role MyRole= (Role)s_rand.Next(0,Enum.GetValues(typeof(Role)).Length);
            bool IsActive = true;
            double MaximumDistance = s_rand.Next(5, 50);
            DistanceType MyDistanceType = (DistanceType)s_rand.Next(0, Enum.GetValues(typeof(DistanceType)).Length);
            s_dalVolunteer!.Create(new Volunteer(id,name, email, phone, MyRole, IsActive, MyDistanceType, MaximumDistance, password, Latitude, Longitude));

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

