using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
    public enum TileStatus  //格子的状态
    {
        DEAD = 0,
        ROAD = 1,
        GUARD = 2,
    }

    public float m_attackArea = 2.0f;
    public int m_power = 1;
    public float m_attackInterval = 2.0f;
    protected Enemy m_targetEnemy;
    protected bool m_isFaceEnemy;
    protected GameObject m_model;
    protected Animator m_an;

    public static T create<T>(Vector3 pos, Vector3 angle) where T : Defender
    {
        GameObject go = new GameObject("defender");
        go.transform.position = pos;
        go.transform.eulerAngles = angle;
        T d = go.AddComponent<T>();
        d.Init();

        TileObject.Instance.setDataFromPosition(d.transform.position.x, d.transform.position.z, (int)TileStatus.DEAD);
        return d;
    }

    protected virtual void Init()
    {
        m_attackArea = 2.0f;
        m_power = 2;
        m_attackInterval = 2.0f;

        CreateModel("swordman");
        StartCoroutine(Attack());
    }

    protected void CreateModel(string name)
    {
        GameObject model = Resources.Load<GameObject>(name);
        this.m_model = Instantiate(model, this.transform.position, this.transform.rotation, this.transform);  //在实际项目中不建议在Update中调用Resource.Load
        this.m_an = m_model.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()  //找到敌人  朝向敌人 攻击敌人
    {
        FindEnemy();
        RotateTo();
        Attack();
    }

    public void RotateTo()
    {
        if (this.m_targetEnemy == null)
            return;

        var targetdir = this.m_targetEnemy.transform.position - this.transform.position;
        targetdir.y = 0;
        Vector3 rotDate = Vector3.RotateTowards(this.transform.forward, targetdir, 20.0f * Time.deltaTime, 0.0F);
        Quaternion targetQuaternion = Quaternion.LookRotation(rotDate);

        float angle = Vector3.Angle(targetdir, this.transform.forward);
        if (angle < 1.0f)
        {
            m_isFaceEnemy = true;
        }
        else
            m_isFaceEnemy = false;

        this.transform.rotation = targetQuaternion;


    }

    public void  FindEnemy()
    {
        if (m_targetEnemy != null)
            return;
        m_targetEnemy = null;
        int minlife = 0;
        foreach (Enemy enemy in GameManager.Instance.m_EnemyList)
        {
            if (enemy.m_life <= 0)
                continue;
            Vector3 pos1 = this.transform.position;
            pos1.y = 0;
            Vector3 pos2 = enemy.transform.position;
            pos2.y = 0;
            float distance = Vector3.Distance(pos1, pos2); //均投影到xz平面上做判断，所以画面显示是3d，但是游戏逻辑是2d的,没有Y轴
            if (distance > m_attackArea)
                continue;

            //查找生命最低的敌人
            if (minlife == 0 || minlife > enemy.m_life)
            {
                m_targetEnemy = enemy;
                minlife = enemy.m_life;
            }
        }

        
    }

    protected virtual IEnumerator Attack()
    {
        while (m_targetEnemy == null || !m_isFaceEnemy)
            yield return 0;

        m_an.CrossFade("attack", 0.1f);  //使用标准化时间创建从当前状态到任何其他状态的交叉淡入淡出, 实现从其他状态转到attck状态的淡出效果

        while (!m_an.GetCurrentAnimatorStateInfo(0).IsName("attack"))
            yield return 0;

        float aniLenght = m_an.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(aniLenght * 0.5f);

        if (m_targetEnemy != null)
        {
            m_targetEnemy.SetDamage(m_power);
        }
        yield return new WaitForSeconds(aniLenght * 0.5f);

        this.m_an.CrossFade("idle", 0.1f);
        yield return new WaitForSeconds(m_attackInterval);
        StartCoroutine(Attack());
    }
}
