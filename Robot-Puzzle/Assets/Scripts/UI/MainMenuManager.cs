using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    /// <summary>
    /// Lädt das Testlevel.
    /// </summary>
	public void StartLevel() {
        SceneManager.LoadScene("Level1");
    }

    /// <summary>
    /// Beendet das Spiel.
    /// </summary>
    public void ExitGame() {
        Application.Quit();
    }
}
