namespace EcosCLM.Domain.Entities.Certificates
{
    /// <summary>
    /// Registra um endereço IP (Subject Alternative Name - SAN) adicional 
    /// solicitado em uma requisição de certificado.
    /// </summary>
    public class CertificateRequestSanIp
    {
        /// <summary>Identificador único do registro.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) proprietário desta solicitação.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>ID da solicitação de certificado (CertificateRequest) à qual este SAN pertence.</summary>
        public Guid RequestId { get; set; }

        /// <summary>O endereço IP (ex: '192.168.1.10').</summary>
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>Data de criação do registro.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Navegação para a solicitação de certificado pai.</summary>
        public virtual CertificateRequest? Request { get; set; }
    }
}
