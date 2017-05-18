using System.Collections.Generic;

namespace S4Analytics
{
    public class CoordinateSystem_Server
    {
        public string Type { get; set; }
        public int Srid { get; set; }
    }

    public class ServerOptions
    {
        public string WarehouseSchema { get; set; }
        public string SpatialSchema { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }
        public string WarehouseConnStr { get { return ConnectionStrings[WarehouseSchema]; } }
        public Dictionary<string, CoordinateSystem_Server> CoordinateSystems { get; set; }
    }
}
