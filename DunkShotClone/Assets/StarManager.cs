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

    bool isTouch = false;
    private void Start()
    {
        // StarManager'in baþlatýldýðýnda starUIImageTransform referansýný al
        starUIImageTransform = GameManager.Instance.GetStarUIImageTransform();
        starText = GameManager.Instance.GetStarText();
        animator = GetComponent<Animator>();

        // Oyunun baþýnda PlayerPrefs'ten star miktarýný çek ve starCount deðiþkenine ata
        starCount = PlayerPrefs.GetInt("StarAmount", 0);
        starText.text = starCount.ToString(); // Baþlangýçtaki star miktarýný starText'e ata
    }

    public void IncreaseStar()
    {
        starCount = GetStarAmount();
        starCount++;
        SetStarAmount(starCount);
        starText.text = starCount.ToString();

        // Star image'ine doðru hareket ettir
        //MoveStarToUI();
    }

    public void SetStarAmount(int starCount)
    {
        PlayerPrefs.SetInt("StarAmount", starCount);
    }

    public int GetStarAmount()
    {
        return PlayerPrefs.GetInt("StarAmount", 0);
    }

    private void MoveStarToUI()
    {
        Vector3 targetPosition = GetWorldPositionFromUI();
        animator.enabled = false;
        transform.DOMove(targetPosition, 1f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            OnReachedTarget();
            IncreaseStar();
        });


    }

    private Vector3 GetWorldPositionFromUI()
    {
        Vector3 uiPos = starUIImageTransform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(uiPos); // WorldToScreenPoint kullanarak ekran pozisyonunu al
        screenPos.z = Camera.main.nearClipPlane; // Z eksenini ayarla
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPos); // Ekran pozisyonunu dünya pozisyonuna çevir
        worldPosition.z = 0; // Z eksenini sýfýrla

        // Hedef pozisyonu biraz aþaðý çek
        worldPosition.y -= 1.5f; // Bu deðeri ihtiyaca göre ayarlayýn

        return worldPosition;
    }


    private void OnReachedTarget()
    {
        Destroy(gameObject); // Oyundan yýldýz objesini tamamen kaldýr
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball" && !isTouch)
        {
            // Yýldýz objesini harekete geçirmeden önce yýldýzýn collider'ýný devre dýþý býrakýn
            isTouch = true;
            GetComponent<Collider2D>().enabled = false;
            //IncreaseStar();
            MoveStarToUI();
        }
    }
}
