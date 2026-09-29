using UnityEngine;

//Author : MERFOUD Kelyan

namespace Tooling.Attributes
{
    public class AutoScriptable : ScriptableObject
    {
        public const string PATH = FOLDER_PATH + "/" + NAME + ".asset";
        public const string PATH_AS_RESOURCE = "Tooling/AutoLoad" + "/" + NAME;


        public const string ASSET_PATH = "Assets";
        public const string RESOURCE_PATH = ASSET_PATH + "/" + RESOURCE;
        public const string TOOLING_PATH = RESOURCE_PATH + "/" + TOOLING;
        public const string AUTOLOAD_PATH = TOOLING_PATH + "/" + AUTOLOAD;

        public const string FOLDER_PATH = "Assets/Resources/Tooling/AutoLoad";
        public const string NAME = "AutoLoadResource";
        public const string RESOURCE = "Resources";
        public const string TOOLING = "Tooling";
        public const string AUTOLOAD = "AutoLoad";

        public LoadableObject[] LoadableArray;
        public static string GetNewPathWithName(string pName) => FOLDER_PATH + "/" + pName + ".asset";
    }
}