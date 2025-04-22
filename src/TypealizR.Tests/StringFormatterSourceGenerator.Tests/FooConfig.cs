using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypealizR;

namespace Foo;
public class Config
{

    [CLSCompliant(false)]
    private static void Configure(Arguments arg)
    {
        arg.Foo = true;
        arg.Bar = "hello from user space";
    }
  
}
