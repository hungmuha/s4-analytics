using System.Collections.Generic;

namespace S4Analytics.Models
{
    public class ServerOptions
    {
        public string MembershipSchema { get; set; }
        public string WarehouseSchema { get; set; }
        public string IdentitySchema { get; set; }
        public string SpatialSchema { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }
        public string MembershipConnStr { get { return ConnectionStrings[MembershipSchema]; } }
        public string WarehouseConnStr { get { return ConnectionStrings[WarehouseSchema]; } }
        public string IdentityConnStr { get { return ConnectionStrings[IdentitySchema]; } }
        public Dictionary<string, CoordinateSystem_Server> CoordinateSystems { get; set; }
        public EmailOptions EmailOptions { get; set;}
        public ContractShareOptions ContractShare { get; set; }
        public string MembershipApplicationName { get; set; }
    }

    public class ContractShareOptions
    {
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class CoordinateSystem_Server
    {
        public string Type { get; set; }
        public int Srid { get; set; }
    }

    public class EmailOptions
    {
        public string SmtpServer { get; set; }
        public string PrimaryDomain { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
        public string GlobalAdminEmail { get; set; }
        public string SupportEmail { get; set; }
    }
}
