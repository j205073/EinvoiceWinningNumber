using API.Entities;
using API.Enums;

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
    }
}