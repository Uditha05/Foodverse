using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Vuforia;
public class SideLoadFromWeb : MonoBehaviour
{
    Texture2D imageFromWeb;
    void Start()
    {
        VuforiaApplication.Instance.OnVuforiaInitialized += OnVuforiaInitialized;
    }
    void OnVuforiaInitialized(VuforiaInitError error)
    {
        StartCoroutine(RetrieveTextureFromWeb());
    }
    IEnumerator RetrieveTextureFromWeb()
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("https://library.vuforia.com/sites/default/files/vuforia-library/articles/solution/Magic%20Leap%20Related%20Content/Astronaut-scaled.jpg"))
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
                CreateImageTargetFromDownloadedTexture();
            }
        }
    }
    void CreateImageTargetFromDownloadedTexture()
    {
        var mTarget = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(
    imageFromWeb,
    0.1f,
    "Astronaut");
        // Add the DefaultObserverEventHandler to the newly created game object
        mTarget.gameObject.AddComponent<DefaultObserverEventHandler>();
        Debug.Log("Target created and active" + mTarget);
    }
}