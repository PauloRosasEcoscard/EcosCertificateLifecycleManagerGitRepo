namespace EcosCLM.Domain.Entities.Security
{
    /// <summary>
    /// Define a configuração de uma Autoridade Certificadora (CA) externa.
    /// Esta entidade armazena os parâmetros necessários para que o Ecos CLM interaja 
    /// com provedores de certificados (como Let's Encrypt, DigiCert, Entrust, etc.) 
    /// para solicitar, validar e emitir certificados.
    /// </summary>
    public class CertificateAuthority
    {
        /// <summary>Identificador único da configuração de autoridade.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) dono desta configuração de CA.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>Nome amigável da autoridade (ex: 'DigiCert Prod', 'Let's Encrypt Staging').</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Identificador do provedor/driver (ex: 'ACME', 'DIGICERT_API', 'ENTRUST_REST').</summary>
        public string ProviderType { get; set; } = string.Empty;

        /// <summary>URL base do endpoint da API da autoridade certificadora.</summary>
        public string? BaseUrl { get; set; }

        /// <summary>Referência ou identificador da conta do cliente dentro da autoridade externa.</summary>
        public string? AccountRef { get; set; }

        /// <summary>Flag que indica se a autoridade suporta o protocolo ACME (1 = Sim, 0 = Não).</summary>
        public short SupportsAcme { get; set; } = 0;

        /// <summary>Status atual da configuração (ex: 'ACTIVE', 'MAINTENANCE', 'DISABLED').</summary>
        public string Status { get; set; } = "ACTIVE";

        /// <summary>Configurações adicionais e credenciais criptografadas em formato JSON.</summary>
        public string MetadataJson { get; set; } = "{}";

        /// <summary>Data de criação deste registro.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Data da última atualização das configurações da CA.</summary>
        public DateTime UpdatedAt { get; set; }
    }
}
