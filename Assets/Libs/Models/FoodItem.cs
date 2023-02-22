using Firebase.Firestore;
using System;
using System.Collections.Generic;

[FirestoreData]
public class FoodItem 
{
    [FirestoreProperty]
    public string id { get; set; }

    [FirestoreProperty]
    public string name { get; set; }

    [FirestoreProperty]
    public string category { get; set; }

    [FirestoreProperty]
    public bool availability { get; set; }

    [FirestoreProperty]
    public string description { get; set; }

    [FirestoreProperty]
    public string modelURL { get; set; }

    [FirestoreProperty]
    public int price { get; set; }

    
}
