using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(ShaderEffect))]
public class AssetBuilderEditor : EditorWindow
{

    [MenuItem("Beat Saber Shader Extensions/Asset Builder")]
    static void Init() {
        AssetBuilderEditor window = (AssetBuilderEditor) GetWindow(typeof(AssetBuilderEditor), true, "Shader Extensions - Asset Builder");
        window.Show();
    }

    private string assetName = "";
    private string shaderName = "";
    private string authorName = "";
    private string description = "";
    private bool isScreenSpace = true;

    private Material shaderMaterial;
    private Texture2D previewImage;

    private static string EXTENSION = "bsfx";

    void OnGUI() {



        assetName = EditorGUILayout.TextField("Shader Reference Name: ", assetName);

        shaderName = EditorGUILayout.TextField("Shader Name: ", shaderName);
        authorName = EditorGUILayout.TextField("Author: ", authorName);

        isScreenSpace = EditorGUILayout.Toggle("Is Screen Space Shader: ", isScreenSpace);

        previewImage = (Texture2D) EditorGUILayout.ObjectField("Preview Image:", previewImage, typeof(Texture2D));

        shaderMaterial = (Material) EditorGUILayout.ObjectField("Shader Material:", shaderMaterial, typeof(Material));

        EditorGUILayout.LabelField("Additional Info / Description:");
        description = EditorGUILayout.TextArea(description);

        if (GUILayout.Button("Build Asset Bundle")) {

            if (shaderMaterial == null) {
                Debug.LogError("A Material must be set!");
                return;
            }

            if (assetName.Equals("")) {
                Debug.LogError("Shader Reference Name must be set!");
                return;
            }

            if (assetName.Contains(" ")) {
                Debug.LogError("Shader Reference Name can not include spaces!");
                return;
            }

            if (shaderName.Equals("")) {
                Debug.LogError("Shader Name must be set!");
                return;
            }

            GameObject shaderEffectMetadataGO = new GameObject("ShaderEffectMetadata");
            ShaderEffect shaderEffect = shaderEffectMetadataGO.AddComponent<ShaderEffect>();

            shaderEffect.material = shaderMaterial;
            shaderEffect.referenceName = assetName;
            shaderEffect.name = shaderName;
            shaderEffect.description = description;
            shaderEffect.author = authorName;
            shaderEffect.isScreenSpace = isScreenSpace;
            shaderEffect.previewImage = previewImage;

            string localPath = "Assets/" + shaderEffectMetadataGO.name + ".prefab";

            // Make sure the file name is unique, in case an existing Prefab has the same name.
            //localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            // Create the new Prefab.
            PrefabUtility.SaveAsPrefabAssetAndConnect(shaderEffectMetadataGO, localPath, InteractionMode.AutomatedAction);

            // Create the array of bundle build details.
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

            buildMap[0].assetBundleName = assetName + "." + EXTENSION;

            string materialPath = AssetDatabase.GetAssetPath(shaderMaterial);


            AssetDatabase.CopyAsset(materialPath, "Assets/ShaderEffect.mat");

            string[] assetsToBundle = new string[2];
            assetsToBundle[0] = "Assets/ShaderEffect.mat";
            assetsToBundle[1] = localPath;

            buildMap[0].assetNames = assetsToBundle;

            string[] guids = AssetDatabase.FindAssets("bsfx", new[] { "Assets/Output" });
            Debug.Log("guids: " + guids.Length);
            if (guids.Length < 1) {
                AssetDatabase.CreateFolder("Assets", "Output");
            }


            BuildPipeline.BuildAssetBundles("Assets/Output/", buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

            AssetDatabase.DeleteAsset("Assets/ShaderEffect.mat");
            AssetDatabase.DeleteAsset(localPath);
            GameObject.DestroyImmediate(shaderEffectMetadataGO);

            Debug.Log($"ShaderAssetBundle {assetName} has been built!");

        }

    }
}