using Microsoft.Extensions.DependencyInjection;
using Monitoramento_Aeroespacial_Agro.Exceptions;
using Monitoramento_Aeroespacial_Agro.Interfaces;
using Monitoramento_Aeroespacial_Agro.Models;
using Monitoramento_Aeroespacial_Agro.Services;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Monitoramento_Aeroespacial_Agro
{
    class Program
    {
        static void Main()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IDataRepository, DataRepository>();
            services.AddSingleton<IAlertService, ConsoleAlertService>();
            services.AddTransient<IDataLoader, CsvProcessor>();

            var provider = services.BuildServiceProvider();

            var repository = provider.GetRequiredService<IDataRepository>();
            var alertService = provider.GetRequiredService<IAlertService>();
            var dataLoader = provider.GetRequiredService<IDataLoader>();

            Console.Clear();
            ExibirCabecalho();

            try
            {
                Console.WriteLine("  Carregando base de dados espacial...\n");
                dataLoader.LoadData("Data/sat_data.csv");
                Console.WriteLine();
            }
            catch (SpaceDataException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  [ERRO CRÍTICO] Falha no carregamento: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("  O sistema não pode operar sem dados. Pressione qualquer tecla para sair.");
                Console.ReadKey();
                return;
            }

            bool running = true;
            while (running)
            {
                ExibirMenu();
                var option = Console.ReadLine()?.Trim();

                switch (option)
                {
                    case "1": ExecutarRelatorioGeral(repository, alertService); break;
                    case "2": ExecutarFiltroPorSatelite(repository, alertService); break;
                    case "3": ExecutarFiltroPorRegiao(repository, alertService); break;
                    case "4": ExecutarFiltroPorData(repository, alertService); break;
                    case "5":
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

        // ── Opção 1: Relatório geral ──────────────────────────────────────────

        private static void ExecutarRelatorioGeral(IDataRepository repository, IAlertService alertService)
        {
            Console.Clear();
            ExibirTituloSecao("RELATÓRIO GERAL — TODOS OS SETORES");

            var dados = repository.GetAllData().ToList();
            if (!dados.Any())
            {
                Console.WriteLine("  Nenhum dado disponível.\n");
                AguardarTecla();
                return;
            }

            foreach (var data in dados)
                alertService.EmitAlert(data, data.AnalyzeCropState());

            ExibirResumoAlertas(dados);
            AguardarTecla();
        }

        // ── Opção 2: Filtrar por Satélite com lista numerada ──────────────────

        private static void ExecutarFiltroPorSatelite(IDataRepository repository, IAlertService alertService)
        {
            Console.Clear();
            ExibirTituloSecao("FILTRAR TELEMETRIA POR SATÉLITE");

            var satelites = repository.GetSatelitesDisponiveis().ToList();

            if (!satelites.Any())
            {
                Console.WriteLine("  Nenhum satélite disponível.\n");
                AguardarTecla();
                return;
            }

            // Exibe lista numerada com região e contagem de capturas
            Console.WriteLine("  Satélites disponíveis:\n");
            ExibirListaSatelites(satelites, repository);

            Console.Write("\n  Digite o número ou o ID do satélite: ");
            var input = Console.ReadLine()?.Trim();
            Console.Clear();

            var satId = ResolverEscolha(input, satelites);

            if (satId == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  Entrada inválida. Nenhum satélite selecionado.\n");
                Console.ResetColor();
                AguardarTecla();
                return;
            }

            var dados = repository.GetBySatellite(satId).ToList();
            ExibirTituloSecao($"SATÉLITE: {satId.ToUpper()}  —  {dados.First().Regiao}");

            foreach (var data in dados)
                alertService.EmitAlert(data, data.AnalyzeCropState());

            ExibirResumoAlertas(dados);
            AguardarTecla();
        }

        // ── Opção 3: Filtrar por Região com lista numerada ────────────────────

        private static void ExecutarFiltroPorRegiao(IDataRepository repository, IAlertService alertService)
        {
            Console.Clear();
            ExibirTituloSecao("FILTRAR TELEMETRIA POR REGIÃO / ESTADO");

            var regioes = repository.GetRegioesDisponiveis().ToList();

            if (!regioes.Any())
            {
                Console.WriteLine("  Nenhuma região disponível.\n");
                AguardarTecla();
                return;
            }

            // Exibe lista numerada com satélite associado e capturas
            Console.WriteLine("  Regiões monitoradas:\n");
            ExibirListaRegioes(regioes, repository);

            Console.Write("\n  Digite o número ou o nome da região: ");
            var input = Console.ReadLine()?.Trim();
            Console.Clear();

            var regiao = ResolverEscolha(input, regioes);

            if (regiao == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  Entrada inválida. Nenhuma região selecionada.\n");
                Console.ResetColor();
                AguardarTecla();
                return;
            }

            var dados = repository.GetByRegiao(regiao).ToList();
            ExibirTituloSecao($"REGIÃO: {regiao.ToUpper()}");

            foreach (var data in dados)
                alertService.EmitAlert(data, data.AnalyzeCropState());

            ExibirResumoAlertas(dados);
            AguardarTecla();
        }

        // ── Opção 4: Filtrar por data ─────────────────────────────────────────

        private static void ExecutarFiltroPorData(IDataRepository repository, IAlertService alertService)
        {
            Console.Clear();
            ExibirTituloSecao("FILTRAR POR INTERVALO DE DATAS");
            Console.WriteLine();

            try
            {
                Console.Write("  Data inicial (dd/MM/yyyy): ");
                var startInput = Console.ReadLine()?.Trim();

                Console.Write("  Data final   (dd/MM/yyyy): ");
                var endInput = Console.ReadLine()?.Trim();

                if (!DateTime.TryParseExact(startInput, "dd/MM/yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out DateTime start))
                    throw new FormatException($"Data inicial inválida: '{startInput}'");

                if (!DateTime.TryParseExact(endInput, "dd/MM/yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out DateTime end))
                    throw new FormatException($"Data final inválida: '{endInput}'");

                end = end.AddDays(1).AddSeconds(-1);
                Console.Clear();

                var dados = repository.GetByDateRange(start, end).ToList();

                if (!dados.Any())
                {
                    Console.WriteLine($"  Nenhum dado entre {start:dd/MM/yyyy} e {end:dd/MM/yyyy}.\n");
                }
                else
                {
                    ExibirTituloSecao($"PERÍODO: {start:dd/MM/yyyy} até {end:dd/MM/yyyy}");
                    foreach (var data in dados)
                        alertService.EmitAlert(data, data.AnalyzeCropState());
                    ExibirResumoAlertas(dados);
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

        // ── Helpers de listagem ───────────────────────────────────────────────

        private static void ExibirListaSatelites(List<string> satelites, IDataRepository repository)
        {
            var todos = repository.GetAllData().ToList();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  {"Nº",-4} {"Satélite",-14} {"Região",-26} {"Capturas",8}  Status");
            Console.WriteLine($"  {new string('─', 62)}");
            Console.ResetColor();

            for (int i = 0; i < satelites.Count; i++)
            {
                var sat = satelites[i];
                var capturas = todos.Where(d => d.SatelliteId == sat).ToList();
                var regiao = capturas.First().Regiao;
                var criticos = capturas.Count(d => d.AnalyzeCropState().Contains("CRÍTICO"));
                var atencao = capturas.Count(d => d.AnalyzeCropState().Contains("ATENÇÃO"));

                var (cor, status) = ResolverStatusCor(criticos, atencao);
                Console.ForegroundColor = cor;
                Console.WriteLine($"  [{i + 1,2}] {sat,-14} {regiao,-26} {capturas.Count,8}  {status}");
                Console.ResetColor();
            }
        }

        private static void ExibirListaRegioes(List<string> regioes, IDataRepository repository)
        {
            var todos = repository.GetAllData().ToList();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  {"Nº",-4} {"Região",-28} {"Satélite",-14} {"Capturas",8}  Status");
            Console.WriteLine($"  {new string('─', 64)}");
            Console.ResetColor();

            for (int i = 0; i < regioes.Count; i++)
            {
                var reg = regioes[i];
                var capturas = todos.Where(d => d.Regiao == reg).ToList();
                var sat = capturas.First().SatelliteId;
                var criticos = capturas.Count(d => d.AnalyzeCropState().Contains("CRÍTICO"));
                var atencao = capturas.Count(d => d.AnalyzeCropState().Contains("ATENÇÃO"));

                var (cor, status) = ResolverStatusCor(criticos, atencao);
                Console.ForegroundColor = cor;
                Console.WriteLine($"  [{i + 1,2}] {reg,-28} {sat,-14} {capturas.Count,8}  {status}");
                Console.ResetColor();
            }
        }

        // Resolve entrada do usuário: número da lista ou texto direto
        private static string? ResolverEscolha(string? input, List<string> opcoes)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            // Tentativa 1: número da lista
            if (int.TryParse(input, out int idx) && idx >= 1 && idx <= opcoes.Count)
                return opcoes[idx - 1];

            // Tentativa 2: texto digitado diretamente
            var match = opcoes.FirstOrDefault(o =>
                o.Equals(input, StringComparison.OrdinalIgnoreCase));

            return match;
        }

        private static (ConsoleColor cor, string status) ResolverStatusCor(int criticos, int atencao)
        {
            if (criticos > 0) return (ConsoleColor.Red, $"{criticos} CRÍTICO(S)");
            if (atencao > 0) return (ConsoleColor.Yellow, $"{atencao} ATENÇÃO");
            return (ConsoleColor.Green, "[ OK] Saudável");
        }

        // ── Helpers de UI ─────────────────────────────────────────────────────

        private static void ExibirResumoAlertas(List<SatelliteData> dados)
        {
            var criticos = dados.Count(d => d.AnalyzeCropState().Contains("CRÍTICO"));
            var atencao = dados.Count(d => d.AnalyzeCropState().Contains("ATENÇÃO"));
            var ok = dados.Count - criticos - atencao;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  RESUMO: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{ok} OK  ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{atencao} ATENÇÃO  ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{criticos} CRÍTICO");
            Console.ResetColor();
        }

        private static void ExibirCabecalho()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║     Arandu - SISTEMA AEROESPACIAL DO AGRONEGÓCIO     ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void ExibirMenu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ┌──────────────────────────────────────────────────────┐");
            Console.WriteLine("  │                MENU PRINCIPAL                        │");
            Console.WriteLine("  ├──────────────────────────────────────────────────────┤");
            Console.WriteLine("  │  1. Relatório geral de todos os setores              │");
            Console.WriteLine("  │  2. Filtrar telemetria por Satélite                  │");
            Console.WriteLine("  │  3. Filtrar telemetria por Região / Estado           │");
            Console.WriteLine("  │  4. Filtrar por intervalo de datas                   │");
            Console.WriteLine("  │  5. Sair                                             │");
            Console.WriteLine("  └──────────────────────────────────────────────────────┘");
            Console.ResetColor();
            Console.Write("  Escolha uma opção: ");
        }

        private static void ExibirTituloSecao(string titulo)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n  ══ {titulo} ══\n");
            Console.ResetColor();
        }

        private static void AguardarTecla()
        {
            Console.WriteLine("\n  Pressione qualquer tecla para voltar ao menu...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}