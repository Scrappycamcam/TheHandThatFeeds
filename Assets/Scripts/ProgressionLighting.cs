using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionLighting : MonoBehaviour
{
    
    private Transform _Torch;
    BoxCollider _myCollider;
    List<GameObject> _torchFireHolder;

    private void Awake()
    {
        _myCollider = GetComponent<BoxCollider>();
        //Take all torches and turn off the fire
        _torchFireHolder = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject Torch = transform.GetChild(i).gameObject;
            GameObject Fire = Torch.transform.GetChild(1).gameObject;

            _torchFireHolder.Add(Fire);
        }

        for (int i = 0; i < _torchFireHolder.Count; i++)
        {
            _torchFireHolder[i].SetActive(false);
        }
    }

    public void TurnOnTorch()
    {
        for (int i = 0; i < _torchFireHolder.Count; i++)
        {
            _torchFireHolder[i].SetActive(true);
        }
        _myCollider.enabled = false;
    }
}
