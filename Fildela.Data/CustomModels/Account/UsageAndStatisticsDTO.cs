using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fildela.Data.CustomModels.Account
{
    public class UsageAndStatisticsDTO
    {
        public UsageDTO UsageDTO { get; set; }

        public StatisticsDTO AccountStatisticsDTO { get; set; }
    }
}
