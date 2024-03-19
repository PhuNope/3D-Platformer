using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEditor.AssetDatabase;
using static System.IO.Path;

public static class Setup {

    [MenuItem("Tools/Setup/Create Default Folders")]
    public static void CreateDefaultFolders() {

        Folders.CreateDefault("_Project", "Animation", "Art", "Materials", "Prefabs", "ScriptableObjects", "Scripts", "Settings");
        Refresh();
    }

    [MenuItem("Tools/Setup/Import My Favorite Assets")]
    public static void ImportMyFavoriteAssets() {
        Assets.ImportAssets("DOTween HOTween v2.unitypackage", "Demigiant/ScriptingAnimation");
    }

    static class Folders {
        public static void CreateDefault(string root, params string[] folders) {
            string fullpath = Path.Combine(Application.dataPath, root);
            foreach (string folder in folders) {
                string path = Path.Combine(fullpath, folder);
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
            }
        }
    }

    static class Assets {
        public static void ImportAssets(string asset, string subfolder, string folder = "C:/Users/Admin/AppData/Roaming/Unity/Asset Store-5.x") {
            AssetDatabase.ImportPackage(Combine(folder, subfolder, asset), false);
        }
    }
}