using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fildela.Business.Domains.Account.Models
{
    public class StatisticSeriesDTOModel
    {
        public String Name { get; set; }
        public List<int> Data { get; set; }
    }
}
