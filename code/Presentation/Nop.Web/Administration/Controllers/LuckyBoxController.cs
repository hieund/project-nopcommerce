using Nop.Admin.Models.Game;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Game;
using Nop.Services.Customers;
using Nop.Services.Game;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Web.Framework;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class LuckyBoxController : BaseNopController
    {
        #region Fields

        private readonly ILuckyBoxService _luckyBoxService;
        private readonly ICustomerService _customerService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructors

        public LuckyBoxController(ILuckyBoxService luckyBoxService,
            ICustomerService customerService, IPictureService pictureService,
            IUrlRecordService urlRecordService, IPermissionService permissionService,
            ILocalizationService localizationService, IAclService aclService,
            ICustomerActivityService customerActivityService, AdminAreaSettings adminAreaSettings,
            CatalogSettings catalogSettings)
        {
            this._luckyBoxService = luckyBoxService;
            this._customerService = customerService;
            this._pictureService = pictureService;
            this._urlRecordService = urlRecordService;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
            this._aclService = aclService;
            this._customerActivityService = customerActivityService;
            this._adminAreaSettings = adminAreaSettings;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Actions

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGames) && !_permissionService.Authorize(StandardPermissionProvider.ExecuteGames))
                return AccessDeniedView();

            var model = new LuckyBoxListModel();
            var luckyboxes = _luckyBoxService.GetAllLuckyBox();
            model.LuckyBoxes = new GridModel<LuckyBoxModel>
            {
                Data = luckyboxes.Select(x => x.ToModel())
            };
            return View(model);
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGames))
                return AccessDeniedView();
            ViewBag.AllowManageGames = _permissionService.Authorize(StandardPermissionProvider.ManageGames);
            var model = new LuckyBoxModel();

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(LuckyBoxModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGames))
                return AccessDeniedView();
            ViewBag.AllowManageGames = _permissionService.Authorize(StandardPermissionProvider.ManageGames);
            if (ModelState.IsValid)
            {
                var luckyBox = model.ToEntity();
                luckyBox.CreatedOnUtc = DateTime.Now;
                luckyBox.UpdatedOnUtc = DateTime.Now;

                _luckyBoxService.InsertLuckyBox(luckyBox);

                //activity log
                _customerActivityService.InsertActivity("AddNewLuckyBox", _localizationService.GetResource("ActivityLog.AddNewLuckyBox"), luckyBox.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Game.LuckyBox.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = luckyBox.Id }) : RedirectToAction("List");
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGames))
                return AccessDeniedView();

            var luckyBox = _luckyBoxService.GetLuckyBoxById(id);
            if (luckyBox == null)
                //No manufacturer found with the specified id
                return RedirectToAction("List");

            _luckyBoxService.DeleteLuckyBox(luckyBox);

            //activity log
            _customerActivityService.InsertActivity("DeleteLuckyBox", _localizationService.GetResource("ActivityLog.DeleteLuckyBox"), luckyBox.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Game.LuckyBox.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ListDraws1(int luckyBoxId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGames))
                return AccessDeniedView();

            var model = new List<LuckyBoxDrawModel>();
            var luckyBoxDraws = _luckyBoxService.GetAllLuckyBoxDrawsByLuckyBoxId(luckyBoxId);
            model = luckyBoxDraws.Select(x =>
            {
                return new LuckyBoxDrawModel()
                {
                    Id = x.Id,
                    LuckyBoxId = x.LuckyBoxId,
                    GiftName = x.GiftName,
                    CreatedOnUtd = x.CreatedOnUtc,
                    CustomerAddress = x.CustomerAddress,
                    CustomerEmail = x.CustomerEmail,
                    CustomerName  = x.CustomerName,
                    CustomerPhone = x.CustomerPhone,
                    LuckyBoxCode = x.LuckyBoxCode,
                    LuckyBoxDetailId = x.LuckyBoxDetailId,
                    ReceiptNo = x.ReceiptNo
                };
            }).ToList();
            return new JsonResult
            {
                Data = model
            };
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ListDraws(int luckyBoxId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGames) && !_permissionService.Authorize(StandardPermissionProvider.ExecuteGames))
                return AccessDeniedView();

            var model = new List<LuckyBoxDrawModel>();
            var luckyBoxDraws = _luckyBoxService.GetAllLuckyBoxDrawsByLuckyBoxId(luckyBoxId);
            model = luckyBoxDraws.Select(x =>
            {
                return new LuckyBoxDrawModel()
                {
                    Id = x.Id,
                    LuckyBoxId = x.LuckyBoxId,
                    GiftName = x.GiftName,
                    CreatedOnUtd = x.CreatedOnUtc,
                    CustomerAddress = x.CustomerAddress,
                    CustomerEmail = x.CustomerEmail,
                    CustomerName = x.CustomerName,
                    CustomerPhone = x.CustomerPhone,
                    LuckyBoxCode = x.LuckyBoxCode,
                    LuckyBoxDetailId = x.LuckyBoxDetailId,
                    ReceiptNo = x.ReceiptNo,
                    IsCompleted = x.IsCompleted
                };
            })
            .ForCommand(command)
            .ToList();
            var gridmodel = new GridModel<LuckyBoxDrawModel>
            {
                Data = model.PagedForCommand(command),
                Total = model.Count
            };
            return new JsonResult
            {
                Data = gridmodel
            };
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ListGifts(int luckyBoxId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGames))
                return AccessDeniedView();

            var model = new List<LuckyBoxDetailModel>();
            var luckyBoxGifts = _luckyBoxService.GetAllLuckyBoxDetailsByLuckyBoxId(luckyBoxId);
            model = luckyBoxGifts.Select(x =>
                {
                    return new LuckyBoxDetailModel()
                    {
                        Id = x.Id,
                        LuckyBoxId = x.LuckyBoxId,
                        GiftName = x.GiftName,
                        PictureId = x.PictureId,
                        PictureThumbnailUrl = _pictureService.GetPictureUrl(x.PictureId),
                        NumberPerCycle = x.NumberPerCycle
                    };
                }).ToList();
            var gridmodel = new GridModel<LuckyBoxDetailModel> { 
                Data = model,
                Total = model.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdateGift(LuckyBoxDetailModel luckyBoxDetailModel, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGames))
                return AccessDeniedView();
            var luckyBox = _luckyBoxService.GetLuckyBoxById(luckyBoxDetailModel.LuckyBoxId);
            var luckyBoxDetails = _luckyBoxService.GetAllLuckyBoxDetailsByLuckyBoxId(luckyBoxDetailModel.LuckyBoxId);
            var luckyBoxDetailOld = _luckyBoxService.GetLuckyBoxDetailById(luckyBoxDetailModel.Id);
            int totalGifts = luckyBoxDetails.Sum(lbd => lbd.NumberPerCycle);
            if (totalGifts - luckyBoxDetailOld.NumberPerCycle + luckyBoxDetailModel.NumberPerCycle > luckyBox.WinCycle)
            {
                Response.StatusCode = 500;
                return Content("Tổng số quà (" + (totalGifts - luckyBoxDetailOld.NumberPerCycle + luckyBoxDetailModel.NumberPerCycle) + ") không thể lớn hơn tổng số lần chơi (" + luckyBox.WinCycle + ")");
            }
            var luckyBoxGift = _luckyBoxService.GetLuckyBoxDetailById(luckyBoxDetailModel.Id);
            luckyBoxGift.GiftName = luckyBoxDetailModel.GiftName;
            luckyBoxGift.NumberPerCycle = luckyBoxDetailModel.NumberPerCycle;
            luckyBoxGift.UpdatedOnUtc = DateTime.Now;

            _luckyBoxService.UpdateLuckyBoxDetail(luckyBoxGift);

            //activity log
            _customerActivityService.InsertActivity("UpdateLuckyBoxDetail", _localizationService.GetResource("ActivityLog.UpdateLuckyBoxDetail"), luckyBoxGift.GiftName);

            return ListGifts(luckyBoxGift.LuckyBoxId);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult DeleteGift(LuckyBoxDetailModel luckyBoxDetailModel, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGames))
                return AccessDeniedView();

            var luckyBoxGift = _luckyBoxService.GetLuckyBoxDetailById(luckyBoxDetailModel.Id);
            luckyBoxGift.UpdatedOnUtc = DateTime.Now;
            luckyBoxGift.Deleted = true;

            _luckyBoxService.UpdateLuckyBoxDetail(luckyBoxGift);

            //activity log
            _customerActivityService.InsertActivity("DeleteLuckyBoxDetail", _localizationService.GetResource("ActivityLog.DeleteLuckyBoxDetail"), luckyBoxGift.GiftName);

            return ListGifts(luckyBoxGift.LuckyBoxId);
        }
        public ActionResult AddGift(int pictureId, int numberPerCycle,string giftName, int luckyBoxId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGames))
                return AccessDeniedView();

            var luckyBox = _luckyBoxService.GetLuckyBoxById(luckyBoxId);
            var luckyBoxDetails = _luckyBoxService.GetAllLuckyBoxDetailsByLuckyBoxId(luckyBoxId);
            int totalGifts = luckyBoxDetails.Sum(lbd => lbd.NumberPerCycle);
            if (totalGifts + numberPerCycle > luckyBox.WinCycle)
            {
                Response.StatusCode = 500;
                return Content("Tổng số quà (" + (totalGifts + numberPerCycle) + ") không thể lớn hơn tổng số lần chơi (" + luckyBox.WinCycle + ")");
            }

            var luckyBoxDetail = new LuckyBoxDetail();
            luckyBoxDetail.LuckyBoxId = luckyBoxId;
            luckyBoxDetail.NumberPerCycle = numberPerCycle;
            luckyBoxDetail.PictureId = pictureId;
            luckyBoxDetail.GiftName = giftName;
            luckyBoxDetail.UpdatedOnUtc = DateTime.Now;
            luckyBoxDetail.CreatedOnUtc = DateTime.Now;
            luckyBoxDetail.Deleted = false;

            _luckyBoxService.InsertLuckyBoxDetail(luckyBoxDetail);
            //activity log
            _customerActivityService.InsertActivity("AddLuckyBoxDetail", _localizationService.GetResource("ActivityLog.AddLuckyBoxDetail"), luckyBoxDetail.GiftName);

            return new JsonResult
            {
                Data = 1
            };
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGames) && !_permissionService.Authorize(StandardPermissionProvider.ExecuteGames))
                return AccessDeniedView();
            ViewBag.AllowManageGames = _permissionService.Authorize(StandardPermissionProvider.ManageGames);
            var luckyBox = _luckyBoxService.GetLuckyBoxById(id);
            if (luckyBox == null || luckyBox.Deleted)
                //No manufacturer found with the specified id
                return RedirectToAction("List");

            var model = luckyBox.ToModel();
            model.AddGiftModel = new LuckyBoxDetailModel { LuckyBoxId = id };
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(LuckyBoxModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGames))
                return AccessDeniedView();
            ViewBag.AllowManageGames = _permissionService.Authorize(StandardPermissionProvider.ManageGames);
            var luckyBox = _luckyBoxService.GetLuckyBoxById(model.Id);
            if (luckyBox == null || luckyBox.Deleted)
                //No manufacturer found with the specified id
                return RedirectToAction("List");

            var luckyBoxDetails = _luckyBoxService.GetAllLuckyBoxDetailsByLuckyBoxId(model.Id);
            int totalGifts = luckyBoxDetails.Sum(lbd => lbd.NumberPerCycle);
            if (totalGifts > model.WinCycle)
            {
                Response.StatusCode = 500;
                return Content("Tổng số quà (" + (totalGifts) + ") không thể lớn hơn tổng số lần chơi (" + model.WinCycle + ")");
            }

            if (ModelState.IsValid)
            {
                luckyBox = model.ToEntity(luckyBox);
                luckyBox.UpdatedOnUtc = DateTime.UtcNow;
                _luckyBoxService.UpdateLuckyBox(luckyBox);

                //activity log
                _customerActivityService.InsertActivity("EditLuckyBox", _localizationService.GetResource("ActivityLog.EditLuckyBox"), luckyBox.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Game.LuckyBox.Updated"));
                return continueEditing ? RedirectToAction("Edit", luckyBox.Id) : RedirectToAction("List");
            }

            return View(model);
        }

        #endregion
    }
}
