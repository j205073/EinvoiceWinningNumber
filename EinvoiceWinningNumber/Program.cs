using EinvoiceWinningNumber.Repositoies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EinvoiceWinningNumber
{
    /// <summary>
    /// 此對獎機制會依照財政部當月最近的中獎名單筆對號碼
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            EinvoiceWinningNumberRepository repo = new EinvoiceWinningNumberRepository();
            repo.SandMailHandlerFromStudyOpinion();
        }
    }
}
