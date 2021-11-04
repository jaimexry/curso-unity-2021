using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyHUD : MonoBehaviour
{
    private PartyMemberHUD[] memberHuds;
    [SerializeField] private Text messageText;

    private List<Pokymon> pokymons;
    [SerializeField] private float charactersPerSecond;
    public bool isWriting;
    public void InitPartyHUD()
    {
        memberHuds = GetComponentsInChildren<PartyMemberHUD>(true);
    }

    public void SetPartyData(List<Pokymon> pokymons)
    {
        this.pokymons = pokymons;
        messageText.text = "Selecciona un Pokemon.";
        
        for (int i = 0; i < memberHuds.Length; i++)
        {
            if (i < pokymons.Count)
            {
                memberHuds[i].gameObject.SetActive(true);
                memberHuds[i].SetPokymonData(pokymons[i]);
            }
            else
            {
                memberHuds[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateSelectedPokymon(int selectedPokymon)
    {
        for (int i = 0; i < pokymons.Count; i++)
        {
            memberHuds[i].SetSelectedPokymon(i == selectedPokymon);
        }
    }

    public IEnumerator SetMessage(string message)
    {
        messageText.text = "";
        isWriting = true;
        foreach (var character in message)
        {
            messageText.text += character;
            yield return new WaitForSeconds(1 / charactersPerSecond);
        }

        isWriting = false;
    }
}
