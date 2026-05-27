using Monitoramento_Aeroespacial_Agro.Models;
using Monitoramento_Aeroespacial_Agro.Services;
using Monitoramento_Aeroespacial_Agro.Services.Monitoramento_Aeroespacial_Agro.Services;
using System;
using System.Linq;

namespace Monitoramento_Aeroespacial_Agro
{
    class Program
    {
        static void Main()
        {
            var repository = new DataRepository();
            var processor = new CsvProcessor(repository);
            var alertService = new ConsoleAlertService();

            Console.WriteLine("Carregando base de dados espacial...");
            processor.LoadData("Data/sat_data.csv");
            Console.WriteLine("Dados carregados com sucesso!\n");

            bool running = true;
            while (running)
            {
                Console.WriteLine("=== PAINEL AEROESPACIAL DO AGRONEGÓCIO ===");
                Console.WriteLine("1. Ver relatório geral de todos os setores");
                Console.WriteLine("2. Filtrar dados por Satélite/Região");
                Console.WriteLine("3. Sair");
                Console.Write("Escolha uma opção: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("--- RELATÓRIO GERAL ---");
                        foreach (var data in repository.GetAllData())
                        {
                            alertService.EmitAlert(data, data.AnalyzeCropState());
                        }
                        Console.WriteLine("\nPressione qualquer tecla para voltar...");
                        Console.ReadKey();
                        Console.Clear();
                        break;

                    case "2":
                        Console.Write("\nDigite o nome do Satélite (ex: SAT-MATOGROSSO, SAT-GOIAS): ");
                        var satId = Console.ReadLine();
                        var filteredData = repository.GetBySatellite(satId);

                        Console.Clear();
                        if (!filteredData.Any())
                        {
                            Console.WriteLine($"Nenhum dado encontrado para a região do {satId}.\n");
                        }
                        else
                        {
                            Console.WriteLine($"--- RELATÓRIO DA REGIÃO: {satId.ToUpper()} ---");
                            foreach (var data in filteredData)
                            {
                                alertService.EmitAlert(data, data.AnalyzeCropState());
                            }
                        }
                        Console.WriteLine("\nPressione qualquer tecla para voltar...");
                        Console.ReadKey();
                        Console.Clear();
                        break;

                    case "3":
                        running = false;
                        Console.WriteLine("Encerrando o sistema...");
                        break;

                    default:
                        Console.WriteLine("Opção inválida!\n");
                        break;
                }
            }
        }
    }
}