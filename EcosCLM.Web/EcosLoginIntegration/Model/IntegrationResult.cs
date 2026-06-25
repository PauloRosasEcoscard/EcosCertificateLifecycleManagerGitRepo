namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class IntegrationResult<T>
    {
        public T? Data { get; set; }
        public bool IsSuccessful { get; set; }
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }

        public static IntegrationResult<T> Success(T data, int statusCode) =>
            new() { Data = data, IsSuccessful = true, StatusCode = statusCode };

        public static IntegrationResult<T> Failure(int statusCode, string errorMessage) =>
            new() { IsSuccessful = false, StatusCode = statusCode, ErrorMessage = errorMessage };
    }
}
