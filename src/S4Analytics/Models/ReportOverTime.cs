namespace S4Analytics.Models
{
    // variable names are short to keep the json payload small

    public class ReportOverTimeByYear
    {
        public string s; // series
        public int y; // year
        public int v; // value
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
