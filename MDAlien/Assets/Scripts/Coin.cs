using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, Item
{
    public int worth = 5;

    public void Collect() {
        Destroy(gameObject);
    }
}
