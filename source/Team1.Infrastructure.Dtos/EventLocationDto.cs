using Team1.Model;

namespace Team1.Infrastructure.Dtos
{
    public class EventLocationDto : EventLocationBase
    {
        public bool IsDeleted { get; set; }
    }
}