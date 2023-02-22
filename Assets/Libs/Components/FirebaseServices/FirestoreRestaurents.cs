using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using UnityEngine;


public class FirestoreRestaurents 
{

    FirebaseFirestore db;
    public FirestoreRestaurents()
    {
        this.db = FirebaseFirestore.DefaultInstance;
    }

    public void GetAllRestaurents()
    {
        List<Restaurant> restaurantsList = new List<Restaurant>();

        this.db.Collection("Restaurant").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot allCitiesQuerySnapshot = task.Result;
            //Restaurant counter = task.Result.ConvertTo<Restaurant>();
            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot)
            {
                Dictionary<string, object> city = documentSnapshot.ToDictionary();
                Restaurant restaurant = documentSnapshot.ConvertTo<Restaurant>();
                restaurantsList.Add(restaurant);
                Debug.Log(restaurant.createdAt);

                ///Uncomment for check key-val pairs
                //foreach (KeyValuePair<string, object> pair in city)
                //{
                //    Debug.Log(pair.Key + " " + pair.Value);
                //}

            }
        });
    }

    

    public void UpdateShopName(Restaurant restaurant)
    {
        try
        {
            DocumentReference restaurentRef = db.Collection("Restaurant").Document(restaurant.id);
            restaurentRef.SetAsync(restaurant).ContinueWithOnMainThread(task =>
            {
                Debug.Log("Updated Restaurent");
            });
        }
        catch (Exception error)
        {
            Debug.LogError(error);
        }

    }


}
