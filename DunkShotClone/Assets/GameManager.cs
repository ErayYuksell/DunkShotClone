using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject hoopWrapper;
    [SerializeField] List<GameObject> hoopstypes = new List<GameObject>();
    [SerializeField] List<GameObject> hoops = new List<GameObject>(); // Queue kullanabilirdin ama ayni sey 
    [Header("StarSection")]
    [SerializeField] RectTransform starUIImageTransform; // UI'daki star image'ine referans
    [SerializeField] TextMeshProUGUI starText;

    int creationAmount = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    private void Start()
    {
        int starAmount = PlayerPrefs.GetInt("StarAmount", 0); // 'StarAmount' anahtarýndan veri çek ve varsayýlan deðeri 0 olarak ayarla
        starText.text = starAmount.ToString();
    }

    public void SetLocation(GameObject hoop)
    {
        float newY;
        float newX;
        bool positionIsValid = false;

        while (!positionIsValid)
        {
            newY = hoops[0].transform.position.y + Random.Range(2f, 3f); // 0 dememin nedeni zaten kesin 2 pota olucak listede ve her zaman 1 indisli eleman degisecek 

            if (hoops[0].transform.position.x < 0)
            {
                newX = Random.Range(0.5f, 1.5f); // Pozisyonu biraz daha dýþarýda tutmak için aralýðý deðiþtirdik
            }
            else
            {
                newX = Random.Range(-1.5f, -0.5f); // Pozisyonu biraz daha dýþarýda tutmak için aralýðý deðiþtirdik
            }

            Vector3 location = new Vector3(newX, newY, 0);

            // Mevcut potalarýn konumlarýna göre yeni pozisyonun geçerli olup olmadýðýný kontrol et
            positionIsValid = true;
            foreach (var existingHoop in hoops)
            {
                // Yeni pozisyon mevcut potalarýn konumuyla tamamen çakýþmamalý
                if (Vector3.Distance(location, existingHoop.transform.position) < 2f ||
                    Mathf.Abs(location.x - existingHoop.transform.position.x) < 0.1f)
                {
                    positionIsValid = false;
                    break;
                }
            }

            if (positionIsValid)
            {
                hoop.transform.position = location;
            }
        }
    }


    public void CreateHoop()
    {
        if (hoops.Count < 2) // Eðer 2'den az pota varsa
        {
            creationAmount++;
            GameObject type;

            if (creationAmount < 2) // 3 kere pota olusturduktan sonra 1 kere starli pota olustur
            {
                type = hoopstypes[0];
            }
            else
            {
                type = hoopstypes[1];
                creationAmount = 0;
            }

            GameObject hoopObj = Instantiate(type, hoopWrapper.transform);
            hoops.Add(hoopObj);
            SetLocation(hoopObj);
        }
    }
    // 1. potaya top girdiginde surekli 0. eleman siliniyor artik 1. pota 0. pota haline gelirken yeni eklenen pota 1. pota oluyor bu sekilde dongu ile ilerliyor 
    public void RemoveHoopFromList()
    {
        hoops[0].SetActive(false);
        hoops.Remove(hoops[0]);
        CreateHoop(); // Pota sayýsýný kontrol et ve gerekirse yeni pota ekle
    }

    public bool GetHoopScoredInfo()
    {
        return hoops[1].GetComponent<HoopController>().hasScored;
    }

    public RectTransform GetStarUIImageTransform()
    {
        return starUIImageTransform;
    }
    public TextMeshProUGUI GetStarText()
    {
        return starText;
    }

    public bool HoopIsStrike()
    {
        return hoops[0].gameObject.GetComponent<HoopController>().isStrike;
    }
}
