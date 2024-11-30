namespace Shopee.Application.DTOs
{
    public class UserResponseDTO
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
        public string? PhoneNumber{get;set;}
        public string? Avatar { get; set; }
        public DateTime? BirthOfDay { get; set; }
    }
}