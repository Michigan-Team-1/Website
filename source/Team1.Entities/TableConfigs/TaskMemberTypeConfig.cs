using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Team1.Model;

namespace Team1.Entities.TableConfigs.UserIdentity
{
    public class TaskMemberTypeConfig : IEntityTypeConfiguration<TaskMemberType>
    {
        public void Configure(EntityTypeBuilder<TaskMemberType> builder)
        {
            builder.HasKey(r => new { r.TaskId, r.MemberTypeId });
        }
    }
}
