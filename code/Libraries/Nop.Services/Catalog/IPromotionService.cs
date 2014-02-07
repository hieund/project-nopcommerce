using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// News Category service interface
    /// </summary>
    public partial interface IPromotionService
    {
        void DeletePromotion(Promotion promo);
        IPagedList<Promotion> GetAllPromo(string promoName = "", int pageIndex = 0, int pageSize = int.MaxValue);
        Promotion GetPromotionById(int promoId);
        void InsertPromotion(Promotion promo);
        void UpdatePromotion(Promotion promo);
        IList<Promotion> GetPromotionByIds(int[] promoIds);
        IList<PromotionDetail> GetPromotionDetailByPromotion(int promoId, string productName, int categoryId, int manufactureId);
        void InsertPromotionDetail(PromotionDetail promodetail);
        void UpdatePromotionDetail(PromotionDetail promodetail);
        PromotionDetail GetPromotionDetailById(int promodetailId);
        void DeletePromotionDetail(PromotionDetail promodetail);
        bool CheckPromotionDetailExist(int PromotionId, int ProductId);
        IList<PromotionDetail> GetPromotionDetailByProductId(int PromotionId, int ProductId);
        IList<PromotionDetail> GetPromotionDetailByIds(int[] promodetailIds);

    }
}
