using Team1.Infrastructure.UserIdentity;

namespace Team1.DataSeed
{
    public class UserClaimBuilderTester : UserClaimModel
    {
        public UserClaimBuilderTester()
        {
            UserId = 0;
            IsAdmin = true;

            SetupPolicies();
        }
    }
}
