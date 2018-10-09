using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInteraction : MonoBehaviour {
    public string _ItemText;
    public bool _ItemDisplay;
    private float _maxDist = 10f;
    public Transform _PlayerPos;
    public int _playerId = 0;

    private Player _player;

	// Use this for initialization
	void Start () {
        _ItemText = "Hello World";
        _player = ReInput.players.GetPlayer(_playerId);
    }
	
	// Update is called once per frame
	void Update () {
        if (_player.GetButtonDown("Interact"))
        {
            ShootRay();
        }
    }

    private void OnGUI()
    {
        if (_ItemDisplay)
            DisplayInfo(_ItemText);
    }



    private void OnTriggerExit(Collider other)
    {
        if (_ItemDisplay)
        {
            _ItemDisplay = false;
        }
    }

    private void ShootRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _maxDist))
        {
            GameObject thingHit = hit.collider.gameObject;
            Debug.Log(thingHit);
            if (thingHit.GetComponent<InteractableObject>())
            {
                Debug.Log("Hit" + hit.transform.position);
                thingHit.GetComponent<InteractableObject>().Interact();
                _ItemDisplay = true;
            }

        }
    }

    private void DisplayInfo(string Text)
    {
        GUI.Box(new Rect(10, 10, 400, 200), Text);
        
    }
}
