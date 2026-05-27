using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoramento_Aeroespacial_Agro.Models
{
    // Models/GeoCoordinate.cs (Uso de Struct)
    public struct GeoCoordinate
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GeoCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
        public override string ToString() => $"[{Latitude:F4}, {Longitude:F4}]";
    }

    // Exceptions/SpaceDataException.cs (Tratamento de Exceções)
    public class SpaceDataException : Exception
    {
        public SpaceDataException(string message) : base(message) { }
        public SpaceDataException(string message, Exception inner) : base(message, inner) { }
    }
}
