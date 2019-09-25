using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathTool : ScriptableObject
{
    static PathNode m_parent = null;

    [MenuItem("PathTool/Create PathNode")]
    static void CreatePathNode()
    {
        GameObject go = new GameObject();
        go.AddComponent<PathNode>();
        go.name = "pathnode";
        go.tag = "PathNode";

        //使新创建的路点处于选择状态：
        Selection.activeTransform = go.transform;
    }

    [MenuItem("PathTool/set Parent %q")]
    static void SetParent()
    {
        if (!Selection.activeGameObject || Selection.GetTransforms(SelectionMode.Unfiltered).Length > 1)  //GetTransforms,允许使用SelectionMode位掩码对选择类型进行细粒度控制.这句话表示选中的物体大于一个时，就返回
            return;
        if (Selection.activeGameObject.tag.CompareTo("PathNode") == 0)
        {
            m_parent = Selection.activeGameObject.GetComponent<PathNode>();
        }
    }

    [MenuItem("PathTool/set Next %w")]
    static void SetNext()
    {
        if (!Selection.activeGameObject || m_parent == null || Selection.GetTransforms(SelectionMode.Unfiltered).Length > 1)
            return;
        if (Selection.activeGameObject.tag.CompareTo("PathNode") == 0)
        {
            m_parent.SetNext(Selection.activeGameObject.GetComponent<PathNode>());
            m_parent = null;
        }
    }


}
