using System;

namespace S4Analytics.Models
{
    public class CrashResult
    {
        public int id;
        public DateTime? crashDate;
        public DateTime? crashTime;
        public int? hsmvReportNumber;
        public string hsmvReportNumberDsp;
        public string agencyReportNumber;
        public string agencyReportNumberDsp;
        public double? mapPointX;
        public double? mapPointY;
        public double? centerLineX;
        public double? centerLineY;
        public int? symbolAngle;
        public int? crashSegId;
        public int? nearestIntrsectId;
        public int? nearestIntrsectOffsetFt;
        public int? refIntrsectId;
        public int? refIntrsectOffsetFt;
        public int? nearIntrsectOffsetDir;
        public int? refIntrsectOffsetDir;
        public string imgExtTx;
        public string formType;
        public int? keyCrashSev;
        public int? keyCrashSevDtl;
        public int? keyCrashType;
        public string crashSeverity;
        public string crashSeverityDetailed;
        public string crashType;
        public string crashTypeDetail;
        public string lightCond;
        public string weatherCond;
        public string county;
        public string city;
        public string streetName;
        public string intersectingStreet;
        public string isAlcoholRelated;
        public string isDistracted;
        public string isDrugRelated;
        public double? lat;
        public double? lng;
        public string offsetDir;
        public int? offsetFt;
        public int vehicleCount;
        public int nonmotoristCount;
        public int fatalityCount;
        public int injuryCount;
        public int totDmgAmt;
        public string agncyNm;
        public int agncyId;
        public int cntyCd;
        public int cityCd;
        public string crashTypeDir;
        public string crashRoadSurfCond;
        public string firstHarmfulEvent;
        public string bikeOrPed;
        public string bikePedCrashTypeName;
        public int bikeCount;
        public int pedCount;
        public int injuryNoneCount;
        public int injuryPossibleCount;
        public int injuryNonIncapacitatingCount;
        public int injuryIncapacitatingCount;
        public int injuryFatal30Count;
        public int injuryFatalNonTrafficCount;
    }
}
