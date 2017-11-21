namespace S4Analytics.Models
{
    public class CrashesOverTimeSeverity
    {
        public bool propertyDamageOnly;
        public bool injury;
        public bool fatality;
    }

    public class CrashesOverTimeImpairment
    {
        public bool alcoholRelated;
        public bool drugRelated;
    }

    public class CrashesOverTimeBikePedRelated
    {
        public bool bikeRelated;
        public bool pedRelated;
    }

    public class CrashesOverTimeFormType
    {
        public bool longForm;
        public bool shortForm;
    }

    public class CrashesOverTimeQuery
    {
        public int? geographyId;
        public int? reportingAgencyId;
        public CrashesOverTimeSeverity severity;
        public CrashesOverTimeImpairment impairment;
        public CrashesOverTimeBikePedRelated bikePedRelated;
        public bool? cmvRelated;
        public bool? codeable;
        public CrashesOverTimeFormType formType;
    }
}
