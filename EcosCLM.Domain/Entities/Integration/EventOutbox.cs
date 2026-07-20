namespace EcosCLM.Domain.Entities.Integration
{
    /// <summary>
    /// Implementa o padrão Outbox para garantir a consistência eventual em processos assíncronos.
    /// Armazena eventos que devem ser disparados para outros sistemas ou serviços
    /// após uma transação de banco ser finalizada com sucesso.
    /// </summary>
    public class EventOutbox
    {
        /// <summary>Identificador único do evento na fila.</summary>
        public Guid Id { get; set; }

        /// <summary>Tipo do evento (ex: 'CertificateIssued', 'RenewalFailed').</summary>
        public string EventType { get; set; } = string.Empty;

        /// <summary>Payload do evento em formato JSON, contendo os dados necessários para o processamento.</summary>
        public string PayloadJson { get; set; } = "{}";

        /// <summary>Status atual do processamento: 'PENDING', 'PROCESSED' ou 'FAILED'.</summary>
        public string Status { get; set; } = "PENDING";

        /// <summary>Contador de tentativas de reenvio, útil para políticas de retry exponencial.</summary>
        public int Retries { get; set; }

        /// <summary>Data e hora em que o evento foi registrado.</summary>
        public DateTime CreatedAt { get; set; }
    }
}