using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI starText;
    int starCount = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
        {
            gameObject.SetActive(false);
            IncreaseStar();
        }
    }

    public void IncreaseStar()
    {
        starCount++;
        starText.text = starCount.ToString();
    }
}
