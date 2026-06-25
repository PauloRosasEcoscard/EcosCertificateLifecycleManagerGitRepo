namespace EcosCLM.Domain.Entities.Security
{
    /// <summary>
    /// Define um cluster ou partição de HSM (Hardware Security Module).
    /// Esta entidade abstrai a camada de hardware, permitindo que o Ecos CLM gerencie 
    /// chaves criptográficas que residem fisicamente fora do banco de dados, garantindo
    /// conformidade com padrões de segurança (como FIPS).
    /// </summary>
    public class HsmCluster
    {
        /// <summary>Identificador único do cluster de HSM no sistema.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) dono desta configuração de HSM.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>Nome amigável do cluster de HSM (ex: 'HSM-Produção-01').</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Fabricante do hardware (ex: 'Thales', 'nCipher', 'AWS CloudHSM').</summary>
        public string? Vendor { get; set; }

        /// <summary>Modelo do dispositivo HSM.</summary>
        public string? Model { get; set; }

        /// <summary>Label da partição dentro do HSM, caso o dispositivo suporte múltiplos contextos.</summary>
        public string? PartitionLabel { get; set; }

        /// <summary>Referência de conexão (ex: URL ou IP) para acessar o HSM via API ou PKCS#11.</summary>
        public string? EndpointRef { get; set; }

        /// <summary>Nível de conformidade FIPS (ex: 'FIPS 140-2 Level 3').</summary>
        public string? FipsLevel { get; set; }

        /// <summary>Status atual do cluster (ex: 'ACTIVE', 'OFFLINE', 'MAINTENANCE').</summary>
        public string Status { get; set; } = "ACTIVE";

        /// <summary>Configurações adicionais em JSON (ex: configurações de rede, credenciais de sessão).</summary>
        public string MetadataJson { get; set; } = "{}";

        /// <summary>Data de criação deste registro.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Data da última atualização das configurações do cluster.</summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Lista de referências de chaves (HsmKeyRef) contidas neste cluster.
        /// </summary>
        public virtual ICollection<HsmKeyRef> Keys { get; set; } = new List<HsmKeyRef>();
    }
}
