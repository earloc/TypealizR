﻿//HintName: Some.Root.Namespace.Groupings_NoCode.Typealized.g.cs
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
    /// Wraps a <see cref="IStringLocalizer{Groupings_NoCode}"/> and provides properties for typed-access to resources, generated by TypealizR
    /// </summary>
    [GeneratedCode("TypealizR.TypealizedClassSourceGenerator", "1.0.0.0")]
    internal partial class TypealizedGroupings_NoCode
    {
        private readonly IStringLocalizer<global::Some.Root.Namespace.Groupings_NoCode> localizer;
        /// <summary>
        /// Creates a new instance of <see cref="TypealizedGroupings_NoCode"/>
        /// </summary>
        public TypealizedGroupings_NoCode(IStringLocalizer<global::Some.Root.Namespace.Groupings_NoCode> localizer)
        {
            this.localizer = localizer;
            Log = new LogGroup(localizer);
            Question = new QuestionGroup(localizer);
            Warning = new WarningGroup(localizer);
        }
        /// <summary>
        /// Looks up a localized string similar to 'Greetings {name}, today is {date}'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Greetings {0}, today is {1}'
        /// </returns>
        public LocalizedString Greetings_today_is(object name, object date)
            => localizer["Greetings {name}, today is {date}"].Format(name, date);
        /// <summary>
        /// Looks up a localized string similar to 'Hello'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Hello'
        /// </returns>
        public LocalizedString Hello
            => localizer["Hello"];
        /// <summary>
        /// Looks up a localized string similar to 'Hello {name:s}, today is {date:d}'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Hello {0}, today is {1}'
        /// </returns>
        public LocalizedString Hello_today_is(string name, DateOnly date)
            => localizer["Hello {name:s}, today is {date:d}"].Format(name, date);
        /// <summary>
        /// Gets the Log translator group.
        /// </summary>
        public LogGroup Log { get; }
        /// <summary>
        /// Gets the Question translator group.
        /// </summary>
        public QuestionGroup Question { get; }
        /// <summary>
        /// Gets the Warning translator group.
        /// </summary>
        public WarningGroup Warning { get; }
         /// <summary>
         /// Nested class created to provide grouped translations.
         /// </summary>
        [GeneratedCode("TypealizR.TypealizedClassSourceGenerator", "1.0.0.0")]
        internal partial class LogGroup
        {
            private readonly IStringLocalizer<global::Some.Root.Namespace.Groupings_NoCode> localizer;
            /// <summary>
            /// Creates a new instance of <see cref="LogGroup"/>
            /// </summary>
            [DebuggerStepThrough]
            public LogGroup(IStringLocalizer<global::Some.Root.Namespace.Groupings_NoCode> localizer)
            {
                this.localizer = localizer;
                Critical = new CriticalGroup(localizer);
                Warning = new WarningGroup(localizer);
            }
            /// <summary>
            /// Gets the Critical translator group.
            /// </summary>
            public CriticalGroup Critical { get; }
            /// <summary>
            /// Gets the Warning translator group.
            /// </summary>
            public WarningGroup Warning { get; }
             /// <summary>
             /// Nested class created to provide grouped translations.
             /// </summary>
            [GeneratedCode("TypealizR.TypealizedClassSourceGenerator", "1.0.0.0")]
            internal partial class CriticalGroup
            {
                private readonly IStringLocalizer<global::Some.Root.Namespace.Groupings_NoCode> localizer;
                /// <summary>
                /// Creates a new instance of <see cref="CriticalGroup"/>
                /// </summary>
                [DebuggerStepThrough]
                public CriticalGroup(IStringLocalizer<global::Some.Root.Namespace.Groupings_NoCode> localizer)
                {
                    this.localizer = localizer;
                }
        /// <summary>
        /// Looks up a localized string similar to '[Log.Critical]: Failed to delete {UserName:s}'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Failed to delete {0}'
        /// </returns>
        public LocalizedString Failed_to_delete(string UserName)
            => localizer["[Log.Critical]: Failed to delete {UserName:s}"].Format(UserName);
            }
             /// <summary>
             /// Nested class created to provide grouped translations.
             /// </summary>
            [GeneratedCode("TypealizR.TypealizedClassSourceGenerator", "1.0.0.0")]
            internal partial class WarningGroup
            {
                private readonly IStringLocalizer<global::Some.Root.Namespace.Groupings_NoCode> localizer;
                /// <summary>
                /// Creates a new instance of <see cref="WarningGroup"/>
                /// </summary>
                [DebuggerStepThrough]
                public WarningGroup(IStringLocalizer<global::Some.Root.Namespace.Groupings_NoCode> localizer)
                {
                    this.localizer = localizer;
                }
        /// <summary>
        /// Looks up a localized string similar to '[Log.Warning]: Could not find {UserName:s}'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Could not find {0}'
        /// </returns>
        public LocalizedString Could_not_find(string UserName)
            => localizer["[Log.Warning]: Could not find {UserName:s}"].Format(UserName);
        /// <summary>
        /// Looks up a localized string similar to '[Log.Warning]: Unknown error'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Encountered an unknown error during the operation'
        /// </returns>
        public LocalizedString Unknown_error
            => localizer["[Log.Warning]: Unknown error"];
            }
        }
         /// <summary>
         /// Nested class created to provide grouped translations.
         /// </summary>
        [GeneratedCode("TypealizR.TypealizedClassSourceGenerator", "1.0.0.0")]
        internal partial class QuestionGroup
        {
            private readonly IStringLocalizer<global::Some.Root.Namespace.Groupings_NoCode> localizer;
            /// <summary>
            /// Creates a new instance of <see cref="QuestionGroup"/>
            /// </summary>
            [DebuggerStepThrough]
            public QuestionGroup(IStringLocalizer<global::Some.Root.Namespace.Groupings_NoCode> localizer)
            {
                this.localizer = localizer;
            }
        /// <summary>
        /// Looks up a localized string similar to '[Question]: Abort ?'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Abort?'
        /// </returns>
        public LocalizedString Abort
            => localizer["[Question]: Abort ?"];
        /// <summary>
        /// Looks up a localized string similar to '[Question]: Continue to delete {UserName:s}?'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of 'Continue to delete {0}?'
        /// </returns>
        public LocalizedString Continue_to_delete(string UserName)
            => localizer["[Question]: Continue to delete {UserName:s}?"].Format(UserName);
        }
         /// <summary>
         /// Nested class created to provide grouped translations.
         /// </summary>
        [GeneratedCode("TypealizR.TypealizedClassSourceGenerator", "1.0.0.0")]
        internal partial class WarningGroup
        {
            private readonly IStringLocalizer<global::Some.Root.Namespace.Groupings_NoCode> localizer;
            /// <summary>
            /// Creates a new instance of <see cref="WarningGroup"/>
            /// </summary>
            [DebuggerStepThrough]
            public WarningGroup(IStringLocalizer<global::Some.Root.Namespace.Groupings_NoCode> localizer)
            {
                this.localizer = localizer;
            }
        /// <summary>
        /// Looks up a localized string similar to '[Warning]: {UserName:s} will be deleted!'
        /// </summary>
        /// <returns>
        /// A localized version of the current default value of '{0} will be deleted'
        /// </returns>
        public LocalizedString will_be_deleted(string UserName)
            => localizer["[Warning]: {UserName:s} will be deleted!"].Format(UserName);
        }
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
        /// The wrapped <see cref="IStringLocalizer{Groupings_NoCode}"/>
        /// </summary>
        /// <returns>The wrapped <see cref="IStringLocalizer{Groupings_NoCode}"/>.</returns>
        public IStringLocalizer<global::Some.Root.Namespace.Groupings_NoCode> Localizer => this.localizer;
    }
}