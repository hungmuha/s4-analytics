using System.Collections.Generic;

namespace S4Analytics.Models
{
    // variable names are short to keep the json payload small

    public class ReportSeries<T>
    {
        public string name;
        public IEnumerable<T> data;
    }

    public class ReportOverTimeByYear
    {
        public IEnumerable<string> categories;
        public IEnumerable<ReportSeries<int>> series;
    }

    public class ReportOverTimeByMonth
    {
        public string s; // series
        public int m; // month
        public int v; // value
    }

    public class ReportOverTimeByDay
    {
        public string s; // series
        public int m; // month
        public int d; // day
        public int v; // value
    }
}
