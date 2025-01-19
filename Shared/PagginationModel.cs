using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shareds
{
    public class PagginationModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int LatPage { get; set; }
    }
}
