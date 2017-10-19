using System.ComponentModel;

namespace API.Enums
{
    public enum WinningTypeEnum
    {
        /// <summary>
        /// 未中獎
        /// </summary>
        [Description("未中獎")]
        NONE,

        /// <summary>
        /// 千萬特別獎
        /// </summary>
        [Description("特別獎一千萬")]
        SUPER,

        /// <summary>
        /// 特獎200萬
        /// </summary>
        [Description("特獎200萬")]
        SPECIAL,

        /// <summary>
        /// 頭獎20萬
        /// </summary>
        [Description("頭獎20萬")]
        FIRST,

        /// <summary>
        /// 頭獎20萬
        /// </summary>
        [Description("二獎04萬")]
        SECOND,

        /// <summary>
        /// 頭獎20萬
        /// </summary>
        [Description("三獎01萬")]
        THIRD,

        /// <summary>
        /// 頭獎20萬
        /// </summary>
        [Description("四獎04千")]
        FOURTH,

        /// <summary>
        /// 頭獎20萬
        /// </summary>
        [Description("五獎01千")]
        FIFTH,

        /// <summary>
        /// 頭獎20萬
        /// </summary>
        [Description("六獎200元")]
        SIXTH,
    }
}