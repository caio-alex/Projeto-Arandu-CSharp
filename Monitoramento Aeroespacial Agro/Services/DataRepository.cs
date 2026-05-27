using System;
using System.Collections.Generic;
using System.Linq;
using Monitoramento_Aeroespacial_Agro.Interfaces;
using Monitoramento_Aeroespacial_Agro.Models;

namespace Monitoramento_Aeroespacial_Agro.Services
{
    /// <summary>
    /// Repositório em memória para dados de satélite.
    /// Implementa IDataRepository, permitindo substituição futura
    /// por uma versão com banco de dados sem alterar os consumidores.
    /// </summary>
    public class DataRepository : IDataRepository
    {
        private readonly List<SatelliteData> _spaceData = new();

        public void AddData(SatelliteData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Dado de satélite não pode ser nulo.");

            _spaceData.Add(data);
        }

        public IEnumerable<SatelliteData> GetAllData() =>
            _spaceData.AsReadOnly();

        // LINQ com comparação case-insensitive
        public IEnumerable<SatelliteData> GetBySatellite(string satelliteId) =>
            _spaceData.Where(d =>
                d.SatelliteId.Equals(satelliteId, StringComparison.OrdinalIgnoreCase));

        // Filtro por intervalo de datas — permite análise histórica
        public IEnumerable<SatelliteData> GetByDateRange(DateTime start, DateTime end) =>
            _spaceData.Where(d => d.CaptureDate >= start && d.CaptureDate <= end)
                      .OrderBy(d => d.CaptureDate);
    }
}