

namespace BlApi;
    public interface IVolunteer
    {
         BO.Role Login(string username, string password); // מתודת כניסה למערכת
        IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortBy? sortBy = null); // בקשת רשימת מתנדבים
        BO.Volunteer GetVolunteerDetails(int volunteerId); // בקשת פרטי מתנדב
        void UpdateVolunteerDetails(int id, BO.Volunteer volunteer); // עדכון פרטי מתנדב
        void DeleteVolunteer(int id); // מחיקת מתנדב
        void AddVolunteer(BO.Volunteer volunteer); // הוספת מתנדב
}


