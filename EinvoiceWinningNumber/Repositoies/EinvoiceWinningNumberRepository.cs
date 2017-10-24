using API.Models.ApiModels.EinvoiceModels.EinvoiceApiModels.EinvoiceWinningNumberModels;
using EinvoiceWinningNumber.Enums;
using MailerAPI;
using RinnaiPortal.Repository;
using RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceWinningNumberModels;
using RinnaiPortalOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace EinvoiceWinningNumber.Repositoies
{
    internal class EinvoiceWinningNumberRepository
    {
        private EinvoiceApiRepository m_api = new EinvoiceApiRepository();

        public EinvoiceApiRepository Api { get { return m_api; } }

        public void SandMailHandler()
        {
            string currentYear = (DateTime.UtcNow.AddHours(8).Year - 1911).ToString();
            string currentMonth = (DateTime.UtcNow.AddHours(8).Month - 2).ToString().PadLeft(2, '0');
            string invTerm = Api.ConfirmEinvoicePeriodByDate(string.Concat(currentYear, currentMonth));
            Dictionary<string, List<EinvoiceDataModel>> result = new Dictionary<string, List<EinvoiceDataModel>>();
            try
            {
                string subject = (ProcessUntity.CurrentProcessMode == ProcessModeEnum.DEBUG) ? string.Format("[系統部測試]：電子發票 {0}月份中獎名單", invTerm) : string.Format("[通知]：電子發票 {0}月份中獎名單", invTerm);
                List<string> cc = (ProcessUntity.CurrentProcessMode == ProcessModeEnum.DEBUG) ? new List<string>() { "juncheng.liu" } : new List<string>() { "juncheng.liu" };

                var info = new MailInfo()
                {
                    AddresseeTemp = "{0}@rinnai.com.tw",
                    DomainPattern = @".*@rinnai.com.tw*",
                    Subject = subject,
                    CC = cc,
                };

                StringBuilder mailBody = new StringBuilder();

                EinvoiceWinningNumberResultModel invTermData = Api.GetEinvoiceWinningNumbers(invTerm);
                invTermData.code = "500';";
                if (invTermData.code != "200")
                {
                    mailBody.Append(string.Format("統一發票 期別：{0} API回傳訊息：{1}", invTerm, invTermData.msg));
                    info.To = PublicRepository.AdminEmail;
                    info.Body = mailBody;
                    //寄信
                    Mailer mailer = new Mailer(info);
                    mailer.SendMail();
                }
                else
                {
                    result = Api.ConfirmWhetherToWinThePrize(invTermData.invoYm);

                    StringBuilder adminMailBody = new StringBuilder();
                    foreach (var r in result)
                    {
                        mailBody.AppendLine(@"<p><strong>以下為各單位本期<span style=""color: #ff0000;"">中獎發票號碼</span>清單，請將發票列印出後五天內以雙掛號寄給消費者，謝謝。</strong></p>");
                        mailBody.AppendLine(@"<table style=""width: 100%;border: 1px solid #666;border-spacing: initial;margin: 10px 0;"">");
                        mailBody.AppendLine(@"<thead>");
                        mailBody.AppendLine(@"<tr>");
                        mailBody.AppendLine(@"<th></th>");
                        mailBody.AppendLine(@"<th>開立單位</th>");
                        mailBody.AppendLine(@"<th>訂單號碼</th>");
                        mailBody.AppendLine(@"<th>發票號碼</th>");
                        mailBody.AppendLine(@"<th>中獎期數</th>");
                        mailBody.AppendLine(@"<th>中獎號碼</th>");
                        //mailBody.AppendLine(@"<th>中獎獎別</th>");
                        mailBody.AppendLine(@"</tr>");
                        mailBody.AppendLine(@"</thead>");

                        mailBody.AppendLine(@"<tbody>");
                        foreach (var inv in r.Value as List<EinvoiceDataModel>)
                        {
                            int index = r.Value.IndexOf(inv) + 1;
                            mailBody.AppendLine(@"<tr>");

                            mailBody.AppendLine(@"<td style=""border: 1px solid #ccc;"">");
                            mailBody.AppendLine(index.ToString());
                            mailBody.AppendLine(@"</td>");

                            mailBody.AppendLine(@"<td style=""border: 1px solid #ccc;"">");
                            mailBody.AppendLine(inv.Detalis.DepartmentName);
                            mailBody.AppendLine(@"</td>");

                            mailBody.AppendLine(@"<td style=""border: 1px solid #ccc;"">");
                            mailBody.AppendLine(inv.Detalis.OrderNo);
                            mailBody.AppendLine(@"</td>");

                            mailBody.AppendLine(@"<td style=""border: 1px solid #ccc;"">");
                            mailBody.AppendLine(inv.Data.MInvoiceNumber);
                            mailBody.AppendLine(@"</td>");

                            mailBody.AppendLine(@"<td style=""border: 1px solid #ccc;"">");
                            mailBody.AppendLine(invTerm);
                            mailBody.AppendLine(@"</td>");

                            mailBody.AppendLine(@"<td style=""border: 1px solid #ccc;"">");
                            mailBody.AppendLine(inv.WinningNumber);
                            mailBody.AppendLine(@"</td>");

                            //mailBody.AppendLine(@"<td style=""border: 1px solid #ccc;"">");
                            //mailBody.AppendLine(inv.WinningType.ToString());
                            //mailBody.AppendLine(@"</td>");

                            mailBody.AppendLine(@"</tr>");
                        }
                        mailBody.AppendLine(@"</tbody>");
                        mailBody.AppendLine(@"</table>");
                        info.Body = mailBody;
                        adminMailBody.AppendLine(mailBody.ToString());
                        if (ProcessUntity.CurrentProcessMode == ProcessModeEnum.RELEASE)
                            info.To = r.Value.First().Detalis.MailToObject + "@rinnai.com.tw";
                        else
                            info.To = PublicRepository.AdminEmail;
                        //寄信
                        Mailer mailer = new Mailer(info);
                        mailer.SendMail();
                        mailBody.Clear();
                    }

                    #region 傳送全部資料至資訊管理員

                    var adminInfo = new MailInfo()
                    {
                        AddresseeTemp = "{0}@rinnai.com.tw",
                        DomainPattern = @".*@rinnai.com.tw*",
                        Subject = subject,
                        To = PublicRepository.AdminEmail,
                        CC = cc,
                        Body = adminMailBody
                    };
                    new Mailer(adminInfo).SendMail();

                    #endregion 傳送全部資料至資訊管理員
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Exception Occured! {0}", ex.Message));
                Console.ReadKey();
            }
        }
    }
}