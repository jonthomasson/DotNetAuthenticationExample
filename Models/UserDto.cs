namespace DotNetAuthentication.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public Role Role { get; set; }
    }
}
