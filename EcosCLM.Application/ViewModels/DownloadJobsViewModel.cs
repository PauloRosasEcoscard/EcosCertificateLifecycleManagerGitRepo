using AutoMapper;
using EcosCLM.Domain.Entities;

namespace EcosCLM.Application.ViewModels
{
    public class DownloadJobsViewModel
    {
        public Guid Id { get; set; }
        public DownloadStatus Status { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string Error { get; set; }
    }

    public class DownloadJobsProfile : Profile
    {
        public DownloadJobsProfile()
        {
            CreateMap<DownloadJobs, DownloadJobsViewModel>().ReverseMap();
        }
    }
}
