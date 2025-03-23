﻿//HintName: TypealizR.Tests.CodeFirst.IMethodsWithXmlCommentParameters.g.cs
// <auto-generated/>
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Localization;
namespace TypealizR.Tests.CodeFirst {
    [GeneratedCode("TypealizR.CodeFirstSourceGenerator", "1.0.0.0")]
    public partial class MethodsWithXmlCommentParameters: IMethodsWithXmlCommentParameters {
        private readonly IStringLocalizer<IMethodsWithXmlCommentParameters> localizer;
        public MethodsWithXmlCommentParameters (IStringLocalizer<IMethodsWithXmlCommentParameters> localizer) {
          this.localizer = localizer;
        }
        #region methods
        #region Greet-method
        private const string Greet_Key = @"Greet";
        private const string Greet_FallbackKey = @"Hello {0}, the current time is: {1}";
        public LocalizedString Greet_Raw => localizer[Greet_Key].Or(localizer[Greet_FallbackKey]);
        public LocalizedString Greet (string user, DateTimeOffset now) => localizer[Greet_Key, user, now].Or(localizer[Greet_FallbackKey, user, now]);
        #endregion
        #region Farewell-method
        private const string Farewell_Key = @"Farewell";
        private const string Farewell_FallbackKey = @"The current time is: {1}, goodbye '{0}'";
        public LocalizedString Farewell_Raw => localizer[Farewell_Key].Or(localizer[Farewell_FallbackKey]);
        public LocalizedString Farewell (string user, DateTimeOffset now) => localizer[Farewell_Key, user, now].Or(localizer[Farewell_FallbackKey, user, now]);
        #endregion
        #region CallForBeetlejuice-method
        private const string CallForBeetlejuice_Key = @"CallForBeetlejuice";
        private const string CallForBeetlejuice_FallbackKey = @"1.{0} 2.{0} 3.{0}";
        public LocalizedString CallForBeetlejuice_Raw => localizer[CallForBeetlejuice_Key].Or(localizer[CallForBeetlejuice_FallbackKey]);
        public LocalizedString CallForBeetlejuice (string name) => localizer[CallForBeetlejuice_Key, name].Or(localizer[CallForBeetlejuice_FallbackKey, name]);
        #endregion
        #region DoIt-method
        private const string DoIt_Key = @"DoIt";
        private const string DoIt_FallbackKey = @"{1}, {0}. {1}!!";
        public LocalizedString DoIt_Raw => localizer[DoIt_Key].Or(localizer[DoIt_FallbackKey]);
        public LocalizedString DoIt (string name, string verb) => localizer[DoIt_Key, name, verb].Or(localizer[DoIt_FallbackKey, name, verb]);
        #endregion
        #endregion
        #region properties
        #endregion
    }
}