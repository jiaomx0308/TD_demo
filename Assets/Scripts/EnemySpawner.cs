using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public PathNode m_startNode;
    private int m_liveEnemy = 0;
    public List<WaveData> waves;  //通过这个waves来配置每波敌人的数量和类型
    int enemyIndex = 0;
    int waveIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {

        yield return new WaitForEndOfFrame(); //保证在Start后执行
        GameManager.Instance.SetWave(waveIndex + 1);  //跟新ui

        WaveData wave = waves[waveIndex];
        yield return new WaitForSeconds(wave.interval);  //间隔生成敌人
        while (enemyIndex < wave.enemyPrefab.Count)  //如果没有生成全部的敌人
        {
            Vector3 dir = m_startNode.transform.position - this.transform.position;
            GameObject enemy = Instantiate(wave.enemyPrefab[enemyIndex], this.transform.position, Quaternion.LookRotation(dir));
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.m_currentNode = m_startNode;

            enemyScript.m_life = wave.level * 3;
            enemyScript.m_maxLife = enemyScript.m_life;
            m_liveEnemy++;
            enemyScript.onDeath = new System.Action<Enemy>((Enemy e) =>
            {
                m_liveEnemy--;
            });

            enemyIndex++;
            yield return new WaitForSeconds(wave.interval);
        }

        while (m_liveEnemy > 0)
        {
            yield return 0;
        }

        enemyIndex = 0;
        waveIndex++;
        if (waveIndex < waves.Count)
            StartCoroutine(SpawnEnemies());
        else {

            // win
        }

        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.transform.position, "Spwaner.tif");
    }
}
