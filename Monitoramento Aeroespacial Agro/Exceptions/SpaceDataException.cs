using System;

namespace Monitoramento_Aeroespacial_Agro.Exceptions
{
    /// <summary>
    /// Exceção de domínio para erros relacionados ao processamento de dados espaciais.
    /// Sistemas críticos não devem lançar Exception genérica — esta classe permite
    /// captura seletiva em camadas superiores.
    /// </summary>
    public class SpaceDataException : Exception
    {
        public string SatelliteId { get; }

        public SpaceDataException(string message)
            : base(message) { }

        public SpaceDataException(string message, string satelliteId)
            : base(message)
        {
            SatelliteId = satelliteId;
        }

        public SpaceDataException(string message, Exception inner)
            : base(message, inner) { }

        public SpaceDataException(string message, string satelliteId, Exception inner)
            : base(message, inner)
        {
            SatelliteId = satelliteId;
        }
    }
}