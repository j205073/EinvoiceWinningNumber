using API.Entities;
using API.Models.ApiModels.EinvoiceModels.EinvoiceApiModels.EinvoiceWinningNumberModels;
using Newtonsoft.Json;
using RinnaiPortalOpenApi.Models;
using RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceDetalisModels;
using RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceWinningNumberModels;
using RinnaiPortalOpenApi.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace RinnaiPortalOpenApi.Repositories
{
    public class EinvoiceApRepository
    {
        private INVDB m_invdb = new INVDB();
        private INVDB InvDB { get { return m_invdb; } set { m_invdb = value; } }

        private EinvoiceModule m_module = new EinvoiceModule();
        private EinvoiceModule module { get { return m_module; } set { m_module = value; } }

        private APIRepository m_APIRepository = new APIRepository();
        private APIRepository Repository { get { return m_APIRepository; } set { m_APIRepository = value; } }

        #region 對獎

        /// <summary>
        /// 取得財政部中獎號碼名單
        /// </summary>
        /// <param name="invTerm"></param>
        /// <returns></returns>
        public EinvoiceWinningNumberResultModel GetEinvoiceWinningNumbers(string invTerm)
        {
            EinvoiceWinningNumberSendModel send = new EinvoiceWinningNumberSendModel() { action = "QryWinningList", invTerm = invTerm };

            string resultStr = string.Empty;
            WebRequest webRequest = null;
            string sendData = this.Repository.ConvertObjectToQueryString(send);

            byte[] bytes = Encoding.UTF8.GetBytes(sendData);
            //Request
            try
            {
                webRequest = this.Repository.CreateWebRequest(ConnectionStringModel.EinvoiceSearchApiConnectionStr);
                webRequest.ContentLength = bytes.Length;
                this.Repository.SendWebRequest(webRequest, bytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //Get Response
            try
            {
                resultStr = this.Repository.GetResponse(webRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            EinvoiceWinningNumberResultModel result = JsonConvert.DeserializeObject<EinvoiceWinningNumberResultModel>(resultStr);
            return result;
        }

        /// <summary>
        /// 取得資料庫所有已開立的電子發票(依照開獎期別分組)
        /// </summary>
        /// <param name="startDate">ex.10608</param>
        /// <param name="endDate">ex.10610</param>
        private Dictionary<string, List<C0401H>> GetAllEinvoice(string invoYm)
        {
            IEnumerable<IGrouping<string, C0401H>> groupInv = this.InvDB.C0401H.GroupBy(x => x.MInvoiceDate.Substring(0, 6));
            Dictionary<string, List<C0401H>> groupInvLast = new Dictionary<string, List<C0401H>>();
            //發票按月份分組
            foreach (IGrouping<string, C0401H> group in groupInv)
            {
                string groupDate = string.Concat((Convert.ToInt32(group.First().MInvoiceDate.Substring(0, 4)) - 1911).ToString(), group.First().MInvoiceDate.Substring(4, 2));
                string searchDate = ConfirmEinvoicePeriodByDate(groupDate);
                if (!searchDate.Equals(invoYm))
                    continue;
                if (groupInvLast.ContainsKey(searchDate))
                {
                    List<C0401H> tempAdd = new List<C0401H>();
                    foreach (var g in groupInvLast[searchDate])
                        tempAdd.Add(g);
                    foreach (var g in group)
                        tempAdd.Add(g);
                    groupInvLast[searchDate] = tempAdd;
                }
                else
                {
                    List<C0401H> tempAdd = new List<C0401H>();
                    foreach (var g in group)
                        tempAdd.Add(g);
                    groupInvLast[searchDate] = tempAdd;
                }
            }

            return groupInvLast;
        }

        /// <summary>
        /// 確認發票應屬期別
        /// </summary>
        /// <param name="invDate"></param>
        /// <returns></returns>
        public string ConfirmEinvoicePeriodByDate(string invDate)
        {
            string result = string.Empty;
            int year = Convert.ToInt16(invDate.Substring(0, 3));
            int month = Convert.ToInt16(invDate.Substring(3, 2));

            if (month % 2 != 0)
                month++;
            return string.Concat(year, month.ToString().PadLeft(2, '0'));
        }

        /// <summary>
        /// 判斷是否中獎
        /// </summary>
        public Dictionary<string, List<EinvoiceDataModel>> ConfirmWhetherToWinThePrize(string invoYm)
        {
            Dictionary<string, List<C0401H>> invoices = GetAllEinvoice(invoYm);
            Dictionary<string, List<EinvoiceDataModel>> resultData = new Dictionary<string, List<EinvoiceDataModel>>();

            foreach (var inv in invoices)
            {
                EinvoiceWinningNumberResultModel winningModel = GetEinvoiceWinningNumbers(inv.Key);
                if (winningModel.code != "200")
                    continue;
                resultData[inv.Key] = CompareNumbers(winningModel, inv.Value);
            }

            Dictionary<string, List<EinvoiceDataModel>> filterData = FilterWinning(resultData);
            return filterData;
        }

        /// <summary>
        /// 過濾沒有中獎的號碼
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, List<EinvoiceDataModel>> FilterWinning(Dictionary<string, List<EinvoiceDataModel>> data)
        {
            Dictionary<string, List<EinvoiceDataModel>> filterData = new Dictionary<string, List<EinvoiceDataModel>>();
            foreach (var d in data)
            {
                filterData[d.Key] = new List<EinvoiceDataModel>();
                foreach (var inv in d.Value as List<EinvoiceDataModel>)
                {
                    if (inv.WinningType != API.Enums.WinningTypeEnum.NONE)
                        filterData[d.Key].Add(inv);
                }
            }
            return filterData;
        }

        private List<EinvoiceDataModel> CompareNumbers(EinvoiceWinningNumberResultModel winningModel, List<C0401H> invs)
        {
            List<EinvoiceDataModel> invoies = new List<EinvoiceDataModel>();

            foreach (var inv in invs)
            {
                EinvoiceDataModel temp = new EinvoiceDataModel() { Data = inv };
                invoies.Add(temp);
            }

            foreach (var inv in invoies)
            {
                string invNum = inv.Data.MInvoiceNumber.Substring(2, inv.Data.MInvoiceNumber.Length - 2);

                #region 千萬獎

                if (winningModel.superPrizeNo.Equals(invNum))
                {
                    inv.WinningType = API.Enums.WinningTypeEnum.SUPER;
                    inv.WinningNumber = winningModel.superPrizeNo;
                    continue;
                }

                #endregion 千萬獎

                #region 特獎

                if (winningModel.spcPrizeNo.Equals(invNum) ||
                winningModel.spcPrizeNo2.Equals(invNum) ||
                winningModel.spcPrizeNo3.Equals(invNum))
                {
                    inv.WinningType = API.Enums.WinningTypeEnum.SPECIAL;
                    inv.WinningNumber =
                        winningModel.spcPrizeNo.Equals(invNum) ?
                        winningModel.spcPrizeNo : winningModel.spcPrizeNo2.Equals(invNum) ?
                        winningModel.spcPrizeNo2 : winningModel.spcPrizeNo3.Equals(invNum) ?
                        winningModel.spcPrizeNo3 : string.Empty;
                    continue;
                }

                #endregion 特獎

                #region 頭獎(一路比對到六獎)

                CompareFirstPrizeNumber(winningModel.firstPrizeNo1, invNum, inv);
                CompareFirstPrizeNumber(winningModel.firstPrizeNo2, invNum, inv);
                CompareFirstPrizeNumber(winningModel.firstPrizeNo3, invNum, inv);
                CompareFirstPrizeNumber(winningModel.firstPrizeNo4, invNum, inv);
                CompareFirstPrizeNumber(winningModel.firstPrizeNo5, invNum, inv);
                CompareFirstPrizeNumber(winningModel.firstPrizeNo6, invNum, inv);
                CompareFirstPrizeNumber(winningModel.firstPrizeNo7, invNum, inv);
                CompareFirstPrizeNumber(winningModel.firstPrizeNo8, invNum, inv);
                CompareFirstPrizeNumber(winningModel.firstPrizeNo9, invNum, inv);
                CompareFirstPrizeNumber(winningModel.firstPrizeNo10, invNum, inv);

                #endregion 頭獎(一路比對到六獎)

                #region 六獎(含加開)

                CompareSixthPrizeNumber(winningModel.sixthPrizeNo1, invNum, inv);
                CompareSixthPrizeNumber(winningModel.sixthPrizeNo2, invNum, inv);
                CompareSixthPrizeNumber(winningModel.sixthPrizeNo3, invNum, inv);
                CompareSixthPrizeNumber(winningModel.sixthPrizeNo4, invNum, inv);
                CompareSixthPrizeNumber(winningModel.sixthPrizeNo5, invNum, inv);
                CompareSixthPrizeNumber(winningModel.sixthPrizeNo6, invNum, inv);

                #endregion 六獎(含加開)
            }
            return invoies;
        }

        /// <summary>
        /// 頭獎號碼比對(比對到六獎)
        /// </summary>
        /// <param name="firstPrizeNo"></param>
        /// <param name="invNum"></param>
        private void CompareFirstPrizeNumber(string firstPrizeNo, string invNum, EinvoiceDataModel inv)
        {
            if (string.IsNullOrEmpty(firstPrizeNo))
                return;
            var winNum = string.Empty;
            //截斷字元只需6次
            for (int i = 0; i < 6; i++)
            {
                winNum = firstPrizeNo.Substring(i, firstPrizeNo.Length - i);
                if (CompareNums(winNum, invNum))
                {
                    inv.WinningNumber = firstPrizeNo;
                    switch (winNum.Length)
                    {
                        case 8:
                            inv.WinningType = API.Enums.WinningTypeEnum.FIRST;
                            break;

                        case 7:
                            inv.WinningType = API.Enums.WinningTypeEnum.SECOND;
                            break;

                        case 6:
                            inv.WinningType = API.Enums.WinningTypeEnum.THIRD;
                            break;

                        case 5:
                            inv.WinningType = API.Enums.WinningTypeEnum.FOURTH;
                            break;

                        case 4:
                            inv.WinningType = API.Enums.WinningTypeEnum.FIFTH;
                            break;

                        case 3:
                            inv.WinningType = API.Enums.WinningTypeEnum.SIXTH;
                            break;

                        default:
                            inv.WinningType = API.Enums.WinningTypeEnum.NONE;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 六獎號碼比對
        /// </summary>
        /// <param name="sixthPrizeNo"></param>
        /// <param name="invNum"></param>
        /// <param name="inv"></param>
        private void CompareSixthPrizeNumber(string sixthPrizeNo, string invNum, EinvoiceDataModel inv)
        {
            if (string.IsNullOrEmpty(sixthPrizeNo))
                return;

            if (CompareNums(sixthPrizeNo, invNum))
            {
                inv.WinningNumber = sixthPrizeNo;
                inv.WinningType = API.Enums.WinningTypeEnum.SIXTH;
            }
        }

        /// <summary>
        /// 比對頭獎中的中獎號碼
        /// </summary>
        /// <param name="winningNum"></param>
        /// <param name="invNum"></param>
        private bool CompareNums(string winningNum, string invNum)
        {
            string compareNum = invNum.Substring(invNum.Length - winningNum.Length, invNum.Length - (invNum.Length - winningNum.Length));
            if (winningNum.Length != compareNum.Length)
                throw new Exception("Not The Same Text Length.");
            return winningNum.Equals(compareNum);
        }

        #endregion 對獎

        #region 明細

        /// <summary>
        /// 取得訂單的發票號碼
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public string GetEinvoiceNoByOrderNo(string orderNo)
        {
            string einvoiceNo = string.Empty;
            NavisionNewDB db = new NavisionNewDB();
            var order = db.Rinnai_Service_Ledger_Entry.Where(o => o.Service_Order_No_ == orderNo && o.Document_Type == 2).FirstOrDefault();
            if (order == null)
                throw new Exception("[系統]無法取得該訂單相關資料");
            var orderDealis = db.Rinnai_Sales_Invoice_Line.Where(o => o.Document_No_ == order.Document_No_ && o.VAT_Transaction_Number != "").FirstOrDefault();
            if (orderDealis == null)
                throw new Exception("[系統]無法取得該訂單相關資料");
            einvoiceNo = orderDealis.VAT_Transaction_Number;
            return einvoiceNo;
        }

        public EinvoiceDetalisResultModel GetEinvoiceDetalisByNo(string invNo)
        {
            invNo = string.Concat(invNo[0], invNo[1]).ToUpper() + invNo.Substring(2, invNo.Length - 2);
            EinvoiceDetalisSendModel send = new EinvoiceDetalisSendModel();

            #region 測試時請註解 正式請取消註解

            //send = GetEinvoiceProtoDetalisByNo(invNo);

            #endregion 測試時請註解 正式請取消註解

            send.version = 0.3;
            send.type = "Barcode";
            send.invNum = invNo;
            send.action = "qryInvDetail";
            send.generation = "V2";

            #region 測試時請取消註解並給時寄發票的資訊 正式請註解

            send.invTerm = "10610";
            send.invDate = "2017/09/26";
            send.randomNumber = "6157";

            #endregion 測試時請取消註解並給時寄發票的資訊 正式請註解

            string resultStr = string.Empty;
            WebRequest webRequest = null;
            string sendData = this.Repository.ConvertObjectToQueryString(send);

            byte[] bytes = Encoding.UTF8.GetBytes(sendData);
            //Request
            try
            {
                webRequest = this.Repository.CreateWebRequest(ConnectionStringModel.EinvoiceSearchApiConnectionStr);
                webRequest.ContentLength = bytes.Length;
                this.Repository.SendWebRequest(webRequest, bytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //Get Response
            try
            {
                resultStr = this.Repository.GetResponse(webRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            EinvoiceDetalisResultModel result = JsonConvert.DeserializeObject<EinvoiceDetalisResultModel>(resultStr);
            return result;
        }

        private EinvoiceDetalisSendModel GetEinvoiceProtoDetalisByNo(string invNo)
        {
            invNo = string.Concat(invNo[0], invNo[1]).ToUpper() + invNo.Substring(2, invNo.Length - 2);
            EinvoiceDetalisSendModel send = new EinvoiceDetalisSendModel();
            try
            {
                C0401H einvoice = this.module.GetEinvoiceDetalisByNo(invNo);
                if (einvoice == null)
                    throw new Exception("[系統]查無相關發票明細");
                int getMonth = Convert.ToInt16(einvoice.MInvoiceDate.Substring(4, 2));
                var chkMonth = (getMonth % 2) == 1;
                string invTerm = string.Format("{0}{1}", einvoice.MInvoiceDate.Substring(0, 4), chkMonth ? (getMonth + 1).ToString().PadLeft(2, '0') : (getMonth).ToString().PadLeft(2, '0'));
                string y = einvoice.MInvoiceDate.Substring(0, 4);
                string m = einvoice.MInvoiceDate.Substring(4, 2);
                string d = einvoice.MInvoiceDate.Substring(6, 2);
                string date = string.Format("{0}/{1}/{2}", y, m, d);
                send.invTerm = invTerm;
                send.invDate = date;
                send.randomNumber = einvoice.MRandomNumber;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return send;
        }

        #endregion 明細
    }
}