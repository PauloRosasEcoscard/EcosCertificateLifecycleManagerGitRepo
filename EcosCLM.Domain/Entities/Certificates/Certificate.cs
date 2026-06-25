using System;
using System.Collections.Generic;
using System.Linq;
namespace EcosCLM.Domain.Entities.Certificates
{
    /// <summary>
    /// Representa um Certificado Digital emitido e pronto para uso. 
    /// Esta entidade armazena os detalhes técnicos da identidade digital, 
    /// possibilitando o rastreamento, monitoramento de expiração e revogação.
    /// </summary>
    public class Certificate
    {
        /// <summary>Identificador único interno do certificado no Ecos CLM.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) proprietário do certificado.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>ID da solicitação (CertificateRequest) que originou esta emissão.</summary>
        public Guid? RequestId { get; set; }

        /// <summary>ID da aplicação à qual este certificado está vinculado.</summary>
        public Guid? ApplicationId { get; set; }

        /// <summary>ID do domínio (ManagedDomain) principal deste certificado.</summary>
        public Guid? DomainId { get; set; }

        /// <summary>ID da Autoridade Certificadora (CA) que emitiu o certificado.</summary>
        public Guid? CaId { get; set; }

        /// <summary>ID da chave privada armazenada no HSM vinculada a este certificado.</summary>
        public Guid? HsmKeyRefId { get; set; }

        /// <summary>ID do certificado anterior, permitindo rastrear a cadeia de renovações (histórico).</summary>
        public Guid? PreviousCertificateId { get; set; }

        /// <summary>Número de série único atribuído pela CA.</summary>
        public string SerialNumber { get; set; } = string.Empty;

        /// <summary>Hash SHA-256 (Thumbprint) usado para identificar rapidamente o certificado sem ler todo o arquivo.</summary>
        public string ThumbprintSha256 { get; set; } = string.Empty;

        /// <summary>Subject DN (Distinguished Name) que identifica quem detém o certificado.</summary>
        public string SubjectDn { get; set; } = string.Empty;

        /// <summary>Issuer DN (Distinguished Name) que identifica quem emitiu o certificado.</summary>
        public string IssuerDn { get; set; } = string.Empty;

        /// <summary>Data de início da validade (NotBefore).</summary>
        public DateTime NotBefore { get; set; }

        /// <summary>Data de expiração do certificado (NotAfter).</summary>
        public DateTime NotAfter { get; set; }

        /// <summary>Conteúdo completo do certificado em formato PEM.</summary>
        public string CertificatePem { get; set; } = string.Empty;

        /// <summary>Cadeia de certificação (Intermediate CA) em formato PEM.</summary>
        public string? ChainPem { get; set; }

        /// <summary>Status do certificado (ex: 'ISSUED', 'REVOKED', 'EXPIRED').</summary>
        public string Status { get; set; } = "ISSUED";

        /// <summary>Motivo, caso o certificado tenha sido revogado.</summary>
        public string? RevocationReason { get; set; }

        /// <summary>Data e hora da revogação, se aplicável.</summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>Data em que o certificado foi efetivamente instalado em um ambiente (Target).</summary>
        public DateTime? InstalledAt { get; set; }

        /// <summary>Metadados adicionais, como extensões específicas de cliente ou logs de instalação.</summary>
        public string MetadataJson { get; set; } = "{}";

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // --- Navegação ---

        public virtual CertificateRequest? Request { get; set; }

        /// <summary>Referência ao certificado que antecede este na linhagem (histórico de renovações).</summary>
        public virtual Certificate? PreviousCertificate { get; set; }
    }
}
