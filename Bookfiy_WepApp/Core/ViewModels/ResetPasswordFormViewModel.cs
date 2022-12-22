using Bookfiy_WepApp.Core.Const;

namespace Bookfiy_WepApp.Core.ViewModels
{
    public class ResetPasswordFormViewModel
    {

        public string Id { get; set; }

        [DataType(DataType.Password), StringLength(100,
            ErrorMessage = ErrorValidation.MaxLengthPassword, MinimumLength = 8),
            RegularExpression(Regx.Password,
            ErrorMessage = ErrorValidation.InvalidPass)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password), Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = ErrorValidation.InvalidPass)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
