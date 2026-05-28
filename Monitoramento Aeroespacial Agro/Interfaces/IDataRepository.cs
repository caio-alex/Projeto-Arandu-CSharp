using System;
using System.Collections.Generic;
using Monitoramento_Aeroespacial_Agro.Models;

namespace Monitoramento_Aeroespacial_Agro.Interfaces
{
    /// <summary>
    /// Contrato para o repositório de dados de satélite.
    /// Permite substituir por repositório com banco de dados sem alterar consumidores.
    /// </summary>
    public interface IDataRepository
    {
        void AddData(SatelliteData data);
        IEnumerable<SatelliteData> GetAllData();
        IEnumerable<SatelliteData> GetBySatellite(string satelliteId);
        IEnumerable<SatelliteData> GetByRegiao(string regiao);
        IEnumerable<SatelliteData> GetByDateRange(DateTime start, DateTime end);
        IEnumerable<string> GetSatelitesDisponiveis();
        IEnumerable<string> GetRegioesDisponiveis();
    }
}