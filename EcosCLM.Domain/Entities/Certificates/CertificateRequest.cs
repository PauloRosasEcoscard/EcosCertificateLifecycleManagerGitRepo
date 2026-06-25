using EcosCLM.Domain.Entities.Catalog;
using EcosCLM.Domain.Entities.Security;
using static System.Net.Mime.MediaTypeNames;

namespace EcosCLM.Domain.Entities.Certificates
{
    /// <summary>
    /// Representa uma solicitação formal para emissão de um certificado digital.
    /// Esta classe é o ponto de entrada do fluxo de trabalho (Workflow), onde a intenção do cliente 
    /// é validada contra políticas internas (Profiles) e, posteriormente, processada por uma Autoridade Certificadora.
    /// </summary>
    public class CertificateRequest
    {
        /// <summary>Identificador único (GUID) da solicitação.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) proprietário desta solicitação.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>Tipo da requisição (ex: 'NEW' para emissão, 'RENEW' para renovação, 'REKEY' para troca de chave).</summary>
        public string RequestType { get; set; } = string.Empty;

        /// <summary>Estado atual no ciclo de vida (ex: 'DRAFT', 'PENDING_APPROVAL', 'SUBMITTED', 'ISSUED', 'FAILED').</summary>
        public string Status { get; set; } = "DRAFT";

        /// <summary>ID da aplicação (ManagedApp) associada a esta solicitação.</summary>
        public Guid? ApplicationId { get; set; }

        /// <summary>ID do domínio (ManagedDomain) que será incluído no certificado.</summary>
        public Guid? DomainId { get; set; }

        /// <summary>Perfil de segurança (CertificateProfile) utilizado para aplicar as regras de emissão.</summary>
        public Guid? ProfileId { get; set; }

        /// <summary>ID da Autoridade Certificadora (CertificateAuthority) que processará o pedido.</summary>
        public Guid? CaId { get; set; }

        /// <summary>ID do HSM Cluster caso a chave privada precise ser gerada em hardware.</summary>
        public Guid? HsmClusterId { get; set; }

        /// <summary>Referência da chave privada vinculada dentro do HSM.</summary>
        public Guid? HsmKeyRefId { get; set; }

        /// <summary>ID do usuário (EcosLogin) que abriu o chamado de emissão.</summary>
        public Guid? RequestedBy { get; set; }

        /// <summary>Distinguished Name (DN) do assunto do certificado (ex: CN=api.empresa.com, O=Ecos, C=BR).</summary>
        public string SubjectDn { get; set; } = string.Empty;

        /// <summary>Regras de política da chave (JSON) contendo algoritmos, permissões de uso e outras restrições técnicas.</summary>
        public string KeyPolicyJson { get; set; } = "{}";

        /// <summary>Conteúdo PEM (Base64) do Certificate Signing Request (CSR) enviado pelo cliente.</summary>
        public string? CsrPem { get; set; }

        /// <summary>Campo para logar o motivo técnico da falha, caso o status torne-se 'FAILED'.</summary>
        public string? FailureReason { get; set; }

        /// <summary>Metadados dinâmicos necessários para automações específicas ou integrações externas.</summary>
        public string MetadataJson { get; set; } = "{}";

        /// <summary>Data de criação da solicitação.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Data da última alteração no status ou dados da solicitação.</summary>
        public DateTime UpdatedAt { get; set; }

        // --- Navegação (Relacionamentos) ---

        /// <summary>Navegação para a Aplicação relacionada.</summary>
        public virtual CLMApplication? Application { get; set; }

        /// <summary>Navegação para o Domínio relacionado.</summary>
        public virtual ManagedDomain? Domain { get; set; }

        /// <summary>Navegação para o Perfil de emissão utilizado.</summary>
        public virtual CertificateProfile? Profile { get; set; }

        /// <summary>Navegação para a CA (Autoridade Certificadora) que processará o pedido.</summary>
        public virtual CertificateAuthority? CertificateAuthority { get; set; }

        /// <summary>Lista de nomes DNS (Subject Alternative Names) para este certificado.</summary>
        public virtual ICollection<CertificateRequestSanDns> SanDns { get; set; } = new List<CertificateRequestSanDns>();

        /// <summary>Lista de endereços IP (Subject Alternative Names) para este certificado.</summary>
        public virtual ICollection<CertificateRequestSanIp> SanIps { get; set; } = new List<CertificateRequestSanIp>();

        /// <summary>Lista de tarefas de aprovação que precisam ser concluídas antes da submissão.</summary>
        public virtual ICollection<ApprovalTask> ApprovalTasks { get; set; } = new List<ApprovalTask>();
    }
}
