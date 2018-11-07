using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIKillPuzzleMelee : AIMovement {

    protected InteractableObject _puzzleRefernce;
    protected PuzzleManager _pzManagerRef;

    protected override void DeadActivate(Vector3 _dirToDie)
    {
        Debug.Log(transform.position);
        if (_puzzleRefernce.CheckForDeathArea(gameObject, transform.position))
        {
            base.DeadActivate(_dirToDie);
        }
        else
        {
            Debug.Log("cant die yet");
        }
    }

    protected override void Die()
    {
        base.Die();
    }

    public InteractableObject SetPuzzle { set { _puzzleRefernce = value; } }
}
