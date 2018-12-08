using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour {

    [SerializeField]
    float _percentToWin = .65f;

    private float enemiesKilled;
    private float totalEnemies;
    [SerializeField]
    private bool BossDead;
    [SerializeField]
    private GameObject _EndLevelDoor;
    private SphereCollider _mycollider;
    private MeshRenderer _myRenderer;

    // Use this for initialization
    void Start()
    {

        _mycollider = GetComponent<SphereCollider>();
        _myRenderer = GetComponent<MeshRenderer>();

        _mycollider.enabled = false;
        _myRenderer.enabled = false;
        _EndLevelDoor.SetActive(true);
        ResetWin();

        PlayerCanvas.Instance.SetGameReset += ResetWin;
    }

    private void CountEnemies()
    {
        AIEnemy[] _enemies = FindObjectsOfType<AIEnemy>();
        totalEnemies = _enemies.Length;
        Debug.Log(totalEnemies);
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
        //_EndLevelDoor.SetActive(false);
    }

    public void ResetWin()
    {
        _myRenderer.enabled = false;
        _mycollider.enabled = false;
        enemiesKilled = 0;
        BossDead = false;
        CountEnemies();
        //_EndLevelDoor.SetActive(true);
    }

    public float GetKilledEnemiesCount { get { return enemiesKilled; } }
}
