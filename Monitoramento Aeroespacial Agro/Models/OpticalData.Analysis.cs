using System;

namespace Monitoramento_Aeroespacial_Agro.Models
{
    public partial class OpticalData
    {
        public override string AnalyzeCropState()
        {
            return ClassifyByNdvi(NDVI);
        }

        private static string ClassifyByNdvi(double ndvi)
        {
            if (ndvi < 0.2)
                return "ALERTA CRÍTICO: Possível solo exposto ou degradação severa da lavoura.";

            if (ndvi < 0.5)
                return "ATENÇÃO: Cultura em estágio inicial ou sob estresse hídrico/nutricional.";

            return "OK: Lavoura saudável com alta biomassa detectada.";
        }

        // Ajustado para imprimir apenas os dados que possuímos na memória
        public string GetDetailedReport() =>
            $"NDVI: {NDVI:F4} | Red: {RedBand:F4} | NIR: {NirBand:F4}";
    }
}