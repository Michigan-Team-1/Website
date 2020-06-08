using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Team1.Model.UserIdentity;

namespace Team1.Entities.TableConfigs.UserIdentity
{
    public class UserMemberTypeConfig : IEntityTypeConfiguration<UserMemberType>
    {
        public void Configure(EntityTypeBuilder<UserMemberType> builder)
        {
            builder.HasKey(r => new { r.UserId, r.MemberTypeId });
        }
    }
}
