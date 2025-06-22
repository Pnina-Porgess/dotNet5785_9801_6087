using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using BlApi;
using BO;

namespace PL.Call
{
    public partial class CallWindow : Window
    {
        private static readonly IBl s_bl = BlApi.Factory.Get();
        public CallWindow(int callId)
        {
            InitializeComponent();
            Call = s_bl.Call.GetCallDetails(callId);
        }
        public BO.Call Call { get; set; }
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