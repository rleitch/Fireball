using System;

namespace Fireball.Common.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToQueryString(this TimeSpan timeSpan, string queryStringKey)
        {
            return $"{queryStringKey}={timeSpan.Ticks}";
        }
    }
}
