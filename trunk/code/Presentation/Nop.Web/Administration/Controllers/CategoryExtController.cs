using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models.Catalog;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;
using System.Text;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class CategoryController : BaseNopController
    {
        #region Product Properties Group
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyGroupList(GridCommand command, int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
            Customer customer = _customerService.GetCustomerByUsername(HttpContext.User.Identity.Name);
            var productpropertiesgroupCategories = _categoryService.GetProductPropertyGroupByCategoryId(categoryId);
            var model = new GridModel<CategoryModel.ProductPropertyGroupModel>
            {
                Data = productpropertiesgroupCategories
                .Select(x =>
                {
                    return new CategoryModel.ProductPropertyGroupModel()
                    {
                        Id = x.Id,
                        CategoryId = x.CategoryId,
                        Name = x.Name,
                        IsMapping = x.IsMapping,
                        IsActived = x.IsActived,
                        DisplayOrder1 = x.DisplayOrder,
                        Description = x.Description
                    };
                }),
                Total = productpropertiesgroupCategories.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }
        string compareRow = "[row title=\"{0}\" id=\"{1}\"]";
        private string GenerateComparationTemplate(int categoryId)
        {
            StringBuilder sb = new StringBuilder();
            var lstPropertyGroup = _categoryService.GetProductPropertyGroupByCategoryId(categoryId);
            foreach (var pg in lstPropertyGroup)    
            {
                string propertyList = "";
                var lstProperty = _categoryService.GetProductPropertiesByProductPropertyGroupId(pg.Id);
                foreach (var p in lstProperty)
                {
                    if (!string.IsNullOrEmpty(propertyList))
                        propertyList += ",";
                    propertyList += p.Id;
                }
                sb.AppendFormat(compareRow, pg.Name, propertyList);
            }
            return sb.ToString();
        }

        private void UpdateComparationTemplate(int categoryId)
        {
            Category category = _categoryService.GetCategoryById(categoryId);
            category.HtmlCompare = GenerateComparationTemplate(categoryId);
            _categoryService.UpdateCategory(category);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyGroupUpdate(GridCommand command, CategoryModel.ProductPropertyGroupModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var productpropertygroupCategory = _categoryService.GetProductPropertyGroupById(model.Id);
            if (productpropertygroupCategory == null)
                throw new ArgumentException("No product properties group category mapping found with the specified id");
            productpropertygroupCategory.Name = model.Name;
            productpropertygroupCategory.IsMapping = model.IsMapping;
            productpropertygroupCategory.IsActived = model.IsActived;
            productpropertygroupCategory.Description = model.Description;

            if (model.IsActived)
            {
                productpropertygroupCategory.ActivedAt = DateTime.UtcNow;
                //activity log active 
                _customerActivityService.InsertActivity("ActiveProductPropertyGroup", _localizationService.GetResource("ActivityLog.ActiveProductPropertyGroup"), productpropertygroupCategory.Name);
            }
            else
            {
                //activity log upactive
                _customerActivityService.InsertActivity("UnactiveProductPropertyGroup", _localizationService.GetResource("ActivityLog.UnactiveProductPropertyGroup"), productpropertygroupCategory.Name);
            }
            productpropertygroupCategory.DisplayOrder = model.DisplayOrder1;
            _categoryService.UpdateProductPropertyGroup(productpropertygroupCategory);
            //Update htmlcompare
            UpdateComparationTemplate(productpropertygroupCategory.CategoryId);

            //activity log update
            _customerActivityService.InsertActivity("EditProductPropertyGroup", _localizationService.GetResource("ActivityLog.EditProductPropertyGroup"), productpropertygroupCategory.Name);
            return ProductPropertyGroupList(command, productpropertygroupCategory.CategoryId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyGroupDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var productpropertygroupCategory = _categoryService.GetProductPropertyGroupById(id);
            if (productpropertygroupCategory == null)
                throw new ArgumentException("No product category mapping found with the specified id");

            var categoryId = productpropertygroupCategory.CategoryId;
            productpropertygroupCategory.IsDeleted = true;
            _categoryService.DeleteProductPropertyGroup(productpropertygroupCategory);

            //Update htmlcompare
            UpdateComparationTemplate(productpropertygroupCategory.CategoryId);

            //activity log
            _customerActivityService.InsertActivity("DeleteProductPropertyGroup", _localizationService.GetResource("ActivityLog.DeleteProductPropertyGroup"), productpropertygroupCategory.Name);

            return ProductPropertyGroupList(command, categoryId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyGroupInsert(GridCommand command, CategoryModel.ProductPropertyGroupModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var categoryId = model.CategoryId;
            ProductPropertyGroup ppg = new ProductPropertyGroup();
            ppg.CreatedAt = DateTime.UtcNow;
            ppg.IsActived = model.IsActived;
            ppg.CategoryId = model.CategoryId;
            ppg.Name = model.Name;
            ppg.DisplayOrder = model.DisplayOrder1;
            ppg.IsMapping = model.IsMapping;
            ppg.Description = model.Description;
            _categoryService.InsertProductPropertyGroup(ppg);

            //Update htmlcompare
            UpdateComparationTemplate(model.CategoryId);

            //activity log
            _customerActivityService.InsertActivity("AddNewProductPropertyGroup", _localizationService.GetResource("ActivityLog.AddNewProductPropertyGroup"), model.Name);
            return ProductPropertyGroupList(command, categoryId);
        }

        #region Popup
        public ActionResult ProductPropertyGroupAddPopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var model = new ProductPropertyGroupModel();
            if (Request["categoryID"] != null)
                model.CategoryId = Convert.ToInt32(Request["categoryID"]);
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult ProductPropertyGroupAddPopup(ProductPropertyGroupModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
            //if (ModelState.IsValid)
            //{
            ProductPropertyGroup ppg = new ProductPropertyGroup();
            ppg.CreatedAt = DateTime.UtcNow;
            ppg.IsActived = true;
            ppg.CategoryId = model.CategoryId;
            ppg.Name = model.Name;
            ppg.DisplayOrder = model.DisplayOrder;
            ppg.IsMapping = model.IsMapping;
            _categoryService.InsertProductPropertyGroup(ppg);
            //}
            ViewBag.RefreshPage = true;
            //ViewBag.btnId = btnId;
            //ViewBag.formId = formId;
            return View(model);
        }
        #endregion

        #endregion

        #region Product Property
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyList(GridCommand command, int groupId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var productProperties = _categoryService.GetProductPropertiesByProductPropertyGroupIdForCategory(groupId);
            var productPropertyModels = productProperties
                .Select(x =>
                    {
                        return new CategoryModel.ProductPropertyModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            GroupId = x.GroupId,
                            IsMapping = x.IsMapping,
                            IsActived = x.IsActived,
                            PropertyType = x.Type.ToString(),
                            DisplayOrder1 = x.DisplayOrder,
                            Description = x.Description,
                            IsFilterExtension = x.IsFilterExtension
                        };
                    })
                    .ToList();
            var model = new GridModel<CategoryModel.ProductPropertyModel>
            {
                Data = productPropertyModels,
                Total = productPropertyModels.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyInsert(GridCommand command, CategoryModel.ProductPropertyModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var groupId = model.GroupId;
            ProductProperty productproperty = new ProductProperty();
            productproperty.CreatedAt = DateTime.UtcNow;
            productproperty.IsActived = model.IsActived;
            productproperty.GroupId = model.GroupId;
            productproperty.Name = model.Name;
            productproperty.DisplayOrder = model.DisplayOrder1;
            productproperty.IsMapping = model.IsMapping;
            productproperty.Type = Convert.ToByte(model.PropertyType);
            productproperty.Description = model.Description;
            productproperty.IsFilterExtension = model.IsFilterExtension;
            _categoryService.InsertProductProperty(productproperty);

            //Update htmlcompare
            ProductPropertyGroup ppg = _categoryService.GetProductPropertyGroupById(model.GroupId);
            UpdateComparationTemplate(ppg.CategoryId);

            //activity log insert
            _customerActivityService.InsertActivity("AddNewProductProperty", _localizationService.GetResource("ActivityLog.AddNewProductProperty"), productproperty.Name);
            return ProductPropertyList(command, groupId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyUpdate(GridCommand command, CategoryModel.ProductPropertyModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var productproperty = _categoryService.GetProductPropertyById(model.Id);
            if (productproperty == null)
                throw new ArgumentException("No product properties group category mapping found with the specified id");
            if (TryUpdateModel(productproperty))
            {
                productproperty.IsMapping = model.IsMapping;
                productproperty.IsActived = model.IsActived;
                productproperty.ActivedAt = DateTime.UtcNow;
                productproperty.DisplayOrder = model.DisplayOrder1;
                productproperty.Type = Convert.ToByte(model.PropertyType);
                productproperty.Name = model.Name;
                productproperty.Description = model.Description;
                productproperty.IsFilterExtension = model.IsFilterExtension;
                if (model.IsActived)
                {
                    productproperty.ActivedAt = DateTime.UtcNow;
                    //activity log active
                    _customerActivityService.InsertActivity("ActiveProductProperty", _localizationService.GetResource("ActivityLog.ActiveNewProductProperty"), productproperty.Name);
                }
                else
                {
                    //activity log unactive
                    _customerActivityService.InsertActivity("UnactiveProductProperty", _localizationService.GetResource("ActivityLog.UnactiveProductProperty"), productproperty.Name);
                }
                _categoryService.UpdateProductProperty(productproperty);

                //Update htmlcompare
                ProductPropertyGroup ppg = _categoryService.GetProductPropertyGroupById(productproperty.GroupId);
                UpdateComparationTemplate(ppg.CategoryId);

                //activity log
                _customerActivityService.InsertActivity("AddNewProductProperty", _localizationService.GetResource("ActivityLog.AddNewProductProperty"), productproperty.Name);
            }

            return ProductPropertyList(command, productproperty.GroupId);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var productproperty = _categoryService.GetProductPropertyById(id);
            if (productproperty == null)
                throw new ArgumentException("No product category mapping found with the specified id");

            var groupid = productproperty.GroupId;
            productproperty.IsDeleted = true;
            _categoryService.DeleteProductProperty(productproperty);

            //Update htmlcompare
            ProductPropertyGroup ppg = _categoryService.GetProductPropertyGroupById(productproperty.GroupId);
            UpdateComparationTemplate(ppg.CategoryId);

            //activity log delete
            _customerActivityService.InsertActivity("DeleteProductProperty", _localizationService.GetResource("ActivityLog.DeleteProductProperty"), productproperty.Name);
            return ProductPropertyList(command, groupid);
        }

        public ActionResult ProductPropertyAddPopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var model = new CategoryModel.ProductPropertyModel();
            if (Request["groupId"] != null)
                model.GroupId = Convert.ToInt32(Request["groupId"]);
            //locales
            //AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult ProductPropertyAddPopup(CategoryModel.ProductPropertyModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
            //if (ModelState.IsValid)
            //{
            ProductProperty productproperty = new ProductProperty();
            productproperty.CreatedAt = DateTime.UtcNow;
            productproperty.IsActived = true;
            productproperty.GroupId = model.GroupId;
            productproperty.Name = model.Name;
            productproperty.DisplayOrder = model.DisplayOrder1;
            productproperty.IsMapping = model.IsMapping;
            _categoryService.InsertProductProperty(productproperty);
            //}
            ViewBag.RefreshPage = true;
            //ViewBag.btnId = btnId;
            //ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Product Property Value

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyValueList(GridCommand command, int propertyId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var productPropertyValues = _categoryService.GetProductPropertyValuesByProductPropertyIdForCategory(propertyId);
            var productPropertyValueModels = productPropertyValues
                .Select(x =>
                {
                    return new CategoryModel.ProductPropertyValueModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ProductPropertyId = x.PropertyId,
                        Value = x.Value,
                        IsActived = x.IsActived,
                        DisplayOrder1 = x.DisplayOrder

                    };
                })
                .ToList();
            var model = new GridModel<CategoryModel.ProductPropertyValueModel>
            {
                Data = productPropertyValueModels,
                Total = productPropertyValueModels.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyValueInsert(GridCommand command, CategoryModel.ProductPropertyValueModel model, int propertyId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            int intpropertyId = model.ProductPropertyId;
            if (intpropertyId == 0)
                intpropertyId = propertyId;
            ProductPropertyValue productpropertyvalue = new ProductPropertyValue();
            productpropertyvalue.CreatedAt = DateTime.UtcNow;
            productpropertyvalue.IsActived = model.IsActived;
            if (model.IsActived)
                productpropertyvalue.ActivedAt = DateTime.UtcNow;
            productpropertyvalue.PropertyId = intpropertyId;
            productpropertyvalue.Name = model.Name;
            productpropertyvalue.DisplayOrder = model.DisplayOrder1;
            productpropertyvalue.Value = model.Value;
            _categoryService.InsertProductPropertyValue(productpropertyvalue);
            //activity log insert
            _customerActivityService.InsertActivity("AddNewProductPropertyValue", _localizationService.GetResource("ActivityLog.AddNewProductPropertyValue"), productpropertyvalue.Name);

            return ProductPropertyValueList(command, intpropertyId);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyValueUpdate(GridCommand command, CategoryModel.ProductPropertyValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
            var productpropertyvalue = _categoryService.GetProductPropertyValueById(model.Id);
            if (productpropertyvalue == null)
                throw new ArgumentException("No product properties value category mapping found with the specified id");

            var propertyId = model.ProductPropertyId;
            productpropertyvalue.CreatedAt = DateTime.UtcNow;
            productpropertyvalue.IsActived = model.IsActived;
            if (model.IsActived)
            {
                productpropertyvalue.ActivedAt = DateTime.UtcNow;
                //activity log active
                _customerActivityService.InsertActivity("ActiveProductPropertyValue", _localizationService.GetResource("ActivityLog.ActiveProductPropertyValue"), productpropertyvalue.Name);
            }
            else
            {
                //activity log unactive
                _customerActivityService.InsertActivity("UnactiveProductPropertyValue", _localizationService.GetResource("ActivityLog.UnactiveProductPropertyValue"), productpropertyvalue.Name);
            }
            productpropertyvalue.PropertyId = model.ProductPropertyId;
            productpropertyvalue.Name = model.Name;
            productpropertyvalue.DisplayOrder = model.DisplayOrder1;
            productpropertyvalue.Value = model.Value;
            _categoryService.UpdateProductPropertyValue(productpropertyvalue);

            //activity log update
            _customerActivityService.InsertActivity("EditProductPropertyValue", _localizationService.GetResource("ActivityLog.EditProductPropertyValue"), productpropertyvalue.Name);
            return ProductPropertyValueList(command, propertyId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPropertyValueDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var productpropertyvalue = _categoryService.GetProductPropertyValueById(id);
            if (productpropertyvalue == null)
                throw new ArgumentException("No product properties value category mapping found with the specified id");

            var propertyId = productpropertyvalue.PropertyId;
            productpropertyvalue.IsDeleted = true;
            _categoryService.DeleteProductPropertyValue(productpropertyvalue);

            //activity log delete
            _customerActivityService.InsertActivity("DeleteProductPropertyValue", _localizationService.GetResource("ActivityLog.DeleteProductPropertyValue"), productpropertyvalue.Name);

            return ProductPropertyValueList(command, propertyId);
        }


        #endregion

        #region Product Property Value Mapping

        //[ChildActionOnly]
        //public ActionResult ValueMapping(int PropertyId)
        //{
        //    var productproperty = _categoryService.GetProductPropertyById(PropertyId);
        //    return PartialView();
        //}

        public JsonResult ValueMapping(int Id)
        {
            RouteData.Values["propertyId"] = Id;
            ViewData["propertyId"] = Id;
            var productproperty = _categoryService.GetProductPropertyById(Id);
            return Json(new { productpropertyvalue = productproperty });
        }

        public ActionResult LoadProductPropertyValueByPropertyId(int productpropertyId)
        {
            var productproperty = _categoryService.GetProductPropertyById(productpropertyId);
            ViewBag.productpropertyName = productproperty.Name;
            ViewBag.productpropertyId = productpropertyId;
            return View();
        }

        public ActionResult DataTemp(int productpropertyvalueId)
        {
            var ppvsource = _categoryService.GetProductPropertyValueById(productpropertyvalueId);
            var lstppvm = _categoryService.GetListProductPropertyValueMapping(productpropertyvalueId);
            var lstppvmmodel = new List<CategoryModel.ProductPropertyValueMappingModel>();
            GridModel<CategoryModel.ProductPropertyValueMappingModel> data = new GridModel<CategoryModel.ProductPropertyValueMappingModel>();
            if (lstppvm != null)
            {
                foreach (var ppvp in lstppvm)
                {
                    var ppvdestination = _categoryService.GetProductPropertyValueById(ppvp.DestinationId);
                    var temp = new CategoryModel.ProductPropertyValueMappingModel()
                    {
                        SourceId = productpropertyvalueId,
                        SourceName = ppvsource.Name,
                        DestinationId = ppvdestination.Id,
                        DestinationName = ppvdestination.Name,
                        MappingType = ppvp.MapType
                    };
                    lstppvmmodel.Add(temp);
                }
                data = new GridModel<CategoryModel.ProductPropertyValueMappingModel> { Data = lstppvmmodel };
            }
            return View("~/Administration/Views/Category/GridProductPropertyValueMapping.cshtml", data);
        }


        [GridAction]
        public ActionResult GridProductPropertyValueMapping(GridModel<CategoryModel.ProductPropertyValueMappingModel> data)
        {
            return View(data);
        }

        [ChildActionOnly]
        public ActionResult InsertProductPropertyValueMapping(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
            var productPropertyValues = _categoryService.GetProductPropertyValuesByProductPropertyIdForCategory(Id);
            var productPropertyValueModels = productPropertyValues
                .Select(x =>
                {
                    return new CategoryModel.ProductPropertyValueModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ProductPropertyId = x.PropertyId,
                        Value = x.Value,
                        IsActived = x.IsActived,
                        DisplayOrder1 = x.DisplayOrder

                    };
                })
                .ToList();

            ViewBag.ProductPropertyValueList = productPropertyValueModels;
            return PartialView();
        }

        [HttpPost]
        public ActionResult InsertProductPropertyValueMapping(CategoryModel.ProductPropertyValueMappingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            ProductPropertyValueMapping productpropertyvaluemapping = new ProductPropertyValueMapping();
            productpropertyvaluemapping.SourceId = model.SourceId;
            productpropertyvaluemapping.DestinationId = model.DestinationId;
            productpropertyvaluemapping.MapType = model.MappingType;
            _categoryService.InsertProductPropertyValueMapping(productpropertyvaluemapping);

            return View();
        }

        public JsonResult GetProductPropertyByCategoryId(int CategoryId)
        {
            List<ProductProperty> lstproductproperty = _categoryService.GetProductPropertiesByProductCategoryId(CategoryId);
            var productproperty = new List<SelectListItem>();
            foreach (var c in lstproductproperty)
            {
                var item = new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                };
                productproperty.Add(item);
            }
            return Json(new { data = productproperty }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductPropertyValueByPropertyId(int propertyId)
        {
            List<ProductPropertyValue> lstproductpropertyvalue = _categoryService.GetProductPropertyValuesByProductPropertyId(propertyId);
            var productpropertyvalue = new List<SelectListItem>();
            foreach (var c in lstproductpropertyvalue)
            {
                var item = new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                };
                productpropertyvalue.Add(item);
            }
            return Json(new { data = productpropertyvalue }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
