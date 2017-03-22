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
        public IEnumerable<string> offsetDirection; // N(orth), S(outh), E(ast), W(est), U(nknown)
    }

    public class StreetParameters
    {
        public IEnumerable<int> linkIds;
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
        public bool? schoolBus;
        public bool? withinCityLimits;
        public bool? withinInterchange;
        public bool? workZone;
        public bool? workersInWorkZone;
        public bool? lawEnforcementInWorkZone;
    }

    public class CrashQuery
    {
        public DateRange dateRange;
        public IEnumerable<int> dayOfWeek;
        public TimeRange timeRange;
        public IEnumerable<int> dotDistrict;
        public IEnumerable<int> mpoTpo;
        public IEnumerable<int> county;
        public IEnumerable<int> city;
        public IEnumerable<Coordinates> customArea;
        public Extent customExtent;
        public IntersectionParameters intersection;
        public StreetParameters street;
        public IEnumerable<int> customNetwork;
        public bool? onPublicRoad;
        public IEnumerable<string> formType; // L(ong), S(hort)
        public bool? codeable;
        public IEnumerable<int> reportingAgency;
        public IEnumerable<string> driverGender; // M(ale), F(emale), U(nknown)
        public IEnumerable<string> driverAgeRange; // <15, 15-19, 20-24, ..., U(nknown)
        public IEnumerable<string> pedestrianAgeRange; // <5, 5-9, 10-14, ..., U(nknown)
        public IEnumerable<string> cyclistAgeRange; // <5, 5-9, 10-14, ..., U(nknown)
        public NonAutoModesOfTravel nonAutoModesOfTravel;
        public SourcesOfTransport sourcesOfTransport;
        public BehavioralFactors behavioralFactors;
        public CommonViolations commonViolations;
        public IEnumerable<int> vehicleType;
        public IEnumerable<int> crashTypeSimple;
        public IEnumerable<int> crashTypeDetailed;
        public IEnumerable<int> bikePedCrashType;
        public IEnumerable<int> cmvConfiguration;
        public IEnumerable<int> environmentalCircumstance;
        public IEnumerable<int> roadCircumstance;
        public IEnumerable<int> firstHarmfulEvent;
        public IEnumerable<int> lightCondition;
        public IEnumerable<int> roadSystemIdentifier;
        public IEnumerable<int> weatherCondition;
        public LaneDepartures laneDepartures;
        public OtherCircumstances otherCircumstances;
    }
}
