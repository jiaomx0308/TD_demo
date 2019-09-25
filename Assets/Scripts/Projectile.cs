using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    System.Action<Enemy> onAttack;
    Transform m_target;
    Bounds m_targetCenter; //边框

    public static void Create(Transform target, Vector3 spwanPos, System.Action<Enemy> onAttack)
    {
        GameObject prefab = Resources.Load<GameObject>("arrow");
        GameObject go = Instantiate(prefab, spwanPos, Quaternion.LookRotation(target.position - spwanPos));

        Projectile arrowModel = go.AddComponent<Projectile>();
        arrowModel.m_target = target;
        arrowModel.m_targetCenter = target.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        arrowModel.onAttack = onAttack;

        Destroy(go, 3.0f);
    }

    private void Update()  //在实际项目中不要在Update中直接调用resources.load Instantiate Destory；
    {
        if (m_target != null)
        {
            this.transform.LookAt(m_target.position);
        }
        this.transform.Translate(new Vector3(0, 0, 10*Time.deltaTime));
        if (m_target != null)
        {
            if (Vector3.Distance(this.transform.position, m_target.position) < 0.5f)
            {
                onAttack(m_target.GetComponent<Enemy>());
                Destroy(this.gameObject);
            }
        }
    }
}
