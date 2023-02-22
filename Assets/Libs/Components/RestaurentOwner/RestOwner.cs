using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestOwner : MonoBehaviour
{
    public FirebaseUser User;
   

    public string restID;

    public static RestOwner Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void setUser(FirebaseUser fUser)
    {
        this.User = fUser;
        this.restID = fUser.UserId;
    }
}
