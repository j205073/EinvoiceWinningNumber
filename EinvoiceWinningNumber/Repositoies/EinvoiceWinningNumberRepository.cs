using API.Models.ApiModels.EinvoiceModels.EinvoiceApiModels.EinvoiceWinningNumberModels;
using EinvoiceWinningNumber.Enums;
using MailerAPI;
using RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceWinningNumberModels;
using RinnaiPortalOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace EinvoiceWinningNumber.Repositoies
{
    internal class EinvoiceWinningNumberRepository
    {
        private EinvoiceApRepository m_api = new EinvoiceApRepository();

        public EinvoiceApRepository Api { get { return m_api; } }

        public void SandMailHandler()
        {
            string currentYear = (DateTime.UtcNow.AddHours(8).Year - 1911).ToString();
            string currentMonth = (DateTime.UtcNow.AddHours(8).Month - 2).ToString().PadLeft(2, '0');
            string invTerm = Api.ConfirmEinvoicePeriodByDate(string.Concat(currentYear, currentMonth));
            Dictionary<string, List<EinvoiceDataModel>> result = new Dictionary<string, List<EinvoiceDataModel>>();
            try
            {
                string subject = (ProcessUntity.CurrentProcessMode == ProcessModeEnum.DEBUG) ? "[系統部測試]：電子發票中獎名單" : "[通知]：電子發票中獎名單";
                List<string> cc = (ProcessUntity.CurrentProcessMode == ProcessModeEnum.DEBUG) ? new List<string>() { "juncheng.liu" } : new List<string>() { "juncheng.liu" };

                var info = new MailInfo()
                {
                    AddresseeTemp = "{0}@rinnai.com.tw",
                    DomainPattern = @".*@rinnai.com.tw*",
                    Subject = subject,
                    To = "juncheng.liu@rinnai.com.tw",
                    CC = cc,
                };

                StringBuilder mailBody = new StringBuilder();

                EinvoiceWinningNumberResultModel invTermData = Api.GetEinvoiceWinningNumbers(invTerm);
                if (invTermData.code != "200")
                    mailBody.Append(string.Format("統一發票 期別：{0} API回傳訊息：{1}", invTerm, invTermData.msg));
                else
                {
                    result = Api.ConfirmWhetherToWinThePrize(invTermData.invoYm);

                    mailBody.AppendLine(@"<table style=""width: 100%;border: 1px solid #666;border-spacing: initial;margin: 10px 0;"">");
                    mailBody.AppendLine(@"<thead>");
                    mailBody.AppendLine(@"<tr>");
                    mailBody.AppendLine(@"<th></th>");
                    mailBody.AppendLine(@"<th>發票號碼</th>");
                    mailBody.AppendLine(@"<th>中獎期數</th>");
                    mailBody.AppendLine(@"<th>中獎號碼</th>");
                    mailBody.AppendLine(@"<th>中獎獎別</th>");
                    mailBody.AppendLine(@"</tr>");
                    mailBody.AppendLine(@"</thead>");

                    mailBody.AppendLine(@"<tbody>");

                    foreach (var r in result)
                    {
                        foreach (var inv in r.Value as List<EinvoiceDataModel>)
                        {
                            int index = r.Value.IndexOf(inv) + 1;
                            mailBody.AppendLine(@"<tr>");

                            mailBody.AppendLine(@"<td style=""border: 1px solid #ccc;"">");
                            mailBody.AppendLine(index.ToString());
                            mailBody.AppendLine(@"</td>");

                            mailBody.AppendLine(@"<td style=""border: 1px solid #ccc;"">");
                            mailBody.AppendLine(inv.Data.MInvoiceNumber);
                            mailBody.AppendLine(@"</td>");

                            mailBody.AppendLine(@"<td style=""border: 1px solid #ccc;"">");
                            mailBody.AppendLine(r.Key);
                            mailBody.AppendLine(@"</td>");

                            mailBody.AppendLine(@"<td style=""border: 1px solid #ccc;"">");
                            mailBody.AppendLine(inv.WinningNumber);
                            mailBody.AppendLine(@"</td>");

                            mailBody.AppendLine(@"<td style=""border: 1px solid #ccc;"">");
                            mailBody.AppendLine(inv.WinningType.ToString());
                            mailBody.AppendLine(@"</td>");

                            mailBody.AppendLine(@"</tr>");
                        }
                    }

                    mailBody.AppendLine(@"</tbody>");
                    mailBody.AppendLine(@"</table>");
                    info.Body = mailBody;
                }

                //寄信
                Mailer mailer = new Mailer(info);
                mailer.SendMail();
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Exception Occured! {0}", ex.Message));
                Console.ReadKey();
            }
        }
    }
}