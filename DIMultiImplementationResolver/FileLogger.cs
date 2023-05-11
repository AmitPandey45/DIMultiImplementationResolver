using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIMultiImplementationResolver
{
    public class FileLogger : ICustomLogger
    {
        public bool Write(string data)
        {
            Console.WriteLine($"FileLogger => {data} ");
            return true;
        }
    }
}
