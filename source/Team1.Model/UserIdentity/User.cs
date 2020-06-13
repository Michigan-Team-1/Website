using Team1.Model.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team1.Model.UserIdentity
{
    public class UserRoot
    {
        [Key]
        public int UserId { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = ErrorMessages.RequiredField), StringLength(Constants.FieldSizes.NameLength, ErrorMessage = ErrorMessages.StringLengthMax)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = ErrorMessages.RequiredField), StringLength(Constants.FieldSizes.NameLength, ErrorMessage = ErrorMessages.StringLengthMax)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address for this user.
        /// </summary>
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [Display(Name = "Email")]
        [StringLength(Constants.FieldSizes.EmailLength, ErrorMessage = ErrorMessages.StringLengthMax), EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a telephone number for the user.
        /// </summary>
        [Display(Name = "Phone Number")]
        [StringLength(Constants.FieldSizes.PhoneLength)]
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if two factor authentication is enabled for this user.
        /// </summary>
        /// <value>True if 2fa is enabled, otherwise false.</value>
        [Display(Name ="Two Factor Enable?")]
        public bool TwoFactorEnabled { get; set; }
        
        /// <summary>
        /// true to allow the user to login
        /// </summary>
        [Display(Name ="Enable Login")]
        public bool IsLoginEnabled { get; set; }

        [Display(Name ="Cert Level")]
        [Range(0, 3, ErrorMessage = ErrorMessages.RangeMinMax)]
        public byte CertificationLevel { get; set; }

        [Display(Name = "Tripoli #")]
        [StringLength(100, ErrorMessage = ErrorMessages.StringLengthMax)]
        public string TripoliNumber { get; set; }

        [Display(Name = "NAR #")]
        [StringLength(100, ErrorMessage = ErrorMessages.StringLengthMax)]
        public string NarNumber { get; set; }

        [Display(Name ="Paid Through")]
        public int PaidThroughYear { get; set; }

        [Display(Name = "Opt Out of Member Emails")]
        public bool OptOutOfMemberEmails { get; set; }

        [Display(Name = "Opt Out of General Emails")]
        public bool OptOutOfGeneralEmails { get; set; }

        [Display(Name = "Mobile Carrier")]
        public int? MobileCarrierId { get; set; }
    }

    public class UserBase : UserRoot
    {
        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        [NotMapped]
        public string UserName { get { return Email; } set { Email = value; } }

        /// <summary>
        /// Gets or sets the normalized user name for this user.
        /// </summary>
        [NotMapped]
        public string NormalizedUserName { get { return NormalizedEmail; } set { NormalizedEmail = value; } }

        /// <summary>
        /// Gets or sets the normalized email address for this user.
        /// </summary>
        [Required]
        [StringLength(Constants.FieldSizes.EmailLength), EmailAddress]
        public string NormalizedEmail { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their email address.
        /// </summary>
        /// <value>True if the email address has been confirmed, otherwise false.</value>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        [StringLength(200)]
        public string PasswordHash { get; set; }

        /// <summary>
        /// A random value that must change whenever a users credentials change (password changed, login removed)
        /// </summary>
        [Required]
        [StringLength(256)]
        public string SecurityStamp { get; set; }

        /// <summary>
        /// A random value that must change whenever a user is persisted to the store
        /// </summary>
        [StringLength(Constants.FieldSizes.ConcurrencyStampLength)]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their telephone address.
        /// </summary>
        /// <value>True if the telephone number has been confirmed, otherwise false.</value>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when any user lockout ends.
        /// </summary>
        /// <remarks>
        /// A value in the past means the user is not locked out.
        /// </remarks>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if the user could be locked out.
        /// </summary>
        /// <value>True if the user could be locked out, otherwise false.</value>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets the number of failed login attempts for the current user.
        /// </summary>
        public int AccessFailedCount { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time, in UTC, when the user last signed in.
        /// </summary>
        public DateTimeOffset? LastSignInDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when the user last changed his password.
        /// </summary>
        public DateTimeOffset? LastPasswordChangeDateTime { get; set; }

        /// <summary>
        /// Ip Address of the user from last login
        /// </summary>
        [StringLength(FieldSizes.IpAddressLength)]
        public string IpAddress { get; set; }

        /// <summary>
        /// Twilio Authy User Id
        /// </summary>
        public int? AuthyUserId { get; set; }
    }

    [Attributes.AuditLog(Enums.LogTypeEnum.User)]
    public class User : UserBase
    {
        public OwnedTypes.AuditFields AuditFields { get; set; }

        #region Navigation Links

        [InverseProperty(nameof(UserIdentity.UserClaim.User))]
        public ICollection<UserClaim> Claims { get; set; }

        [InverseProperty(nameof(UserIdentity.UserRole.User))]
        public ICollection<UserRole> UserRoles { get; set; }

        [InverseProperty(nameof(UserIdentity.UserLogin.User))]
        public ICollection<UserLogin> Logins { get; set; }

        [InverseProperty(nameof(UserIdentity.UserToken.User))]
        public ICollection<UserToken> Tokens { get; set; }

        [InverseProperty(nameof(Address.User))]
        public virtual ICollection<Address> Addresses { get; set; }

        public virtual ICollection<UserMemberType> UserMemberTypes { get; set; }

        public virtual ICollection<Picture> Pictures { get; set; }

        public virtual MobileCarrier MobileCarrier { get; set; }

        #endregion
    }
}
