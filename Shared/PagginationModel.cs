using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PagginationModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int LastPage { get; set; }
    }
}
