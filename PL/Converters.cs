using BO;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL.Converter;  // Converter: if ButtonText is "Update" => true
    public class ConvertUpdateToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value as string) == "Update";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    // Converter: if ButtonText is "Update" => Visible, else Collapsed
    public class ConvertUpdateToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value as string) == "Update" ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    // Converter: true → Visible, false → Collapsed
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && b ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    // Converter: null → Collapsed (or Hidden), not-null → Visible
    public class NullToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; } = false;
        public bool UseHidden { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNullOrEmpty =
                value == null ||
                (value is string str && string.IsNullOrWhiteSpace(str)) ||
                (value is System.Collections.IEnumerable enumerable && !enumerable.GetEnumerator().MoveNext());

            if (Invert)
                isNullOrEmpty = !isNullOrEmpty;

            return isNullOrEmpty ? Visibility.Visible : (UseHidden ? Visibility.Hidden : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class UpdateModeToBoolConverter : IValueConverter
    {
        // אם מצב הוא Add → מחזיר true (אפשר לערוך ID), אחרת false
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string mode && mode == "Add";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
public class CanDeleteCallConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CallInList call)
        {
            return (call.CallStatus == CallStatus.Open && call.AssignmentId == null)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToTextConverter : IValueConverter
{
    public string TrueText { get; set; } = "Stop";
    public string FalseText { get; set; } = "Start";

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        => (value is bool b && b) ? TrueText : FalseText;

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        => Binding.DoNothing;
}

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        => !(value is bool b && b);

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        => Binding.DoNothing;
}



