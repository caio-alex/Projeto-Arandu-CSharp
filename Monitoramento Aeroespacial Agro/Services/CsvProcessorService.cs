using Monitoramento_Aeroespacial_Agro.Exceptions;
using Monitoramento_Aeroespacial_Agro.Interfaces;
using Monitoramento_Aeroespacial_Agro.Models;
using System;
using System.IO;
using System.Linq;

namespace Monitoramento_Aeroespacial_Agro.Services
{
    public partial class CsvProcessor : IDataLoader
    {
        private readonly IDataRepository _repository;

        public CsvProcessor(IDataRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void LoadData(string filePath)
        {
            if (!File.Exists(filePath))
                throw new SpaceDataException($"ALERTA CRÍTICO: O arquivo de dados do satélite não foi encontrado em '{filePath}'");

            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines.Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        var parts = line.Split(',');

                        // CSV agora tem 7 colunas: Id, Regiao, Lat, Lon, Date, Red, Nir
                        if (parts.Length < 7)
                            throw new FormatException($"Esperados 7 campos, encontrados {parts.Length}.");

                        var id = parts[0].Trim();
                        var regiao = parts[1].Trim();
                        var lat = double.Parse(parts[2].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                        var lon = double.Parse(parts[3].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                        var date = DateTime.Parse(parts[4].Trim());
                        var red = double.Parse(parts[5].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                        var nir = double.Parse(parts[6].Trim(), System.Globalization.CultureInfo.InvariantCulture);

                        var coord = new GeoCoordinate(lat, lon);
                        _repository.AddData(new OpticalData(id, regiao, coord, date, red, nir));
                    }
                    catch (Exception ex) when (ex is FormatException || ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException)
                    {
                        // O sistema espacial não pode quebrar por causa de 1 linha corrompida.
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[AVISO] Linha de dados ignorada devido a ruído de telemetria: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }
            catch (IOException ex)
            {
                throw new SpaceDataException("Falha de hardware ao tentar ler o arquivo de dados espaciais.", ex);
            }
        }
    }
}