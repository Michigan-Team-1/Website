namespace Team1.Infrastructure.Dtos.Token
{
    public class TokenUserDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyName { get; set; }
        public bool IsImpersonatingCompany { get; set; }
        public bool IsImpersonatingUser { get; set; }
    }
}
