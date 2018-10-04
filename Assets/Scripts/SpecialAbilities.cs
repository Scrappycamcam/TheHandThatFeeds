using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAbilities : MonoBehaviour {

    enum SpecialAbility
    {
        NONE,
        CYCLONE,
        THEGOODSUCC,
    }

    [SerializeField]
    float cycloneHealthBurden;
    [SerializeField]
    float stealHealthBurden;
    bool _usingSpecial = false;
    SpecialAbility _myability = SpecialAbility.NONE;

    PlayerStats _pStats;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void SpecialAbilityActive()
    {
        switch (_myability)
        {
            case SpecialAbility.NONE:
                _usingSpecial = false;
                break;
            case SpecialAbility.CYCLONE:
                if(cycloneHealthBurden >= _pStats.GetHealth)
                {
                    UsingCyclone();
                }
                else
                {
                    _myability = SpecialAbility.NONE;
                }
                break;
            case SpecialAbility.THEGOODSUCC:
                if (stealHealthBurden >= _pStats.GetHealth)
                {
                    GiveEmTheGoodSucc();
                }
                else
                {
                    _myability = SpecialAbility.NONE;
                }                
                break;
            default:
                break;
        }
    }

    private void UsingCyclone()
    {
        
    }

    private void GiveEmTheGoodSucc()
    {

    }
}
