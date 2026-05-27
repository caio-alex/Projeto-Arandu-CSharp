using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoramento_Aeroespacial_Agro.Models
{
    // Models/SatelliteData.cs (Classe Abstrata)
    public abstract class SatelliteData
    {
        public string SatelliteId { get; set; }
        public GeoCoordinate Location { get; set; }
        public DateTime CaptureDate { get; set; } // Uso preciso de DateTime

        protected SatelliteData(string id, GeoCoordinate location, DateTime captureDate)
        {
            SatelliteId = id;
            Location = location;
            CaptureDate = captureDate;
        }

        // Método polimórfico
        public abstract string AnalyzeCropState();
    }
}
