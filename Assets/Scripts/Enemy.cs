using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public PathNode m_currentNode;
    public int m_life = 15;
    public int m_maxLife = 15;
    public float m_speed = 2;
    public System.Action<Enemy> onDeath;

    Transform m_lifeBarObj;
    Slider m_lifebar;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.m_EnemyList.Add(this);

        GameObject prefab = (GameObject)Resources.Load("Canvas3D");
        m_lifeBarObj = Instantiate(prefab, Vector3.zero, Camera.main.transform.rotation, this.transform).transform;
        m_lifeBarObj.localPosition = new Vector3(0, 2.0f, 0);
        m_lifeBarObj.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        m_lifebar = m_lifeBarObj.GetComponentInChildren<Slider>();

        StartCoroutine(UpdateLifeBar());
    }

    IEnumerator UpdateLifeBar()
    {
        m_lifebar.value = (float)m_life / m_maxLife;
        m_lifeBarObj.transform.eulerAngles = Camera.main.transform.eulerAngles;
        yield return 0;
        StartCoroutine(UpdateLifeBar());
    }

    // Update is called once per frame
    public void Update()
    {
        RotateTo();
        MoveTo();
    }

    void RotateTo()
    {
        //仅仅向y轴旋转：
        var position = m_currentNode.transform.position - this.transform.position;
        position.y = 0;  //y轴置为0，表示该旋转方向只在和zx轴平面上旋转，也就是绕y轴旋转
        var targetRotation = Quaternion.LookRotation(position);
        
        float next = Mathf.MoveTowardsAngle(this.transform.eulerAngles.y, targetRotation.eulerAngles.y, 120 * Time.deltaTime); //这里也可以直接使用Quaternion.Lerp
        this.transform.eulerAngles = new Vector3(0, next, 0);
    }

    public void MoveTo()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_currentNode.transform.position;
        float dist = Vector2.Distance((new Vector2(pos1.x, pos1.z)), new Vector2(pos2.x, pos2.z));
        if (dist < 1.0f)
        {
            if (m_currentNode.m_next == null)
            {
                GameManager.Instance.SetDamage(1);
                GameManager.Instance.m_EnemyList.Remove(this);
                Destroy(this.gameObject);
            }
            else
                m_currentNode = m_currentNode.m_next;
        }

        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime)); //Translate默认以自身坐标系移动，z方向就是forward方向，我们通过先RotateTo函数调用，先转向了路点的方向，保证了朝着路点前进
    }


    public void DestroyMe()
    {
        GameManager.Instance.m_EnemyList.Remove(this);
        onDeath(this);
        Destroy(this.gameObject);
    }

    public void SetDamage(int damage)
    {
        m_life -= damage;
        if (m_life < 0)
        {
            m_life = 0;
            GameManager.Instance.SetPoint(5);
            DestroyMe();
        }
    }
}
