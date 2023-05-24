using Feudality.GlobalMap.TimeSystem;
using TMPro;
using UnityEngine;

namespace Feudality.GlobalMap.HUD {
    public class HudDateTime : MonoBehaviour {
        [SerializeField] TMP_Text dateText;
        [SerializeField] TMP_Text timeText;
        [SerializeField] DayNightCycle dayNightCycle;

        private string zeroString = "0";
        private ulong lastYear;
        private ulong lastMonth;
        private ulong lastDay;

        private ulong lastHour;
        private ulong lastMinute;

        private void Update() {
            var dateTime = dayNightCycle.GetDateTime();
            var year = dateTime.Year;
            var month = dateTime.Month;
            var day = dateTime.Day;
            var hour = dateTime.Hour;
            var minute = dateTime.Minute;

            if (IsDateChanged(year, month, day)) UpdateDate(year, month, day);

            if (IsTimeChanged(hour, minute)) UpdateTime(hour, minute);

            lastYear = year;
            lastMonth = month;
            lastDay = day;
            lastHour = hour;
            lastMinute = minute;
        }

        private bool IsDateChanged(ulong year, ulong month, ulong day) {
            return year != lastYear || month != lastMonth || day != lastDay;
        }

        private string monthYearCache = "";

        private void UpdateDate(ulong year, ulong month, ulong day) {
            var dayPrefix = day < 10 ? zeroString : string.Empty;
            monthYearCache = month != lastMonth ? $"{month}.{year}" : monthYearCache;
            dateText.text = $"{dayPrefix}{day}.{monthYearCache}";
            //dateText.text = $"{dayPrefix}{day}.{month}.{year}";
        }

        private bool IsTimeChanged(ulong hour, ulong minute) {
            return hour != lastHour || minute != lastMinute;
        }

        private void UpdateTime(ulong hour, ulong minute) {
            var hourPrefix = hour < 10 ? zeroString : string.Empty;
            var minutesPrefix = minute < 10 ? zeroString : string.Empty;
            timeText.text = $"{hourPrefix}{hour}:{minutesPrefix}{minute}";
        }
    }
}
