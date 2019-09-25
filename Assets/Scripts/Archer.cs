using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Defender
{
    protected override void Init()
    {
        m_attackArea = 5.0f;
        m_power = 1;
        m_attackInterval = 1.0f;

        CreateModel("archer");
        StartCoroutine(Attack());
    }

    protected override IEnumerator Attack()
    {
        while (m_targetEnemy == null || !m_isFaceEnemy)
            yield return 0;
        m_an.CrossFade("attck", 0.1f);

        while (!m_an.GetCurrentAnimatorStateInfo(0).IsName("attck"))
            yield return 0;

        float aniLenght = m_an.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(aniLenght * 0.5f);

        if (this.m_targetEnemy != null)
        {
            Vector3 pos = this.m_model.transform.Find("atkpoint").position;
            Projectile.Create(m_targetEnemy.transform, pos, (Enemy Enemy) =>
            {
                Enemy.SetDamage(m_power);
                m_targetEnemy = null;

            });
        }
        yield return new WaitForSeconds(aniLenght * 0.5f);
        m_an.CrossFade("idle", 0.1f);
        yield return new WaitForSeconds(m_attackInterval);
        StartCoroutine(Attack());
    }
}
