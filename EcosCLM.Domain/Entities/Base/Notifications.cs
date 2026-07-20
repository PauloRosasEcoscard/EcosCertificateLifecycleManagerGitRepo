using System.ComponentModel.DataAnnotations;

namespace EcosCLM.Domain.Entities.Base
{
    public class Notifications
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string User { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}