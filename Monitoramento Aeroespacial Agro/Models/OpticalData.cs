using System;

namespace Monitoramento_Aeroespacial_Agro.Models
{
    public class OpticalData : SatelliteData
    {
        public double RedBand { get; }
        public double NirBand { get; }

        // Propriedade calculada dinamicamente (Encapsulamento e Regra de Negócio)
        public double NDVI => (NirBand + RedBand) == 0 ? 0 : (NirBand - RedBand) / (NirBand + RedBand);

        public OpticalData(string id, GeoCoordinate loc, DateTime date, double redBand, double nirBand)
            : base(id, loc, date)
        {
            RedBand = redBand;
            NirBand = nirBand;
        }

        public override string AnalyzeCropState()
        {
            var ndvi = NDVI; // Chama o cálculo

            if (ndvi < 0.2) return "ALERTA CRÍTICO: Possível solo exposto ou degradação severa.";
            if (ndvi >= 0.2 && ndvi < 0.5) return "ATENÇÃO: Cultura em estágio inicial ou leve estresse.";

            return "OK: Lavoura saudável com alta biomassa.";
        }
    }
}