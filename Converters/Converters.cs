using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using GeminiChat.Models;

namespace GeminiChat.Converters
{
    public class BoolToVisConverter : IValueConverter
    {
        public static readonly BoolToVisConverter Instance = new();
        public object Convert(object? v, Type t, object? p, CultureInfo c)  => v is true;
        public object ConvertBack(object? v, Type t, object? p, CultureInfo c) => v is true;
    }

    public class InverseBoolConverter : IValueConverter
    {
        public static readonly InverseBoolConverter Instance = new();
        public object Convert(object? v, Type t, object? p, CultureInfo c)  => v is not true;
        public object ConvertBack(object? v, Type t, object? p, CultureInfo c) => v is not true;
    }

    public class RoleToAlignConverter : IValueConverter
    {
        public static readonly RoleToAlignConverter Instance = new();
        public object Convert(object? v, Type t, object? p, CultureInfo c)
            => v is MessageRole r && r == MessageRole.User
               ? HorizontalAlignment.Right
               : HorizontalAlignment.Left;
        public object ConvertBack(object? v, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
    }

    public class RoleToBubbleBrushConverter : IValueConverter
    {
        public static readonly RoleToBubbleBrushConverter Instance = new();
        public object Convert(object? v, Type t, object? p, CultureInfo c)
            => v is MessageRole r && r == MessageRole.User
               ? new SolidColorBrush(Color.Parse("#2563EB"))
               : new SolidColorBrush(Color.Parse("#1E293B"));
        public object ConvertBack(object? v, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
    }

    public class RoleToTextBrushConverter : IValueConverter
    {
        public static readonly RoleToTextBrushConverter Instance = new();
        public object Convert(object? v, Type t, object? p, CultureInfo c)
            => new SolidColorBrush(Color.Parse("#E2E8F0"));
        public object ConvertBack(object? v, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
    }

    public class ApiKeyValidColorConverter : IValueConverter
    {
        public static readonly ApiKeyValidColorConverter Instance = new();
        public object Convert(object? v, Type t, object? p, CultureInfo c)
            => v is true
               ? new SolidColorBrush(Color.Parse("#10B981"))
               : new SolidColorBrush(Color.Parse("#F59E0B"));
        public object ConvertBack(object? v, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
    }

    public class DateTimeConverter : IValueConverter
    {
        public static readonly DateTimeConverter Instance = new();
        public object Convert(object? v, Type t, object? p, CultureInfo c)
            => v is DateTime dt ? dt.ToString("HH:mm") : string.Empty;
        public object ConvertBack(object? v, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
    }

    public class IsUserToAvatarTextConverter : IValueConverter
    {
        public static readonly IsUserToAvatarTextConverter Instance = new();
        public object Convert(object? v, Type t, object? p, CultureInfo c)
            => v is true ? "U" : "G";
        public object ConvertBack(object? v, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
    }

    public class IsUserToAvatarBrushConverter : IValueConverter
    {
        public static readonly IsUserToAvatarBrushConverter Instance = new();
        public object Convert(object? v, Type t, object? p, CultureInfo c)
            => v is true
               ? new SolidColorBrush(Color.Parse("#1E40AF"))
               : new SolidColorBrush(Color.Parse("#1E3A8A"));
        public object ConvertBack(object? v, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
    }

    public class StatusToColorConverter : IValueConverter
    {
        public static readonly StatusToColorConverter Instance = new();
        public object Convert(object? v, Type t, object? p, CultureInfo c)
            => v switch
            {
                MessageStatus.Error     => new SolidColorBrush(Color.Parse("#EF4444")),
                MessageStatus.Streaming => new SolidColorBrush(Color.Parse("#10B981")),
                _                       => new SolidColorBrush(Colors.Transparent)
            };
        public object ConvertBack(object? v, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
    }

    public class IntToVisConverter : IValueConverter
    {
        public static readonly IntToVisConverter Instance = new();
        public object Convert(object? v, Type t, object? p, CultureInfo c)
            => v is int i && i == 0;   // true (visible) when count == 0
        public object ConvertBack(object? v, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
    }
}