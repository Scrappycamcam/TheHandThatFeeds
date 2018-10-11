using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour {
    [Header("Puzzle Type")]
    [SerializeField] private bool _Pressed;

    [Header("Puzzle Items")]
    [Tooltip("This allows you to assign the object to be affected by solving the puzzle. If Working With Multiple Gameobjects Parent All Under One Object.")]
    [SerializeField]
    private GameObject InteractionObj;

    [Tooltip("This Should be Empty This Is the Players Progress So Far.")]
    [SerializeField]
    public string _OrderProg;

    [Tooltip("Sets the Combination")]
    [SerializeField]
    private string _SequenceToSolvePuzzle;

    [Tooltip("Add Levers to be pulled")]
    List<InteractableObject> _Levers;

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
        PzReset();
    }


    //Enum Not Currently In Use.
    private enum PzType
    {
        LeverPz,
        KillPz,
        StepPz
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
                }
            }
        } 
    }

    public void PzReset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _Levers[i].gameObject.GetComponent<MeshRenderer>().material.color = DefaultColor;
        }

        _OrderProg = "";
        StartCoroutine(Timer(1.0f));
    }


    public void moveFloor()
    {
        Debug.Log("puzzle complete");
        if(this.InteractionObj.activeSelf == true)
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
}
