using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileObject))]  //我们为TileObject拓展Editor的功能
public class TileEditor : Editor //编辑器类都要继承自Editor
{
    protected bool editMode = false;
    protected TileObject tileObject;

    private void OnEnable()
    {
        tileObject = (TileObject)target;  //target是Editor的成员变量，当我们指定CustomEditor的类型的时候就把这个成员变量传了进去
    }

    private void OnSceneGUI()
    {
        if (editMode)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));  //取消编辑器的选择功能，FocusType.Passive表示不接受键盘输入功能
            tileObject.debug = true;
            Event e = Event.current;  //相当于Input事件，只不过一个在游戏运行时检测，一个在Editor模式下检测

            if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && !e.alt)  //e.button表示按下了哪个鼠标按钮。0表示鼠标左键，1表示鼠标右键，2表示鼠标中键
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);  //editor模式下的鼠标坐标转射线
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, tileObject.layerMask))
                {
                    tileObject.setDataFromPosition(hitInfo.point.x, hitInfo.point.z, tileObject.dataID);//当我们点击鼠标时，就可以为当前x，z坐标下的棋盘格的棋子指定类型
                }
            }
        }
        HandleUtility.Repaint();  //重新绘制当前视图
    }

    public override void OnInspectorGUI() //自定义Inspector窗口UI
    {
        GUILayout.Label("Tile Editor");  //画一个标签
        editMode = EditorGUILayout.Toggle("Edit", editMode);  //一个Toggle开关
        tileObject.debug = EditorGUILayout.Toggle("Debug", tileObject.debug);

        string[] editDataStr = { "Dead", "Road", "Guard" };
        tileObject.dataID = GUILayout.Toolbar(tileObject.dataID, editDataStr);  //选择当前格子的类型, //Toolbar中传入的是当前的索引，当点击Toolbar时，返回点击的index，然后又更新显示toolbar上显示的索引

        EditorGUILayout.Separator();  //一个分隔符
        if (GUILayout.Button("Reset"))
        {
            tileObject.Reset();  //重新分配格子

        }
       DrawDefaultInspector(); //绘制默认的Inspector内容：TileObject脚本中的public变量 
    }
}
