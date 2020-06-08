using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Team1.Model;

namespace Team1.Entities.TableConfigs
{
    public class EventLocationConfig : IEntityTypeConfiguration<EventLocation>
    {
        public void Configure(EntityTypeBuilder<EventLocation> builder)
        {
            builder.HasKey(r => new { r.EventId, r.LocationId });
        }
    }
}
