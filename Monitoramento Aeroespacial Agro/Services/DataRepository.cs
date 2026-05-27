namespace Monitoramento_Aeroespacial_Agro.Services
{
    using global::Monitoramento_Aeroespacial_Agro.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    namespace Monitoramento_Aeroespacial_Agro.Services
    {
        public class DataRepository
        {
            private readonly List<SatelliteData> _spaceData = new();

            public void AddData(SatelliteData data) => _spaceData.Add(data);

            // Uso de LINQ para permitir que o usuário filtre pelo "espaço/satélite"
            public IEnumerable<SatelliteData> GetBySatellite(string satelliteId)
                => _spaceData.Where(d => d.SatelliteId.Equals(satelliteId, StringComparison.OrdinalIgnoreCase));

            public IEnumerable<SatelliteData> GetAllData() => _spaceData;
        }
    }
}
