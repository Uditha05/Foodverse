using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;



public class ARViewController : MonoBehaviour
{
    [Header("UI components")]
    public TextMeshProUGUI FoodNameText;
    public TextMeshProUGUI FoodPriceText;


    public void updateText(string foodName, string foodPrice)
    {
        FoodNameText.text = foodName;
        FoodPriceText.text = foodPrice;
    }

    public void goBack()
    {
        SceneManager.LoadScene("Shop");
    }

}
