using System.Collections.Generic;

namespace Fildela.Business.Domains.Account.Models
{
    public class StatisticsDTOModel
    {
        public List<int> XAxis { get; set; }

        public List<StatisticSeriesDTOModel> Series { get; set; }
    }
}
