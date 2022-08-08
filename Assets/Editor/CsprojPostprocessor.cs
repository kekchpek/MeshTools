using System.Text.RegularExpressions;
using UnityEditor;

// ReSharper disable once CheckNamespace
public class CsprojPostprocessor : AssetPostprocessor
{
    
    public static string OnGeneratedCSProject(string path, string content)
    {
        if (!path.EndsWith("Editor.csproj") && !path.EndsWith("Tests.csproj"))
        {
            var newContent = 
                Regex.Replace(content, "<Reference Include=\"UnityEditor(.|\n)*?</Reference>", "");

            return newContent;
        }

        return content;
    }
}
