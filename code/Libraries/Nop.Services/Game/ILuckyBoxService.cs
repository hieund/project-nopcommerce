using Nop.Core.Domain.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Game
{
    public interface ILuckyBoxService
    {
        /// <summary>
        /// Get all Lucky Box programs
        /// </summary>
        /// <returns></returns>
        IList<LuckyBox> GetAllLuckyBox();

        /// <summary>
        /// Get Lucky Box detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LuckyBox GetLuckyBoxById(int id);

        /// <summary>
        /// Update a Lucky Box program
        /// </summary>
        /// <param name="luckyBox"></param>
        void UpdateLuckyBox(LuckyBox luckyBox);

        /// <summary>
        /// Insert a new Lucky Box program
        /// </summary>
        /// <param name="luckyBox"></param>
        void InsertLuckyBox(LuckyBox luckyBox);

        /// <summary>
        /// Delete an existing Lucky Box program
        /// </summary>
        /// <param name="luckyBox"></param>
        void DeleteLuckyBox(LuckyBox luckyBox);

        /// <summary>
        /// Get all Lucky Box's gifts by it's id
        /// </summary>
        /// <returns></returns>
        IList<LuckyBoxDetail> GetAllLuckyBoxDetailsByLuckyBoxId(int id);

        /// <summary>
        /// Get Lucky Box gift by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LuckyBoxDetail GetLuckyBoxDetailById(int id);

        /// <summary>
        /// Update a Lucky Box gift
        /// </summary>
        /// <param name="luckyBox"></param>
        void UpdateLuckyBoxDetail(LuckyBoxDetail luckyBoxDetail);

        /// <summary>
        /// Insert a new Lucky Box program
        /// </summary>
        /// <param name="luckyBox"></param>
        void InsertLuckyBoxDetail(LuckyBoxDetail luckyBoxDetail);

        /// <summary>
        /// Delete an existing Lucky Box program
        /// </summary>
        /// <param name="luckyBox"></param>
        void DeleteLuckyBoxDetail(LuckyBoxDetail luckyBoxDetail);

        /// <summary>
        /// Get all Lucky Box's draws by it's id
        /// </summary>
        /// <returns></returns>
        IList<LuckyBoxDraw> GetAllLuckyBoxDrawsByLuckyBoxId(int id);

        /// <summary>
        /// Get Lucky Box draw by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LuckyBoxDraw GetLuckyBoxDrawById(int id);

        /// <summary>
        /// Update a Lucky Box draw
        /// </summary>
        /// <param name="luckyBox"></param>
        void UpdateLuckyBoxDraw(LuckyBoxDraw luckyBoxDraw);

        /// <summary>
        /// Insert a new Lucky Box draw
        /// </summary>
        /// <param name="luckyBox"></param>
        void InsertLuckyBoxDraw(LuckyBoxDraw luckyBoxDraw);

        /// <summary>
        /// Search lucky draw by phone and receiptNo
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        List<LuckyBoxDraw> GetLuckyDrawByPhoneAndReceipt(int luckyBoxId, string phone, string receiptNo);
    }
}
