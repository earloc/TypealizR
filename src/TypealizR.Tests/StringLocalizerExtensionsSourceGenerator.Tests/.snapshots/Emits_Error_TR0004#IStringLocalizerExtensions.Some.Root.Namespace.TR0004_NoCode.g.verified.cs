﻿//HintName: IStringLocalizerExtensions.Some.Root.Namespace.TR0004_NoCode.g.cs
// <auto-generated/>
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Some.Root.Namespace;
using Some.Root.Namespace.TypealizR;
namespace Microsoft.Extensions.Localization
{
    /// <summary>
    /// Extensions for <see cref="Some.Root.Namespace.TR0004_NoCode"/> to utilize typed-access to ressources generated by TypealizR.
    /// </summary>
    [GeneratedCode("TypealizR.StringLocalizerExtensionsSourceGenerator", "1.0.0.0")]
    internal static partial class IStringLocalizerExtensionsSomeRootNamespaceTR0004_NoCode
    {
        /// <summary>
        /// Looks up a localized string similar to 'Greetings {name:wtf}'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Greetings {0}'
        /// </returns>
        [DebuggerStepThrough]
        public static LocalizedString Greetings__name(this IStringLocalizer<global::Some.Root.Namespace.TR0004_NoCode> that, object name) => global::Some.Root.Namespace.TypealizR_StringFormatter.Format(that[_Greetings__name], name);
        private const string _Greetings__name = "Greetings {name:wtf}";
        /// <summary>
        /// wraps the specified <see cref="IStringLocalizer{TR0004_NoCode}"/> into a generated type providing properties to access [Some.Nested.Group]: via properties
        /// IStringLocalizer{TR0004_NoCode} localize = ...
        /// localize.Some.Nested.Group...
        /// </summary>
        [DebuggerStepThrough]
        public static TypealizedTR0004_NoCode Typealize(this IStringLocalizer<global::Some.Root.Namespace.TR0004_NoCode> that)
            => new TypealizedTR0004_NoCode(that);
    }
}