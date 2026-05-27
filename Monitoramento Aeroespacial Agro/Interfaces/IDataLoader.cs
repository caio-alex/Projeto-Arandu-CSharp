namespace Monitoramento_Aeroespacial_Agro.Interfaces
{
    /// <summary>
    /// Contrato para qualquer serviço que carregue dados de uma fonte externa.
    /// Permite trocar CSV por banco de dados ou API sem alterar o Program.cs.
    /// </summary>
    public interface IDataLoader
    {
        void LoadData(string source);
    }
}