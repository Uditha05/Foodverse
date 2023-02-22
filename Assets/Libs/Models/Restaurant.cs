using Firebase.Firestore;
using System;
using System.Collections.Generic;

[FirestoreData]
public struct Restaurant
{
    [FirestoreProperty]
    public string id { get; set; }

    [FirestoreProperty]
    public string name { get; set; }

    [FirestoreProperty]
    public string city { get; set; }

    [FirestoreProperty]
    public string imageQR { get; set; }

    [FirestoreProperty]
    public string email { get; set; }

    [FirestoreProperty]
    public string profileImage { get; set; }

    [FirestoreProperty]
    public string coverImage { get; set; }

    [FirestoreProperty]
    public string searchName { get; set; }

    [FirestoreProperty]
    public bool isOpen { get; set; }

    [FirestoreProperty]
    public bool isDelete { get; set; }

    [FirestoreProperty]
    public DateTime createdAt { get; set; }

    [FirestoreProperty]
    public DateTime updatedAt { get; set; }

    [FirestoreProperty]
    public List<string> categoryList { get; set; }
}
