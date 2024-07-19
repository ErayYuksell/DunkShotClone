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

    private void Start()
    {
        // StarManager'in ba�lat�ld���nda starUIImageTransform referans�n� al
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

        // Star image'ine do�ru hareket ettir
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
        uiPos.z = 0; // Z eksenini s�f�rlayarak 2D d�nyas�nda kalmas�n� sa�l�yoruz
        return uiPos;
    }

    private void OnReachedTarget()
    {
        Destroy(gameObject); // Oyundan y�ld�z objesini tamamen kald�r
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
        {
            // Y�ld�z objesini harekete ge�irmeden �nce y�ld�z�n collider'�n� devre d��� b�rak�n
            GetComponent<Collider2D>().enabled = false;
            IncreaseStar();
        }
    }
}
