using Monitoramento_Aeroespacial_Agro.Interfaces;
using Monitoramento_Aeroespacial_Agro.Models;
using Monitoramento_Aeroespacial_Agro.Services.Monitoramento_Aeroespacial_Agro.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoramento_Aeroespacial_Agro.Services
{
    // Services/CsvProcessor.cs
    public class CsvProcessor
    {
        private readonly DataRepository _repository;

        // 2. O construtor recebe o repositório do Program.cs e salva na variável acima
        public CsvProcessor(DataRepository repository)
        {
            _repository = repository;
        }

        public void LoadData(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(',');
                var id = parts[0];
                var lat = double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                var lon = double.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                var date = DateTime.Parse(parts[3]);

                // Lendo dados brutos das bandas em vez do índice pronto
                var redBand = double.Parse(parts[4], System.Globalization.CultureInfo.InvariantCulture);
                var nirBand = double.Parse(parts[5], System.Globalization.CultureInfo.InvariantCulture);

                var coord = new GeoCoordinate(lat, lon);
                _repository.AddData(new OpticalData(id, coord, date, redBand, nirBand));
            }
        }
    }  
}
