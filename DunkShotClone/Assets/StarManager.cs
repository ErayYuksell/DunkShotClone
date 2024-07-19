using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening; // DOTween kullan�yorsan�z ekleyin

public class StarManager : MonoBehaviour
{
    int starCount = 0;
    Transform starUIImageTransform; // UI'daki star image'ine referans
    TextMeshProUGUI starText;
    Animator animator;

    bool isTouch = false;
    private void Start()
    {
        // StarManager'in ba�lat�ld���nda starUIImageTransform referans�n� al
        starUIImageTransform = GameManager.Instance.GetStarUIImageTransform();
        starText = GameManager.Instance.GetStarText();
        animator = GetComponent<Animator>();

        // Oyunun ba��nda PlayerPrefs'ten star miktar�n� �ek ve starCount de�i�kenine ata
        starCount = PlayerPrefs.GetInt("StarAmount", 0);
        starText.text = starCount.ToString(); // Ba�lang��taki star miktar�n� starText'e ata
    }

    public void IncreaseStar()
    {
        starCount = GetStarAmount();
        starCount++;
        SetStarAmount(starCount);
        starText.text = starCount.ToString();

        // Star image'ine do�ru hareket ettir
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
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPos); // Ekran pozisyonunu d�nya pozisyonuna �evir
        worldPosition.z = 0; // Z eksenini s�f�rla

        // Hedef pozisyonu biraz a�a�� �ek
        worldPosition.y -= 1.5f; // Bu de�eri ihtiyaca g�re ayarlay�n

        return worldPosition;
    }


    private void OnReachedTarget()
    {
        Destroy(gameObject); // Oyundan y�ld�z objesini tamamen kald�r
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball" && !isTouch)
        {
            // Y�ld�z objesini harekete ge�irmeden �nce y�ld�z�n collider'�n� devre d��� b�rak�n
            isTouch = true;
            GetComponent<Collider2D>().enabled = false;
            //IncreaseStar();
            MoveStarToUI();
        }
    }
}
