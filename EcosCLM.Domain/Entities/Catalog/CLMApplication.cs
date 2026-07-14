using EcosCLM.Domain.Entities.Deployment;
using System.Security.Cryptography.X509Certificates;

namespace EcosCLM.Domain.Entities.Catalog
{
    public class CLMApplication
    {
        /// <summary>Identificador único da aplicação.</summary>
        public Guid Id { get; set; }
        /// <summary>ID do cliente (EcosLogin) dono desta aplicação.</summary>
        public Guid CustomerId { get; set; }
        /// <summary>Código mnemônico único da aplicação.</summary>
        public string Code { get; set; } = string.Empty;
        /// <summary>Nome amigável da aplicação.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>Descrição detalhada.</summary>
        public string? Description { get; set; }
        /// <summary>ID do usuário proprietário (EcosLogin).</summary>
        public Guid? OwnerUserId { get; set; }
        /// <summary>Nível de criticidade (ex: LOW, MEDIUM, HIGH).</summary>
        public string Criticality { get; set; } = "MEDIUM";
        /// <summary>Status operacional (ex: ACTIVE, INACTIVE).</summary>
        public string Status { get; set; } = "ACTIVE";
        /// <summary>Metadados extras em JSON.</summary>
        public string MetadataJson { get; set; } = "{}";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<ManagedDomain> Domains { get; set; } = new List<ManagedDomain>();
        public virtual ICollection<CertificateRequest> CLMApplicationCertificateRequests { get; set; } = new List<CertificateRequest>();
        public virtual ICollection<DeploymentTarget> DeploymentTargets { get; set; } = new List<DeploymentTarget>();
    }
}
