using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopController : MonoBehaviour
{
    public bool hasScored = false;

    public void CloseTheHoop()
    {
        if (GameManager.Instance.GetHoopScoredInfo())
        {
            GameManager.Instance.RemoveHoopFromList();
        }
    }
}
