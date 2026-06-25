using EcosCLM.Domain.Entities.Certificates;

namespace EcosCLM.Domain.Entities.Deployment
{
    /// <summary>
    /// Registra a tentativa ou sucesso de instalação de um certificado em um destino específico.
    /// Esta entidade permite rastrear a dispersão dos certificados pela infraestrutura, 
    /// facilitando a identificação de quais alvos possuem versões desatualizadas.
    /// </summary>
    public class CertificateDeployment
    {
        /// <summary>Identificador único deste registro de instalação.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) dono desta operação de deploy.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>ID do certificado que foi ou está sendo instalado.</summary>
        public Guid CertificateId { get; set; }

        /// <summary>ID do alvo (DeploymentTarget) onde o certificado será aplicado.</summary>
        public Guid TargetId { get; set; }

        /// <summary>Status da implantação (ex: 'PENDING', 'SUCCESS', 'FAILED').</summary>
        public string Status { get; set; } = "PENDING";

        /// <summary>Data e hora em que a instalação foi confirmada com sucesso no alvo.</summary>
        public DateTime? DeployedAt { get; set; }

        /// <summary>Mensagem de erro detalhando falhas de conexão ou permissão durante o deploy.</summary>
        public string? ErrorMessage { get; set; }

        /// <summary>Data de criação deste registro de deployment.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Data da última atualização deste registro.</summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>Navegação para o certificado instalado.</summary>
        public virtual Certificate? Certificate { get; set; }

        /// <summary>Navegação para o alvo onde o certificado foi instalado.</summary>
        public virtual DeploymentTarget? Target { get; set; }
    }
}
