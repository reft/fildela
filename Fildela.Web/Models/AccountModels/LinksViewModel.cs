using Fildela.Business.Domains.Account.Models;
using System.Collections.Generic;

namespace Fildela.Web.Models.AccountModels
{
    public class LinksViewModel : AccountViewModel
    {
        public List<LinkModel> Links { get; set; }

        public int AllowedLinkCount { get; set; }
    }
}