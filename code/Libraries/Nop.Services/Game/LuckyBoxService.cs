using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Game;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Game
{
    public class LuckyBoxService : ILuckyBoxService
    {
        #region Constants

        

        #endregion

        #region Fields

        private readonly IRepository<LuckyBox> _luckyBoxRepository;
        private readonly IRepository<LuckyBoxDetail> _luckyBoxDetailRepository;
        private readonly IRepository<LuckyBoxDraw> _luckyBoxDrawRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="categoryRepository">Category repository</param>
        /// <param name="productCategoryRepository">ProductCategory repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="eventPublisher">Event publisher</param>
        public LuckyBoxService(ICacheManager cacheManager,
            IRepository<LuckyBox> luckyBoxRepository,
            IRepository<LuckyBoxDetail> luckyBoxDetailRepository,
            IRepository<LuckyBoxDraw> luckyBoxDrawRepository,
            IWorkContext workContext,
            IStoreContext storeContext,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._luckyBoxRepository = luckyBoxRepository;
            this._luckyBoxDetailRepository = luckyBoxDetailRepository;
            this._luckyBoxDrawRepository = luckyBoxDrawRepository;
            this._workContext = workContext;
            this._storeContext = storeContext;
            _eventPublisher = eventPublisher;
        }

        #endregion

        /// <summary>
        /// Get all Lucky Box programs
        /// </summary>
        /// <returns></returns>
        public IList<LuckyBox> GetAllLuckyBox()
        {
            var query = from lbs in _luckyBoxRepository.Table
                        where !lbs.Deleted && lbs.Published
                        select lbs;
            return query.ToList();
        }

        /// <summary>
        /// Get detail Lucky Box program
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LuckyBox GetLuckyBoxById(int id)
        {
            if (id == 0)
                return null;
            return _luckyBoxRepository.GetById(id);
        }

        /// <summary>
        /// Update an existing Lucky Box program
        /// </summary>
        /// <param name="luckyBox"></param>
        public void UpdateLuckyBox(LuckyBox luckyBox)
        {
            if (luckyBox == null)
                throw new ArgumentNullException("luckyBox");

            _luckyBoxRepository.Update(luckyBox);

            //event notification
            _eventPublisher.EntityUpdated(luckyBox);
        }

        /// <summary>
        /// Add a new Lucky Box program
        /// </summary>
        /// <param name="luckyBox"></param>
        public void InsertLuckyBox(LuckyBox luckyBox)
        {
            if (luckyBox == null)
                throw new ArgumentNullException("luckyBox");

            _luckyBoxRepository.Insert(luckyBox);

            //event notification
            _eventPublisher.EntityInserted(luckyBox);
        }

        /// <summary>
        /// Delete a Lucky Box program
        /// </summary>
        /// <param name="luckyBox"></param>
        public void DeleteLuckyBox(LuckyBox luckyBox)
        {
            if (luckyBox == null)
                throw new ArgumentNullException("luckyBox");

            luckyBox.Deleted = true;
            _luckyBoxRepository.Update(luckyBox);

            //event notification
            _eventPublisher.EntityUpdated(luckyBox);
        }


        public IList<LuckyBoxDetail> GetAllLuckyBoxDetailsByLuckyBoxId(int id)
        {
            if (id == 0)
                return null;

            var query = from lbds in _luckyBoxDetailRepository.Table
                        where !lbds.Deleted && lbds.LuckyBoxId == id
                        select lbds;
            return query.ToList();
        }

        public LuckyBoxDetail GetLuckyBoxDetailById(int id)
        {
            if (id == 0)
                return null;

            return _luckyBoxDetailRepository.GetById(id);
        }

        public void UpdateLuckyBoxDetail(LuckyBoxDetail luckyBoxDetail)
        {
            if (luckyBoxDetail == null)
                throw new ArgumentNullException("luckyBoxDetail");

            _luckyBoxDetailRepository.Update(luckyBoxDetail);

            //event notification
            _eventPublisher.EntityUpdated(luckyBoxDetail);
        }

        public void InsertLuckyBoxDetail(LuckyBoxDetail luckyBoxDetail)
        {
            if (luckyBoxDetail == null)
                throw new ArgumentNullException("luckyBoxDetail");

            _luckyBoxDetailRepository.Insert(luckyBoxDetail);

            //event notification
            _eventPublisher.EntityInserted(luckyBoxDetail);
        }

        public void DeleteLuckyBoxDetail(LuckyBoxDetail luckyBoxDetail)
        {
            if (luckyBoxDetail == null)
                throw new ArgumentNullException("luckyBoxDetail");

            luckyBoxDetail.Deleted = true;
            _luckyBoxDetailRepository.Update(luckyBoxDetail);

            //event notification
            _eventPublisher.EntityUpdated(luckyBoxDetail);
        }


        public IList<LuckyBoxDraw> GetAllLuckyBoxDrawsByLuckyBoxId(int id)
        {
            if (id == 0)
                return null;
            var query = from lbdraws in _luckyBoxDrawRepository.Table
                        join lbgift in _luckyBoxDetailRepository.Table on lbdraws.LuckyBoxDetailId equals lbgift.Id into gd
                        from s in gd.DefaultIfEmpty()
                        where lbdraws.LuckyBoxId == id
                        select new
                        { 
                            Id = lbdraws.Id,
                            LuckyBoxCode = lbdraws.LuckyBoxCode,
                            LuckyBoxDetailId = lbdraws.LuckyBoxDetailId,
                            LuckyBoxId = lbdraws.LuckyBoxId,
                            ReceiptNo = lbdraws.ReceiptNo,
                            CreatedOnUtc = lbdraws.CreatedOnUtc,
                            CustomerAddress = lbdraws.CustomerAddress,
                            CustomerEmail = lbdraws.CustomerEmail,
                            CustomerName = lbdraws.CustomerName,
                            CustomerPhone = lbdraws.CustomerPhone,
                            IsCompleted = lbdraws.IsCompleted,
                            GiftName = ( s == null ? string.Empty : s.GiftName )
                        };
            List<LuckyBoxDraw> lstResult = new List<LuckyBoxDraw>();
            foreach (var item in query.ToList())
            {
                lstResult.Add(new LuckyBoxDraw 
                { 
                    CreatedOnUtc = item.CreatedOnUtc,
                    CustomerAddress = item.CustomerAddress,
                    CustomerEmail = item.CustomerEmail,
                    CustomerName = item.CustomerName,
                    CustomerPhone = item.CustomerPhone,
                    GiftName = item.GiftName,
                    Id = item.Id,
                    LuckyBoxCode = item.LuckyBoxCode,
                    LuckyBoxDetailId = item.LuckyBoxDetailId,
                    LuckyBoxId = item.LuckyBoxId,
                    ReceiptNo = item.ReceiptNo,
                    IsCompleted = item.IsCompleted
                });
            }
            return lstResult;
        }

        public LuckyBoxDraw GetLuckyBoxDrawById(int id)
        {
            if (id == 0)
                return null;

            return _luckyBoxDrawRepository.GetById(id);
        }

        public void UpdateLuckyBoxDraw(LuckyBoxDraw luckyBoxDraw)
        {
            if (luckyBoxDraw == null)
                throw new ArgumentNullException("luckyBoxDraw");

            _luckyBoxDrawRepository.Update(luckyBoxDraw);

            //event notification
            _eventPublisher.EntityUpdated(luckyBoxDraw);
        }

        public void InsertLuckyBoxDraw(LuckyBoxDraw luckyBoxDraw)
        {
            if (luckyBoxDraw == null)
                throw new ArgumentNullException("luckyBoxDraw");

            _luckyBoxDrawRepository.Insert(luckyBoxDraw);

            //event notification
            _eventPublisher.EntityInserted(luckyBoxDraw);
        }


        public List<LuckyBoxDraw> GetLuckyDrawByPhoneAndReceipt(int luckyBoxId, string phone, string receiptNo)
        {
            if (string.IsNullOrEmpty(phone))
                throw new ArgumentNullException("phone");
            var query = from l in _luckyBoxDrawRepository.Table
                        where l.CustomerPhone == phone && l.LuckyBoxId == luckyBoxId && l.IsCompleted
                        select l;
            return query.ToList();
        }
    }
}
