namespace EcosCLM.Domain.Entities.Deployment
{
    /// <summary>
    /// Define um alvo de implantação onde os certificados digitais serão instalados.
    /// Representa a infraestrutura física ou lógica (como um Cluster Kubernetes, um Balanceador de Carga
    /// ou um Servidor Web) que consumirá o certificado para habilitar a comunicação segura (TLS).
    /// </summary>
    public class DeploymentTarget
    {
        /// <summary>Identificador único do destino de implantação.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) dono deste alvo de implantação.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>ID da aplicação (ManagedApp) à qual este alvo de implantação está subordinado.</summary>
        public Guid? ApplicationId { get; set; }

        /// <summary>ID do ambiente (DeploymentEnvironment) onde este alvo reside (ex: 'Prod', 'Staging').</summary>
        public Guid? EnvironmentId { get; set; }

        /// <summary>Nome amigável do destino (ex: 'Cluster K8s - Região Sul', 'Nginx Web Server 01').</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Tipo do alvo (ex: 'KUBERNETES', 'NGINX', 'AWS_ELB', 'IIS').</summary>
        public string TargetType { get; set; } = string.Empty;

        /// <summary>Referência de conexão (URL, IP ou ARN) para o alvo de implantação.</summary>
        public string? EndpointRef { get; set; }

        /// <summary>Referência segura (Secret ID ou Key Vault Ref) para autenticação no alvo.</summary>
        public string? SecretRef { get; set; }

        /// <summary>Flag que indica se a automação está habilitada (1 = Sim, 0 = Não).</summary>
        public short AutomationEnabled { get; set; }

        /// <summary>Status do alvo (ex: 'ACTIVE', 'OFFLINE', 'ERROR').</summary>
        public string Status { get; set; } = "ACTIVE";

        /// <summary>Data de criação do registro.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Data da última alteração no alvo.</summary>
        public DateTime UpdatedAt { get; set; }
    }
}