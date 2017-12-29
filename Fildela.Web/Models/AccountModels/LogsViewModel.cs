using Fildela.Business.Domains.Account.Models;
using System.Collections.Generic;

namespace Fildela.Web.Models.AccountModels
{
    public class LogsViewModel : AccountViewModel
    {
        public List<LogModel> Logs { get; set; }
    }
}