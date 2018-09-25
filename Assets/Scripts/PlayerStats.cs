using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    public Image _currentHealthBar;
    public Text _HealthDisplay;
    public float _PHealth = 100;
    private float _PmaxHealth = 100;
 





    private void Start()
    {
        //PDamage(15);
    }

    private void OnGUI()
    {
        DisplayHealth();
    }

    void DisplayHealth()
    {
        float ratio = _PHealth / _PmaxHealth; //creates the health ratio
        _currentHealthBar.rectTransform.localScale = new Vector3(ratio, 1, 1); // sets the scale transform for the health bar
        //_HealthDisplay.text = (ratio * 100).ToString() + '%'; //use if display text is desired
    }
    public float PDamage(float DtoTake)//function for taking damage
    {
        if(_PHealth >= DtoTake)
        {
            _PHealth = _PHealth - DtoTake;

        }
        else
        {
            _PHealth = 0;
        }

        return _PHealth;
    }

    public float PHeal(float Heal)//function for healing
    {
        if(_PHealth < _PmaxHealth)
        {
            _PHealth = _PHealth + Heal;
        }
        else
        {
            _PHealth = _PmaxHealth;
        }
        return _PHealth;
    }

    

}
