using AutoMapper;
using EcosCLM.Domain.Entities;

namespace EcosCLM.Application.ViewModels
{
    public class SessionEntryViewModel
    {
        public string Id { get; set; }
        public byte[] Value { get; set; }
        public DateTime ExpiresAtTime { get; set; }
        public long? SlidingExpirationInSeconds { get; set; }
        public DateTime? AbsoluteExpiration { get; set; }
    }

    public class SessionEntryProfile : Profile
    {
        public SessionEntryProfile()
        {
            CreateMap<SessionEntry, SessionEntryViewModel>().ReverseMap();
        }
    }
}
