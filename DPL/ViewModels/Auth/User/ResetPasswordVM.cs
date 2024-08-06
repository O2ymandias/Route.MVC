using System.ComponentModel.DataAnnotations;

namespace DPL.ViewModels.Auth.User
{
    public class ResetPasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Password & Confirm Password Don't Match")]
        public string ConfirmNewPassword { get; set; }
    }
}
