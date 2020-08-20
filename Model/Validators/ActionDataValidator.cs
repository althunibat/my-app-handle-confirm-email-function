using FluentValidation;

namespace Godwit.HandleConfirmEmail.Model.Validators {
    public class ActionDataValidator : AbstractValidator<ActionData> {
        public ActionDataValidator() {
            RuleFor(x => x.Action).NotNull();
            RuleFor(x => x.Action.Name).Must(x => x.Equals("confirmEmail"));
            RuleFor(x => x.Input).NotNull();
            RuleFor(x => x.Input.UserName).NotEmpty();
            RuleFor(x => x.Input.Token).NotEmpty();
        }
    }
}