using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInteraction : MonoBehaviour {

    public bool _ItemDisplay;
    public Transform _PlayerPos;
    public int _playerId = 0;

    RaycastHit hit;
    InteractableObject _currObject;

    private Player _player;
    [SerializeField]
    float _interactDistance;
    [SerializeField]
    bool _debug = false;

	// Use this for initialization
	void Start () {
        _player = ReInput.players.GetPlayer(_playerId);
    }
	
	// Update is called once per frame
	void Update () {
        if (_player.GetButtonDown("Interact") && _currObject != null)
        {
            ActivateObject();
        }

        CheckForObject();
    }


    private void CheckForObject()
    {
        if(_debug)
        {
            Debug.DrawLine(transform.position + Vector3.up, transform.position + (transform.forward * _interactDistance) + Vector3.up);
        }

        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, _interactDistance))
        {
            if (_currObject == null)
            {
                GameObject thingHit = hit.collider.gameObject;
                //Debug.Log(thingHit);
                if (thingHit.GetComponent<InteractableObject>())
                {
                    _currObject = thingHit.GetComponent<InteractableObject>();
                    _currObject.ShowIcon();
                }    
            }
        }
        else
        {
            if (_currObject != null)
            {
                _currObject.TurnOffIcon();
                _currObject = null;
            }
        }

    }

    private void ActivateObject()
    {
        _currObject.Interact();
    }

    

}
