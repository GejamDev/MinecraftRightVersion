using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu()]
public class CraftingRecipe : ScriptableObject
{
    public Item[] inputItem = new Item[9];
    public Item outputItem;
    public int outPutCount = 1;
}
#if UNITY_EDITOR
[CustomEditor(typeof(CraftingRecipe))]
class CraftingRecipeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cr = (CraftingRecipe)target;
        if(cr == null)
            return;

        Undo.RecordObject(cr, "Undo CraftingRecipe");

        
        
        GUILayout.Space(15);

        int index = 0;
        for (int y = 0; y < 3; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < 3; x++)
            {
                cr.inputItem[index] = (Item)EditorGUILayout.ObjectField(cr.inputItem[index], typeof(Item), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false), GUILayout.Width(70), GUILayout.Height(70));
                index++;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Delete All"))
        {
            for(int i = 0; i < cr.inputItem.Length; i++)
            {
                cr.inputItem[i] = null;
            }
        }

        GUILayout.BeginHorizontal();
        GUILayout.Space(75);
        GUILayout.Box(Resources.Load<Texture2D>("CraftingArrow"), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false), GUILayout.Width(70), GUILayout.Height(70));

        GUILayout.EndHorizontal();

        GUILayout.Space(10);


        GUILayout.BeginHorizontal();
        GUILayout.Space(75);

        cr.outputItem = (Item)EditorGUILayout.ObjectField(cr.outputItem, typeof(Item), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false), GUILayout.Width(70), GUILayout.Height(70));
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(85);
        cr.outPutCount = EditorGUILayout.IntField(cr.outPutCount, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false), GUILayout.Width(40), GUILayout.Height(20));
        GUILayout.EndHorizontal();
        EditorUtility.SetDirty(cr);
    }
}
#endif
