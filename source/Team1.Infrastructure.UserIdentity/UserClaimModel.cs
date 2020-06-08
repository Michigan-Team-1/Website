using Team1.Model.SerializedObjects;

namespace Team1.Infrastructure.UserIdentity
{
    public class UserClaimModel
    {
        public bool IsAuthenticated { get { return UserId > 0; } }

        public int UserId { get; protected set; }
        public int UserIdOriginal { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public string Email { get; protected set; }

        public string IpAddress { get; protected set; }

        public int UtcOffset { get; protected set; }

        // roles
        public bool IsAdmin { get; protected set; }

        public RoleData RoleData { get; set; }

        // policies
        public UserPolicies UserPolicies { get; set; }

        protected void SetupPolicies()
        {
            UserPolicies = new UserPolicies();
            UserPolicies.CanImpersonate = IsAdmin;
            UserPolicies.UserAddEditDelete = IsAdmin;
            UserPolicies.UserProfileEdit = true;// everyone can

            UserPolicies.AnnouncementAddEditDelete = IsAdmin;
            UserPolicies.CanApprovePicture = IsAdmin;
            UserPolicies.EventAddEditDelete = IsAdmin;
            UserPolicies.LocationAddEditDelete = IsAdmin;
            UserPolicies.PictureAddEditDelete = true; // everyone can
            UserPolicies.TaskAddEditDelete = IsAdmin;
        }
    }

    public class UserPolicies
    {
        public UserPolicies() { }
        public UserPolicies(UserPolicies userPolicies)
        {
            var properties = typeof(UserPolicies).GetProperties();
            foreach (var item in properties)
            {
                item.SetValue(this, item.GetValue(userPolicies));
            }
        }

        public bool AnnouncementAddEditDelete { get; set; }
        public bool CanImpersonate { get; set; }
        public bool CanApprovePicture { get; set; }
        public bool EventAddEditDelete { get; set; }
        public bool LocationAddEditDelete { get; set; }
        public bool PictureAddEditDelete { get; set; }
        public bool TaskAddEditDelete { get; set; }
        public bool UserAddEditDelete { get; set; }
        public bool UserProfileEdit { get; set; }
    }
}
