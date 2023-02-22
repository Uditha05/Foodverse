using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopDataHolder : MonoBehaviour
{
    public Restaurant selectedRestaurant;
    public Texture2D ScanImage;

    private void Awake()
    {        
        DontDestroyOnLoad(this.gameObject);
    }
    
    public void setRestaurant(Restaurant restaurant, Texture2D qrImage )
    {
        this.selectedRestaurant = new Restaurant();
        this.selectedRestaurant = restaurant;
        this.ScanImage = qrImage;
    }
}
