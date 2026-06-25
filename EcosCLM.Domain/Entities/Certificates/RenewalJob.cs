namespace EcosCLM.Domain.Entities.Certificates
{
    /// <summary>
    /// Representa uma tarefa agendada para renovação automática de um certificado digital.
    /// Esta entidade permite que o serviço de background do CLM consulte quais certificados 
    /// precisam de atenção e controle o estado da operação de renovação.
    /// </summary>
    public class RenewalJob
    {
        /// <summary>Identificador único do Job de renovação.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) proprietário do certificado a ser renovado.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>ID do certificado que deve ser renovado.</summary>
        public Guid CertificateId { get; set; }

        /// <summary>Data e hora em que a tarefa de renovação foi agendada para execução.</summary>
        public DateTime ScheduledAt { get; set; }

        /// <summary>Data limite (deadline) pela qual a renovação deve ser concluída antes da expiração.</summary>
        public DateTime DueAt { get; set; }

        /// <summary>Status atual do Job (ex: 'SCHEDULED', 'PROCESSING', 'COMPLETED', 'FAILED').</summary>
        public string Status { get; set; } = "SCHEDULED";

        /// <summary>Data de criação do job no sistema.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Data da última atualização do estado do job.</summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>Navegação para o certificado que está sendo alvo deste job de renovação.</summary>
        public virtual Certificate? Certificate { get; set; }
    }
}
