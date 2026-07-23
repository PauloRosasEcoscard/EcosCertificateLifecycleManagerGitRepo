namespace EcosCLM.Domain.Entities.Security
{
    /// <summary>
    /// Representa a referência (ponteiro) de uma chave criptográfica armazenada fisicamente em um HSM.
    /// O Ecos CLM não armazena a chave privada, apenas este registro que indica onde a chave
    /// reside e quais são suas propriedades técnicas para fins de auditoria e uso.
    /// </summary>
    public class HsmKeyRef
    {
        /// <summary>Identificador único desta referência de chave no banco de dados.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) dono desta chave.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>ID do cluster de HSM ao qual esta chave pertence.</summary>
        public Guid HsmClusterId { get; set; }

        /// <summary>Label (rótulo) definido no HSM para identificar esta chave (ex: 'app-prod-rsa-2024').</summary>
        public string KeyLabel { get; set; } = string.Empty;

        /// <summary>Handle (identificador interno do HSM) usado para invocar operações criptográficas com esta chave.</summary>
        public string KeyHandle { get; set; } = string.Empty;

        /// <summary>Algoritmo da chave (ex: 'RSA', 'ECDSA').</summary>
        public string Algorithm { get; set; } = string.Empty;

        /// <summary>Tamanho da chave em bits (usado principalmente para RSA).</summary>
        public int? KeySize { get; set; }

        /// <summary>Nome da curva (ex: 'P-256', 'P-384') caso o algoritmo seja ECDSA.</summary>
        public string? CurveName { get; set; }

        /// <summary>Indica se a chave pode ser exportada do HSM (1 = Sim, 0 = Não). Geralmente definido como '0' por segurança.</summary>
        public short Extractable { get; set; }

        /// <summary>Status da chave (ex: 'ACTIVE', 'REVOKED', 'EXPIRED').</summary>
        public string Status { get; set; } = "ACTIVE";

        /// <summary>Data de criação do registro desta referência no banco.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Data da última atualização desta referência.</summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Propriedade de navegação para o cluster de HSM pai.
        /// Permite acessar as configurações do dispositivo que protege esta chave.
        /// </summary>
        public virtual HsmCluster? HsmCluster { get; set; }
    }
}