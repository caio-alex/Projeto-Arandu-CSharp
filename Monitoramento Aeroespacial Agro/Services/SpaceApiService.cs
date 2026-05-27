using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Monitoramento_Aeroespacial_Agro.Models;

namespace Monitoramento_Aeroespacial_Agro.Services
{
    public class SpaceApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "SUA_CHAVE_DE_API_AQUI"; // Chave gerada no site da API

        public SpaceApiService()
        {
            _httpClient = new HttpClient();
            // URL base fictícia para o exemplo
            _httpClient.BaseAddress = new Uri("https://api.agromonitoring.com/");
        }

        // Método assíncrono para não travar o console enquanto a internet responde
        public async Task<OpticalData> GetCurrentDataAsync(string polyId, GeoCoordinate coord)
        {
            // Monta a requisição pedindo os dados daquele polígono (fazenda)
            var response = await _httpClient.GetAsync($"agro/1.0/ndvi/history?polyid={polyId}&appid={_apiKey}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                // Aqui você usaria o JsonSerializer para converter o texto JSON em um objeto C#
                // Abaixo é um retorno simulado de como o dado entraria no seu sistema

                // Exemplo: extraindo bandas do JSON (pseudo-código da desserialização)
                double redBandFromApi = 0.08;
                double nirBandFromApi = 0.75;

                return new OpticalData($"SAT-API-{polyId}", coord, DateTime.Now, redBandFromApi, nirBandFromApi);
            }

            throw new Exception($"Erro de comunicação com o satélite. Status: {response.StatusCode}");
        }
    }
}