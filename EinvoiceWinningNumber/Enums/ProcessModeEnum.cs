using System.ComponentModel;

namespace EinvoiceWinningNumber.Enums
{
    /// <summary>
    /// 設定目前作業模式
    /// </summary>
    public enum ProcessModeEnum
    {
        /// <summary>
        /// 正式
        /// </summary>
        [Description("正式")]
        RELEASE,

        /// <summary>
        /// 測試
        /// </summary>
        [Description("測試")]
        DEBUG,
    }

}