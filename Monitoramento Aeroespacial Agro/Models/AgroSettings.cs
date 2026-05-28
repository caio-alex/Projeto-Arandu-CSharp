// Novo arquivo: Models/AgroSettings.cs
namespace Monitoramento_Aeroespacial_Agro.Models
{
	public static class AgroSettings
	{
		// Uma configuração estática que pode ser usada em qualquer lugar do sistema
		public static double LimiteCriticoNDVI { get; set; } = 0.2;

		// Um método estático puro
		public static string GetSystemVersion()
		{
			return "Arandu - Global Solution";
		}
	}
}