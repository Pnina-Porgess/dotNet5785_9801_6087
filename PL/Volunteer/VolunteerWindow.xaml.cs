using System;
using System.Windows;
using BO;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // נשמור את הפונקציה כדי שנוכל להסיר אותה בדיוק
        private readonly Action _volunteerObserver;

        public VolunteerWindow(int id = 0)
        {
            ButtonText = id == 0 ? "Add" : "Update";
            InitializeComponent();

            if (id == 0)
            {
                CurrentVolunteer = new BO.Volunteer();
            }
            else
            {
                try
                {
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Close();
                    return;
                }
            }

            // יצירת המשקיף באופן חד-פעמי לפר instance
            _volunteerObserver = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (CurrentVolunteer == null || CurrentVolunteer.Id == 0) return;

                    var updated = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id);
                    SetCurrentValue(CurrentVolunteerProperty, updated);
                });
            };


            // הרשמה למשקיף
            if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
                s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, _volunteerObserver);

            // הסרה של המשקיף בסגירת החלון
            this.Closed += (s, e) =>
            {
                if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
                    s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, _volunteerObserver);
            };
        }

        // תכונת תלות לאובייקט
        public BO.Volunteer? CurrentVolunteer
        {
            get => (BO.Volunteer?)GetValue(CurrentVolunteerProperty);
            set => SetValue(CurrentVolunteerProperty, value);
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        // תכונת תלות לטקסט הכפתור
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata("Add"));

        // אירוע כפתור
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                    MessageBox.Show("Volunteer added successfully!");
                }
                else
                {
                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer!.Id, CurrentVolunteer!);
                    MessageBox.Show("Volunteer updated successfully!");
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
