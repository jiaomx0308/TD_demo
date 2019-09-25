using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public LayerMask groudLayer;
    public int wave = 1;
    public int waveMax = 10;
    public int life = 10;
    public int point = 30;
    public bool m_debug = true;
    public List<PathNode> m_PathNode;
    public List<Enemy> m_EnemyList = new List<Enemy>();

    Text waveText;
    Text lifeText;
    Text pointText;

    Button ReplayButton;
    bool isSelectedButton = false;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UnityAction<BaseEventData> downAction = new UnityAction<BaseEventData>(OnButtonCreateDefenderBown);  //UnityAction，UnityEvents使用的零参数委托， 一种函数指针，和C#的Action一样的作用
        UnityAction<BaseEventData> upAction = new UnityAction<BaseEventData>(OnButtonCreateDefenderUp);

        //按键按下事件
        EventTrigger.Entry down = new EventTrigger.Entry();  //EventTrigger 从EventSystem接收事件并为每个事件调用已注册的函数。EventTrigger可用于指定希望为每个EventSystem事件调用的函数。您可以为单个事件分配多个函数，每当EventTrigger接收到该事件时，它将按照它们提供的顺序调用这些函数。
        down.eventID = EventTriggerType.PointerDown;  //拦截IPointerDownHandler.OnPointerDown。 
        down.callback.AddListener(downAction);  //要调用的所需TriggerEvent

        //按键抬起事件
        EventTrigger.Entry up = new EventTrigger.Entry();
        up.eventID = EventTriggerType.PointerUp; //拦截IPointerUpHandler.OnPointerUP。 
        up.callback.AddListener(upAction);

        //查找子物体，根据名称获取UI控件
        waveText = transform.root.Find("Canvas/wave").GetComponent<Text>();
        lifeText = transform.root.Find("Canvas/life").GetComponent<Text>();
        pointText = transform.root.Find("Canvas/point").GetComponent<Text>();
        ReplayButton = transform.root.Find("Canvas/ButtonTry").GetComponent<Button>();

        ReplayButton.onClick.AddListener(() => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
        ReplayButton.gameObject.SetActive(false);


        waveText.text = $"波数： <color=yellow>{wave}/{waveMax}</color>";
        lifeText.text = $"生命：<color=yellow>{life}</color>";
        pointText.text = $"铜钱：<color=yellow>{point}</color>";

        //为按键添加按钮事件  //相当于实现了IPointDownHandler.OnPointerDown和IpointUpHandler.OnPointerUp的接口
        Transform player1 = transform.root.Find("Canvas/ButtonPlayer1");
        EventTrigger trigger = player1.gameObject.AddComponent<EventTrigger>(); //EventTrigger 从EventSystem接收事件并为每个事件调用已注册的函数
        trigger.triggers = new List<EventTrigger.Entry>();   //triggers，在此EventTrigger中注册的所有函数
        trigger.triggers.Add(down);
        trigger.triggers.Add(up);

        Transform player2 = transform.root.Find("Canvas/ButtonPlayer2");
        trigger = player2.gameObject.AddComponent<EventTrigger>();
        trigger.triggers = new List<EventTrigger.Entry>();
        trigger.triggers.Add(down);
        trigger.triggers.Add(up);

        /*
         * 官方案例，可以写给类继承自EventTrigger，然后override各种监听的方法，
         * 还可以想本例一样使用Eventtrigger组件自己动态添加：
            void Start()
            {
                EventTrigger trigger = GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
                trigger.triggers.Add(entry);
            }

            public void OnPointerDownDelegate(PointerEventData data)
            {
                Debug.Log("OnPointerDownDelegate called.");
            }   
         */


        BuildPath();
    }

    private void OnButtonCreateDefenderUp(BaseEventData data)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, groudLayer))
        {
            if (TileObject.Instance.getDataFromPosition(hitInfo.point.x, hitInfo.point.z) == (int)Defender.TileStatus.GUARD)
            {
                Vector3 hitpos = new Vector3(hitInfo.point.x, 0, hitInfo.point.z);
                Vector3 gridPos = TileObject.Instance.transform.position;
                float tileSize = TileObject.Instance.tileSize;

                hitpos.x = gridPos.x + (int)((hitpos.x - gridPos.x) / tileSize) * tileSize + tileSize * 0.5f;
                hitpos.z = gridPos.z + (int)((hitpos.z - gridPos.z) / tileSize) * tileSize + tileSize * 0.5f;

                GameObject go = data.selectedObject;

                if (go.name.Contains("1"))
                {
                    if (SetPoint(-15))
                        Defender.create<Defender>(hitpos, new Vector3(0, 180, 0));
                }
                else if (go.name.Contains("2"))
                {
                    if (SetPoint(-20))
                    {
                        Defender.create<Archer>(hitpos, new Vector3(0, 180, 0));
                    }
                }
            }
        }

        isSelectedButton = false;
    }

    private void OnButtonCreateDefenderBown(BaseEventData data)
    {
     //   throw new NotImplementedException();
    }

    public void SetWave(int wave)
    {
        this.wave = wave;
        waveText.text = $"波数： <color=yellow>{this.wave}/{this.waveMax}</color>";
    }

    public void SetDamage(int damage)
    {
        life -= damage;
        if (life <= 0)
        {
            life = 0;
            ReplayButton.gameObject.SetActive(true);
        }
        lifeText.text = $"生命：<color=yellow>{life}</color>";
    }

    public bool SetPoint(int point)
    {
        if (this.point + point < 0)
            return false;
        this.point += point;
        pointText.text = $"铜钱：<color=yellow>{this.point}</color>";
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelectedButton)
            return;
#if (UNITY_IOS || UNITY_ANDRIOD) && !UNITY_EDITOR
        bool press = Input.touches.Lenth > 0 ? true ： false ; //手指是否触屏
        float x = 0;
        float y = 0;
        if (press)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved) //获得手指移动距离
            {
                x = Input.GetTouch(0).deltaPosition.x * 0.01f;
                y = Input.GetTouch(0).deltaPosition.y * 0.01f;
            }
        }
#else
        bool press = Input.GetMouseButton(0);
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
#endif
        GameCamera.Instance.Control(press, x, y);
    }

    [ContextMenu("BuildPath")]
    void BuildPath()
    {
        m_PathNode = new List<PathNode>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("PathNode");
        for (int i = 0; i < objs.Length; i++)
        {
            m_PathNode.Add(objs[i].GetComponent<PathNode>());
        }
    }

    private void OnDrawGizmos()
    {
        if (!m_debug || m_PathNode == null)
            return;
        Gizmos.color = Color.yellow;
        foreach (var node in m_PathNode)
        {
            if (node.m_next != null)
            {
                Gizmos.DrawLine(node.transform.position, node.m_next.transform.position);
            }
        }
    }
}
