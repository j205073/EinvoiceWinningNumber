namespace RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceWinningNumberModels
{
    /// <summary>
    /// 財政部電子票API 中獎號碼查詢回船模型
    /// </summary>
    public class EinvoiceWinningNumberResultModel
    {
        /*
        "v":"<版本號碼>"，
        "code":"<訊息回應碼>"，
        "msg":"<系統回應訊息>"，
        "invoYm":"<查詢開獎期別>"，
        "superPrizeNo":"<千萬特獎號碼>"，
        "spcPrizeNo":"<特獎號碼>"，
        "spcPrizeNo2":"<特獎號碼 2>"，
        "spcPrizeNo3":"<特獎號碼3>"，
        "firstPrizeNo1":"<頭獎號碼 1>"，
        "firstPrizeNo2":"<頭獎號碼2>"，
        "firstPrizeNo3":"<頭獎號碼3>"，
        "firstPrizeNo4":"<頭獎號碼4>"，
        "firstPrizeNo5":"<頭獎號碼5>"，
        "firstPrizeNo6":"<頭獎號碼6>"，
        "firstPrizeNo7":"<頭獎號碼7>"，
        "firstPrizeNo8":"<頭獎號碼8>"，
        "firstPrizeNo9":"<頭獎號碼9>"，
        "firstPrizeNo10":"<頭獎號碼10>"，
        "sixthPrizeNo1":"<六獎號碼 1>"，
        "sixthPrizeNo2":"<六獎號碼2>"，
        "sixthPrizeNo3":"<六獎號碼3>"，
        "superPrizeAmt":"<千萬特獎金額>"，
        "spcPrizeAmt":"<特獎金額>"，
        "firstPrizeAmt":"<頭獎金額>"，
        "secondPrizeAmt":"<二獎金額>"，
        "thirdPrizeAmt":"<三獎金額>"，
        "fourthPrizeAmt":"<四獎金額>"，
        "fifthPrizeAmt":"<五獎金額>"，
        "sixthPrizeAmt":"<六獎金額>"，
        "sixthPrizeNo4":"<六獎號碼 4>"，
        "sixthPrizeNo5":"<六獎號碼5>"，
        "sixthPrizeNo6":"<六獎號碼6>"
        */
        /// <summary>
        /// 五獎金額
        /// </summary>
        //public string fifthPrizeAmt { get; set; }
        /// <summary>
        /// 頭獎金額
        /// </summary>
        //public string firstPrizeAmt { get; set; }

        /// <summary>
        /// 頭獎號碼1
        /// </summary>
        public string firstPrizeNo1 { get; set; }

        /// <summary>
        /// 頭獎號碼10
        /// </summary>
        public string firstPrizeNo10 { get; set; }

        /// <summary>
        /// 頭獎號碼2
        /// </summary>
        public string firstPrizeNo2 { get; set; }

        /// <summary>
        /// 頭獎號碼3
        /// </summary>
        public string firstPrizeNo3 { get; set; }

        /// <summary>
        /// 頭獎號碼4
        /// </summary>
        public string firstPrizeNo4 { get; set; }

        /// <summary>
        /// 頭獎號碼5
        /// </summary>
        public string firstPrizeNo5 { get; set; }

        /// <summary>
        /// 頭獎號碼6
        /// </summary>
        public string firstPrizeNo6 { get; set; }

        /// <summary>
        /// 頭獎號碼7
        /// </summary>
        public string firstPrizeNo7 { get; set; }

        /// <summary>
        /// 頭獎號碼8
        /// </summary>
        public string firstPrizeNo8 { get; set; }

        /// <summary>
        /// 頭獎號碼9
        /// </summary>
        public string firstPrizeNo9 { get; set; }

        /// <summary>
        /// 四獎金額
        /// </summary>
        //public string fourthPrizeAmt { get; set; }

        /// <summary>
        /// 查詢開獎期別
        /// </summary>
        public string invoYm { get; set; }

        /// <summary>
        /// 二獎金額
        /// </summary>
        //public string secondPrizeAmt { get; set; }

        /// <summary>
        /// 六獎金額
        /// </summary>
        //public string sixthPrizeAmt { get; set; }

        /// <summary>
        /// 六獎號碼1
        /// </summary>
        public string sixthPrizeNo1 { get; set; }

        /// <summary>
        /// 六獎號碼2
        /// </summary>
        public string sixthPrizeNo2 { get; set; }

        /// <summary>
        /// 六獎號碼3
        /// </summary>
        public string sixthPrizeNo3 { get; set; }

        /// <summary>
        /// 六獎號碼4
        /// </summary>
        public string sixthPrizeNo4 { get; set; }

        /// <summary>
        /// 六獎號碼5
        /// </summary>
        public string sixthPrizeNo5 { get; set; }

        /// <summary>
        /// 六獎號碼6
        /// </summary>
        public string sixthPrizeNo6 { get; set; }

        /// <summary>
        /// 特獎金額
        /// </summary>
        //public string spcPrizeAmt { get; set; }

        /// <summary>
        /// 特獎號碼1
        /// </summary>
        public string spcPrizeNo { get; set; }

        /// <summary>
        /// 特獎號碼2
        /// </summary>
        public string spcPrizeNo2 { get; set; }

        /// <summary>
        /// 特獎號碼3
        /// </summary>
        public string spcPrizeNo3 { get; set; }

        /// <summary>
        /// 千萬特獎金額
        /// </summary>
        //public string superPrizeAmt { get; set; }

        /// <summary>
        /// 千萬特獎號碼
        /// </summary>
        public string superPrizeNo { get; set; }

        /// <summary>
        /// 三獎金額
        /// </summary>
        //public string thirdPrizeAmt { get; set; }

        /// <summary>
        /// 時間戳記
        /// </summary>
        public Timestamp timeStamp { get; set; }

        public string updateDate { get; set; }

        /// <summary>
        /// 版本號碼
        /// </summary>
        public string v { get; set; }

        /// <summary>
        /// 訊息回應碼
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 系統回應訊息
        /// </summary>
        public string msg { get; set; }
    }

    public class Timestamp
    {
        public int date { get; set; }
        public int day { get; set; }
        public int hours { get; set; }
        public int minutes { get; set; }
        public int month { get; set; }
        public int seconds { get; set; }
        public long time { get; set; }
        public int timezoneOffset { get; set; }
        public int year { get; set; }
    }
}