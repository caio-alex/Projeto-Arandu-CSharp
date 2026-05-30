namespace Monitoramento_Aeroespacial_Agro.Models
{
    /// <summary>
    /// Parte 3 da classe parcial OpticalData — Formatação e Relatórios.
    /// Responsabilidade única: formatar os dados para exibição.
    /// Se o formato mudar, apenas este arquivo precisa ser alterado.
    /// </summary>
    public partial class OpticalData
    {
        public string GetDetailedReport() =>
            $"NDVI: {NDVI:F4} | Red: {RedBand:F4} | NIR: {NirBand:F4}";
    }
}