namespace EcosCLM.Domain.Entities.Certificates
{
    /// <summary>
    /// Representa uma tarefa de aprovação humana necessária no fluxo de emissão de certificados.
    /// Permite que múltiplos níveis de alçada (Approval Steps) sejam configurados, garantindo que 
    /// solicitações críticas passem pelo crivo de usuários ou papéis autorizados.
    /// </summary>
    public class ApprovalTask
    {
        /// <summary>Identificador único da tarefa de aprovação.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) dono desta tarefa.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>ID da solicitação de certificado (CertificateRequest) à qual esta aprovação está atrelada.</summary>
        public Guid RequestId { get; set; }

        /// <summary>Ordem desta etapa de aprovação (ex: 1 para gerente, 2 para segurança da informação).</summary>
        public int StepOrder { get; set; } = 1;

        /// <summary>ID do papel (Role) do EcosLogin autorizado a realizar esta aprovação.</summary>
        public Guid? ApproverRoleId { get; set; }

        /// <summary>ID de um usuário específico (EcosLogin) autorizado a realizar esta aprovação.</summary>
        public Guid? ApproverUserId { get; set; }

        /// <summary>Status da tarefa (ex: 'PENDING', 'APPROVED', 'REJECTED').</summary>
        public string Status { get; set; } = "PENDING";

        /// <summary>Comentário opcional incluído pelo aprovador ou reprovador.</summary>
        public string? DecisionComment { get; set; }

        /// <summary>ID do usuário (EcosLogin) que tomou a decisão de aprovar ou reprovar.</summary>
        public Guid? DecidedBy { get; set; }

        /// <summary>Data e hora em que a decisão foi tomada.</summary>
        public DateTime? DecidedAt { get; set; }

        /// <summary>Data de criação da tarefa no sistema.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Data da última atualização da tarefa.</summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>Navegação para a solicitação de certificado relacionada.</summary>
        public virtual CertificateRequest? Request { get; set; }
    }
}
