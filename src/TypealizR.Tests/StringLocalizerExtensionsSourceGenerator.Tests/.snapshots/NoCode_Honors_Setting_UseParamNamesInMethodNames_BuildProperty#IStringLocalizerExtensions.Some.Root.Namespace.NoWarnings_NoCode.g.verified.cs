﻿//HintName: IStringLocalizerExtensions.Some.Root.Namespace.NoWarnings_NoCode.g.cs
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
    /// Extensions for <see cref="Some.Root.Namespace.NoWarnings_NoCode"/> to utilize typed-access to ressources generated by TypealizR.
    /// </summary>
    [GeneratedCode("TypealizR.StringLocalizerExtensionsSourceGenerator", "1.0.0.0")]
    internal static partial class IStringLocalizerExtensionsSomeRootNamespaceNoWarnings_NoCode
    {
        /// <summary>
        /// Looks up a localized string similar to 'Greetings {name}, today is {date}'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Greetings {0}, today is {1}'
        /// </returns>
        [DebuggerStepThrough]
        public static LocalizedString Greetings_today_is(this IStringLocalizer<global::Some.Root.Namespace.NoWarnings_NoCode> that, object name, object date) => that[_Greetings_today_is].Format(name, date);
        private const string _Greetings_today_is = "Greetings {name}, today is {date}";
        /// <summary>
        /// Looks up a localized string similar to 'Hello'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Hello'
        /// </returns>
        [DebuggerStepThrough]
        public static LocalizedString Hello(this IStringLocalizer<global::Some.Root.Namespace.NoWarnings_NoCode> that) => that[_Hello];
        private const string _Hello = "Hello";
        /// <summary>
        /// Looks up a localized string similar to 'Hello {name:s}, today is {date:d}'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Hello {0}, today is {1}'
        /// </returns>
        [DebuggerStepThrough]
        public static LocalizedString Hello_today_is(this IStringLocalizer<global::Some.Root.Namespace.NoWarnings_NoCode> that, string name, DateOnly date) => that[_Hello_today_is].Format(name, date);
        private const string _Hello_today_is = "Hello {name:s}, today is {date:d}";
        /// <summary>
        /// wraps the specified <see cref="IStringLocalizer{NoWarnings_NoCode}"/> into a generated type providing properties to access [Some.Nested.Group]: via properties
        /// IStringLocalizer{NoWarnings_NoCode} localize = ...
        /// localize.Some.Nested.Group...
        /// </summary>
        [DebuggerStepThrough]
        public static TypealizedNoWarnings_NoCode Typealize(this IStringLocalizer<global::Some.Root.Namespace.NoWarnings_NoCode> that)
            => new TypealizedNoWarnings_NoCode(that);
    }
}