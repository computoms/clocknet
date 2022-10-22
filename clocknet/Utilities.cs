﻿using System.Globalization;

namespace clocknet;

public static class Utilities
{
    public static string PrintDuration(TimeSpan duration) 
	    => Math.Floor(duration.TotalHours).ToString().PadLeft(2, '0') + ":" + Math.Round((60 * (duration.TotalHours - Math.Floor(duration.TotalHours)))).ToString().PadLeft(2, '0');

    public static int GetWeekNumber(DateTime date) => CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

    public static DateTime DayOfSameWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    public static string Duration(this IEnumerable<Record> records)
    {
        var duration = records.Aggregate<Record, TimeSpan>(TimeSpan.Zero, (aggr, rec) => aggr + rec.Duration);
        return PrintDuration(duration);
    }

    public static string Duration(this IEnumerable<Activity> activities)
    {
        var duration = activities.Aggregate<Activity, TimeSpan>(TimeSpan.Zero, (aggr, rec) => aggr + rec.Duration);
        return PrintDuration(duration);
    }
}

