using System.Collections.Generic;

namespace S4Analytics.Models
{
    public class ServerOptions
    {
        public OracleSchemaNames OracleSchemas { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }
        public string FlatConnStr { get { return ConnectionStrings[OracleSchemas.Flat]; } }
        public string IdentityConnStr { get { return ConnectionStrings[OracleSchemas.Identity]; } }
        public string MembershipConnStr { get { return ConnectionStrings[OracleSchemas.Membership]; } }
        public Dictionary<string, CoordinateSystem_Server> CoordinateSystems { get; set; }
        public EmailOptions EmailOptions { get; set;}
        public ContractShareOptions ContractShare { get; set; }
        public string MembershipApplicationName { get; set; }
        public string NewUserDocumentsUrl { get; set; }
        public string WebinarUrl { get; set; }
    }

    public class OracleSchemaNames
    {
        public string Flat { get; set; }
        public string Identity { get; set; }
        public string Membership { get; set; }
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
