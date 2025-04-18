﻿//HintName: Some.Root.Namespace.TR0005_NoCode.Typealized.g.cs
// <auto-generated/>
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Localization;
using Some.Root.Namespace;
namespace Some.Root.Namespace.TypealizR
{
    /// <summary>
    /// Wraps a <see cref="IStringLocalizer{TR0005_NoCode}"/> and provides properties for typed-access to resources, generated by TypealizR
    /// </summary>
    [GeneratedCode("TypealizR.TypealizedClassSourceGenerator", "1.0.0.0")]
    internal partial class TypealizedTR0005_NoCode
    {
        private readonly IStringLocalizer<global::Some.Root.Namespace.TR0005_NoCode> localizer;
        /// <summary>
        /// Creates a new instance of <see cref="TypealizedTR0005_NoCode"/>
        /// </summary>
        public TypealizedTR0005_NoCode(IStringLocalizer<global::Some.Root.Namespace.TR0005_NoCode> localizer)
        {
            this.localizer = localizer;
        }
        /// <summary>
        /// Looks up a localized string similar to 'class {name:s}'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Greetings {0}'
        /// </returns>
        public LocalizedString _class(string name)
            => global::Some.Root.Namespace.TypealizR_StringFormatter.Format(localizer["class {name:s}"], name);
        /// <summary>
        /// Looks up a localized string similar to '[Log.Warning] class {brother:s}'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Warn-a-{0}'
        /// </returns>
        public LocalizedString LogWarning_class(string brother)
            => global::Some.Root.Namespace.TypealizR_StringFormatter.Format(localizer["[Log.Warning] class {brother:s}"], brother);
        /// <summary>
        /// Gets the string resource with the given name.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <returns>The string resource as a <see cref="LocalizedString"/>.</returns>
        public LocalizedString this[string name] => this.localizer[name];
        /// <summary>
        /// Gets the string resource with the given name and formatted with the supplied arguments.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <param name="arguments">The values to format the string with.</param>
        /// <returns>The formatted string resource as a <see cref="LocalizedString"/>.</returns>
        public LocalizedString this[string name, params object[] arguments] => this.localizer[name, arguments];
        /// <summary>
        /// Gets all string resources.
        /// </summary>
        /// <param name="includeParentCultures">
        /// A <see cref="System.Boolean"/> indicating whether to include strings from parent cultures.
        /// </param>
        /// <returns>The strings.</returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => this.localizer.GetAllStrings(includeParentCultures);
        /// <summary>
        /// The wrapped <see cref="IStringLocalizer{TR0005_NoCode}"/>
        /// </summary>
        /// <returns>The wrapped <see cref="IStringLocalizer{TR0005_NoCode}"/>.</returns>
        public IStringLocalizer<global::Some.Root.Namespace.TR0005_NoCode> Localizer => this.localizer;
    }
}