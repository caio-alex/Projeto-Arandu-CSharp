using System;
using Monitoramento_Aeroespacial_Agro.Models;

namespace Monitoramento_Aeroespacial_Agro.Models
{
    /// <summary>
    /// Parte 1 da classe parcial OpticalData.
    /// Contém propriedades, construtor e lógica de cálculo dos índices espectrais.
    /// Uso de 'partial' permite separar propriedades/dados da lógica de análise.
    /// </summary>
    public partial class OpticalData : SatelliteData
    {
        // Bandas espectrais brutas capturadas pelo sensor óptico
        public double RedBand { get; }   // Banda do vermelho (reflectância)
        public double NirBand { get; }   // Banda do infravermelho próximo

        // NDVI calculado dinamicamente: (NIR - Red) / (NIR + Red)
        // Encapsula a regra de negócio; evita divisão por zero
        public double NDVI =>
            (NirBand + RedBand) == 0 ? 0
            : (NirBand - RedBand) / (NirBand + RedBand);

        // NDWI calculado dinamicamente: (Green - NIR) / (Green + NIR)
        // Aproximação usando RedBand como proxy de Green quando não há banda verde
        public double NDWI =>
            (RedBand + NirBand) == 0 ? 0
            : (RedBand - NirBand) / (RedBand + NirBand);

        public OpticalData(string id, GeoCoordinate loc, DateTime date,
                           double redBand, double nirBand)
            : base(id, loc, date)
        {
            RedBand = redBand;
            NirBand = nirBand;
        }
    }
}