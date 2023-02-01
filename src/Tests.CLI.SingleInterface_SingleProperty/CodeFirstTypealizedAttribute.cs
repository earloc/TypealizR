using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.CLI.SingleInterface_SingleProperty;
[AttributeUsage(AttributeTargets.Interface)]
public class CodeFirstTypealizedAttribute : Attribute
{
}