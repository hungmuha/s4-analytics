using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess;
using Microsoft.Extensions.Options;
using Lib.PBCAT;

namespace S4Analytics.Models
{
    public class PbcatPedRepository : IPbcatPedRepository
    {
        private string _warehouseConnStr;

        public PbcatPedRepository(IOptions<DbOptions> dbOptions)
        {
            _warehouseConnStr = dbOptions.Value.WarehouseConnStr;
        }

        public PBCATPedestrianInfo Find(int hsmvRptNbr) {
            return new PBCATPedestrianInfo();
        }

        public PBCATPedestrianInfo Add(int hsmvRptNbr, PBCATPedestrianInfo pedInfo, CrashTypePedestrian crashType)
        {
            return pedInfo;
        }

        public void Update(int hsmvRptNbr, PBCATPedestrianInfo pedInfo, CrashTypePedestrian crashType) {

        }

        public void Remove(int hsmvRptNbr) {

        }

        public CrashTypePedestrian GetCrashType(PBCATPedestrianInfo pedInfo) {
            return new CrashTypePedestrian();
        }

        public bool HsmvNumberExists(int hsmvRptNbr)
        {
            return true;
        }
    }
}
