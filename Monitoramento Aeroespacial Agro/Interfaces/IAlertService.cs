using Monitoramento_Aeroespacial_Agro.Models;

namespace Monitoramento_Aeroespacial_Agro.Interfaces
{
    /// <summary>
    /// Contrato para qualquer serviço de alertas do sistema.
    /// Desacopla o emissor do canal de saída (console, e-mail, SMS, etc.),
    /// permitindo substituição sem alterar o código cliente.
    /// </summary>
    public interface IAlertService
    {
        void EmitAlert(SatelliteData data, string message);
    }
}