using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Enum Not Currently In Use.
public enum PzType
{
    LeverPz,
    KillPz,
    StepPz
}

public class PuzzleManager : MonoBehaviour {
    [Header("Puzzle Type")]
    [SerializeField]
    PzType TypeOfPuzzle;
    [SerializeField] private bool _Pressed;

    [Header("Puzzle Items")]
    [Tooltip("This allows you to assign the object to be affected by solving the puzzle. If Working With Multiple Gameobjects Parent All Under One Object.")]
    [SerializeField]
    private GameObject InteractionObj;

    [Tooltip("This Object is used to Lock the doors to a room.  If Working With Multiple Gameobjects Parent All Under One Object.")]
    [SerializeField]
    private GameObject _LockedObjects;

    [Tooltip("This Should be Empty This Is the Players Progress So Far.")]
    [SerializeField]
    public string _OrderProg;

    [Tooltip("Sets the Combination")]
    [SerializeField]
    private string _SequenceToSolvePuzzle;

    [Tooltip("Add Levers to be pulled")]
    List<InteractableObject> _Levers;

    [Tooltip("This controls the amount of time to complete the puzzle.")]
    [SerializeField]
    private float _PzTime;

    [Tooltip("This Activates a Timer for the puzzle")]
    [SerializeField]
    private bool _HasTimer = false;

   // [Tooltip("This is the players current kill count.")]
   // [SerializeField]
   // private uint _CurKillCount;

    [Tooltip("This sets the puzzle Completion.")]
    [SerializeField]
    private bool _PuzzleComplete = false;
    
    private bool _PzResetting = false;
    private Color DefaultColor = Color.white;



    private void Awake()
    {
        _Levers = new List<InteractableObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            InteractableObject _leverToAdd = transform.GetChild(i).gameObject.GetComponent<InteractableObject>();
            _Levers.Add(_leverToAdd);
        }

        for (int i = 0; i < _Levers.Count; i++)
        {
            _Levers[i].Init();
        }
        PzReset();
    }

    private void Start()
    {
        if (TypeOfPuzzle == PzType.KillPz)
        {
            _LockedObjects.SetActive(false);

        }
    }




    public void PzCheck()
    {
        if(_OrderProg.Length == _Levers.Count && _OrderProg == _SequenceToSolvePuzzle)
        {
            //Interact With Lever
            //Pulls Can Be Set
            moveFloor();
            for(int i = 0; i < _Levers.Count; i++)
            {
                Debug.Log("Puzzle Correct Seq.");
                _Levers[i].gameObject.tag = "Untagged";
            }
            
        }
        else
        {
            for (int i = 0; i < _OrderProg.Length; i++)
            {
                if(_OrderProg[i] != _SequenceToSolvePuzzle[i])
                {
                    PzReset();
                    Debug.Log("Puzzle Reset.");
                }
            }
        } 
    }



    public void PzReset()
    {
        for (int i = 0; i < _Levers.Count; i++)
        {
            _Levers[i].PuzzleReset();
        }

        _OrderProg = "";
        StartCoroutine(Timer(1.0f));
    }

    public void CheckEnemiesLeft()
    {
        /*if (_CurKillCount >= _KillsToGet)
        {
            moveFloor();
        }
        else
        {
            Debug.Log("You Need " + (_KillsToGet - _CurKillCount) + " Kills left.");
        }*/
    }

    public void LockZone()
    {
        //Activate Doors 
        if (!_PuzzleComplete)
        {
            _LockedObjects.SetActive(true);

        }
    }

    public void moveFloor()
    {
        Debug.Log("puzzle complete");
        //Temp Disable Lock Zone.
        switch (TypeOfPuzzle)
        {
            case PzType.LeverPz:
                break;
            case PzType.StepPz:
                break;
            case PzType.KillPz:
                Debug.Log("Unlocked Doors");
                _LockedObjects.SetActive(false);
                _PuzzleComplete = true;
                //Enemies Killed.
                break;
            default:
                break;
        }
        if (this.InteractionObj.activeSelf == true)
        {
            this.InteractionObj.SetActive(false);
            

        }
        else
        {
            this.InteractionObj.SetActive(true);

        }
        //this.InteractionObj.transform.position += transform.forward * Time.deltaTime;
    }

    IEnumerator Timer(float waitTime)
    {
        _PzResetting = true;
        yield return new WaitForSeconds(waitTime);
        _PzResetting = false;
    }

    public PzType GetPzType()
    {
        return TypeOfPuzzle;
    }
    /*
    public uint GetCurKillCount()
    {
        return _CurKillCount;
    }
    public uint CurrentKCPlusOne()
    {
        _CurKillCount++;
        return _CurKillCount;
    }
    public uint SetCurKillCount(uint value)
    {
        _CurKillCount = value;
        return _CurKillCount;
    }
    */
    public bool GetPuzzleComplete()
    {
        return _PuzzleComplete;
    }
    /*
    public uint KillsToGet()
    {
        return _KillsToGet;
    }
    */
   
}
