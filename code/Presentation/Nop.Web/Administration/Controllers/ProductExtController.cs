using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Admin.Models.Catalog;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Nop.Services.Seo;
using Nop.Services.Customers;
using Nop.Core.Domain.Customers;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class ProductController : BaseNopController
    {
        #region Utilities

        [NonAction]
        private void PrepareSpecInfo(ProductModel model)
        {

        }
        [NonAction]
        private string GetProductPropertyValue(ProductDetail pd)
        {
            switch (pd.PropertyType)
            {
                case 0: return _categoryService.GetProductPropertyValueById(Convert.ToInt32(pd.Value)).Name;
                case 1:
                    string result = "";
                    string[] values = pd.Value.Split(new char[] { ',' });
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(result))
                            result += ", ";
                        result += _categoryService.GetProductPropertyValueById(Convert.ToInt32(values[i])).Name;
                    }
                    return result;
                case 2: return pd.Value;
            }
            return "";
        }
        #endregion

        #region Product Property Groups

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyGroupList(GridCommand command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var productCategories = _categoryService.GetProductCategoriesByProductId(productId, true);
            var categoryId = 0;
            if (productCategories.Count > 0)
                categoryId = productCategories.FirstOrDefault().CategoryId;
            else
                return null;

            var productPropertyGroups = _categoryService.GetProductPropertyGroupByCategoryId(categoryId);
            var productPropertyGroupsModel = productPropertyGroups
                .Select(x =>
                {
                    return new ProductModel.ProductPropertyGroupModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        DisplayOrder = x.DisplayOrder
                    };
                })
                .ToList();

            var model = new GridModel<ProductModel.ProductPropertyGroupModel>
            {
                Data = productPropertyGroupsModel,
                Total = productPropertyGroupsModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        #endregion

        #region Product Properties

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyList(GridCommand command, int groupId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var productProperties = _categoryService.GetProductPropertiesByProductPropertyGroupId(groupId);
            var productPropertyModels = productProperties
                .Select(x =>
                    {
                        return new ProductModel.ProductPropertyModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            GroupId = x.GroupId,
                            Value = "",
                            Type = x.Type,
                            DisplayOrder = x.DisplayOrder
                        };
                    })
                    .ToList();
            var model = new GridModel<ProductModel.ProductPropertyModel>
            {
                Data = productPropertyModels,
                Total = productPropertyModels.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        #endregion

        #region Product Property Values

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyValueList(GridCommand command, int propertyId, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var productPropertyValues = _categoryService.GetProductPropertyValuesByProductPropertyId(propertyId);
            var productPropertyValueModels = productPropertyValues
                .Select(x =>
                {
                    return new ProductModel.ProductPropertyValueModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        PropertyId = x.PropertyId,
                        Value = x.Value,
                        DisplayOrder = x.DisplayOrder
                    };
                })
                .ToList();
            var model = new GridModel<ProductModel.ProductPropertyValueModel>
            {
                Data = productPropertyValueModels,
                Total = productPropertyValueModels.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        #endregion

        #region Product Detail

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetProductDetail(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            IList<ProductDetail> productDetails = _productService.GetProductDetailByProductId(productId);
            var productDetailModels = productDetails.Select(x =>
                {
                    return new ProductModel.ProductDetailModel()
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        PropertyId = x.PropertyId,
                        CategoryId = x.CategoryId,
                        GroupId = x.GroupId,
                        PropertyType = x.PropertyType,
                        Value = x.Value
                    };
                })
                .ToList();
            return new JsonResult
            {
                Data = productDetailModels
            };
        }

        #endregion

        #region Html Generation

        private const string HTML_DETAIL_TABLE = "<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" class=\"p-dt\" style=\"width: 100%;\"><tbody>{0}</tbody></table>";
        private const string HTML_DETAIL_GROUP_ROW = "<tr class=\"r\"><td valign=\"top\" class=\"g\" rowspan=\"{0}\"{1}>{2}</td><td{3} class=\"p f\">{4}</td><td class=\"v f\">{5}</td></tr>";
        private const string HTML_DETAIL_PROPERTY_ROW = "<tr><td class=\"p\"{0}>{1}</td><td class=\"v\">{2}</td></tr>";
        /// <summary>
        /// Generate HTML for product detail
        /// </summary>
        /// <param name="model"></param>
        /// <param name="continueEditing"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GenerateHtml(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                //No product found with the specified id
                return RedirectToAction("List");
            StringBuilder sbHtml = new StringBuilder();
            ProductCategory category = product.ProductCategories.OrderBy(x => x.DisplayOrder).FirstOrDefault();
            if (category != null)
            {
                IList<ProductPropertyGroup> lstPropertyGroups = _categoryService.GetProductPropertyGroupByCategoryId(category.CategoryId);
                IList<ProductProperty> lstProperties = _categoryService.GetProductPropertiesByProductCategoryId(category.CategoryId);
                IList<ProductDetail> lstProductDetails = _productService.GetProductDetailByProductId(product.Id);
                foreach (ProductPropertyGroup pg in lstPropertyGroups)
                {
                    //Get properties to determine rowspan on this property group row
                    IList<ProductProperty> pps = lstProperties.Where(x => x.GroupId == pg.Id).ToList();
                    bool isFirst = true;

                    foreach (ProductProperty property in pps)
                    {
                        ProductDetail pd = lstProductDetails.FirstOrDefault(x => x.PropertyId == property.Id);
                        if (pd != null)
                        {
                            if (isFirst)
                            {
                                string strGroupRow = string.Format(HTML_DETAIL_GROUP_ROW,
                                    pps.Count,
                                    (string.IsNullOrEmpty(pg.Description) ? "" : " title=\"" + pg.Description + "\""),
                                    pg.Name,
                                    (string.IsNullOrEmpty(property.Description) ? "" : " title=\"" + property.Description + "\""),
                                    property.Name,
                                    GetProductPropertyValue(pd));
                                isFirst = false;
                                sbHtml.Append(strGroupRow);
                            }
                            else
                            {
                                string strPropertyRow = string.Format(HTML_DETAIL_PROPERTY_ROW,
                                    (string.IsNullOrEmpty(property.Description) ? "" : " title=\"" + property.Description + "\""),
                                    property.Name,
                                    GetProductPropertyValue(pd));
                                sbHtml.Append(strPropertyRow);
                            }
                        }
                        else
                        {
                            if (isFirst)
                            {
                                string strGroupRow = string.Format(HTML_DETAIL_GROUP_ROW,
                                    pps.Count,
                                    (string.IsNullOrEmpty(pg.Description) ? "" : " title=\"" + pg.Description + "\""),
                                    pg.Name,
                                    (string.IsNullOrEmpty(property.Description) ? "" : " title=\"" + property.Description + "\""),
                                    property.Name,
                                    "&nbsp;");
                                isFirst = false;
                                sbHtml.Append(strGroupRow);
                            }
                            else
                            {
                                string strPropertyRow = string.Format(HTML_DETAIL_PROPERTY_ROW,
                                        (string.IsNullOrEmpty(property.Description) ? "" : " title=\"" + property.Description + "\""),
                                        property.Name,
                                        "&nbsp;");
                                sbHtml.Append(strPropertyRow);
                            }
                        }
                    }
                }
                string html = string.Format(HTML_DETAIL_TABLE, sbHtml.ToString());
                product.HtmlSpec = html;
                _productService.UpdateProduct(product);
            }
            return new JsonResult { Data = 1 };
        }

        [HttpPost]
        public ActionResult PreviewHtml(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(productId);
            return new JsonResult { Data = product.HtmlSpec };
        }

        #endregion
    }
}
