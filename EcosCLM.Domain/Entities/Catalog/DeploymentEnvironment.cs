using EcosCLM.Domain.Entities.Deployment;
using System;
using System.Collections.Generic;
namespace EcosCLM.Domain.Entities.Catalog
{
    /// <summary>
    /// Define os ambientes de implantação (ex: Produção, Staging, Desenvolvimento) 
    /// onde os certificados serão instalados. Esta classe permite segregar os alvos de deploy 
    /// logicamente por etapa do ciclo de vida da aplicação.
    /// </summary>
    public class DeploymentEnvironment
    {
        /// <summary>Identificador único do ambiente.</summary>
        public Guid Id { get; set; }

        /// <summary>ID do cliente (EcosLogin) proprietário deste ambiente.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>Código mnemônico do ambiente (ex: 'PROD', 'STG', 'DEV').</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>Nome amigável exibido na interface para o ambiente (ex: 'Ambiente de Produção').</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Descrição opcional sobre a finalidade ou restrições do ambiente.</summary>
        public string? Description { get; set; }

        /// <summary>Data e hora em que o registro do ambiente foi criado.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Data e hora da última alteração nas configurações do ambiente.</summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Lista de destinos de implantação (DeploymentTargets) vinculados a este ambiente.
        /// Ex: Um ambiente de 'Produção' pode ter múltiplos servidores ou clusters como alvos.
        /// </summary>
        public virtual ICollection<DeploymentTarget> DeploymentTargets { get; set; } = new List<DeploymentTarget>();
    }
}
