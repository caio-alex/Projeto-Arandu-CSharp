using Microsoft.Extensions.DependencyInjection;
using Monitoramento_Aeroespacial_Agro.Exceptions;
using Monitoramento_Aeroespacial_Agro.Interfaces;
using Monitoramento_Aeroespacial_Agro.Models;
using Monitoramento_Aeroespacial_Agro.Services;
using System;
using System.Linq;
namespace Monitoramento_Aeroespacial_Agro
{
    class Program
    {
        static void Main()
        {
            // ── Configuração do Container de Injeção de Dependência ──────────────
            // O container resolve automaticamente qual implementação usar para cada
            // interface. Para trocar ConsoleAlertService por EmailAlertService,
            // basta alterar o registro aqui — sem tocar nas classes consumidoras.
            var services = new ServiceCollection();
            services.AddSingleton<IDataRepository, DataRepository>();
            services.AddSingleton<IAlertService, ConsoleAlertService>();
            services.AddTransient<IDataLoader, CsvProcessor>();

            var provider = services.BuildServiceProvider();  // ← só depois dos registros

            // Resolução das dependências pelo container
            var repository = provider.GetRequiredService<IDataRepository>();
            var alertService = provider.GetRequiredService<IAlertService>();
            var dataLoader = provider.GetRequiredService<IDataLoader>();

            // ── Carregamento inicial com tratamento de exceção ───────────────────
            try
            {
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║   SISTEMA AEROESPACIAL DO AGRONEGÓCIO v1.0   ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("  Carregando base de dados espacial...");
                dataLoader.LoadData("Data/sat_data.csv");
                Console.WriteLine();
            }
            catch (SpaceDataException ex)
            {
                // Erro de domínio esperado: exibe mensagem clara e encerra
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  [ERRO CRÍTICO] Falha no carregamento: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("  O sistema não pode operar sem dados. Pressione qualquer tecla para sair.");
                Console.ReadKey();
                return;
            }

            // ── Loop principal do menu ───────────────────────────────────────────
            bool running = true;
            while (running)
            {
                ExibirMenu();
                var option = Console.ReadLine()?.Trim();

                switch (option)
                {
                    case "1":
                        ExecutarRelatorioGeral(repository, alertService);
                        break;

                    case "2":
                        ExecutarFiltroPorSatelite(repository, alertService);
                        break;

                    case "3":
                        ExecutarFiltroPorData(repository, alertService);
                        break;

                    case "4":
                        running = false;
                        Console.WriteLine("\n  Encerrando o sistema. Até logo.\n");
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("  Opção inválida. Tente novamente.\n");
                        Console.ResetColor();
                        break;
                }
            }
        }

        // ── Métodos privados estáticos: cada opção de menu em seu próprio método ─

        private static void ExibirMenu()
        {
            Console.WriteLine("=== PAINEL AEROESPACIAL DO AGRONEGÓCIO ===");
            Console.WriteLine("  1. Relatório geral de todos os setores");
            Console.WriteLine("  2. Filtrar por Satélite/Região");
            Console.WriteLine("  3. Filtrar por intervalo de datas");
            Console.WriteLine("  4. Sair");
            Console.Write("Escolha uma opção: ");
        }

        private static void ExecutarRelatorioGeral(IDataRepository repository, IAlertService alertService)
        {
            Console.Clear();
            Console.WriteLine("─── RELATÓRIO GERAL ───\n");

            var dados = repository.GetAllData().ToList();

            if (!dados.Any())
            {
                Console.WriteLine("  Nenhum dado disponível.\n");
            }
            else
            {
                foreach (var data in dados)
                    alertService.EmitAlert(data, data.AnalyzeCropState());
            }

            AguardarTecla();
        }

        private static void ExecutarFiltroPorSatelite(IDataRepository repository, IAlertService alertService)
        {
            Console.Write("\n  Digite o ID do Satélite (ex: SAT-AGRO-01): ");
            var satId = Console.ReadLine()?.Trim();

            Console.Clear();

            // Validação de entrada do usuário
            if (string.IsNullOrEmpty(satId))
            {
                Console.WriteLine("  ID de satélite não informado.\n");
                AguardarTecla();
                return;
            }

            var dados = repository.GetBySatellite(satId).ToList();

            if (!dados.Any())
            {
                Console.WriteLine($"  Nenhum dado encontrado para '{satId}'.\n");
            }
            else
            {
                Console.WriteLine($"─── RELATÓRIO: {satId.ToUpper()} ───\n");
                foreach (var data in dados)
                    alertService.EmitAlert(data, data.AnalyzeCropState());
            }

            AguardarTecla();
        }

        private static void ExecutarFiltroPorData(IDataRepository repository, IAlertService alertService)
        {
            Console.WriteLine();

            try
            {
                Console.Write("  Data inicial (dd/MM/yyyy): ");
                var startInput = Console.ReadLine()?.Trim();

                Console.Write("  Data final   (dd/MM/yyyy): ");
                var endInput = Console.ReadLine()?.Trim();

                // Captura específica: entrada de data mal formatada pelo usuário
                if (!DateTime.TryParseExact(startInput, "dd/MM/yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out DateTime start))
                    throw new FormatException($"Data inicial inválida: '{startInput}'");

                if (!DateTime.TryParseExact(endInput, "dd/MM/yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out DateTime end))
                    throw new FormatException($"Data final inválida: '{endInput}'");

                // Inclui o dia inteiro da data final
                end = end.AddDays(1).AddSeconds(-1);

                Console.Clear();
                var dados = repository.GetByDateRange(start, end).ToList();

                if (!dados.Any())
                {
                    Console.WriteLine($"  Nenhum dado encontrado entre {start:dd/MM/yyyy} e {end:dd/MM/yyyy}.\n");
                }
                else
                {
                    Console.WriteLine($"─── RELATÓRIO: {start:dd/MM/yyyy} até {end:dd/MM/yyyy} ───\n");
                    foreach (var data in dados)
                        alertService.EmitAlert(data, data.AnalyzeCropState());
                }
            }
            catch (FormatException ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n  [AVISO] {ex.Message}. Use o formato dd/MM/yyyy.\n");
                Console.ResetColor();
            }

            AguardarTecla();
        }

        private static void AguardarTecla()
        {
            Console.WriteLine("\n  Pressione qualquer tecla para voltar ao menu...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}