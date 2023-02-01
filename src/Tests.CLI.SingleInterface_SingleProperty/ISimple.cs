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

    public LocalizedString SomeOtherValueWithoutComment { get; }

    /// <summary>
    /// Hello <paramref name="userName"/>, you are the <paramref name="count"/> visitor today!
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public LocalizedString SomeInterpolated(string userName, int count);

    public LocalizedString SomeInterpolatedWithoutComment(int count, string userName);
}
