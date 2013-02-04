using FluentValidation;
using Gather.Web.Models.Project;

namespace Gather.Web.Validators.BaseProject
{
    public class BaseProjectValidator : AbstractValidator<BaseProjectModel>
    {
        public BaseProjectValidator()
        {
            RuleFor(x => x.StartDate).Must((x, u) => x.StartDate != null).WithMessage("Start date must be in the future");
            RuleFor(x => x.EndDate).Must((x, u) => x.StartDate.Value.AddDays(-1) <= x.EndDate.Value).WithMessage("End date must be greater than start date");
        }
    }
}