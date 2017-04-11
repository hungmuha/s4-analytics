using System;
using System.Collections.Generic;

namespace S4Analytics.Models
{
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

    public class Coordinates
    {
        public double x;
        public double y;

        public bool IsValid {
            get {
                return x != 0 && y != 0;
            }
        }
    }

    public class Extent
    {
        public double minX;
        public double minY;
        public double maxX;
        public double maxY;

        public Extent(double minX, double minY, double maxX, double maxY)
        {
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }

        public bool IsValid {
            get {
                return minX != 0 && minY != 0 && maxX != 0 && maxY != 0;
            }
        }
    }

    public class IntersectionParameters
    {
        /// <summary>Intersection ID for intersection filter.</summary>
        public int intersectionId;

        /// <summary>Intersection offset in feet for intersection filter.</summary>
        public int offsetInFeet;

        /// <summary>List of intersection offset directions for intersection filter, given as a list of direction descriptors (North, South, East, West).</summary>
        public IList<string> offsetDirection;
    }

    public class StreetParameters
    {
        /// <summary>List of link IDs for street filter.</summary>
        public IList<int> linkIds;

        /// <summary>If true, also match crashes on cross streets within 100 feet.</summary>
        public bool includeCrossStreets;
    }

    public class NonAutoModesOfTravel
    {
        /// <summary>If true, match crashes with a pedestrian participant.</summary>
        public bool? pedestrian;

        /// <summary>If true, match crashes with a bicyclist participant.</summary>
        public bool? bicyclist;

        /// <summary>If true, match crashes with a moped participant.</summary>
        public bool? moped;

        /// <summary>If true, match crashes with a motorcycle participant.</summary>
        public bool? motorcycle;
    }

    public class SourcesOfTransport
    {
        /// <summary>If true, match crashes where a participant was transported by EMS.</summary>
        public bool? ems;

        /// <summary>If true, match crashes where a participant was transported by law enforcement.</summary>
        public bool? lawEnforcement;

        /// <summary>If true, match crashes where a participant was transported by means other than EMS or law enforcement.</summary>
        public bool? other;
    }

    public class BehavioralFactors
    {
        /// <summary>If true, match crashes where alcohol was involved.</summary>
        public bool? alcohol;

        /// <summary>If true, match crashes where drugs were involved.</summary>
        public bool? drugs;

        /// <summary>If true, match crashes where distraction was involved.</summary>
        public bool? distraction;

        /// <summary>If true, match crashes where aggressive driving was involved.</summary>
        public bool? aggressiveDriving;
    }

    public class CommonViolations
    {
        /// <summary>If true, match crashes where a speed violation was issued.</summary>
        public bool? speed;

        /// <summary>If true, match crashes where a red light violation was issued, or driver actions include "Ran Red Light".</summary>
        public bool? redLight;

        /// <summary>If true, match crashes where a right of way or failure to yield violation was issued.</summary>
        public bool? rightOfWay;

        /// <summary>If true, match crashes where a traffic control device violation was issued.</summary>
        public bool? trafficControlDevice;

        /// <summary>If true, match crashes where a careless driving violation was issued.</summary>
        public bool? carelessDriving;

        /// <summary>If true, match crashes where a DUI violation was issued.</summary>
        public bool? dui;
    }

    public class BikePedCrashType
    {
        /// <summary>List of bike/ped crash type IDs.</summary>
        public IList<int> bikePedCrashTypeIds;

        /// <summary>If true, match bike/ped crashes that have not been typed yet.</summary>
        public bool? includeUntyped;
    }

    public class LaneDepartures
    {
        /// <summary>If true, match all off-road crashes (per crash type).</summary>
        public bool? offRoadAll;

        /// <summary>If true, match all off-road rollover crashes (per crash type and harmful event).</summary>
        public bool? offRoadRollover;

        /// <summary>If true, match all off-road collision with fixed object crashes (per crash type and harmful event).</summary>
        public bool? offRoadCollisionWithFixedObject;

        /// <summary>If true, match all "crossed into oncoming traffic" crashes (per crash type).</summary>
        public bool? crossedIntoOncomingTraffic;

        /// <summary>If true, match all sideswipe crashes (per crash type).</summary>
        public bool? sideswipe;
    }

    public class OtherCircumstances
    {
        /// <summary>If true, match all hit and run crashes.</summary>
        public bool? hitAndRun;

        /// <summary>If true, match all school bus-related crashes.</summary>
        public bool? schoolBusRelated;

        /// <summary>If true, match all crashes within city limits.</summary>
        public bool? withinCityLimits;

        /// <summary>If true, match all crashes within an interchange.</summary>
        public bool? withinInterchange;

        /// <summary>If true, match all work zone-related crashes.</summary>
        public bool? workZoneRelated;

