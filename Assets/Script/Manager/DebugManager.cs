using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    LightingManager lm;
    public Text debugText;
    public GameObject debugUI;
    public bool debugging;
    public GameObject player;
    void Awake()
    {
        lm = usm.lightingManager;
        debugUI.SetActive(debugging);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            debugging = !debugging;
            debugUI.SetActive(debugging);
        }
        if (debugging)
        {
            debugText.text =
                "fps : " + (Mathf.Ceil(1 / Time.deltaTime)).ToString() + "\r\n" +
                "player pos : " + player.transform.position.ToString() + "\r\n" +
                "time of day : " + (Mathf.RoundToInt(lm.TimeOfDay)).ToString() + " (" + (lm.isNight ? "night" : "day") + ")" + "\r\n" +
                "seed : " + usm.seedManager.seed.ToString() + "\r\n" +
                "world name : " + usm.saveManager.currentWorldName.ToString()

                ;
        }

    }
}
