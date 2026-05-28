using System;
using Monitoramento_Aeroespacial_Agro.Interfaces;
using Monitoramento_Aeroespacial_Agro.Models;

namespace Monitoramento_Aeroespacial_Agro.Services
{
    public class ConsoleAlertService : IAlertService
    {
        public void EmitAlert(SatelliteData data, string message)
        {
            Console.ForegroundColor = ResolveColor(message);
            Console.WriteLine($"[{data.CaptureDate:dd/MM/yyyy HH:mm}] Satélite {data.SatelliteId} em {data.Location}: {message}");
            Console.ResetColor();
        }

        private static ConsoleColor ResolveColor(string message)
        {
            if (message.Contains("CRÍTICO")) return ConsoleColor.Red;
            if (message.Contains("ATENÇÃO")) return ConsoleColor.Yellow;
            return ConsoleColor.Green;
        }
    }
}