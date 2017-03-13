using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Identity;


namespace S4Analytics.Models
{
    public class Agency
    {
        #region Public Properties

        public string AgencyName { get; set; }
        public AgencyType AgencyTypeCd { get; set; }
        public int AgencyId { get; set; }
        public int ParentAgencyId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<UserCounty> DefaultViewableCounties { get; set; }
        public CrashReportAccess CrashReportAccess { get; set; }
        public string EmailDomain { get; set; }

        public bool IsLawEnforcementAgency
        {
            get
            {
                switch (AgencyTypeCd)
                {
                    case AgencyType.FHP:
                    case AgencyType.PoliceDept:
                    case AgencyType.SheriffsOffice:
                        return true;
                    default:
                        return false;
                }
            }
            set { }
        }

        #endregion

        // Default constructor
        public Agency()
        {

            //TODO
            IsActive = true;
            CrashReportAccess = CrashReportAccess.Unknown;
            DefaultViewableCounties = new List<UserCounty>();

        }

        //public Agency(string agencyNm, AgencyType agencyTypeCd, int agencyId)
        //{

        //    AgencyName = agencyNm;
        //    AgencyTypeCd = agencyTypeCd;
        //    AgencyId = agencyId;
        //    //TODO
        //    IsActive = true;
        //    CrashReportAccess = CrashReportAccess.Unknown;

        //    var connection = new OracleConnection(_warehouseConnStr);
        //    using (connection)
        //    {
        //        connection.Open();
        //        SetDefaultEditableAndViewableCounties(connection);
        //    }
        //}

        //public Agency(string agencyNm, AgencyType agencyTypeCd, int agencyId, OracleConnection connection, string schema)
        //{

        //    AgencyName = agencyNm;
        //    AgencyTypeCd = agencyTypeCd;
        //    AgencyId = agencyId;
        //    //TODO
        //    IsActive = true;
        //    CrashReportAccess = CrashReportAccess.Unknown;

        //    SetDefaultEditableAndViewableCounties(connection);
        //}

        ///// <summary>
        ///// Get all the counties associated with this county
        ///// </summary>
        //private void SetDefaultEditableAndViewableCounties(OracleConnection connection)
        //{
        //    DefaultViewableCounties = new List<UserCounty>();

        //    var selectText = string.Format(
        //        "SELECT CC.CNTY_NM, AC.CNTY_CD, AC.CAN_EDIT FROM {0}.AGNCY_CNTY AC " +
        //        "JOIN {0}.CNTY_CITY CC ON CC.CNTY_CD = AC.CNTY_CD AND CC.CITY_CD = 0 " +
        //        "WHERE AC.AGNCY_ID = :agId", _warehouseSchema);

        //    using (var selectCmd = new OracleCommand(selectText, connection) { BindByName = true })
        //    {
        //        selectCmd.Parameters.Add(":agId", OracleDbType.Decimal).Value = AgencyId;
        //        using (var reader = selectCmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var county = new UserCounty();
        //                DefaultViewableCounties.Add(county);
        //                if (!reader.IsDBNull(0)) { county.CntyNm = reader.GetString(0); }
        //                if (!reader.IsDBNull(1)) { county.CntyCd = decimal.ToInt32(reader.GetDecimal(1)); }
        //                // Editable counties
        //                if (!reader.IsDBNull(2))
        //                {
        //                    if (reader.GetString(2) == "Y")
        //                    {
        //                        county.CanEdit = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        public List<UserCounty> GetDefaultEditableCounties()
        {
            var _editableCounties = new List<UserCounty>();
            if (DefaultViewableCounties != null)
            {
                _editableCounties.AddRange(DefaultViewableCounties.Where(cnty => cnty.CanEdit));
            }
            return _editableCounties;
        }
    }


}