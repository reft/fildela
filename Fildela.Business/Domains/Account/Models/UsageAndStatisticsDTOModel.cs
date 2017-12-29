using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fildela.Business.Domains.Account.Models
{
    public class UsageAndStatisticsDTOModel
    {
        public UsageDTOModel UsageDTOModel { get; set; }

        public StatisticsDTOModel AccountStatisticsDTOModel { get; set; }
    }
}
