using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Catalog
{
    public class PromotionValidator : AbstractValidator<PromotionModel>
    {
        public PromotionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PromotionName).NotNull().WithMessage("Vui lòng nhập khuyến mãi");
            //RuleFor(x => x.Description).NotNull().WithMessage("Vui lòng nhập nội dung");
            RuleFor(x => x.StartDate).NotNull().WithMessage("Vui lòng nhập ngày bắt đầu");
            RuleFor(x => x.EndDate).NotNull().WithMessage("Vui lòng nhập ngày kết thúc");
            RuleFor(x => x.DisplayOrder).NotNull().WithMessage("Vui lòng nhập thứ tự hiển thị");
        }
    }
}