using System;

namespace Monitoramento_Aeroespacial_Agro.Models
{
    // Struct: tipo de valor imutável, ideal para coordenadas geográficas simples
    public struct GeoCoordinate
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GeoCoordinate(double latitude, double longitude)
        {
            // Validação dentro do próprio struct garante consistência
            if (latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException(nameof(latitude),
                    $"Latitude inválida: {latitude}. Deve estar entre -90 e 90.");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException(nameof(longitude),
                    $"Longitude inválida: {longitude}. Deve estar entre -180 e 180.");

            Latitude = latitude;
            Longitude = longitude;
        }

        public override string ToString() => $"[Lat: {Latitude:F4}, Lon: {Longitude:F4}]";
    }
}