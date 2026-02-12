using System.Collections.Generic;

namespace UI.Dates
{
    public static class DatePickerCache
    {
        internal static Dictionary<string, bool> _DateFallsWithinMonthResults = new Dictionary<string, bool>();
    }  
}
