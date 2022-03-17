using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadingManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    public GameObject loadingUI;
    public bool loading = true;
    public int curProgress;
    public int goal;
    public Text progressText;
    void Awake()
    {
        StartCoroutine(Load());
    }
    IEnumerator Load()
    {
        loadingUI.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        yield return new WaitUntil(()=>!loading);


        loadingUI.SetActive(false);
    }
    void Update()
    {
        if (loading)
        {
            progressText.text = curProgress.ToString() + " / " + goal.ToString();
        }
    }
}
