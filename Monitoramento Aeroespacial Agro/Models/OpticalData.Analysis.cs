namespace Monitoramento_Aeroespacial_Agro.Models
{
    /// <summary>
    /// Classe estática com os limiares do NDVI.
    /// Centraliza as constantes — elimina magic numbers em todo o sistema.
    /// </summary>
    public static class NdviThresholds
    {
        public const double Critico = 0.2;
        public const double Atencao = 0.5;
    }

    /// <summary>
    /// Parte 2 da classe parcial OpticalData — Lógica de Análise Agronômica.
    /// Responsabilidade única: classificar o estado da lavoura pelo NDVI.
    /// </summary>
    public partial class OpticalData
    {
        public override string AnalyzeCropState() => ClassifyByNdvi(NDVI);

        private static string ClassifyByNdvi(double ndvi)
        {
            if (ndvi < NdviThresholds.Critico)
                return "ALERTA CRÍTICO: Possível solo exposto ou degradação severa da lavoura.";

            if (ndvi < NdviThresholds.Atencao)
                return "ATENÇÃO: Cultura em estágio inicial ou sob estresse hídrico/nutricional.";

            return "OK: Lavoura saudável com alta biomassa detectada.";
        }
    }
}