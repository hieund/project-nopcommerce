using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.Mvc;
using Nop.Admin.Models.Game;
using Nop.Core.Domain.Game;
using Telerik.Web.Mvc;
using Nop.Services.Game;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Security;
using Nop.Services.Logging;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using System.Text.RegularExpressions;
using System.Text;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class GameController : BaseNopController
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

        public GameController(ILuckyBoxService luckyBoxService,
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

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExecuteGames))
                return AccessDeniedView();

            var model = new LuckyBoxListModel();
            var luckyboxes = _luckyBoxService.GetAllLuckyBox();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < luckyboxes.Count; i++)
            {
                if (i % 2 == 0)
                    sb.Append("<div class=\"r\">");
                sb.Append("<div class=\"c" + i % 2 + "\">");
                sb.Append("<div><a href=\"" + Url.Action("LuckyBox") + "/" + luckyboxes[i].Id + "\"><img src=\"" + Url.Content("~/Themes/DefaultClean/Content/images/logo.png") + "\" alt=\"GadgetCity Logo\"/></a></div>");
                sb.Append("<div>" + luckyboxes[i].Name + "</div>");
                sb.Append("</div>");
                if (i == luckyboxes.Count - 1 || (i % 2 == 1 && i > 0))
                    sb.Append("<div style=\"clear:both;\"></div></div>");
            }
            return View(sb);
        }

        public ActionResult LuckyBox(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExecuteGames))
                return AccessDeniedView();

            var luckyBox = _luckyBoxService.GetLuckyBoxById(id);
            if (luckyBox.Deleted || !luckyBox.Published)
                return AccessDeniedView();

            var luckyBoxModel = luckyBox.ToModel();
            var luckyBoxDetails = _luckyBoxService.GetAllLuckyBoxDetailsByLuckyBoxId(id);
            //int totalGifts = luckyBoxDetails.Sum(lbd => lbd.NumberPerCycle);
            luckyBoxModel.Gifts = luckyBoxDetails.Select(x =>
                {
                    return new LuckyBoxDetailModel
                    {
                        GiftName = x.GiftName,
                        Id = x.Id,
                        LuckyBoxId = x.LuckyBoxId,
                        NumberPerCycle = x.NumberPerCycle,
                        PictureId = x.PictureId,
                        PictureThumbnailUrl = _pictureService.GetPictureUrl(x.PictureId)
                    };
                }).ToList();
            var luckyBoxDraws = _luckyBoxService.GetAllLuckyBoxDrawsByLuckyBoxId(id);
            luckyBoxModel.Draws = luckyBoxDraws.Where(x => x.IsCompleted).Select(x =>
                {
                    return new LuckyBoxDrawModel
                    {
                        Id = x.Id,
                        LuckyBoxDetailId = x.LuckyBoxDetailId,
                        LuckyBoxId = x.LuckyBoxId,
                        CustomerAddress = x.CustomerAddress,
                        CustomerEmail = x.CustomerEmail,
                        CustomerName = x.CustomerName,
                        CustomerPhone = x.CustomerPhone,
                        ReceiptNo = x.ReceiptNo
                    };
                }).ToList();
            luckyBoxModel.LuckyTable = GenerateLuckyBoxTable(luckyBoxModel);
            luckyBoxModel.LuckyTableDisplay = GenerateLuckyBoxTableDisplay(luckyBoxModel);
            return View(luckyBoxModel);
        }

        private List<LuckyBoxDetailModel> GenerateLuckyBoxTableDisplay(LuckyBoxModel luckyBoxModel)
        {
            List<LuckyBoxDetailModel> lstLuckyTable = new List<LuckyBoxDetailModel>();
            int tableCellsCount = luckyBoxModel.Rows * luckyBoxModel.Columns;
            List<int> lstTracking = new List<int>(tableCellsCount);
            for (int i = 0; i < tableCellsCount; i++)
            {
                lstTracking.Add(i);
                lstLuckyTable.Add(null);
            }
            Dictionary<int, int> dicGift = new Dictionary<int, int>(luckyBoxModel.Gifts.Count);
            Random rnd = new Random();
            //Init each gift has 1 occurance in table
            for (int g = 0; g < luckyBoxModel.Gifts.Count; g++)
            {
                dicGift.Add(luckyBoxModel.Gifts[g].Id, 1);
            }
            //Loops through all of gifts
            for (int g = 0; g < luckyBoxModel.Gifts.Count; g++)
            {
                var gifted = luckyBoxModel.Draws.FindAll(x => x.LuckyBoxDetailId == luckyBoxModel.Gifts[g].Id);
                decimal remainGiftRate = (decimal)(luckyBoxModel.Gifts[g].NumberPerCycle * luckyBoxModel.Rows * luckyBoxModel.Columns) / luckyBoxModel.WinCycle;
                int numberToDisplay = (int)Math.Floor(remainGiftRate);
                dicGift[luckyBoxModel.Gifts[g].Id] += (numberToDisplay > 1 ? numberToDisplay - 1 : 0);
                #region old
                /*
                //Find all draws that win this gift
                var gifted = luckyBoxModel.Draws.FindAll(x => x.LuckyBoxDetailId == luckyBoxModel.Gifts[g].Id);
                //Gifts with numbers = 0, display 1 to defraud players
                if (luckyBoxModel.Gifts[g].NumberPerCycle == 0)
                {
                    int idx = rnd.Next(lstTracking.Count);
                    LuckyBoxDetailModel lbm = new LuckyBoxDetailModel();
                    lbm.GiftName = luckyBoxModel.Gifts[g].GiftName;
                    lbm.LuckyBoxId = luckyBoxModel.Gifts[g].LuckyBoxId;
                    lbm.PictureId = luckyBoxModel.Gifts[g].PictureId;
                    lbm.PictureThumbnailUrl = _pictureService.GetPictureUrl(luckyBoxModel.Gifts[g].PictureId);

                    lstLuckyTable[lstTracking[idx]] = lbm;
                    if (lstTracking.Count > 0)
                        lstTracking.RemoveAt(idx);
                }
                //If this gift is still remaining, fill it to table
                else if (gifted.Count < luckyBoxModel.Gifts[g].NumberPerCycle)
                {
                    int remainGift = 0;
                    if (luckyBoxModel.Gifts[g].NumberPerCycle == 1)
                        remainGift = 1;
                    else
                    {
                        decimal remainGiftRate = (decimal)(luckyBoxModel.Gifts[g].NumberPerCycle * luckyBoxModel.Rows * luckyBoxModel.Columns) / luckyBoxModel.WinCycle;

                        if (remainGiftRate < 1)
                            remainGift = (int)(remainGiftRate * luckyBoxModel.Rows * luckyBoxModel.Columns / 10);
                        else
                            remainGift = (int)remainGiftRate;
                        if (remainGift < 1)
                            remainGift = 1;
                    }
                    for (int rmg = 0; rmg < remainGift; rmg++)
                    {
                        //Make a random number from remaining tracking list, to prevent one cell is used twice case.
                        int idx = rnd.Next(lstTracking.Count);
                        LuckyBoxDetailModel lbm = new LuckyBoxDetailModel();
                        lbm.GiftName = luckyBoxModel.Gifts[g].GiftName;
                        lbm.LuckyBoxId = luckyBoxModel.Gifts[g].LuckyBoxId;
                        lbm.PictureId = luckyBoxModel.Gifts[g].PictureId;
                        lbm.PictureThumbnailUrl = _pictureService.GetPictureUrl(luckyBoxModel.Gifts[g].PictureId);

                        lstLuckyTable[lstTracking[idx]] = lbm;
                        if (lstTracking.Count > 0)
                            lstTracking.RemoveAt(idx);
                    }
                }
                */
                #endregion
            }
            int totalCell = luckyBoxModel.Rows * luckyBoxModel.Columns;
            int sumAll = dicGift.Sum(x => x.Value);
            while (sumAll > totalCell)
            {
                int max = dicGift.Max(x => x.Value);
                int idMax = dicGift.Where(x => x.Value == max).Select(x => x.Key).First();
                dicGift[idMax]--;
                sumAll = dicGift.Sum(x => x.Value);
            }
            foreach (var item in dicGift)
            {
                LuckyBoxDetailModel currentGift = luckyBoxModel.Gifts.Where(x => x.Id == item.Key).First();
                for (int i = 0; i < item.Value; i++)
                {
                    int idx = rnd.Next(lstTracking.Count);
                    LuckyBoxDetailModel lbm = new LuckyBoxDetailModel();
                    lbm.GiftName = currentGift.GiftName;
                    lbm.LuckyBoxId = currentGift.LuckyBoxId;
                    lbm.PictureId = currentGift.PictureId;
                    lbm.PictureThumbnailUrl = _pictureService.GetPictureUrl(currentGift.PictureId);

                    lstLuckyTable[lstTracking[idx]] = lbm;
                    if (lstTracking.Count > 0)
                        lstTracking.RemoveAt(idx);
                }
            }
            return lstLuckyTable;
        }
        [HttpPost]
        public ActionResult PostWinnerInfo(int luckyBoxId, string custName, string custPhone, string custEmail, string receiptNo, string custAddress)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExecuteGames))
                return AccessDeniedView();

            if (luckyBoxId == 0)
                throw new ArgumentNullException("luckyBoxId");
            if (string.IsNullOrEmpty(custName))
                return new JsonResult
                {
                    Data = "Vui lòng nhập Họ tên người chơi!"
                };
            if (string.IsNullOrEmpty(custPhone))
                return new JsonResult
                {
                    Data = "Vui lòng nhập số điện thoại người chơi!"
                };
            Regex regPhone = new Regex(@"^((09[0-9]{8})|(01[0-9]{9})|(0[2-8]{8})|(0[2-8]{9}))$");
            if (!regPhone.IsMatch(custPhone))
                return new JsonResult
                {
                    Data = "Vui lòng nhập số điện thoại hợp lệ!"
                };
            if (!string.IsNullOrEmpty(custEmail))
            {
                Regex regEmail = new Regex(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$");
                if (!regEmail.IsMatch(custEmail))
                    return new JsonResult
                    {
                        Data = "Vui lòng nhập email hợp lệ!"
                    };
            }
            //Check this phone & receiptNo
            List<LuckyBoxDraw> lst = _luckyBoxService.GetLuckyDrawByPhoneAndReceipt(luckyBoxId, custPhone, receiptNo);
            if (lst.Count > 0)
                return new JsonResult
                {
                    Data = "Số điện thoại này đã được sử dụng để mở Ô may mắn!"
                };

            LuckyBox lb = _luckyBoxService.GetLuckyBoxById(luckyBoxId);
            if (lb.IsReceiptRequired && string.IsNullOrEmpty(receiptNo))
                return new JsonResult
                {
                    Data = "Chương trình này yêu cầu phải có số hóa đơn mua hàng!"
                };

            LuckyBoxDraw lbdraw = new LuckyBoxDraw
            {
                LuckyBoxId = luckyBoxId,
                CustomerAddress = custAddress,
                CustomerName = custName,
                CustomerPhone = custPhone,
                CustomerEmail = custEmail,
                ReceiptNo = receiptNo,
                CreatedOnUtc = DateTime.Now
            };
            _luckyBoxService.InsertLuckyBoxDraw(lbdraw);
            _customerActivityService.InsertActivity("InsertLuckyBoxDraw", _localizationService.GetResource("ActivityLog.InsertLuckyBoxDraw"), lbdraw.Id);
            return new JsonResult
            {
                Data = lbdraw
            };
        }
        [HttpPost]
        public ActionResult OpenLuckyBox(int luckyBoxId, int boxId, int luckyDrawId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExecuteGames))
                return AccessDeniedView();
            LuckyBoxDraw winDraw = _luckyBoxService.GetLuckyBoxDrawById(luckyDrawId);
            if (winDraw.IsCompleted)
            {
                return AccessDeniedView();
            }
            int i = 0;
            try
            {
                //int boxId = Convert.ToInt32("0" + Request["boxId"]);
                string cacheKey = "LuckyBoxTable_" + luckyBoxId;
                List<LuckyBoxDetailModel> lstLuckyTable = HttpContext.Cache[cacheKey] as List<LuckyBoxDetailModel>;
                if (lstLuckyTable == null)
                {
                    LuckyBox(luckyBoxId);
                    lstLuckyTable = HttpContext.Cache[cacheKey] as List<LuckyBoxDetailModel>;
                }
                i = 1;
                Random rnd = new Random();
                int openBoxIdx = rnd.Next(lstLuckyTable.Count);
                int winId = 0;
                LuckyBoxDetailModel openBox = lstLuckyTable[openBoxIdx];
                if (openBox != null)
                {
                    winId = openBox.Id;
                }
                i = 2;
                //Write this draw to data
                winDraw.LuckyBoxCode = GenerateLuckyCode(openBox);
                winDraw.LuckyBoxDetailId = winId;
                i = 3;
                if (openBox != null)
                    openBox.WinDraw = winDraw;
                winDraw.IsCompleted = true;
                _luckyBoxService.UpdateLuckyBoxDraw(winDraw);
                i = 4;
                _customerActivityService.InsertActivity("UpdateLuckyBoxDraw", _localizationService.GetResource("ActivityLog.UpdateLuckyBoxDraw"), winDraw.Id);
                i = 5;
                //Clear cache
                HttpContext.Cache.Remove(cacheKey);
                i = 6;
                //Rebuild lucky table into cache
                LuckyBox(luckyBoxId);
                i = 7;

                return new JsonResult
                {
                    Data = openBox
                };
            }
            catch (Exception objEx)
            {
                return new JsonResult
                {
                    Data = objEx.Message + " i= " + i
                };
            }
        }

        private string GenerateLuckyCode(LuckyBoxDetailModel openBox)
        {
            if (openBox == null)
                return null;
            string pattern = "ABCDEF1234567890";
            string s = "";
            Random rnd = new Random();
            for (int i = 0; i < 4; i++)
                s += pattern[rnd.Next(pattern.Length)];
            return openBox.LuckyBoxId + s;
        }
        private List<LuckyBoxDetailModel> GenerateLuckyBoxTable(LuckyBoxModel luckyBoxModel)
        {
            string cacheKey = "LuckyBoxTable_" + luckyBoxModel.Id;
            List<LuckyBoxDetailModel> lstLuckyTable = HttpContext.Cache[cacheKey] as List<LuckyBoxDetailModel>;
            if (lstLuckyTable == null)
            {
                Random rnd = new Random();
                int cell = luckyBoxModel.Rows * luckyBoxModel.Columns;

                //Create a list of remaining cells with remaining empty slots
                int remainCell = luckyBoxModel.WinCycle - luckyBoxModel.Draws.Count;
                lstLuckyTable = new List<LuckyBoxDetailModel>(remainCell);
                List<int> lstTracking = new List<int>(remainCell);
                for (int i = 0; i < remainCell; i++)
                {
                    lstTracking.Add(i);
                    lstLuckyTable.Add(null);
                }

                //Loops through all of gifts
                for (int g = 0; g < luckyBoxModel.Gifts.Count; g++)
                {
                    //Find all draws that win this gift
                    var gifted = luckyBoxModel.Draws.FindAll(x => x.LuckyBoxDetailId == luckyBoxModel.Gifts[g].Id);
                    //If this gift is still remaining, fill it to table
                    if (gifted.Count < luckyBoxModel.Gifts[g].NumberPerCycle)
                    {
                        int remainGift = luckyBoxModel.Gifts[g].NumberPerCycle - gifted.Count;
                        for (int rmg = 0; rmg < remainGift; rmg++)
                        {
                            //Make a random number from remaining tracking list, to prevent one cell is used twice case.
                            int idx = rnd.Next(lstTracking.Count);
                            LuckyBoxDetailModel lbm = new LuckyBoxDetailModel();
                            lbm.GiftName = luckyBoxModel.Gifts[g].GiftName;
                            lbm.LuckyBoxId = luckyBoxModel.Gifts[g].LuckyBoxId;
                            lbm.PictureId = luckyBoxModel.Gifts[g].PictureId;
                            lbm.PictureThumbnailUrl = _pictureService.GetPictureUrl(luckyBoxModel.Gifts[g].PictureId);
                            lbm.Id = luckyBoxModel.Gifts[g].Id;

                            lstLuckyTable[lstTracking[idx]] = lbm;
                            if (lstTracking.Count > 0)
                                lstTracking.RemoveAt(idx);
                        }
                    }
                }
                HttpContext.Cache.Add(cacheKey, lstLuckyTable, null, DateTime.Now.AddMinutes(3), TimeSpan.Zero, CacheItemPriority.Normal, null);
            }
            return lstLuckyTable;
        }
    }
}
