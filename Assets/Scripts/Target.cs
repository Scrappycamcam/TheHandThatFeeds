
using UnityEngine;
using UnityEngine.UI;
public class Target : MonoBehaviour {
    [SerializeField]
    private CanvasGroup healthGroup;
    public Image _currentHealthBar;
    public float _health = 50f;
    private float _MaxHealth = 50f;

    //public int xpGain = 0;
    //public int BonusXp = 200;

    //private int Random;
    private void OnGUI()
    {
        //DisplayHealth();
    }
    public void TakeDamage (float amount)
    {
        int randomNum = Random.Range(0, 10);
        //Debug.Log(randomNum);
        if (randomNum == 0)
        {
            //GameObject.Find("Player").GetComponent<Skills>().attackXp += BonusXp;
            //Debug.Log("Passed");
        }
        else
        {
            //GameObject.Find("Player").GetComponent<Skills>().attackXp += xpGain;
            //GameObject.Find("Player").GetComponent<Skills>().StrengthXp += xpGain;
        }
        
        _health -= amount;
        if(_health <= _MaxHealth * 0.99f)
        {
            healthGroup.alpha = 1;
        }
        if(_health <= 0f)
        {
            Die();
        }

    }

    void DisplayHealth()
    {
        float ratio = _health / _MaxHealth; //creates the health ratio
        _currentHealthBar.rectTransform.localScale = new Vector3(ratio, 1, 1); // sets the scale transform for the health bar
        //_HealthDisplay.text = (ratio * 100).ToString() + '%'; //use if display text is desired
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
