using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities;

namespace EcosCLM.Data.Repositories
{
    public class DownloadJobsRepository : BaseRepository<DownloadJobs, DownloadJobsViewModel>, IDownloadJobsRepository
    {
        public DownloadJobsRepository(EcosDashboardContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }
    }
}
