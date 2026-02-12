using System;

[Serializable]
public class HabitContent
{
    public string id;
    public string notes;
    public string title;
    public int difficulty;
    public int repetition;
    public bool isCompleted;
    public bool isActive;
    public int count;
    public string lastPositiveCheck;
    public int lastMark;

    public HabitContent(string title, string notes, int difficulty, int repetition, bool isCompleted, int count, string lastPositiveCheck, int lastMark, bool isActive)
    {
        this.difficulty = difficulty;
        this.title = title;
        this.notes = notes;
        this.repetition = repetition;
        this.isCompleted = isCompleted;
        this.count = count;
        this.lastPositiveCheck = lastPositiveCheck;
        this.lastMark = lastMark;
        this.isActive = isActive;
    }

    bool CanMark(DateTime a, DateTime b, Repetition repetition)
    {
        a = a.Date;
        b = b.Date;

        switch (repetition)
        {
            case Repetition.Daily:
                return a == b;

            case Repetition.Weekly:
                return IsSameWeek(a, b);

            case Repetition.Monthly:
                return a.Year == b.Year &&
                       a.Month == b.Month;

            case Repetition.Yearly:
                return a.Year == b.Year;

            default:
                return false;
        }
    }

    public static bool IsSameWeek(DateTime a, DateTime b)
    {
        return GetStartOfWeek(a) == GetStartOfWeek(b);
    }

    public static DateTime GetStartOfWeek(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Sunday)) % 7;
        return date.AddDays(-diff).Date;
    }
}