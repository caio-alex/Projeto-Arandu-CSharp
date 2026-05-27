using System;
using Monitoramento_Aeroespacial_Agro.Interfaces;
using Monitoramento_Aeroespacial_Agro.Models;

namespace Monitoramento_Aeroespacial_Agro.Services
{
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