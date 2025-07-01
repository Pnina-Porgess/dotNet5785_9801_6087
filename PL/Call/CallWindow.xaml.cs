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
                    throw new Exception("יש לבחור זמן מקסימלי לסיום.");

                if (CurrentCall.MaxEndTime <= CurrentCall.OpeningTime)
                    throw new Exception("זמן הסיום חייב להיות לאחר זמן הפתיחה.");

                // מניעת עדכון בפרטי קריאה סגורה או שפג תוקפה
                if (CurrentCall.Status is BO.CallStatus.Closed or BO.CallStatus.Expired)
                    throw new Exception("אין אפשרות לעדכן קריאה שנסגרה או שפג תוקפה.");

                if (CurrentCall.Status is BO.CallStatus.InProgress or BO.CallStatus.InProgressAtRisk)
                {
                    s_bl.Call.UpdateCall(CurrentCall);
                    MessageBox.Show("הזמן המקסימלי עודכן בהצלחה!");
                    Close();
                    return;
                }

                // עדכון מלא אם פתוחה
                if (string.IsNullOrWhiteSpace(CurrentCall?.Address))
                    throw new Exception("יש להזין כתובת.");

                if (CurrentCall.Type == null)
                    throw new Exception("יש לבחור סוג קריאה.");

                if (ButtonText == "Add")
                {
                    s_bl.Call.AddCall(CurrentCall!);
                    MessageBox.Show("הקריאה נוספה בהצלחה!");
                }
                else
                {
                    s_bl.Call.UpdateCall(CurrentCall!);
                    MessageBox.Show("הקריאה עודכנה בהצלחה!");
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
