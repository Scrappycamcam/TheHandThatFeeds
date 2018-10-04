using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSelection_Script : MonoBehaviour {

    //[SerializeField]private Canvas MainMenu;
    
    public void StartGame()
    {
        Debug.Log("Level 1");
        //Temp Start Game Most Likely Will be Changed.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
