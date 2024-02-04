namespace Fireball.Common.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToQueryString(this TimeSpan timeSpan, string queryStringKey)
        {
            return $"{queryStringKey}={timeSpan.ToUriSafeString()}";
        }

        public static string ToUriSafeString(this TimeSpan timeSpan)
        {
            return Uri.EscapeDataString(timeSpan.ToString("c"));
        }
    }
}
