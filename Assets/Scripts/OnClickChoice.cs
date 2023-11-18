using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickChoice : MonoBehaviour
{
    public int ChoiceNumber;

    public void onClick() 
    { 
        if(GameManager.Instance.currentEvent != null)
            GameManager.Instance.currentEvent.GetChoices()[ChoiceNumber].ApplyEffect(); 
    }
}
