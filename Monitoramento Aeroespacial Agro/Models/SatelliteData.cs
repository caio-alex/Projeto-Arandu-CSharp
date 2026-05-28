using System;

namespace Monitoramento_Aeroespacial_Agro.Models
{
    /// <summary>
    /// Classe abstrata que representa qualquer dado capturado por satélite.
    /// Não pode ser instanciada diretamente — força a existência de tipos
    /// concretos (OpticalData, RadarData, etc.) que implementam a análise.
    /// </summary>
    public abstract class SatelliteData
    {
        public string SatelliteId { get; protected set; }
        public string Regiao { get; protected set; }
        public GeoCoordinate Location { get; protected set; }
        public DateTime CaptureDate { get; protected set; }

        protected SatelliteData(string id, string regiao, GeoCoordinate location, DateTime captureDate)
        {
            SatelliteId = id;
            Regiao = regiao;
            Location = location;
            CaptureDate = captureDate;
        }

        // Método polimórfico: cada sensor analisa à sua maneira
        public abstract string AnalyzeCropState();

        // Método concreto compartilhado por todas as subclasses
        public string GetFormattedDate() =>
            CaptureDate.ToString("dd/MM/yyyy HH:mm:ss");
    }
}