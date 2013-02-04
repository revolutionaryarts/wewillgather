using FluentValidation;
using Gather.Core.Seo;
using Gather.Services.Users;
using Gather.Web.Models.User;

namespace Gather.Web.Validators.User
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator(IUserService userService)
        {
            RuleFor(x => x.Email)
                .Must((x, email) =>
                {
                    if (string.IsNullOrEmpty(x.TwitterProfile))
                        return !string.IsNullOrEmpty(email);
                    return true;
                })
                .WithMessage("An email address or Twitter account, linked to your profile, is required so we can get in touch.")
                .EmailAddress()
                .WithMessage("The email address entered is not valid.")
                .Must((x, email) => !userService.EmailExists(email, x.Id))
                .WithMessage("A member already exists with that email address.");

            RuleFor(x => x.UserName)
                .Must((x, userName) => !userService.UserNameExists(SeoExtensions.GetSeoName(userName), x.Id))
                .WithMessage("A member already exists with that username.");

            RuleFor(x => x.ContactUsBio)
                .Must((x, bio) =>
                {
                    if (x.ShowOnContactUs)
                        return !string.IsNullOrEmpty(bio);
                    return true;
                })
                .WithMessage("Please enter a 'contact us' bio.");
        }
    }
}