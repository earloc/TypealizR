﻿//HintName: Some.Root.Namespace.Simple_with_Blanks.Typealized.g.cs
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
    /// Wraps a <see cref="IStringLocalizer{Simple_with_Blanks}"/> and provides properties for typed-access to resources, generated by TypealizR
    /// </summary>
    [GeneratedCode("TypealizR.TypealizedClassSourceGenerator", "1.0.0.0")]
    internal partial class TypealizedSimple_with_Blanks
    {
        private readonly IStringLocalizer<global::Some.Root.Namespace.Simple_with_Blanks> localizer;
        /// <summary>
        /// Creates a new instance of <see cref="TypealizedSimple_with_Blanks"/>
        /// </summary>
        public TypealizedSimple_with_Blanks(IStringLocalizer<global::Some.Root.Namespace.Simple_with_Blanks> localizer)
        {
            this.localizer = localizer;
        }
        /// <summary>
        /// Looks up a localized string similar to 'Hello'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'World'
        /// </returns>
        public LocalizedString Hello
            => localizer["Hello"];
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
        /// The wrapped <see cref="IStringLocalizer{Simple_with_Blanks}"/>
        /// </summary>
        /// <returns>The wrapped <see cref="IStringLocalizer{Simple_with_Blanks}"/>.</returns>
        public IStringLocalizer<global::Some.Root.Namespace.Simple_with_Blanks> Localizer => this.localizer;
    }
}