using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public static TileObject Instance { get; set; }
    public LayerMask layerMask;
    public float tileSize = 1;
    public int xTileCount = 2;
    public int zTileCount = 2;

    public int[] tileData; //格子的数值，0表示无法摆放物体，1表示敌人通道，2表示可摆放防守单位

    [HideInInspector]
    public int dataID = 0;  //当前数据id
   // [HideInInspector]
    public bool debug = false; //是否显示数据信息

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()  //当用户点击Inspector上下文菜单中的Reset按钮或第一次添加组件时，将调用Reset。此功能仅在编辑器模式下调用。重置最常用于在Inspector中提供良好的默认值。
    {
        tileData = new int[xTileCount * zTileCount];
    }

    public int getDataFromPosition(float pox, float poz)
    {
        int index = (int)((pox - transform.position.x) / tileSize) * zTileCount + (int)((poz - transform.position.z) / tileSize);
        if (index < 0 || index >= tileData.Length)
            return 0;
        return tileData[index];
    }

    public void setDataFromPosition(float pox, float poz, int number)
    {
        int index = (int)((pox - transform.position.x) / tileSize) * zTileCount + (int)((poz - transform.position.z) / tileSize);
        if (index < 0 || index >= tileData.Length)
            return ;
        tileData[index] = number;
    }

    private void OnDrawGizmos() //只能在编辑器界面被看到
    {
        if (!debug)
            return;

        if (tileData == null)
        {
            Debug.Log("Please reset data first");
            return;
        }

        Vector3 pos = transform.position;

        for (int i = 0; i < xTileCount; i++) //画z轴辅助线；
        {
            Gizmos.color = new Color(0,0,1,1);
            Gizmos.DrawLine(pos + new Vector3(tileSize * i, pos.y, 0), transform.TransformPoint(tileSize * i, pos.y, tileSize * zTileCount));  //两点之间画一条线，只不过一个用的TransformPoint，一个是用的向量的加法

            for (int k = 0; k < zTileCount; k++) //高亮显示当前数值的格子
            {
                if ((i * zTileCount + k) < tileData.Length && tileData[i * zTileCount + k] == dataID)
                {
                    Gizmos.color = new Color(1, 0, 0, 0.3f);
                    Gizmos.DrawCube(new Vector3(pos.x + i * tileSize + tileSize * 0.5f, pos.y, pos.z + k * tileSize + tileSize * 0.5f), new
                         Vector3(tileSize, 0.2f, tileSize));  //画一个cube，指定中心点（在这里是这个cube的中心）和size
                }
            }
        }

        for (int k = 0; k < zTileCount; k++) //画x方向辅助线
        {
            Gizmos.color = new Color(0, 0, 1, 1);
            Gizmos.DrawLine(pos + new Vector3(0, pos.y, tileSize * k), this.transform.TransformPoint(tileSize * xTileCount, pos.y, tileSize * k));
        }
    }
}
