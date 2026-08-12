using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
   public GameObject oyuncuPrefab;

   public List<PlayerData> oyuncuListesi;

    private void Start()
    {
        OyunculariListele();
    }
    void OyunculariListele()
    {
        for(int i=0 ; i< oyuncuListesi.Count ; i++)
        {
            GameObject oyuncu = Instantiate(oyuncuPrefab);
            oyuncu.transform.SetParent(this.transform);
            oyuncu.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i+1).ToString();
            oyuncu.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = oyuncuListesi[i].oyuncuAdi;
            oyuncu.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = oyuncuListesi[i].oyuncuPuani.ToString();
        }
    }
}

