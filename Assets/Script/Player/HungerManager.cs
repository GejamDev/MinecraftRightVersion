using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    FirstPersonController fpc;
    GameObject player;
    [Header("Hunger")]
    public int hunger;
    public GameObject hungerPrefab;
    public Transform HungerUI;
    public Sprite full, half, none;
    List<Image> hungerImages = new List<Image>();
    public float hungerLoseTime;

    void Awake()
    {
        fpc = usm.firstPersonController;
        player = usm.player;
        hunger = 20;
        for (int i = 0; i < 10; i++)
        {
            GameObject h = Instantiate(hungerPrefab, HungerUI);
            h.transform.localPosition = new Vector2(-80 + i * 17, 0);
            hungerImages.Add(h.GetComponent<Image>());
        }
        UpdateHungerUI();
        StartCoroutine(LoseHunger_LongTerm());
    }

    public void UpdateHungerUI()
    {
        hunger = Mathf.Clamp(hunger, 0, 20);
        for (int i = 0; i < 10; i++)
        {
            Image h = hungerImages[i];
            int type = (hunger - (int)i * 2);

            if (type >= 2)
            {
                h.sprite = full;
            }
            else if (type == 1)
            {
                h.sprite = half;
            }
            else
            {
                h.sprite = none;
            }

        }

    }

    IEnumerator LoseHunger_LongTerm()
    {
        while (true)
        {
            yield return new WaitForSeconds(hungerLoseTime);
            hunger--;
            UpdateHungerUI();
        }
    }
}
