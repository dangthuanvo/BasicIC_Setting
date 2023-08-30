using System;

namespace Common
{
    public static class LayoutType
    {
        public const string RightBig = "right_big";
        public const string RightSmall = "right_small";
        public const string RightMiddle = "right_middle";
        public const string LeftBig = "left_big";
        public const string LeftSmall = "left_small";
        public const string LeftMiddle = "left_middle";
        public const string BottomBig = "bottom_big";
        public const string BottomSmall = "bottom_small";
        public const string Full = "full";
    }
    public static class FrequencyPopup
    {
        public const string OnlySession = "only_session";
        public const string OnlyCustomer = "only_customer";
        public const string OnlyEveryhour = "only_everyhour";
        public const string OnlyEveryday = "only_everyday";
        public const string MostShow = "most_show";
    }
    public static class ItemType
    {
        public const string Text = "text";
        public const string Input = "input";
    }
    public static class InputType
    {
        public const string String = "string";
        public const string Number = "number";
        public const string Date = "date";
        public const string Datetime = "datetime";
        public const string Droplist = "droplist";
    }
    public static class Weekday
    {
        public const string Monday = "monday";
        public const string Tuesday = "tuesday";
        public const string Wednesday = "wednesday";
        public const string Thursday = "thursday";
        public const string Friday = "friday";
        public const string Saturday = "saturday";
        public const string Sunday = "sunday";
    }
    public static class AuthenType
    {
        public const string BEARER_TOKEN = "bearer_token";
        public const string BASIC_AUTH = "basic_auth";
        public const string NO_AUTH = "no_auth";
    }
    public static class FieldMapType
    {
        public const string Customer = "customer";
        public const string Ticket = "ticket";
    }
    public static class DeviceType
    {
        public const string Desktop = "desktop";
        public const string Mobile = "mobile";
    }
    public static class DefaultSchedule
    {
        public static TimeSpan StartTime = new TimeSpan(8, 0, 0);
        public static TimeSpan EndTime = new TimeSpan(17, 0, 0);
    }
}