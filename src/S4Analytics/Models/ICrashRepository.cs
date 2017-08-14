using System.Collections.Generic;

namespace S4Analytics.Models
{
    public interface ICrashRepository
    {
        (string, Dictionary<string, object>) CreateQueryTest(CrashQuery query);
        string CreateQuery(CrashQuery query);
        bool QueryExists(string queryToken);
        IEnumerable<CrashResult> GetCrashes(string queryToken, int fromIndex, int toIndex);
        IEnumerable<AttributeSummary> GetCrashSeveritySummary(string queryToken);
        EventFeatureSet GetCrashFeatureCollection(string queryToken, Extent mapExtent);
    }
}
