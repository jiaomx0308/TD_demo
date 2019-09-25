using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileWnd : UnityEditor.EditorWindow
{
    protected static TileObject tileObject;

    [MenuItem("Tools/Tile Window")]
    static void Create()
    {
        EditorWindow.GetWindow(typeof(TileWnd));  //当点击的时候，这步创建了一个空白的窗口

        if (Selection.activeTransform != null)
        {
            tileObject = Selection.activeTransform.GetComponent<TileObject>();  //当前正在活动中的selection Transform组件
        }
    }

    private void OnSelectionChange()  //当选中物体被改变时，重新获取档期被选中的物体
    {
        if (Selection.activeTransform != null)
        {
            tileObject = Selection.activeTransform.GetComponent<TileObject>();
        }
    }

    private void OnGUI()  //这个是在Window界面上绘制label，toggle，toolbar，sepatator
    {
        if (tileObject == null)
            return;

        GUILayout.Label("Tile Editor");
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GUI/butPlayer1.png");
        GUILayout.Label(tex);
        tileObject.debug = EditorGUILayout.Toggle("Debug", tileObject.debug);
        string[] editDataStr = { "Dead", "Road", "Guard" };
        tileObject.dataID = GUILayout.Toolbar(tileObject.dataID, editDataStr);  
        EditorGUILayout.Separator();
        if (GUILayout.Button("Reset"))
        {
            tileObject.Reset(); 

        }
    }
}
