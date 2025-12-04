using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AutumnFramework
{


    public class AutumnEditor : EditorWindow
    {

        [Autowired]
        private static AutumnConfig autumnConfig;

        [MenuItem("Autumn/Beans")]
        public static void ShowWindows()
        {
            EditorWindow.GetWindow<AutumnEditor>();
        }

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        [RuntimeInitializeOnLoadMethod]
        public static void ModifyHierarchyWindowItemOnGUI()
        {
            EditorApplication.hierarchyWindowItemOnGUI += hierarchyWindowItemOnGUI;
        }

        private static void hierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {

            if (Application.isPlaying)
            {
                try
                {
                    var sceneGuard = Autumn.Harvest<AutumnSceneGuard>();
                    if (sceneGuard != null && sceneGuard.gameObject != null && instanceID == sceneGuard.gameObject.GetInstanceID())
                    {
                        EditorGUI.DrawRect(selectionRect, Color.black);
                        EditorGUI.LabelField(selectionRect, "Autumn Framework Powered (Dont Kill Me)");

                        selectionRect.width = selectionRect.height;
                        selectionRect.x -= selectionRect.width;
                        GUI.DrawTexture(selectionRect, EditorGUIUtility.IconContent("_Popup").image as Texture2D);
                    }
                }
                catch
                {
                    // 忽略异常，可能 IOC 还未初始化
                }
            }


        }
        private Vector2 scrollPosition;
        private void OnGUI()
        {
            if (Application.isPlaying)
            {

                if (GUILayout.Button("Autowired"))
                {
                    Autumn.Autowired();
                }
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                foreach (KeyValuePair<System.Type, BeanConfig> kvp in Autumn.IOC)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(kvp.Key.FullName);
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("PUSHBEAN"))
                    {
                        Autumn.NewBean(kvp.Key);
                    }
                    if (GUILayout.Button("HARVEST"))
                    {
                        Debug.Log(Autumn.Harvest(kvp.Key));
                    }
                    if (GUILayout.Button("OPEN"))
                    {
                        // new BeanDetailEditorWindow(kvp.Value).Show();
                        BeanDetailEditorWindow beanDetailEditorWindow = (BeanDetailEditorWindow)ScriptableObject.CreateInstance(typeof(BeanDetailEditorWindow));
                        beanDetailEditorWindow.beanConfig = kvp.Value;
                        beanDetailEditorWindow.Show();
                    }


                    GUILayout.EndHorizontal();
                    foreach (object bean in kvp.Value.Beans)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        switch (kvp.Value.BeanEntity)
                        {
                            case BeanConfig.Entity.ScriptalObject:
                                if (GUILayout.Button("ScriptableObject"))
                                    AssetDatabase.OpenAsset(MonoScript.FromScriptableObject((ScriptableObject)bean));

                                break;

                            case BeanConfig.Entity.Monobehaviour:
                                if (GUILayout.Button("MonoBehaviour"))
                                    // AssetDatabase.OpenAsset(MonoScript.FromMonoBehaviour((MonoBehaviour)bean));
                                    EditorUtility.OpenWithDefaultApp(AssetDatabase.GetAssetPath((MonoBehaviour)bean));
                                break;
                        }
                        {
                            GUIStyle style = new GUIStyle(GUI.skin.label);
                            style.normal.textColor = Color.green;
                            style.fontStyle = FontStyle.Bold;
                            GUILayout.Label(bean.ToString(), style);
                        }
                        GUILayout.EndHorizontal();
                    }
                    // GUILayout.Label("------------------------");
                    GUILayout.Space(20);
                }
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Bean仅存在于运行时");
            }

        }
    }


    [System.Serializable]
    public class BeanDetailEditorWindow : EditorWindow
    {

        [SerializeField]
        public BeanConfig beanConfig;
        private SerializedObject serializedObject;

        private void OnEnable()
        {
            this.serializedObject = new SerializedObject(this);
        }
        private void OnGUI()
        {
            serializedObject.Update();
            SerializedProperty property = serializedObject.FindProperty("beanConfig");
            EditorGUILayout.PropertyField(property, true);
            serializedObject.ApplyModifiedProperties();

        }

    }

}