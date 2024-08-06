using System.ComponentModel.DataAnnotations;

namespace DPL.ViewModels.Auth.User
{
    public class ForgerPasswordVM
    {
        [Required]
        [EmailAddress]
        public string RecoveryEmail { get; set; }
    }
}
