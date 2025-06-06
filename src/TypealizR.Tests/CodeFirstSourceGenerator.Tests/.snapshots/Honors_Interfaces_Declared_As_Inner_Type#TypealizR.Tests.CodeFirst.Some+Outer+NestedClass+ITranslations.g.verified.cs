﻿//HintName: TypealizR.Tests.CodeFirst.Some+Outer+NestedClass+ITranslations.g.cs
// <auto-generated/>
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Localization;
namespace TypealizR.Tests.CodeFirst {
    partial class Some {
        partial class Outer {
            partial class NestedClass {
                [GeneratedCode("TypealizR.CodeFirstSourceGenerator", "1.0.0.0")]
                internal partial class Translations: ITranslations {
                    private readonly IStringLocalizer<ITranslations> localizer;
                    public Translations (IStringLocalizer<ITranslations> localizer) {
                      this.localizer = localizer;
                    }
                    #region methods
                    #region Hello-method
                    private const string Hello_Key = @"Hello";
                    private const string Hello_FallbackKey = @"Hello {0}";
                    public LocalizedString Hello_Raw
                    {
                        get
                        {
                          var localizedString = localizer[Hello_Key];
                          if (!localizedString.ResourceNotFound)
                          {
                              return localizedString;
                          }
                          return localizer[Hello_FallbackKey];
                        }
                    }
                    public LocalizedString Hello (string world)
                    {
                        var localizedString = localizer[Hello_Key, world];
                        if (!localizedString.ResourceNotFound)
                        {
                            return localizedString;
                        }
                      return localizer[Hello_FallbackKey, world];
                    }
                    #endregion
                    #endregion
                    #region properties
                    #region World-property
                    private const string World_Key = @"World";
                    private const string World_FallbackKey = @"World";
                    public LocalizedString World
                    {
                      get
                        {
                          var localizedString = localizer[World_Key];
                          if (!localizedString.ResourceNotFound)
                          {
                              return localizedString;
                          }
                          return localizer[World_FallbackKey];
                      }
                    }
                    #endregion
                    #endregion
                }
            }
        }
    }
}