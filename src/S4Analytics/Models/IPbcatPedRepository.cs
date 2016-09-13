using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.PBCAT;

namespace S4Analytics.Models
{
    public interface IPbcatPedRepository
    {
        PBCATPedestrianInfo Find(int hsmvRptNbr);
        void Add(int hsmvRptNbr, PBCATPedestrianInfo pedInfo, CrashTypePedestrian crashType);
        void Update(int hsmvRptNbr, PBCATPedestrianInfo pedInfo, CrashTypePedestrian crashType);
        void Remove(int hsmvRptNbr);
        CrashTypePedestrian GetCrashType(PBCATPedestrianInfo pedInfo);
        PbcatParticipantInfo GetParticipantInfo(int hsmvRptNbr);
        bool CrashReportExists(int hsmvRptNbr);
    }
}
