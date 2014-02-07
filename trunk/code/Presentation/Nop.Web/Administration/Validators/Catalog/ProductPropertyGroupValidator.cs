using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Catalog
{
    public class ProductPropertyGroupValidator : AbstractValidator<ProductPropertyGroupModel>
    {
        public ProductPropertyGroupValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.ProductPropertyGroup.Fields.Name.Required"));
        }
    }
}