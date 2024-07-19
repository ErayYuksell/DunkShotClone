using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening; // DOTween kullanýyorsanýz ekleyin

public class StarManager : MonoBehaviour
{
    int starCount = 0;
    Transform starUIImageTransform; // UI'daki star image'ine referans
    TextMeshProUGUI starText;
    Animator animator;

    private void Start()
    {
        // StarManager'in baþlatýldýðýnda starUIImageTransform referansýný al
        starUIImageTransform = GameManager.Instance.GetStarUIImageTransform();
        starText = GameManager.Instance.GetStarText();
        animator = GetComponent<Animator>();
    }

    public void IncreaseStar()
    {
        starCount = GetStarAmount();
        starCount++;
        SetStarAmount(starCount);
        starText.text = starCount.ToString();

        // Star image'ine doðru hareket ettir
        MoveStarToUI();
    }

    public void SetStarAmount(int starCount)
    {
        PlayerPrefs.SetInt("StarAmount", starCount);
    }

    public int GetStarAmount()
    {
        return PlayerPrefs.GetInt("StarAmount");
    }

    private void MoveStarToUI()
    {
        Vector3 targetPosition = GetWorldPositionFromUI();
        animator.enabled = false;
        transform.DOMove(targetPosition, 1f).SetEase(Ease.InQuad).OnComplete(OnReachedTarget);
    }

    private Vector3 GetWorldPositionFromUI()
    {
        Vector3 uiPos = starUIImageTransform.position;
        uiPos = Camera.main.ScreenToWorldPoint(uiPos);
        uiPos.z = 0; // Z eksenini sýfýrlayarak 2D dünyasýnda kalmasýný saðlýyoruz
        return uiPos;
    }

    private void OnReachedTarget()
    {
        Destroy(gameObject); // Oyundan yýldýz objesini tamamen kaldýr
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
        {
            // Yýldýz objesini harekete geçirmeden önce yýldýzýn collider'ýný devre dýþý býrakýn
            GetComponent<Collider2D>().enabled = false;
            IncreaseStar();
        }
    }
}
