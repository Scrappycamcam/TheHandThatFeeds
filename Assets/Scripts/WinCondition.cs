using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour {

    [SerializeField]
    float _percentToWin = .65f;

    private float enemiesKilled;
    private float totalEnemies;
    private bool BossDead;

    // Use this for initialization
    void Start()
    {
        CountEnemies();
    }

    private void CountEnemies()
    {
        AIEnemy[] _enemies = FindObjectsOfType<AIEnemy>();
        totalEnemies = _enemies.Length;
    }

	// Update is called once per frame
	void Update () {
        float ratio = enemiesKilled / totalEnemies;
        if (ratio > _percentToWin && BossDead)
        {
            OpenEnd();
        }
	}

    public void EnemyDied()
    {
        enemiesKilled++;
    }

    public void BossDied()
    {
        BossDead = true;
    }

    private void OpenEnd()
    {

    }
}
