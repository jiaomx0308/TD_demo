using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public static GameCamera Instance;
    protected float distance = 15; //距离地面高度
    protected Vector3 cameraRotate = new Vector3(-55, 180, 0); //摄像机角度   //向x轴旋转-55是想斜仰观察地形，向y轴旋转180度，是适应从屏幕鼠标接受到的Mouse Y的值
                                                                                //如果这里不向y轴旋转180度，在接收Mouse Y的值时，就要乘以-1
    protected float moveSpeed = 60;
    protected float veloctyX = 0;
    protected float veloctyY = 0;
    protected Transform m_transform;
    protected Transform m_cameraPoint; // 摄像机的焦点

    private void Awake()
    {
        Instance = this;
        m_transform = this.transform;
    }


    // Start is called before the first frame update
    void Start()
    {
        m_cameraPoint = CameraPoint.Instance.transform;
        Follow();
    }


    private void LateUpdate() //摄像机一般使用LateUpdate进行更新
    {

        Follow();
    }


    public void Control(bool mouse, float x, float y)
    {
        if (!mouse)
            return;
        m_cameraPoint.eulerAngles = Vector3.zero;  //先调整角度再平移，Translate默认沿着space.self的方向移动
        m_cameraPoint.Translate(-x, 0, -y); //跟着鼠标平移  //这里取-x， -y使摄像机向相反方向移动，然玩家以为是拖动的地图在上下移动
    }

    void Follow()
    {
        m_cameraPoint.eulerAngles = cameraRotate;
        m_transform.position = m_cameraPoint.TransformPoint(new Vector3(0, 0, distance));
        transform.LookAt(m_cameraPoint);
    }
}
