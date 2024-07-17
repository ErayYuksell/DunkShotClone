using UnityEngine;

public class PotaController : MonoBehaviour
{
    public Transform ball; // Pota içindeki topun referansý
    public Rigidbody2D ballRb;
    public float maxDragDistance = 2f;
    public Transform nextPota; // Bir sonraki potanýn referansý
    private Vector3 dragStartPosition;
    private bool isDragging = false;
    private Quaternion initialRotation;
    private bool isBallInPota = false; // Topun pota içinde olup olmadýðýný takip eder

    void Start()
    {
        initialRotation = transform.rotation; // Potanýn baþlangýçtaki dönüþünü kaydet
    }

    void Update()
    {
        if (isBallInPota)
        {
            ball.position = transform.position; // Topu potanýn içinde tut
        }

        // Ekrana dokunduðumuzda
        if (Input.GetMouseButtonDown(0) && isBallInPota)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0; // Z eksenini sýfýrla
            Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition);

            if (hitCollider != null && hitCollider.transform == ball)
            {
                isDragging = true;
                dragStartPosition = ball.position; // Topun baþlangýç pozisyonunu kaydet
            }
        }

        // Dokunmayý sürdürdüðümüzde
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0; // Z eksenini sýfýrla
            Vector3 dragVector = worldPosition - dragStartPosition;

            if (dragVector.magnitude > maxDragDistance)
            {
                dragVector = dragVector.normalized * maxDragDistance;
            }

            // Potanýn pozisyonunu ve yönünü ayarla
            transform.position = dragStartPosition + dragVector / 2;
            ball.position = transform.position;

            float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90); // Potanýn yönünü doðru ayarlamak için +90 derece ekle
        }

        // Dokunmayý býraktýðýmýzda
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            Vector3 releasePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            releasePosition.z = 0; // Z eksenini sýfýrla
            Vector3 releaseVector = dragStartPosition - releasePosition;

            ballRb.velocity = Vector2.zero; // Önceki hýzlarý sýfýrlayýn
            ballRb.angularVelocity = 0f; // Önceki açýsal hýzlarý sýfýrlayýn
            ballRb.AddForce(releaseVector * 100); // Kuvveti uygulamak için çarpan

            // Potanýn baþlangýç pozisyonuna ve yönüne dönmesini saðla
            transform.position = dragStartPosition;
            transform.rotation = initialRotation; // Potanýn baþlangýçtaki dönüþünü geri yükle

            isBallInPota = false; // Top artýk potada deðil
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == ball)
        {
            isBallInPota = true;
            if (nextPota != null)
            {
                PotaController nextPotaController = nextPota.GetComponent<PotaController>();
                if (nextPotaController != null)
                {
                    nextPotaController.ReceiveBall(ball);
                }
            }
        }
    }

    public void ReceiveBall(Transform ball)
    {
        this.ball = ball;
        ballRb = ball.GetComponent<Rigidbody2D>();
        isBallInPota = true;
    }
}
