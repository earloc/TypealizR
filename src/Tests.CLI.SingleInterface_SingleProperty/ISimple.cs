using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace Tests.CLI.SingleInterface_SingleProperty;

[CodeFirstTypealized]
public interface ISimple
{
    /// <summary>
    /// Some localized value
    /// </summary>
    public LocalizedString SomeValue { get; }
}
