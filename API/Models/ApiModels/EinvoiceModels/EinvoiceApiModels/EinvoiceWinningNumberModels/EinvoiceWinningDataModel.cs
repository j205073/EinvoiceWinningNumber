using API.Entities;
using API.Enums;
using System.Collections.Generic;

namespace API.Models.ApiModels.EinvoiceModels.EinvoiceApiModels.EinvoiceWinningNumberModels
{
    public class EinvoiceDataModel
    {
        private C0401H m_data = new C0401H();

        /// <summary>
        /// 基本資料
        /// </summary>
        public C0401H Data { get { return m_data; } set { m_data = value; } }

        /// <summary>
        /// 中獎號碼
        /// </summary>
        private string m_winningNumber = string.Empty;

        public string WinningNumber { get { return m_winningNumber; } set { m_winningNumber = value; } }

        /// <summary>
        /// 得獎類別
        /// </summary>
        private WinningTypeEnum m_winningType = WinningTypeEnum.NONE;

        public WinningTypeEnum WinningType { get { return m_winningType; } set { m_winningType = value; } }

        private DetalisInfo m_detalis = new DetalisInfo();
        public DetalisInfo Detalis { get { return m_detalis; } set { m_detalis = value; } }
    }

    /// <summary>
    /// 訂單明細
    /// </summary>
    public class DetalisInfo
    {
        /// <summary>
        /// 訂單號碼
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 開立單位代碼
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 開立單位名稱
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 通知Email對像 (取資料庫連絡群組)
        /// </summary>
        private List<string> m_mailToObject = new List<string>();
        public List<string> MailToObject { get { return m_mailToObject; } set { m_mailToObject = value; } }
    }
}