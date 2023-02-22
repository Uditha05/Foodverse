using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopController : MonoBehaviour
{
    public Restaurant selectedRestaurant;
    bool isLoad = true;


    [Header("UI components")]
    public Image CoverImage;
    public Image ProfileImage;
    public TextMeshProUGUI ShopNameText;
    public GameObject LoadingPanel;

    public Texture2D CoverImageTexture;
    public Texture2D ProfileImageTexture;

    private void Awake()
    {
        isLoad = true;
        GameObject ShopDataHoderObj = GameObject.Find("ShopDataHolder");
        ShopDataHolder shopDataHolder = ShopDataHoderObj.GetComponent<ShopDataHolder>();
        selectedRestaurant = shopDataHolder.selectedRestaurant;
        ShopNameText.text = selectedRestaurant.name;
    }

    private void Start()
    {
        StartCoroutine(RetrieveCoverTextureFromWeb(this.selectedRestaurant.coverImage));
        StartCoroutine(RetrieveProfileTextureFromWeb(this.selectedRestaurant.profileImage));
        
    }

    private void Update()
    {
        if (isLoad)
        {
            LoadingPanel.SetActive(true);
        }
        else
        {
            LoadingPanel.SetActive(false);
        }
    }

    IEnumerator RetrieveCoverTextureFromWeb(string url)
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
                CoverImageTexture = texture;
                CoverImage.sprite = ConvertToSprite(CoverImageTexture);
                //Debug.Log("Image downloaded " + uwr);
            }
        }
    }

    IEnumerator RetrieveProfileTextureFromWeb(string url)
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
                ProfileImageTexture = texture;
                ProfileImage.sprite = ConvertToSprite(ProfileImageTexture);
                //Debug.Log("Image downloaded " + uwr);
            }
            isLoad = false;
        }
    }

    public Sprite ConvertToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }


    public void ClickBack()
    {
        SceneManager.LoadScene("Home");
    }

    public void ClickViewARMenu()
    {
        SceneManager.LoadScene("ARView");
    }


}
