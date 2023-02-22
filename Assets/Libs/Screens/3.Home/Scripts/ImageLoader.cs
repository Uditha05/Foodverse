using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ImageLoader : MonoBehaviour
{
    public Texture2D imageFromWeb;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void ClickShop(string sname)
   {
        if(sname == "KFC")
        {
            StartCoroutine(RetrieveTextureFromWeb("https://firebasestorage.googleapis.com/v0/b/test-unity-connect.appspot.com/o/QR_Images%2Fkfc-test.jpg?alt=media&token=4eade5dd-6b26-4c3f-a7b8-0d8cd59a0209"));
        }
        else
        {
            StartCoroutine(RetrieveTextureFromWeb("https://firebasestorage.googleapis.com/v0/b/test-unity-connect.appspot.com/o/QR_Images%2Fburger_king.png?alt=media&token=47aa61d8-a448-4f2e-ae6e-555f60652f8a"));
        }
   }

    IEnumerator RetrieveTextureFromWeb(string url)
    {
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
                imageFromWeb = texture;
                Debug.Log("Image downloaded " + uwr);
                SceneManager.LoadScene("SampleScene");
            }
        }
    }
}
