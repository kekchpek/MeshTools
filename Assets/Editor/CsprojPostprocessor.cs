using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
public class CsprojPostprocessor : AssetPostprocessor
{
    
    public static string OnGeneratedCSProject(string path, string content)
    {
        if (!path.EndsWith("Editor.csproj"))
        {
            Debug.Log(path);
            var newContent = 
                Regex.Replace(content, "<Reference Include=\"UnityEditor(.|\n)*?</Reference>", "");

            return newContent;
        }

        return content;
    }
}
