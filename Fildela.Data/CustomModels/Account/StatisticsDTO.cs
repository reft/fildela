using System.Collections.Generic;

namespace Fildela.Data.CustomModels.Account
{
    public class StatisticsDTO
    {
        public List<int> xAxis { get; set; }

        public List<StatisticSeriesDTO> Series { get; set; }
    }
}
