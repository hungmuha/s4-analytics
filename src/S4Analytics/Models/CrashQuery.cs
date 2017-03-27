using System;
using System.Collections.Generic;

namespace S4Analytics.Models
{
    public class Coordinates
    {
        public double x;
        public double y;
    }

    public class Extent
    {
        public Coordinates point1;
        public Coordinates point2;
    }

    public class DateRange
    {
        public DateTime startDate;
        public DateTime endDate;
    }

    public class TimeRange
    {
        public DateTime startTime;
        public DateTime endTime;
    }

    public class IntersectionParameters
    {
        public int intersectionId;
        public int offsetInFeet;
        public IList<string> offsetDirection; // N(orth), S(outh), E(ast), W(est)
    }

    public class StreetParameters
    {
        public IList<int> linkIds;
        public bool includeCrossStreets;
    }

    public class NonAutoModesOfTravel
    {
        public bool? pedestrian;
        public bool? bicyclist;
        public bool? moped;
        public bool? motorcycle;
    }

    public class SourcesOfTransport
    {
        public bool? ems;
        public bool? lawEnforcement;
        public bool? other;
    }

    public class BehavioralFactors
    {
        public bool? alcohol;
        public bool? drugs;
        public bool? distraction;
        public bool? aggressiveDriving;
    }

    public class CommonViolations
    {
        public bool? speed;
        public bool? redLight;
        public bool? rightOfWay;
        public bool? trafficControlDevice;
        public bool? carelessDriving;
        public bool? dui;
    }

    public class LaneDepartures
    {
        public bool? offRoadAll;
        public bool? offRoadRollover;
        public bool? offRoadCollisionWithFixedObject;
        public bool? crossedIntoOncomingTraffic;
        public bool? sideswipe;
    }

    public class OtherCircumstances
    {
        public bool? hitAndRun;
        public bool? schoolBusRelated;
        public bool? withinCityLimits;
        public bool? withinInterchange;
        public bool? workZoneRelated;
        public bool? workersInWorkZone;
        public bool? lawEnforcementInWorkZone;
    }

    public class CrashQuery
    {
        // TODO: add inline docs
        public DateRange dateRange;
        public IList<int> dayOfWeek;
        public TimeRange timeRange;
        public IList<int> dotDistrict;
        public IList<int> mpoTpo;
        public IList<int> county;
        public IList<int> city;
        public IList<Coordinates> customArea;
        public Extent customExtent;
        public IntersectionParameters intersection;
        public StreetParameters street;
        public IList<int> customNetwork;
        public bool? onPublicRoad;
        public IList<string> formType; // L(ong), S(hort)
        public bool? codeable;
        public IList<int> reportingAgency;
        public IList<int> driverGender;
        public IList<string> driverAgeRange; // <15, 15-19, 20-24, ..., 90+, U(nknown)
        public IList<string> pedestrianAgeRange; // <5, 5-9, 10-14, ..., 75+, U(nknown)
        public IList<string> cyclistAgeRange; // <5, 5-9, 10-14, ..., 75+, U(nknown)
        public NonAutoModesOfTravel nonAutoModesOfTravel;
        public SourcesOfTransport sourcesOfTransport;
        public BehavioralFactors behavioralFactors;
        public CommonViolations commonViolations;
        public IList<int> vehicleType;
        public IList<string> crashTypeSimple;
        public IList<int> crashTypeDetailed;
        public IList<int> bikePedCrashType;
        public IList<int> cmvConfiguration;
        public IList<int> environmentalCircumstance;
        public IList<int> roadCircumstance;
        public IList<int> firstHarmfulEvent;
        public IList<int> lightCondition;
        public IList<int> roadSystemIdentifier;
        public IList<int> weatherCondition;
        public LaneDepartures laneDepartures;
        public OtherCircumstances otherCircumstances;
    }
}
