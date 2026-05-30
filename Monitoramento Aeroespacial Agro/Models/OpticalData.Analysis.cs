using System;

namespace Monitoramento_Aeroespacial_Agro.Models
{
    /// <summary>
    /// Classe estática utilitária para os limiares do índice NDVI.
    /// Centraliza as constantes em um único lugar — elimina magic numbers
    /// e garante que qualquer alteração nos critérios afete todo o sistema.
    /// </summary>
    public static class NdviThresholds
    {
        public const double Critico = 0.2; // Abaixo: solo exposto ou degradação severa
        public const double Atencao = 0.5; // Abaixo: estresse hídrico ou estágio inicial
    }

    /// <summary>
    /// Parte 2 da classe parcial OpticalData — Lógica de Análise Agronômica.
    /// Separada da parte 1 para isolar a regra de negócio dos dados brutos.
    /// Qualquer alteração nos critérios de classificação ocorre apenas aqui.
    /// </summary>
    public partial class OpticalData
    {
        // Implementação do método polimórfico herdado de SatelliteData.
        // Cada tipo de sensor (óptico, radar, etc.) analisa à sua maneira.
        public override string AnalyzeCropState() => ClassifyByNdvi(NDVI);

        // Método privado estático: não depende de estado de instância.
        // Usa NdviThresholds para evitar magic numbers espalhados no código.
        private static string ClassifyByNdvi(double ndvi)
        {
            if (ndvi < NdviThresholds.Critico)
                return "ALERTA CRÍTICO: Possível solo exposto ou degradação severa da lavoura.";

            if (ndvi < NdviThresholds.Atencao)
                return "ATENÇÃO: Cultura em estágio inicial ou sob estresse hídrico/nutricional.";

            return "OK: Lavoura saudável com alta biomassa detectada.";
        }

        // Método auxiliar para exibição dos índices espectrais calculados
        public string GetDetailedReport() =>
            $"NDVI: {NDVI:F4} | Red: {RedBand:F4} | NIR: {NirBand:F4}";
    }
}