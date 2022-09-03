using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;
public class MainMenuManager : MonoBehaviour
{
    public DirectoryInfo worldFolder;
    public GameObject worldList;
    public GameObject worldListObject;
    public InputField worldNameInput;
    public InputField seedInput;


    void Awake()
    {
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        worldList.SetActive(false);



        string path = Application.persistentDataPath;
        worldFolder = new DirectoryInfo(path);
        int preFoundCount = 0;

        foreach (FileInfo file in worldFolder.GetFiles())
        {
            if (file.Name.Contains(".gejam"))
            {
                GameObject o = Instantiate(worldListObject);
                o.transform.SetParent(worldList.transform);
                o.transform.localPosition = new Vector3(0, 85 - preFoundCount * 35, 0);
                o.transform.localScale = Vector3.one;
                WorldListObject wlo = o.GetComponent<WorldListObject>();
                wlo.worldName = file.Name.Substring(0, file.Name.Length - 6);
                wlo.worldNameTxt.text = wlo.worldName;

                preFoundCount++;
            }
        }
    }
    public void OpenWorldList()
    {
        worldList.SetActive(!worldList.activeSelf);
    }
    public void StartWorld(string worldName)
    {
        PlayerPrefs.SetString("LoadWorldName", worldName);
        SceneManager.LoadScene("Game");
    }
    public void CreateNewWorld()
    {
        if (worldNameInput.text.Length == 0)
            return;
        int seed;
        if (seedInput.text == "")
        {
            PlayerPrefs.SetInt("RandomSeed", 1);
        }
        else if (!int.TryParse(seedInput.text, out seed))
        {
            return;
        }
        else
        {
            Debug.Log("setted");
            PlayerPrefs.SetInt("RandomSeed", 0);
            PlayerPrefs.SetInt("LastSeedInput", seed);
        }
        
        StartWorld(worldNameInput.text);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void YT()
    {
        Application.OpenURL("https://www.youtube.com/c/Gejam게임만드는채널");
    }
}
