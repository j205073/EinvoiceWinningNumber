using RinnaiPortal.Repository;
using RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceDetalisModels;
using RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceWinningNumberModels;
using RinnaiPortalOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class EinvoiceApiController : ApiController
    {
        private EinvoiceApRepository m_repository = new EinvoiceApRepository();
        private EinvoiceApRepository Repository { get { return m_repository; } set { this.m_repository = value; } }

        /// <summary>
        /// 查詢統一發票中獎名單
        /// </summary>
        /// <param name="invTerm">查詢月份，需為雙數 ex.10608</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetEinvoiceWinningNumber([FromUri] string invTerm)
        {
            EinvoiceWinningNumberResultModel result = this.Repository.GetEinvoiceWinningNumbers(invTerm);
            return RinnaiPortalOpenApi.Repositories.APIRepository.CreateDataResponse(Request, System.Net.HttpStatusCode.OK, result); ;
        }
    }
}
