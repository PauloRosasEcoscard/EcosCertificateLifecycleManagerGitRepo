using AutoMapper;
using EcosCLM.Application.Validators.Certificates;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Domain.Entities.Certificates;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Application.ViewModels.Certificates
{
    public class ApprovalTaskViewModel : IValidatableObject
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid RequestId { get; set; }
        public int StepOrder { get; set; } = 1;
        public Guid? ApproverRoleId { get; set; }
        public Guid? ApproverUserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? DecisionComment { get; set; }
        public Guid? DecidedBy { get; set; }
        public DateTime? DecidedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual CertificateRequestViewModel? Request { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = validationContext.GetRequiredService<ApprovalTaskValidator>();
            var result = validator.Validate(this);
            return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, [error.PropertyName]));
        }
    }
    public class ApprovalTaskProfile : Profile
    {
        public ApprovalTaskProfile()
        {
            CreateMap<ApprovalTask, ApprovalTaskViewModel>().ReverseMap();
        }
    }
}
