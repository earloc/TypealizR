using System.Collections.Generic;
using TypealizR.Diagnostics;

namespace TypealizR.Core;
internal class DeduplicatingCollection<T> where T : IMemberModel
{
    private readonly Dictionary<string, T> models = [];
    private readonly Dictionary<string, int> duplicates = [];

    public void Add(T model, DiagnosticsCollector diagnostics)
    {
        if (!duplicates.ContainsKey(model.Name))
        {
            duplicates[model.Name] = 1;
        }
        else
        {
            var discriminator = duplicates[model.Name]++;
            model.DeduplicateWith(discriminator);
            diagnostics.Add(fac => fac.AmbigiousRessourceKey_0002(model.Name));
        }

        models[model.Name] = model;
    }

    public IEnumerable<T> Items => models.Values;

}
