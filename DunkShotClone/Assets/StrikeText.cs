using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StrikeText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI strikeText;
    public void CloseStrikeText()
    {
        strikeText.gameObject.SetActive(false);
    }
}
