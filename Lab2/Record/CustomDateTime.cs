using System;

namespace Record
{
    public readonly struct CustomMyDate : IComparable<CustomMyDate>
    {
        public readonly int Year;
        public readonly int Month;
        public readonly int Day;

        private static int DaysInMonth(int year, int month)
        {
            return month switch
            {
                1 or 3 or 5 or 7 or 8 or 10 or 12 => 31,
                4 or 6 or 9 or 11 => 30,
                2 => IsLeapYear(year) ? 29 : 28,
                _ => throw new ArgumentException("Invalid month")
            };
        }

        private static bool IsLeapYear(int year)
        {
            return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
        }
        
        public CustomMyDate(int year, int month, int day)
        {
            if (year < 1 || month < 1 || month > 12 || day < 1 || day > DaysInMonth(year, month))
                throw new ArgumentException("Invalid date");
            
            Year = year;
            Month = month;
            Day = day;
        }
        
        public CustomMyDate(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                throw new ArgumentException("Date string cannot be null or empty");

            var parts = dateStr.Split('-');
            if (parts.Length != 3)
                throw new ArgumentException("Date string must be in format yyyy-MM-dd");

            if (!int.TryParse(parts[0], out var year) ||
                !int.TryParse(parts[1], out var month) ||
                !int.TryParse(parts[2], out var day))
                throw new ArgumentException("Invalid date components");

            if (year < 1 || month < 1 || month > 12 || day < 1 || day > DaysInMonth(year, month))
                throw new ArgumentException("Invalid date");

            Year = year;
            Month = month;
            Day = day;
        }
        
        public CustomMyDate AddDays(int daysToAdd)
        {
            var year = Year;
            var month = Month;
            var day = Day + daysToAdd;

            while (true)
            {
                var daysInMonth = DaysInMonth(year, month);
                if (day <= daysInMonth)
                    break;

                day -= daysInMonth;
                month++;
                if (month <= 12) continue;
                
                month = 1;
                year++;
            }

            while (day < 1)
            {
                month--;
                if (month < 1)
                {
                    month = 12;
                    year--;
                }
                day += DaysInMonth(year, month);
            }

            return new CustomMyDate(year, month, day);
        }
        
        public int CompareTo(CustomMyDate other)
        {
            if (Year != other.Year) return Year.CompareTo(other.Year);
            if (Month != other.Month) return Month.CompareTo(other.Month);
            return Day.CompareTo(other.Day);
        }
        
        public override string ToString()
        {
            return $"{Year:0000}-{Month:00}-{Day:00}";
        }
    }
}
