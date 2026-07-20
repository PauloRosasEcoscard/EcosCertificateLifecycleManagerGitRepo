namespace EcosCLM.Application.Interfaces
{
    public interface IConfigurationService
    {
        public Task<int> GetSessionTimeoutMinutesAsync(string customerName);
    }
}
