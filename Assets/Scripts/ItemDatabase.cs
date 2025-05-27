using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ItemDatabaseLoader : MonoBehaviour
{
    public ItemData[] allItems;

    // Автоматично підвантажує всі ItemData з Resources при зміні скрипта в редакторі
    void OnValidate()
    {
#if UNITY_EDITOR
        allItems = Resources.LoadAll<ItemData>("");
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ItemDatabaseLoader))]
public class ItemDatabaseViewerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var db = (ItemDatabaseLoader)target;
        if (db.allItems != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("ItemData Preview", EditorStyles.boldLabel);

            foreach (var item in db.allItems)
            {
                if (item == null) continue;
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Name:", item.itemName);
                EditorGUILayout.LabelField("Type:", item.itemType.ToString());
                EditorGUILayout.LabelField("Description:", item.description);
                EditorGUILayout.LabelField("Max Stack:", item.maxStack.ToString());
                EditorGUILayout.ObjectField("Icon", item.icon, typeof(Sprite), false);
                EditorGUILayout.ObjectField("Prefab", item.prefab, typeof(GameObject), false);
                EditorGUILayout.EndVertical();
            }
        }
    }
}
#endif