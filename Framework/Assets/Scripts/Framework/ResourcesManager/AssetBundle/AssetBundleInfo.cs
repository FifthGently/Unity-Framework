using System.Collections.Generic;

public class AssetBundleInfo
{
    public string fileName;
    public string assetName;
    public string bundleName;
    public List<string> dependencies;

    public void AddDependence(string dep)
    {
        if (dependencies == null)
            dependencies = new List<string>();

        dependencies.Add(dep);
    }
}
