using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using BO;

namespace PL.Call
{
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public CallWindow(int id = 0)
        {
            InitializeComponent();
            SetCurrentValue(ButtonTextProperty, id == 0 ? "Add" : "Update");

            if (id == 0)
            {
                CurrentCall = new BO.Call
                {
                    Status = BO.CallStatus.Open,
                    OpeningTime = s_bl.Admin.GetClock()
                };
            }
            else
            {
                try
                {
                    CurrentCall = s_bl.Call.GetCallDetails(id);
                    s_bl.Call.AddObserver(id, RefreshCallObserver);
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
                    s_bl.Call.RemoveObserver(CurrentCall.Id, RefreshCallObserver);
            };
        }

        public BO.Call? CurrentCall
        {
            get => (BO.Call?)GetValue(CurrentCallProperty);
            set
            {
                SetValue(CurrentCallProperty, value);
                OnPropertyChanged(nameof(CanEditDetails));
                OnPropertyChanged(nameof(CanEditMaxEndTime));
            }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(CallWindow), new PropertyMetadata("Update"));

        public bool CanEditDetails =>
            CurrentCall is { Status: BO.CallStatus.Open or BO.CallStatus.OpenAtRisk };

        public bool CanEditMaxEndTime =>
            CurrentCall is { Status: BO.CallStatus.Open or BO.CallStatus.OpenAtRisk or BO.CallStatus.InProgress or BO.CallStatus.InProgressAtRisk };

        private void RefreshCallObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    if (CurrentCall == null) return;
                    int id = CurrentCall.Id;
                    CurrentCall = null;
                    CurrentCall = s_bl.Call.GetCallDetails(id);
                });
            }
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentCall?.MaxEndTime == null)
                    throw new Exception("Please select a max end time.");

                if (CurrentCall.MaxEndTime <= CurrentCall.OpeningTime)
                    throw new Exception("Max end time must be after opening time.");

                if (CurrentCall.Status is BO.CallStatus.Closed or BO.CallStatus.Expired)
                    throw new Exception("Cannot update a call that is closed or expired.");

                if (CurrentCall.Status is BO.CallStatus.InProgress or BO.CallStatus.InProgressAtRisk)
                {
                    s_bl.Call.UpdateCall(CurrentCall);
                    MessageBox.Show("Max end time updated successfully!");
                    Close();
                    return;
                }

                if (string.IsNullOrWhiteSpace(CurrentCall?.Address))
                    throw new Exception("Please enter an address.");

                if (CurrentCall.Type == null)
                    throw new Exception("Please select a call type.");

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
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
