using UnityEngine;
using System;

public class ItemUniqueID : MonoBehaviour
{
    [SerializeField]
    public string uniqueID;

    private void Awake()
    {
        if (string.IsNullOrEmpty(uniqueID))
            uniqueID = Guid.NewGuid().ToString();
    }
}