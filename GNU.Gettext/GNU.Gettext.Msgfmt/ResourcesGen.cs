using System;
using System.Resources;

namespace GNU.Gettext.Msgfmt;

public class ResourcesGen
{
    public Options Options { get; private set; }

    public ResourcesGen(Options options)
    {
        Options = options;
    }

    public void Run()
    {
        Catalog catalog = new();
        foreach (string fileName in Options.InputFiles)
        {
            Catalog temp = new();
            temp.Load(fileName);
            catalog.Append(temp);
        }

        using ResourceWriter writer = new ResourceWriter(Options.OutFile);
        foreach (CatalogEntry entry in catalog)
        {
            try
            {
                writer.AddResource(entry.Key, entry.IsTranslated ? entry.GetTranslation(0) : entry.String);
            }
            catch (Exception e)
            {
                string message = string.Format("Error adding item {0}", entry.String);
                if (!string.IsNullOrEmpty(entry.Context))
                    message = string.Format("Error adding item {0} in context '{1}'",
                                            entry.String, entry.Context);
                throw new Exception(message, e);
            }
        }
        writer.Generate();
    }
}
