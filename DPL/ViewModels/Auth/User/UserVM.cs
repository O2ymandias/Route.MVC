using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DPL.ViewModels.Auth.User
{
    public class UserVM
    {
        public string Id { get; set; }

        [DisplayName("First Name")]
        public string? FirstName { get; set; }

        [DisplayName("Last Name")]
        public string? LastName { get; set; }

        [DisplayName("User Name")]
        public string? UserName { get; set; }

        [EmailAddress, DisplayName("Email Address")]
        public string? Email { get; set; }

        [Phone, DisplayName("Phone Number")]
        public string? PhoneNumber { get; set; }

        public IList<string>? Roles { get; set; }
    }
}
