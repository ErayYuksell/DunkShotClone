using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoopController : MonoBehaviour
{
    public bool hasScored = false;
    public bool isStrike = false;

    [SerializeField] TextMeshProUGUI strikeText;

    public void CloseTheHoop()
    {
        if (GameManager.Instance.GetHoopScoredInfo())
        {
            GameManager.Instance.RemoveHoopFromList();
        }
    }

    public void OpenStrikeText(int value)
    {
        if (hasScored)
        {
            strikeText.gameObject.SetActive(true);
            strikeText.text = "+" + value.ToString();
        }
    }
}
