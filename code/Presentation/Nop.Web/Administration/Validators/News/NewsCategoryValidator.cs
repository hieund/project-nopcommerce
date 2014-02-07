using FluentValidation;
using Nop.Admin.Models.News;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.News
{
    public class NewsCategoryValidator : AbstractValidator<NewsCategoryModel>
    {
        public NewsCategoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Catalog.Categories.Fields.Name.Required"));
        }
    }
}