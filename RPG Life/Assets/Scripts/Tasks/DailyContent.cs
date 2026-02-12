using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.VisualScripting;

[Serializable]
public class DailyContent
{
    public string id;
    public string notes;
    public string title;
    public string startDate;
    public int difficulty;
    public int repetition;
    public int repeatEveryDays;
    public List<int> daysOfWeek;
    public bool isCompleted;
    public bool isActive;

    public DailyContent(string title, string notes, int difficulty, string startDate, int repetition, List<int> daysOfWeek, int repeatEveryDays, bool isCompleted, bool isActive)
    {
        this.difficulty = difficulty;
        this.title = title;
        this.notes = notes;
        this.startDate = startDate;
        this.repetition = repetition;
        this.daysOfWeek = daysOfWeek;
        this.repeatEveryDays = repeatEveryDays;
        this.isCompleted = isCompleted;
        this.isActive = isActive;
    }

    // Check if task is active TODAY (local time)
    public bool IsActiveToday()
    {
        return IsActiveOnDate(DateTime.Now.Date);
    }

    // Check if task should be displayed on a specific date
    public bool IsActiveOnDate(DateTime date)
    {
        DateTime utcTime = DateTime.Parse(startDate);
        DateTime localTime = utcTime.ToLocalTime();

        // Normalize dates to compare only date part (ignore time)
        DateTime taskStart = localTime.Date;
        DateTime checkDate = date.Date;

        if ((Repetition)repetition == Repetition.Daily)
        {
            // Task hasn't started yet
            if (checkDate < taskStart)
            {
                return false;
            }

            // repeatEveryDays = 0 means "only on start date"
            if (repeatEveryDays == 0)
            {
                //Debug.Log(checkDate);
                //Debug.Log(taskStart);
                return checkDate == taskStart;
            }

            // Calculate days difference
            int daysDifference = (checkDate - taskStart).Days;
            // Check if this date matches the repeat pattern
            return daysDifference % repeatEveryDays == 0;
        }

        else if ((Repetition)repetition == Repetition.Weekly)
        {
            List<DayOfWeek> activeDays = new();

            foreach (int day in daysOfWeek)
            {
                activeDays.Add((DayOfWeek)day);
            }

            // Task hasn't started yet
            if (checkDate < taskStart)
            {
                return false;
            }

            // Check if this day of the week is in the active days list
            if (!activeDays.Contains(checkDate.DayOfWeek))
            {
                return false;
            }

            // repeatEveryWeeks = 0 means "only in the start week"
            if (repeatEveryDays == 0)
            {
                // Check if checkDate is in the same week as startDate
                return AreDatesInSameWeek(taskStart, checkDate);
            }

            // Calculate weeks difference
            int weeksDifference = GetWeeksDifference(taskStart, checkDate);

            // Check if this week matches the repeat pattern
            return weeksDifference >= 0 && weeksDifference % repeatEveryDays == 0;
        }

        else if ((Repetition)repetition == Repetition.Monthly)
        {
            // Task hasn't started yet
            if (checkDate < taskStart)
                return false;

            // Must match day of month
            if (checkDate.Day != taskStart.Day)
                return false;

            return true;
        }

        else if ((Repetition)repetition == Repetition.Yearly)
        {
            // Task hasn't started yet
            if (checkDate < taskStart)
                return false;

            // Must match month and day
            if (checkDate.Month != taskStart.Month ||
                checkDate.Day != taskStart.Day)
                return false;

            return true;
        }

        else
        {
            return false;
        }

    }

