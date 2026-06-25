namespace EcosCLM.Domain.Entities.Certificates
{
    /// <summary>
    /// Registra um nome DNS (Subject Alternative Name - SAN) adicional 
    /// solicitado em uma requisição de certificado.
    /// </summary>
    public class CertificateRequestSanDns
    {
        /// <summary>Identificador único do registro.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) proprietário desta solicitação.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>ID da solicitação de certificado (CertificateRequest) à qual este SAN pertence.</summary>
        public Guid RequestId { get; set; }

        /// <summary>O nome de domínio DNS (ex: 'app.empresa.com').</summary>
        public string DnsName { get; set; } = string.Empty;

        /// <summary>Data de criação do registro.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Navegação para a solicitação de certificado pai.</summary>
        public virtual CertificateRequest? Request { get; set; }
    }
}
