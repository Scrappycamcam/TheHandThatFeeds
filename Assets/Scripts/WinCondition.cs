using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour {

    [SerializeField]
    float _percentToWin = .65f;

    private float enemiesKilled;
    private float totalEnemies;
    private bool BossDead;

    private BoxCollider _mycollider;
    private MeshRenderer _myRenderer;

    // Use this for initialization
    void Start()
    {

        _mycollider = GetComponent<BoxCollider>();
        _myRenderer = GetComponent<MeshRenderer>();

        _mycollider.enabled = false;
        _myRenderer.enabled = false;

        ResetWin();

        PlayerCanvas.Instance.SetGameReset += ResetWin;
    }

    private void CountEnemies()
    {
        AIEnemy[] _enemies = FindObjectsOfType<AIEnemy>();
        totalEnemies = _enemies.Length;
    }

	// Update is called once per frame
	void Update () {
        float ratio = enemiesKilled / totalEnemies;
        if (ratio >= _percentToWin && BossDead)
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
        _myRenderer.enabled = true;
        _mycollider.enabled = true;
    }

    public void ResetWin()
    {
        enemiesKilled = 0;
        CountEnemies();
    }

    public float GetKilledEnemiesCount { get { return enemiesKilled; } }
}
