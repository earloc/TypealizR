using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.Tests.CodeFirst;

#pragma warning disable CA1515 // Consider making public types internal
[CodeFirstTypealized]
public interface IPublicInterface
#pragma warning restore CA1515 // Consider making public types internal
{
    LocalizedString PublicHello(string world);
}
