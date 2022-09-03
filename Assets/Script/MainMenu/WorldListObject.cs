using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldListObject : MonoBehaviour
{
    public string worldName;
    public Text worldNameTxt;
    public void OnClicked()
    {
        FindObjectOfType<MainMenuManager>().StartWorld(worldName);
    }
}
