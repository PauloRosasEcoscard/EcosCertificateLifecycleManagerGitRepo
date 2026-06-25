using static System.Net.Mime.MediaTypeNames;

namespace EcosCLM.Domain.Entities.Catalog
{
    /// <summary>
    /// Representa um FQDN (Fully Qualified Domain Name) que está sendo monitorado ou gerenciado pelo sistema.
    /// Esta entidade é o ponto central para validações de domínio e futuras renovações automáticas.
    /// </summary>
    public class ManagedDomain
    {
        /// <summary>Identificador único do registro de domínio.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) proprietário deste domínio.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>Referência à aplicação proprietária do domínio (opcional).</summary>
        public Guid? ApplicationId { get; set; }

        /// <summary>O FQDN completo (ex: 'api.ecosclm.com.br' ou '*.ecosclm.com.br').</summary>
        public string Fqdn { get; set; } = string.Empty;

        /// <summary>Método utilizado para validação de propriedade (ex: 'DNS', 'HTTP').</summary>
        public string ValidationMethod { get; set; } = "DNS";

        /// <summary>Status atual da validação do domínio (ex: 'PENDING', 'VALIDATED', 'EXPIRED').</summary>
        public string ValidationStatus { get; set; } = "PENDING";

        /// <summary>Data e hora em que a propriedade do domínio foi validada com sucesso.</summary>
        public DateTime? ValidatedAt { get; set; }

        /// <summary>Data e hora de expiração da validação do domínio, se aplicável.</summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>Metadados adicionais em formato JSON (ex: registros TXT esperados para validação).</summary>
        public string MetadataJson { get; set; } = "{}";

        /// <summary>Data de registro do domínio no sistema.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Data da última atualização do registro.</summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Navegação para a Aplicação relacionada. 
        /// Note que esta relação é lógica e reflete a quem o domínio pertence dentro do catálogo técnico.
        /// </summary>
        public virtual CLMApplication? Application { get; set; }
    }
}
