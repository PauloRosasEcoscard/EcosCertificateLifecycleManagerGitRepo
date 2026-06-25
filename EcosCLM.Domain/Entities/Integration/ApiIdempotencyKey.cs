namespace EcosCLM.Domain.Entities.Integration
{
    /// <summary>
    /// Controla a idempotência de requisições enviadas à API do Ecos CLM.
    /// Garante que uma mesma requisição não seja processada múltiplas vezes acidentalmente,
    /// armazenando a chave única e a resposta original da primeira execução.
    /// </summary>
    public class ApiIdempotencyKey
    {
        /// <summary>Identificador único do registro de controle.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) dono desta requisição.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>A chave hash única (IDempotency-Key) enviada no header da requisição.</summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>O corpo da resposta JSON original, servindo para retornar o mesmo resultado em caso de reenvio.</summary>
        public string ResponseJson { get; set; } = "{}";

        /// <summary>Data e hora de expiração desta chave (após este tempo, o sistema volta a aceitar requisições novas).</summary>
        public DateTime ExpiresAt { get; set; }
    }
}
