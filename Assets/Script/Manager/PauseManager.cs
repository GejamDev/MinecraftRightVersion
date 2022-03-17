using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    InventoryManager im;
    LoadingManager lm;
    HpManager hm;
    public GameObject pauseUI;
    public bool paused;
    public GameObject deathUI;
    public float deathTimeDecreaseSpeed;
    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void Awake()
    {
        im = usm.inventoryManager;
        lm = usm.loadingManager;
        hm = usm.hpManager;
        pauseUI.SetActive(false);
    }
    void Update()
    {
        if (hm.died)
        {

            paused = true;
            pauseUI.SetActive(false);
            if (Time.timeScale <= deathTimeDecreaseSpeed)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale -= deathTimeDecreaseSpeed * Time.deltaTime / Time.timeScale;
            }
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;


            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!im.showingInventoryUI)
            {
                paused = !paused;
                pauseUI.SetActive(paused);
                Time.timeScale = paused ? 0 : 1;
                Cursor.visible = paused;
                Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }
}