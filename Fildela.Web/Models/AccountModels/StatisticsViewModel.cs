using System;
using System.Collections.Generic;

namespace Fildela.Web.Models.AccountModels
{
    public class StatisticsViewModel
    {
        public List<int> xAxis { get; set; }

        public List<AccountStatisticSeriesViewModel> Series { get; set; }
    }
}