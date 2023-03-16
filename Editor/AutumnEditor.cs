using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AutumnFramework
{


    public class AutumnEditor : EditorWindow
    {
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
                if (instanceID == Autumn.Harvest<AutumnSceneThread>().gameObject.GetInstanceID())
                {
                    EditorGUI.DrawRect(selectionRect, Color.black);
                    EditorGUI.LabelField(selectionRect, " Autumn 框架已经启动。 禁用此物件，将禁用Bean的Unity消息支持");

                    selectionRect.width = selectionRect.height;
                    selectionRect.x -= selectionRect.width;
                    GUI.DrawTexture(selectionRect, EditorGUIUtility.IconContent("_Popup").image as Texture2D);

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
                        Autumn.PushBean(kvp.Key);
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
                        if (kvp.Value.BeanEntity == BeanConfig.Entity.ScriptalObject)
                            if (GUILayout.Button("ScriptableObject"))
                            {
                                AssetDatabase.OpenAsset(MonoScript.FromScriptableObject((ScriptableObject)bean));
                            }
                        else if (kvp.Value.BeanEntity == BeanConfig.Entity.Monobehaviour)
                                if (GUILayout.Button("MonoBehaviour"))
                                {
                                    AssetDatabase.OpenAsset(MonoScript.FromMonoBehaviour((MonoBehaviour)bean));
                                }

                        GUIStyle style = new GUIStyle(GUI.skin.label);
                        style.normal.textColor = Color.green;
                        style.fontStyle = FontStyle.Bold;
                        GUILayout.Label(bean.ToString(), style);
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