using AutoMapper;
using EcosCLM.Domain.Entities.Base;

namespace EcosCLM.Application.ViewModels
{
    public class DownloadJobsViewModel
    {
        public Guid Id { get; set; }
        public DownloadStatus Status { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string Error { get; set; } = string.Empty;
    }

    public class DownloadJobsProfile : Profile
    {
        public DownloadJobsProfile()
        {
            CreateMap<DownloadJobs, DownloadJobsViewModel>().ReverseMap();
        }
    }
}