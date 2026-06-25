namespace EcosCLM.Domain.Entities.Security
{
    /// <summary>
    /// Define um perfil de política para emissão de certificados digitais.
    /// Este perfil atua como um "template" de governança, garantindo que todos os certificados 
    /// emitidos sob este perfil sigam os mesmos padrões de segurança (tamanho de chave, algoritmo, validade).
    /// </summary>
    public class CertificateProfile
    {
        /// <summary>Identificador único do perfil.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) proprietário deste perfil de segurança.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>Nome identificador do perfil (ex: 'Web Server RSA 2048', 'ECC High Security').</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Descrição opcional sobre a finalidade deste perfil.</summary>
        public string? Description { get; set; }

        /// <summary>Tipo de certificado esperado (ex: 'TLS_SERVER', 'CODE_SIGNING', 'CLIENT_AUTH').</summary>
        public string CertificateType { get; set; } = "TLS_SERVER";

        /// <summary>Algoritmo da chave privada (ex: 'RSA', 'ECDSA').</summary>
        public string KeyAlgorithm { get; set; } = "RSA";

        /// <summary>Tamanho da chave (em bits) para algoritmos RSA (ex: 2048, 4096).</summary>
        public int? KeySize { get; set; }

        /// <summary>Nome da curva elíptica para algoritmos ECDSA (ex: 'P-256', 'P-384').</summary>
        public string? CurveName { get; set; }

        /// <summary>Algoritmo de assinatura a ser utilizado (ex: 'SHA256withRSA').</summary>
        public string? SignatureAlgorithm { get; set; }

        /// <summary>Prazo de validade do certificado emitido, em dias.</summary>
        public int ValidityDays { get; set; }

        /// <summary>Janela de tempo (em dias antes do vencimento) para disparar o processo de renovação automática.</summary>
        public int RenewalWindowDays { get; set; } = 30;

        /// <summary>Template JSON que define as regras de preenchimento do Subject DN (Distinguished Name).</summary>
        public string SubjectTemplateJson { get; set; } = "{}";

        /// <summary>Políticas JSON que definem regras para inclusão de SANs (Subject Alternative Names).</summary>
        public string SanPolicyJson { get; set; } = "{}";

        /// <summary>Indica se a emissão exige aprovação humana (1 = Sim, 0 = Não).</summary>
        public short RequireApproval { get; set; } = 1;

        /// <summary>Status do perfil no sistema (ex: 'ACTIVE', 'DISABLED').</summary>
        public string Status { get; set; } = "ACTIVE";

        /// <summary>Data de criação deste perfil.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Data da última alteração das configurações deste perfil.</summary>
        public DateTime UpdatedAt { get; set; }
    }
}
