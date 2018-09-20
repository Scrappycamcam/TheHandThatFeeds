using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour {

    NavMeshAgent enemyAgent;

    Vector3 startPoint;
    [SerializeField]
    float sightDistance;
    [SerializeField]
    float sightArea;
    [SerializeField]
    float followThreshold;
    [SerializeField]
    List<GameObject> patrolPoints;
    List<Vector3> patrolRoute;
    int currPath;
    bool alerted;

    //TestingSightScript target;

	// Use this for initialization
	void Start () {
        enemyAgent = GetComponent<NavMeshAgent>();
        patrolRoute = new List<Vector3>(); 
        for (int point = 0; point < patrolPoints.Count; point++)
        {
            patrolRoute.Add(patrolPoints[point].gameObject.transform.position);
        }
        startPoint = gameObject.transform.position;
        patrolRoute.Add(startPoint);
        currPath = 0;
        alerted = false;

        //target = TestingSightScript.tester;
	}
	
	// Update is called once per frame
	void Update () {
        if(!alerted)
        {
            PatrolState();
        }
        else
        {
            CombatStrats();
        }
        
	}

    private void PatrolState()
    {
        if(!enemyAgent.hasPath)
        {
            enemyAgent.SetDestination(patrolRoute[currPath]);
            if (currPath < patrolRoute.Count - 1)
            {
                currPath++;
            }
            else
            {
                currPath = 0;
            }
        }

        if(LookingForPlayer())
        {

        }
    }

    private bool LookingForPlayer()
    {

        return false;
    }

    private void CombatStrats()
    {
       /* if(Vector3.Distance(transform.position, target.transform.position) <= followThreshold)
        {
            FollowPlayer();
        }
        else
        {
            LostSightOfPlayer();
        }*/
    }

    private void FollowPlayer()
    {

    }

    private void LostSightOfPlayer()
    { 

    }
}
