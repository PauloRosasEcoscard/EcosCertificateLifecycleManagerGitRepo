using AutoMapper;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.ViewModels.Certificates;
using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Certificates;

namespace EcosCLM.Data.Repositories
{
    public class CaOrderRepository : BaseRepository<CaOrder, CaOrderViewModel>, ICaOrderRepository
    {
        public CaOrderRepository(EcosCLMContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}