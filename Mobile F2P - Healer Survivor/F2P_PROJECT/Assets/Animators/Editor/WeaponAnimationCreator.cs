using UnityEngine;
using UnityEditor;
using System.IO;

public class WeaponAnimationCreator : EditorWindow
{
    private const string MENU_ITEM_PATH = "Assets/Create/Weapon Animation";

    private string _WeaponName = "NewWeapon";

    [MenuItem(MENU_ITEM_PATH, false, 20)]
    public static void OpenWindow()
    {
        WeaponAnimationCreator lWindow = GetWindow<WeaponAnimationCreator>("Create Weapon");
        lWindow.minSize = new Vector2(300, 130);
        lWindow.maxSize = new Vector2(300, 130);
        lWindow.Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Configuration de l'animation", EditorStyles.boldLabel);

        GUILayout.Space(10);
        _WeaponName = EditorGUILayout.TextField("Nom de l'arme :", _WeaponName);

        GUILayout.Space(20);

        if (GUILayout.Button("Créer les fichiers", GUILayout.Height(30)))
        {
            CreateWeaponAnimation(_WeaponName);
            Close();
        }
    }

    private void CreateWeaponAnimation(string pWeaponName)
    {
        if (string.IsNullOrEmpty(pWeaponName))
        {
            pWeaponName = "NewWeapon";
        }

        string lPath = "Assets";

        if (Selection.activeObject != null)
        {
            lPath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
            if (!string.IsNullOrEmpty(lPath))
            {
                if (!Directory.Exists(lPath))
                {
                    lPath = Path.GetDirectoryName(lPath);
                }
            }
        }

        string lUniqueFolderPath = AssetDatabase.GenerateUniqueAssetPath(lPath + "/" + pWeaponName);
        string lFinalFolderName = Path.GetFileName(lUniqueFolderPath);
        AssetDatabase.CreateFolder(lPath, lFinalFolderName);

        AnimationClip lIdleClip = new AnimationClip();
        AnimationClip lPrepareClip = new AnimationClip();
        AnimationClip lAttackClip = new AnimationClip();

        AssetDatabase.CreateAsset(lIdleClip, lUniqueFolderPath + "/" + pWeaponName + "_Idle.anim");
        AssetDatabase.CreateAsset(lPrepareClip, lUniqueFolderPath + "/" + pWeaponName + "_Prepare.anim");
        AssetDatabase.CreateAsset(lAttackClip, lUniqueFolderPath + "/" + pWeaponName + "_Attack.anim");

        AnimatorOverrideController lOverrideController = new AnimatorOverrideController();
        AssetDatabase.CreateAsset(lOverrideController,lUniqueFolderPath + "/" + pWeaponName + "Animator" + ".overrideController");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Dossier d'animations créé avec succès : " + lUniqueFolderPath);
    }
}