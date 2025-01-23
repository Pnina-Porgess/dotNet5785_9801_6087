using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi;
    public interface IVolunteer
    {
        string Login(string username, string password); // מתודת כניסה למערכת
        IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortBy? sortBy = null); // בקשת רשימת מתנדבים
        BO.Volunteer GetVolunteerDetails(int id); // בקשת פרטי מתנדב
        void UpdateVolunteerDetails(int id, BO.Volunteer volunteer); // עדכון פרטי מתנדב
        void DeleteVolunteer(int id); // מחיקת מתנדב
        void AddVolunteer(BO.Volunteer volunteer); // הוספת מתנדב
    }


