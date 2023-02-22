using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ShopCardPrefabCtrl : MonoBehaviour
{
    public string shopName;
    public string city;
    public string logoURL;
    public string scanImageURL;
    public Restaurant restaurantDTO;
    public Texture2D ScanImage;
    bool isLoad = true;

    [Header("UI components")]
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI CityText;
    public Image LogoImage;

   public void setValues(Restaurant restaurantModel)
    {
        this.shopName = restaurantModel.name;
        this.city = restaurantModel.city;
        this.logoURL = restaurantModel.profileImage;
        this.scanImageURL = restaurantModel.imageQR;
        this.restaurantDTO = restaurantModel;

        UpdateUI();

    }

    private void UpdateUI()
    {
        NameText.text = this.shopName;
        CityText.text = this.city;
        StartCoroutine(RetrieveTextureFromWeb(this.scanImageURL));
        StartCoroutine(RetrieveLogoTextureFromWeb(this.logoURL));
    }

    IEnumerator RetrieveTextureFromWeb(string url)
    {
        //Debug.Log("Start Load image: URL: " + url);
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded texture once the web request completes
                var texture = DownloadHandlerTexture.GetContent(uwr);
                ScanImage = texture;
                //Debug.Log("Scan Image downloaded " + uwr);
                isLoad = false;
            }
        }
    }

    IEnumerator RetrieveLogoTextureFromWeb(string url)
    {
        //Debug.Log("Start Load Logo image: URL: " + url);
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded texture once the web request completes
                var texture = DownloadHandlerTexture.GetContent(uwr);
                LogoImage.sprite = ConvertToSprite(texture);
                //Debug.Log("Logo Image downloaded " + uwr);
               
            }
        }
    }

    public Sprite ConvertToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    public void ClickShopCard()
    {
        if (!isLoad)
        {
            GameObject ShopDataHoderObj = GameObject.Find("ShopDataHolder");
            ShopDataHolder shopDataHolder = ShopDataHoderObj.GetComponent<ShopDataHolder>();
            shopDataHolder.setRestaurant(this.restaurantDTO, this.ScanImage);
            SceneManager.LoadScene("Shop");
        }
        
    }
}
