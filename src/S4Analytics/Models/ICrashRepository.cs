using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S4Analytics.Models
{
    public interface ICrashRepository
    {
        int CreateQuery(CrashQuery query);
        (string, DynamicParameters) CreateQueryTest(CrashQuery query);
        bool QueryExists(int queryId);
        IEnumerable<CrashResult> GetCrashes(int queryId);
    }
}