        /// <summary>If true, match all crashes involving workers in a work zone.</summary>
        public bool? workersInWorkZone;

        /// <summary>If true, match all crashes involving law enforcement in a work zone.</summary>
        public bool? lawEnforcementInWorkZone;
    }

    public class CrashQuery
    {
        /// <summary>Date range filter parameters.</summary>
        public DateRange dateRange;

        /// <summary>Days of the week filter, given as numbers (1 = Sunday, 2 = Monday, etc.).</summary>
        public IList<int> dayOfWeek;

        /// <summary>Time range filter parameters.</summary>
        public TimeRange timeRange;

        /// <summary>DOT district filter, given as DOT district codes.</summary>
        public IList<int> dotDistrict;

        /// <summary>MPO/TPO filter, given as MPO/TPO codes.</summary>
        public IList<int> mpoTpo;

        /// <summary>County filter, given as county codes.</summary>
        public IList<int> county;

        /// <summary>City filter, given as city codes.</summary>
        public IList<int> city;

        /// <summary>Custom area polygon filter, given as a list of coordinates.</summary>
        public IList<Coordinates> customArea;

        /// <summary>Custom extent filter, given as a two-point window.</summary>
        public Extent customExtent;

        /// <summary>Intersection filter parameters.</summary>
        public IntersectionParameters intersection;

        /// <summary>Street filter parameters.</summary>
        public StreetParameters street;

        /// <summary>Custom network filter, given as a list of segment IDs.</summary>
        public IList<int> customNetwork;

        /// <summary>Public road only filter (true = Enabled).</summary>
        public bool? publicRoadOnly;

        /// <summary>Form type filter, given as a list of strings (L = Long, S = Short).</summary>
        public IList<string> formType;

        /// <summary>Codeable filter (true = Enabled).</summary>
        public bool? codeableOnly;

        /// <summary>Reporting agency, given as a list of agency IDs.</summary>
        public IList<int> reportingAgency;

        /// <summary>Driver gender, given as a list of gender attribute codes.</summary>
        public IList<int> driverGender;

        /// <summary>Driver age range, given as a list of age range descriptions (Unknown, Under 15, 15 to 19, etc.).</summary>
        public IList<string> driverAgeRange;

        /// <summary>Pedestrian age range, given as a list of age range descriptions (Unknown, Under 5, 5 to 9, etc.).</summary>
        public IList<string> pedestrianAgeRange;

        /// <summary>Cyclist age range, given as a list of age range descriptions (Unknown, Under 5, 5 to 9, etc.).</summary>
        public IList<string> cyclistAgeRange;

        /// <summary>Collection of non-auto mode of travel sub-filters.</summary>
        public NonAutoModesOfTravel nonAutoModesOfTravel;

        /// <summary>Collection of source of transport sub-filters.</summary>
        public SourcesOfTransport sourcesOfTransport;

        /// <summary>Collection of behavioral factor sub-filters.</summary>
        public BehavioralFactors behavioralFactors;

        /// <summary>Collection of common violation sub-filters.</summary>
        public CommonViolations commonViolations;

        /// <summary>Vehicle type filter, given as a list of vehicle type IDs.</summary>
        public IList<int> vehicleType;

        /// <summary>Simple crash type filter, given as a list of simplified crash type descriptions.</summary>
        public IList<string> crashTypeSimple;

        /// <summary>Detailed crash type filter, given as a list of detailed crash type IDs.</summary>
        public IList<int> crashTypeDetailed;

        /// <summary>Bike/ped crash type filter parameters.</summary>
        public BikePedCrashType bikePedCrashType;

        /// <summary>CMV configuration filter, given as a list of CMV configuration IDs.</summary>
        public IList<int> cmvConfiguration;

        /// <summary>Environmental circumstance filter, given as a list of environmental circumstance IDs.</summary>
        public IList<int> environmentalCircumstance;

        /// <summary>Road circumstance filter, given as a list of road circumstance IDs.</summary>
        public IList<int> roadCircumstance;

        /// <summary>First harmful event filter, given as a list of first harmful event IDs.</summary>
        public IList<int> firstHarmfulEvent;

        /// <summary>Light condition filter, given as a list of light condition IDs.</summary>
        public IList<int> lightCondition;

        /// <summary>Road system identifier filter, given as a list of road system identifier IDs.</summary>
        public IList<int> roadSystemIdentifier;

        /// <summary>Weather condition filter, given as a list of weather condition IDs.</summary>
        public IList<int> weatherCondition;

        /// <summary>Collection of lane departure sub-filters.</summary>
        public LaneDepartures laneDepartures;

        /// <summary>Collection of other circumstance sub-filters.</summary>
        public OtherCircumstances otherCircumstances;
    }
}
