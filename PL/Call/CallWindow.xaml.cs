using BlApi;
using BO;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace PL.Call
{
    public partial class CallWindow : Window
    {
        private static readonly IBl s_bl = BlApi.Factory.Get();
        public BO.Call Call { get; set; }
       
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        public CallWindow(int callId)
        {
            InitializeComponent();
            Call = s_bl.Call.GetCallDetails(callId);
            DataContext = this;

        }

        // Properties for UI logic




        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // בדיקות פורמט בסיסיות (לדוג' תיאור לא רי
                // שלח עדכון ל-BL
                s_bl.Call.UpdateCall(Call);
                MessageBox.Show("הקריאה עודכנה בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בעדכון הקריאה: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}