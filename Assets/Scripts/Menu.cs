using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    public DisplayText dt;
    public string text = "";
    // Start is called before the first frame update

        void Start()
        {
        dt = GameObject.FindGameObjectWithTag("Respawn").GetComponent<DisplayText>();
        text = PlayerPrefs.GetString("ResultText");
        if (dt.firsttime)
        {
            GameObject.FindGameObjectWithTag("PlayButton").GetComponent<TextMeshProUGUI>().text = "Play";
            dt.firsttime = false;
        }
        else {
            GameObject.FindGameObjectWithTag("PlayButton").GetComponent<TextMeshProUGUI>().text = "Play Again";
            GameObject.FindGameObjectWithTag("Result").GetComponent<TextMeshProUGUI>().text = text;
        }

    }

    public void startgame() {

        SceneManager.LoadScene("Game");
    }
}
