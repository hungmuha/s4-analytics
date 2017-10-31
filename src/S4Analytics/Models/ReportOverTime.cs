using System;
using System.Collections.Generic;

namespace S4Analytics.Models
{
    // variable names are short to keep the json payload small

    public class ReportSeries<T>
    {
        public string name;
        public IEnumerable<T> data;
    }

    public class ReportOverTime<T>
    {
        public IEnumerable<string> categories;
        public IEnumerable<ReportSeries<T>> series;
    }
}
