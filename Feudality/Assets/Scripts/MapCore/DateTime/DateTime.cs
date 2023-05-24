using System;
using UnityEngine;

namespace Feudality.GlobalMap.TimeSystem {
    public struct DateTime {
        public enum EMonth {
            Spring = 1,
            Summer = 2,
            Autumn = 3,
            Winter = 4,
        };

        private const int MONTHS_IN_YEAR = 4;
        private const int DAYS_IN_MONTH = 30;
        private const int HOURS_IN_DAY = 24;
        private const int MINUTES_IN_HOUR = 60;

        private ulong minutesTotal;

        public ulong Minute { get { return minutesTotal - HoursToMinutes(HoursTotal); } }
        public ulong Hour { get { return MinutesToHours(minutesTotal - DaysToMinutes(DaysTotal)); } }
        public ulong Day { get { return MinutesToDays(minutesTotal - MonthsToMinutes(MonthsTotal)); } }
        public ulong Month { get { return MinutesToMonths(minutesTotal - YearsToMinutes(YearsTotal)); } }
        public ulong Year { get { return YearsTotal; } }

        public ulong MinutesTotal { get { return minutesTotal; } }
        public ulong HoursTotal { get { return MinutesToHours(minutesTotal); } }
        public ulong DaysTotal { get { return MinutesToDays(minutesTotal); } }
        public ulong MonthsTotal { get { return MinutesToMonths(minutesTotal); } }
        public ulong YearsTotal { get { return MinutesToYears(minutesTotal); } }

        public ulong MinutesFromDayStarts { get { return minutesTotal - DaysToMinutes(DaysTotal); } }
        public static int MinutesInDay { get { return MINUTES_IN_HOUR * HOURS_IN_DAY; } }

        public static ulong MinutesToHours(ulong minutes) {
            return minutes / MINUTES_IN_HOUR;
        }

        public static ulong MinutesToDays(ulong minutes) {
            return MinutesToHours(minutes) / HOURS_IN_DAY;
        }

        public static ulong MinutesToMonths(ulong minutes) {
            return MinutesToDays(minutes) / DAYS_IN_MONTH;

        }

        public static ulong MinutesToYears(ulong minutes) {
            return MinutesToMonths(minutes) / MONTHS_IN_YEAR;
        }

        public static ulong YearsToMinutes(ulong year) {
            return year * MONTHS_IN_YEAR * DAYS_IN_MONTH * HOURS_IN_DAY * MINUTES_IN_HOUR;
        }

        public static ulong MonthsToMinutes(ulong month) {
            return month * DAYS_IN_MONTH * HOURS_IN_DAY * MINUTES_IN_HOUR;
        }

        public static ulong DaysToMinutes(ulong days) {
            return days * HOURS_IN_DAY * MINUTES_IN_HOUR;
        }

        public static ulong HoursToMinutes(ulong hours) {
            return hours * MINUTES_IN_HOUR;
        }

        public DateTime(ulong minutes) {
            minutesTotal = minutes;
        }

        public DateTime(ulong year, ulong month, ulong day, ulong hour, ulong minute) {
            minutesTotal = YearsToMinutes(year) + MonthsToMinutes(month) + DaysToMinutes(day) + HoursToMinutes(hour) + minute;
        }

        public static DateTime operator +(DateTime a, DateTime b) => new DateTime(a.minutesTotal + b.minutesTotal);

        public static DateTime TimeToMinutes(ulong time, float dayDuration, float conversionFactor) {
            var minuteDuration = dayDuration * conversionFactor / HOURS_IN_DAY / MINUTES_IN_HOUR;
            return new DateTime((ulong)(time / minuteDuration));
        }
    }
}
