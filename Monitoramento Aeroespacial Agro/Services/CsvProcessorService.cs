using Monitoramento_Aeroespacial_Agro.Interfaces;
using Monitoramento_Aeroespacial_Agro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monitoramento_Aeroespacial_Agro.Exceptions;

namespace Monitoramento_Aeroespacial_Agro.Services
{
    public partial class CsvProcessor : IDataLoader
    {
        private readonly IDataRepository _repository;

        // 2. O construtor recebe o repositório do Program.cs e salva na variável acima
        public CsvProcessor(IDataRepository repository)
        {
            _repository = repository;
        }

        // Atualize o método LoadData no CsvProcessorService.cs
        public void LoadData(string filePath)
        {
            // Validação de entrada: lança exceção se o arquivo não existir
            if (!File.Exists(filePath))
            {
                throw new SpaceDataException($"ALERTA CRÍTICO: O arquivo de dados do satélite não foi encontrado em '{filePath}'");
            }

            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines.Skip(1))
                {
                    try
                    {
                        var parts = line.Split(',');
                        var id = parts[0];
                        var lat = double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                        var lon = double.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                        var date = DateTime.Parse(parts[3]);

                        var redBand = double.Parse(parts[4], System.Globalization.CultureInfo.InvariantCulture);
                        var nirBand = double.Parse(parts[5], System.Globalization.CultureInfo.InvariantCulture);

                        var coord = new GeoCoordinate(lat, lon);
                        _repository.AddData(new OpticalData(id, coord, date, redBand, nirBand));
                    }
                    catch (Exception ex) when (ex is FormatException || ex is IndexOutOfRangeException)
                    {
                        // Tratamento local: O sistema espacial não pode quebrar por causa de 1 linha corrompida.
                        // Ele ignora o erro, avisa no console e continua processando o resto do arquivo.
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[AVISO] Linha de dados ignorada devido a ruído de telemetria: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }
            catch (IOException ex)
            {
                // Se der problema no disco/permissão, encapsula o erro técnico na nossa exceção de negócio
                throw new SpaceDataException("Falha de hardware ao tentar ler o arquivo de dados espaciais.", ex);
            }
        }
    }  
}
