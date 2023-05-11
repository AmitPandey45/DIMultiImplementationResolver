using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIMultiImplementationResolver
{
    public interface ICustomLogger
    {
        public bool Write(string data);
    }
}
