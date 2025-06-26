using System;
using System.Windows;
using BO;

namespace PL.Call
{
    public partial class CallWindow : Window
    {
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public string ButtonText { get; set; }
        public CallWindow(int id = 0)
        {
            InitializeComponent();
            ButtonText = id == 0 ? "Add" : "Update";

            if (id == 0)
            {
                CurrentCall = new BO.Call
                {
                    Status= BO.CallStatus.Open,
                    OpeningTime = s_bl.Admin.GetClock()  // זמן התחלה = עכשיו
                };
            }
            else
            {
                try
                {
                    CurrentCall = s_bl.Call.GetCallDetails(id);
                    s_bl.Call.AddObserver(id, CallObserver);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Close();
                }
            }

            this.Closed += (s, e) =>
            {
                if (CurrentCall != null && CurrentCall.Id != 0)
                    s_bl.Call.RemoveObserver(CurrentCall.Id, CallObserver);
            };

            DataContext = this;
        }

        public BO.Call? CurrentCall
        {
            get => (BO.Call?)GetValue(CurrentCallProperty);
            set => SetValue(CurrentCallProperty, value);
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        private void CallObserver()
        {
            if (CurrentCall == null) return;
            int id = CurrentCall.Id;
            CurrentCall = null;
            CurrentCall = s_bl.Call.GetCallDetails(id);
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {

                    s_bl.Call.AddCall(CurrentCall!);
                    MessageBox.Show("Call added successfully!");
                }
                else
                {
                    s_bl.Call.UpdateCall(CurrentCall!);
                    MessageBox.Show("Call updated successfully!");
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
