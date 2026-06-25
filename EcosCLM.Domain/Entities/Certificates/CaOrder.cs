using EcosCLM.Domain.Entities.Security;
namespace EcosCLM.Domain.Entities.Certificates
{
    /// <summary>
    /// Registra a ordem de emissão enviada para uma Autoridade Certificadora (CA) externa.
    /// Esta entidade serve como ponte de rastreabilidade, armazenando IDs externos e 
    /// status de sincronização entre o Ecos CLM e o provedor da CA.
    /// </summary>
    public class CaOrder
    {
        /// <summary>Identificador único interno da ordem.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) proprietário desta ordem.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>ID da solicitação original (CertificateRequest) que originou este pedido.</summary>
        public Guid RequestId { get; set; }

        /// <summary>ID da Autoridade Certificadora utilizada para esta emissão.</summary>
        public Guid CaId { get; set; }

        /// <summary>ID do pedido gerado dentro do sistema da CA externa (ex: número da ordem DigiCert).</summary>
        public string? ExternalOrderId { get; set; }

        /// <summary>ID do certificado atribuído pela CA externa após a emissão.</summary>
        public string? ExternalCertificateId { get; set; }

        /// <summary>Status da transação com a CA (ex: 'CREATED', 'SUBMITTED', 'PENDING_VALIDATION', 'ISSUED', 'ERROR').</summary>
        public string Status { get; set; } = "CREATED";

        /// <summary>Data e hora em que a ordem foi enviada para a API da CA.</summary>
        public DateTime? SubmittedAt { get; set; }

        /// <summary>Data e hora em que a ordem foi concluída com sucesso na CA.</summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>Código de erro retornado pela API da CA em caso de falha.</summary>
        public string? ErrorCode { get; set; }

        /// <summary>Mensagem detalhada do erro retornado pelo provedor externo.</summary>
        public string? ErrorMessage { get; set; }

        /// <summary>Referência para a resposta bruta (JSON/XML) recebida da API da CA (útil para debug).</summary>
        public string? RawResponseRef { get; set; }

        /// <summary>Data de criação deste registro de ordem.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Data da última atualização do status da ordem.</summary>
        public DateTime UpdatedAt { get; set; }

        // --- Navegação ---

        /// <summary>Navegação para a solicitação de certificado original.</summary>
        public virtual CertificateRequest? Request { get; set; }

        /// <summary>Navegação para a Autoridade Certificadora utilizada.</summary>
        public virtual CertificateAuthority? CertificateAuthority { get; set; }
    }
}
