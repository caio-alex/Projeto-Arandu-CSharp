using System;

namespace Monitoramento_Aeroespacial_Agro.Models
{
    /// <summary>
    /// Parte 2 da classe parcial OpticalData.
    /// Contém exclusivamente a lógica de análise agronômica dos índices espectrais,
    /// separando responsabilidades dentro do mesmo tipo.
    /// </summary>
    public partial class OpticalData
    {
        // Implementação do método polimórfico herdado de SatelliteData
        public override string AnalyzeCropState()
        {
            return ClassifyByNdvi(NDVI);
        }

        // Método privado estático: não depende de estado de instância
        // e pode ser reutilizado sem criar um objeto
        private static string ClassifyByNdvi(double ndvi)
        {
            if (ndvi < 0.2)
                return "ALERTA CRÍTICO: Possível solo exposto ou degradação severa da lavoura.";

            if (ndvi < 0.5)
                return "ATENÇÃO: Cultura em estágio inicial ou sob estresse hídrico/nutricional.";

            return "OK: Lavoura saudável com alta biomassa detectada.";
        }

        // Método auxiliar público para exibição detalhada dos índices
        public string GetDetailedReport() =>
            $"NDVI: {NDVI:F4} | NDWI: {NDWI:F4} | Red: {RedBand:F4} | NIR: {NirBand:F4}";
    }
}