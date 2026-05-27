using Monitoramento_Aeroespacial_Agro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoramento_Aeroespacial_Agro.Interfaces
{
    // Interfaces/IAlertService.cs
    public interface IAlertService
    {
        void EmitAlert(SatelliteData data, string message);
    }

    // Services/ConsoleAlertService.cs
    public class ConsoleAlertService : IAlertService
    {
        public void EmitAlert(SatelliteData data, string message)
        {
            Console.ForegroundColor = message.Contains("CRÍTICO") ? ConsoleColor.Red : ConsoleColor.Yellow;
            Console.WriteLine($"[{data.CaptureDate:dd/MM/yyyy HH:mm}] Satélite {data.SatelliteId} em {data.Location}: {message}");
            Console.ResetColor();
        }
    }
}