    // Helper: Get start of week (Sunday)
    private DateTime GetStartOfWeek(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Sunday)) % 7;
        return date.AddDays(-diff).Date;
    }

    // Helper: Calculate weeks difference between two dates
    private int GetWeeksDifference(DateTime startDate, DateTime checkDate)
    {
        // Get start of week for both dates
        DateTime startOfStartWeek = GetStartOfWeek(startDate);
        DateTime startOfCheckWeek = GetStartOfWeek(checkDate);

        // Calculate difference in days and convert to weeks
        int daysDifference = (startOfCheckWeek - startOfStartWeek).Days;
        return daysDifference / 7;
    }

    // Helper: Check if two dates are in the same week
    private bool AreDatesInSameWeek(DateTime date1, DateTime date2)
    {
        // Get the start of the week (Sunday) for both dates
        DateTime startOfWeek1 = GetStartOfWeek(date1);
        DateTime startOfWeek2 = GetStartOfWeek(date2);

        return startOfWeek1 == startOfWeek2;
    }

    private DateTime GetTaskStartLocalDate()
    {
        DateTime utcTime = DateTime.Parse(startDate);
        return utcTime.ToLocalTime().Date;
    }


    public DateTime? GetNextActiveDate(DateTime fromDate)
    {
        DateTime taskStart = GetTaskStartLocalDate();
        DateTime checkDate = fromDate.Date;

        if (checkDate < taskStart)
            checkDate = taskStart;

        switch ((Repetition)repetition)
        {
            case Repetition.Daily:
                return GetNextDaily(taskStart, checkDate);

            case Repetition.Weekly:
                return GetNextWeekly(taskStart, checkDate);

            case Repetition.Monthly:
                return GetNextMonthly(taskStart, checkDate);

            case Repetition.Yearly:
                return GetNextYearly(taskStart, checkDate);

            default:
                return null;
        }
    }

    private DateTime? GetNextDaily(DateTime taskStart, DateTime fromDate)
    {
        // Only happens once
        if (repeatEveryDays == 0)
            return fromDate <= taskStart ? taskStart : null;

        if (fromDate < taskStart)
            return taskStart;

        int daysSinceStart = (fromDate - taskStart).Days;

        // If today is already valid, return today
        if (daysSinceStart % repeatEveryDays == 0)
            return fromDate;

        // Otherwise jump to next valid day
        int nextOffset =
            ((daysSinceStart / repeatEveryDays) + 1) * repeatEveryDays;

        return taskStart.AddDays(nextOffset);
    }

    private DateTime? GetNextWeekly(DateTime taskStart, DateTime fromDate)
    {
        List<DayOfWeek> activeDays = daysOfWeek
            .Select(d => (DayOfWeek)d)
            .OrderBy(d => d)
            .ToList();

        if (activeDays.Count == 0)
            return null;

        DateTime cursor = fromDate;

        for (int i = 0; i < 366; i++) // safety cap (1 year)
        {
            if (cursor < taskStart)
            {
                cursor = taskStart;
                continue;
            }

            if (!activeDays.Contains(cursor.DayOfWeek))
            {
                cursor = cursor.AddDays(1);
                continue;
            }

            if (repeatEveryDays == 0)
            {
                if (AreDatesInSameWeek(taskStart, cursor))
                    return cursor;
            }
            else
            {
                int weeksDiff = GetWeeksDifference(taskStart, cursor);
                if (weeksDiff >= 0 && weeksDiff % repeatEveryDays == 0)
                    return cursor;
            }

            cursor = cursor.AddDays(1);
        }

        return null;
    }

    private DateTime? GetNextMonthly(DateTime taskStart, DateTime fromDate)
    {
        int day = taskStart.Day;
        DateTime cursor = new DateTime(fromDate.Year, fromDate.Month, 1);

        for (int i = 0; i < 24; i++) // 2 years safety
        {
            int daysInMonth = DateTime.DaysInMonth(cursor.Year, cursor.Month);
            if (day <= daysInMonth)
            {
                DateTime candidate = new DateTime(cursor.Year, cursor.Month, day);
                if (candidate >= fromDate && candidate >= taskStart)
                    return candidate;
            }

            cursor = cursor.AddMonths(1);
        }

        return null;
    }

    private DateTime? GetNextYearly(DateTime taskStart, DateTime fromDate)
    {
        int month = taskStart.Month;
        int day = taskStart.Day;

        for (int year = fromDate.Year; year <= fromDate.Year + 10; year++)
        {
            if (day > DateTime.DaysInMonth(year, month))
                continue;

            DateTime candidate = new DateTime(year, month, day);

            if (candidate >= fromDate && candidate >= taskStart)
                return candidate;
        }

        return null;
    }

}
