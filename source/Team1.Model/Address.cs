using System.ComponentModel.DataAnnotations;

namespace Team1.Model
{
    public class AddressBase
    {
        [Key]
        public int AddressId { get; set; }

        public int UserId { get; set; }
    }

    [Attributes.AuditLog(Enums.LogTypeEnum.Address)]
    public class Address : AddressBase
    {
        public OwnedTypes.AddressObj AddressObj { get; set; }

        public OwnedTypes.AuditFields AuditFields { get; set; }

        #region Navigation Links

        public virtual UserIdentity.User User { get; set; }
        #endregion
    }
}
