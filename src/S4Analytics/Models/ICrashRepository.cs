using System.Collections.Generic;

namespace S4Analytics.Models
{
    public interface ICrashRepository
    {
        (string, Dictionary<string, object>) CreateQueryTest(CrashQuery query);
        string CreateQuery(CrashQuery query);
        bool QueryExists(string queryToken);
        IEnumerable<CrashResult> GetCrashes(string queryToken);
        IEnumerable<AttributeSummary> GetCrashSeveritySummary(string queryToken);
    }
}
