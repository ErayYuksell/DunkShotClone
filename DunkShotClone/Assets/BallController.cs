using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    Rigidbody2D rb;

    Vector3 touchStartPos;
    Vector3 touchEndPos;
    bool isDragging = false;
    bool inTheAir = false; // havadayken hi�bir �ey yapamayaca��z

    Transform hoop; // Pota
    LineRenderer trajectoryLineRenderer; // �izim i�in LineRenderer

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!inTheAir)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchStartPos.z = 0;
                isDragging = true;
                rb.velocity = Vector2.zero; // Topun hareketini durdur
                trajectoryLineRenderer.positionCount = 2; // �izgiyi etkinle�tir
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3 currentTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentTouchPos.z = 0;

                Vector3 direction = touchStartPos - currentTouchPos; // Y�n� do�ru hesapla
                float distance = Mathf.Clamp(direction.magnitude, 0, 1);

                // Potan�n y�n�n� de�i�tir
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                hoop.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // 90 derece ��karmam�z�n sebebi, ba�lang�� rotasyonunun yukar�y� g�stermesini sa�lamak

                // Topun pozisyonunu ve rotasyonunu ayarla
                rb.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

                // �izgi �izimini g�ncelle
                trajectoryLineRenderer.SetPosition(0, hoop.position);
                trajectoryLineRenderer.SetPosition(1, hoop.position + direction.normalized * (distance + 1.25f));
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;

                // Potan�n alt k�sm�n� s�f�rla
                hoop.rotation = Quaternion.identity;

                // Topa kuvvet uygula
                touchEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchEndPos.z = 0;
                Vector3 force = (touchStartPos - touchEndPos) * 10;

                rb.isKinematic = false;
                rb.AddForce(force, ForceMode2D.Impulse); 

                // �izgiyi gizle
                trajectoryLineRenderer.positionCount = 0;
            }
        }
        else
        {
            BallRotate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Hoop")
        {
            inTheAir = false;
            hoop = collision.gameObject.transform.Find("Hoop");
            HoopController hoopController = collision.gameObject.GetComponent<HoopController>();
            trajectoryLineRenderer = hoopController.GetComponent<LineRenderer>();
            trajectoryLineRenderer.positionCount = 0; // Ba�lang��ta �izgiyi gizle
            trajectoryLineRenderer.useWorldSpace = true; // D�nya koordinatlar�n� kullan
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Hoop")
        {
            hoop = null;
            inTheAir = true;
        }
    }

    public void BallRotate()
    {
        // Topu s�rekli d�nd�r
        transform.Rotate(new Vector3(0, 0, 360) * Time.deltaTime);
    }
}
