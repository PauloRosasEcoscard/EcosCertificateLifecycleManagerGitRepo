namespace EcosCLM.Application.Interfaces
{
    public interface IConfigurationService
    {
        Task<int> GetSessionTimeoutMinutesAsync(string customerName);
    }
}
