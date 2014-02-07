using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.News;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;
using Nop.Services.Media;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public partial class NewsController : BaseNopController
	{
		#region Fields

        private readonly INewsService _newsService;
        private readonly INewsCategoryService _newsCategoryService;
        private readonly ILanguageService _languageService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IPictureService _pictureService;
        
		#endregion

		#region Constructors

        public NewsController(INewsService newsService,
            INewsCategoryService newsCategoryService, 
            ILanguageService languageService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService,
            IUrlRecordService urlRecordService,
            IStoreService storeService,
            AdminAreaSettings adminAreaSettings,
            IStoreMappingService storeMappingService,
            IPictureService pictureService)
        {
            this._newsService = newsService;
            this._newsCategoryService = newsCategoryService;
            this._languageService = languageService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._permissionService = permissionService;
            this._urlRecordService = urlRecordService;
            this._storeService = storeService;
            this._adminAreaSettings = adminAreaSettings;
            this._storeMappingService = storeMappingService;
            this._pictureService = pictureService;
		}

		#endregion 

        #region Utilities

        [NonAction]
        private void PrepareStoresMappingModel(NewsItemModel model, NewsItem newsItem, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableStores = _storeService
                .GetAllStores()
                .Select(s => s.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (newsItem != null)
                {
                    model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(newsItem);
                }
                else
                {
                    model.SelectedStoreIds = new int[0];
                }
            }
        }

        [NonAction]
        protected void SaveStoreMappings(NewsItem newsItem, NewsItemModel model)
        {
            var existingStoreMappings = _storeMappingService.GetStoreMappings(newsItem);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds != null && model.SelectedStoreIds.Contains(store.Id))
                {
                    //new role
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(newsItem, store.Id);
                }
                else
                {
                    //removed role
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        [NonAction]
        protected void UpdateLocales(NewsCategory category, NewsCategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(category,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.Description,
                                                           localized.Description,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                //search engine name
                var seName = category.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(category, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected void UpdatePictureSeoNames(NewsCategory category)
        {
            var picture = _pictureService.GetPictureById(category.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(category.Name));
        }

        [NonAction]
        private void PrepareCategoryMapping(NewsItemModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.NumberOfAvailableCategories = _newsCategoryService.GetAllNewsCategories(showHidden: true).Count;
        }

        #endregion

        #region News items

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var model = new NewsItemListModel();
            //stores
            model.AvailableStores.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });

            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command, NewsItemListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var news = _newsService.GetAllNews(0, model.SearchStoreId, command.Page - 1, command.PageSize, true);
            var gridModel = new GridModel<NewsItemModel>
            {
                Data = news.Select(x =>
                {
                    var m = x.ToModel();
                    if (x.StartDateUtc.HasValue)
                        m.StartDate = _dateTimeHelper.ConvertToUserTime(x.StartDateUtc.Value, DateTimeKind.Utc);
                    if (x.EndDateUtc.HasValue)
                        m.EndDate = _dateTimeHelper.ConvertToUserTime(x.EndDateUtc.Value, DateTimeKind.Utc);
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    m.LanguageName = x.Language.Name;
                    m.Comments = x.CommentCount;
                    return m;
                }),
                Total = news.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            var model = new NewsItemModel();
            //Stores
            PrepareStoresMappingModel(model, null, false);
            PrepareCategoryMapping(model);
            //default values
            model.Published = true;
            model.AllowComments = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(NewsItemModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var newsItem = model.ToEntity();
                newsItem.StartDateUtc = model.StartDate;
                newsItem.EndDateUtc = model.EndDate;
                newsItem.CreatedOnUtc = DateTime.UtcNow;
                _newsService.InsertNews(newsItem);
                
                //search engine name
                var seName = newsItem.ValidateSeName(model.SeName, model.Title, true);
                _urlRecordService.SaveSlug(newsItem, seName, newsItem.LanguageId);

                //Stores
                SaveStoreMappings(newsItem, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = newsItem.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            //Stores
            PrepareStoresMappingModel(model, null, true);
            PrepareCategoryMapping(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItem = _newsService.GetNewsById(id);
            if (newsItem == null)
                //No news item found with the specified id
                return RedirectToAction("List");

            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            var model = newsItem.ToModel();
            model.StartDate = newsItem.StartDateUtc;
            model.EndDate = newsItem.EndDateUtc;
            //Store
            PrepareStoresMappingModel(model, newsItem, false);
            PrepareCategoryMapping(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(NewsItemModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItem = _newsService.GetNewsById(model.Id);
            if (newsItem == null)
                //No news item found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                newsItem = model.ToEntity(newsItem);
                newsItem.StartDateUtc = model.StartDate;
                newsItem.EndDateUtc = model.EndDate;
                newsItem.PictureId = model.PictureId;
                _newsService.UpdateNews(newsItem);

                //search engine name
                var seName = newsItem.ValidateSeName(model.SeName, model.Title, true);
                _urlRecordService.SaveSlug(newsItem, seName, newsItem.LanguageId);

                //Stores
                SaveStoreMappings(newsItem, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = newsItem.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            //Store
            PrepareStoresMappingModel(model, newsItem, true);
            PrepareCategoryMapping(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItem = _newsService.GetNewsById(id);
            if (newsItem == null)
                //No news item found with the specified id
                return RedirectToAction("List");

            _newsService.DeleteNews(newsItem);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult NewsItemList(GridCommand command, int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItemCategories = _newsCategoryService.GetNewsItemCategoriesByCategoryId(categoryId,
                command.Page - 1, command.PageSize, true);
            var model = new GridModel<NewsCategoryModel.CategoryNewsItemModel>
            {
                Data = newsItemCategories
                .Select(x =>
                {
                    return new NewsCategoryModel.CategoryNewsItemModel()
                    {
                        Id = x.Id,
                        CategoryId = x.CategoryId,
                        NewsId = x.NewsId,
                        Title = _newsService.GetNewsById(x.NewsId).Title,
                        IsFeatured = x.IsFeatured,
                        DisplayOrder1 = x.DisplayOrder
                    };
                }),
                Total = newsItemCategories.TotalCount
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult NewsItemUpdate(GridCommand command, NewsCategoryModel.CategoryNewsItemModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItemCategory = _newsCategoryService.GetNewsItemCategoryById(model.Id);
            if (newsItemCategory == null)
                throw new ArgumentException("No newsItem category mapping found with the specified id");

            newsItemCategory.IsFeatured = model.IsFeatured;
            newsItemCategory.DisplayOrder = model.DisplayOrder1;
            _newsCategoryService.UpdateNewsItemCategory(newsItemCategory);

            return NewsItemList(command, newsItemCategory.CategoryId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult NewsItemDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItemCategory = _newsCategoryService.GetNewsItemCategoryById(id);
            if (newsItemCategory == null)
                throw new ArgumentException("No NewsItem category mapping found with the specified id");

            var categoryId = newsItemCategory.CategoryId;
            _newsCategoryService.DeleteNewsItemCategory(newsItemCategory);

            return NewsItemList(command, categoryId);
        }

        #endregion

        #region News categories

        public ActionResult CategoryList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var model = new NewsCategoryListModel();
            var categories = _newsCategoryService.GetAllNewsCategories(null, 0, _adminAreaSettings.GridPageSize, true);
            model.NewsCategories = new GridModel<NewsCategoryModel>
            {
                Data = categories.Select(x =>
                {
                    var categoryModel = x.ToModel();
                    categoryModel.Breadcrumb = x.GetCategoryBreadCrumb(_newsCategoryService);
                    return categoryModel;
                }),
                Total = categories.TotalCount
            };
            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult CategoryList(GridCommand command, NewsCategoryListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var categories = _newsCategoryService.GetAllNewsCategories(model.SearchCategoryName,
                command.Page - 1, command.PageSize, true);
            var gridModel = new GridModel<NewsCategoryModel>
            {
                Data = categories.Select(x =>
                {
                    var categoryModel = x.ToModel();
                    categoryModel.Breadcrumb = x.GetCategoryBreadCrumb(_newsCategoryService);
                    return categoryModel;
                }),
                Total = categories.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        //ajax
        public ActionResult AllCategories(string text, int selectedId)
        {
            var categories = _newsCategoryService.GetAllNewsCategories(showHidden: true);
            categories.Insert(0, new NewsCategory { Name = "[None]", Id = 0 });
            var selectList = new List<SelectListItem>();
            foreach (var c in categories)
                selectList.Add(new SelectListItem()
                {
                    Value = c.Id.ToString(),
                    Text = c.GetCategoryBreadCrumb(_newsCategoryService),
                    Selected = c.Id == selectedId
                });

            //var selectList = new SelectList(categories, "Id", "Name", selectedId);
            return new JsonResult { Data = selectList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult CreateCategory()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var model = new NewsCategoryModel();
            //parent categories
            model.ParentCategories = new List<DropDownItem> { new DropDownItem { Text = "[None]", Value = "0" } };
            //locales
            AddLocales(_languageService, model.Locales);
            //default values
            model.PageSize = 4;
            model.Published = true;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult CreateCategory(NewsCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var category = model.ToEntity();
                category.CreatedOnUtc = DateTime.UtcNow;
                category.UpdatedOnUtc = DateTime.UtcNow;
                _newsCategoryService.InsertNewsCategory(category);
                //search engine name
                model.SeName = category.ValidateSeName(model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);
                //locales
                UpdateLocales(category, model);
                _newsCategoryService.UpdateNewsCategory(category);
                //update picture seo file name
                UpdatePictureSeoNames(category);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = category.Id }) : RedirectToAction("CategoryList");
            }

            //parent categories
            model.ParentCategories = new List<DropDownItem> { new DropDownItem { Text = "[None]", Value = "0" } };
            if (model.ParentCategoryId > 0)
            {
                var parentCategory = _newsCategoryService.GetNewsCategoryById(model.ParentCategoryId);
                if (parentCategory != null && !parentCategory.Deleted)
                    model.ParentCategories.Add(new DropDownItem { Text = parentCategory.GetCategoryBreadCrumb(_newsCategoryService), Value = parentCategory.Id.ToString() });
                else
                    model.ParentCategoryId = 0;
            }
            return View(model);
        }

        public ActionResult EditCategory(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var category = _newsCategoryService.GetNewsCategoryById(id);
            if (category == null || category.Deleted)
                //No category found with the specified id
                return RedirectToAction("List");

            var model = category.ToModel();
            //parent categories
            model.ParentCategories = new List<DropDownItem> { new DropDownItem { Text = "[None]", Value = "0" } };
            if (model.ParentCategoryId > 0)
            {
                var parentCategory = _newsCategoryService.GetNewsCategoryById(model.ParentCategoryId);
                if (parentCategory != null && !parentCategory.Deleted)
                    model.ParentCategories.Add(new DropDownItem { Text = parentCategory.GetCategoryBreadCrumb(_newsCategoryService), Value = parentCategory.Id.ToString() });
                else
                    model.ParentCategoryId = 0;
            }
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = category.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = category.GetLocalized(x => x.Description, languageId, false, false);
                locale.MetaKeywords = category.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = category.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = category.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = category.GetSeName(languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult EditCategory(NewsCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var category = _newsCategoryService.GetNewsCategoryById(model.Id);
            if (category == null || category.Deleted)
                //No category found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                int prevPictureId = category.PictureId;
                category = model.ToEntity(category);
                category.UpdatedOnUtc = DateTime.UtcNow;
                _newsCategoryService.UpdateNewsCategory(category);
                //search engine name
                model.SeName = category.ValidateSeName(model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);
                //locales
                UpdateLocales(category, model);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != category.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                //update picture seo file name
                UpdatePictureSeoNames(category);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Updated"));
                return continueEditing ? RedirectToAction("Edit", category.Id) : RedirectToAction("CategoryList");
            }


            //If we got this far, something failed, redisplay form
            //parent categories
            model.ParentCategories = new List<DropDownItem> { new DropDownItem { Text = "[None]", Value = "0" } };
            if (model.ParentCategoryId > 0)
            {
                var parentCategory = _newsCategoryService.GetNewsCategoryById(model.ParentCategoryId);
                if (parentCategory != null && !parentCategory.Deleted)
                    model.ParentCategories.Add(new DropDownItem { Text = parentCategory.GetCategoryBreadCrumb(_newsCategoryService), Value = parentCategory.Id.ToString() });
                else
                    model.ParentCategoryId = 0;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var category = _newsCategoryService.GetNewsCategoryById(id);
            if (category == null)
                //No category found with the specified id
                return RedirectToAction("CategoryList");

            _newsCategoryService.DeleteNewsCategory(category);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Deleted"));
            return RedirectToAction("CategoryList");
        }
        #endregion

        #region NewsItem categories
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult NewsCategoryList(GridCommand command, int newsId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsCategories = _newsCategoryService.GetNewsItemCategoriesByNewsId(newsId, true);
            var newsCategoriesModel = newsCategories
                .Select(x =>
                {
                    return new NewsItemModel.NewsCategoryModel()
                    {
                        Id = x.Id,
                        Category = _newsCategoryService.GetNewsCategoryById(x.CategoryId).GetCategoryBreadCrumb(_newsCategoryService),
                        NewsId = x.NewsId,
                        CategoryId = x.CategoryId,
                        IsFeatured = x.IsFeatured,
                        DisplayOrder = x.DisplayOrder
                    };
                })
                .ToList();

            var model = new GridModel<NewsItemModel.NewsCategoryModel>
            {
                Data = newsCategoriesModel,
                Total = newsCategoriesModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult NewsCategoryInsert(GridCommand command, NewsItemModel.NewsCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsId = model.NewsId;
            var categoryId = Int32.Parse(model.Category); //use Category property (not CategoryId) because appropriate property is stored in it

            var existingNewsCategories = _newsCategoryService.GetNewsItemCategoriesByCategoryId(categoryId, 0, int.MaxValue, true);
            if (existingNewsCategories.FindNewsItemCategory(newsId, categoryId) == null)
            {
                var newsItemCategory = new NewsItemCategory()
                {
                    NewsId = newsId,
                    CategoryId = categoryId,
                    DisplayOrder = model.DisplayOrder
                };
                newsItemCategory.IsFeatured = model.IsFeatured;
                _newsCategoryService.InsertNewsItemCategory(newsItemCategory);
            }

            return NewsCategoryList(command, newsId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult NewsCategoryUpdate(GridCommand command, NewsItemModel.NewsCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItemCategory = _newsCategoryService.GetNewsItemCategoryById(model.Id);
            if (newsItemCategory == null)
                throw new ArgumentException("No newsitem category mapping found with the specified id");

            //use Category property (not CategoryId) because appropriate property is stored in it
            newsItemCategory.CategoryId = Int32.Parse(model.Category);
            newsItemCategory.DisplayOrder = model.DisplayOrder;
            newsItemCategory.IsFeatured = model.IsFeatured;
            _newsCategoryService.UpdateNewsItemCategory(newsItemCategory);

            return NewsCategoryList(command, newsItemCategory.NewsId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult NewsCategoryDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItemCategory = _newsCategoryService.GetNewsItemCategoryById(id);
            if (newsItemCategory == null)
                throw new ArgumentException("No newsItem category mapping found with the specified id");

            var newsId = newsItemCategory.NewsId;

            _newsCategoryService.DeleteNewsItemCategory(newsItemCategory);

            return NewsCategoryList(command, newsId);
        }
        #endregion

        #region Comments

        public ActionResult Comments(int? filterByNewsItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            ViewBag.FilterByNewsItemId = filterByNewsItemId;
            return View();
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Comments(int? filterByNewsItemId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            IList<NewsComment> comments;
            if (filterByNewsItemId.HasValue)
            {
                //filter comments by news item
                var newsItem = _newsService.GetNewsById(filterByNewsItemId.Value);
                comments = newsItem.NewsComments.OrderBy(bc => bc.CreatedOnUtc).ToList();
            }
            else
            {
                //load all news comments
                comments = _newsService.GetAllComments(0);
            }

            var gridModel = new GridModel<NewsCommentModel>
            {
                Data = comments.PagedForCommand(command).Select(newsComment =>
                {
                    var commentModel = new NewsCommentModel();
                    commentModel.Id = newsComment.Id;
                    commentModel.NewsItemId = newsComment.NewsItemId;
                    commentModel.NewsItemTitle = newsComment.NewsItem.Title;
                    commentModel.CustomerId = newsComment.CustomerId;
                    var customer = newsComment.Customer;
                    commentModel.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    commentModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(newsComment.CreatedOnUtc, DateTimeKind.Utc);
                    commentModel.CommentTitle = newsComment.CommentTitle;
                    commentModel.CommentText = Core.Html.HtmlHelper.FormatText(newsComment.CommentText, false, true, false, false, false, false);
                    return commentModel;
                }),
                Total = comments.Count,
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult CommentDelete(int? filterByNewsItemId, int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var comment = _newsService.GetNewsCommentById(id);
            if (comment == null)
                throw new ArgumentException("No comment found with the specified id");

            var newsItem = comment.NewsItem;
            _newsService.DeleteNewsComment(comment);
            //update totals
            newsItem.CommentCount = newsItem.NewsComments.Count;
            _newsService.UpdateNews(newsItem);

            return Comments(filterByNewsItemId, command);
        }


        #endregion
    }
}
