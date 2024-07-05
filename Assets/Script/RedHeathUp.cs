using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedHeathUp : MonoBehaviour
{
    public int healthAmount = 10; //Luong mau an tang 5

    public void conSume()
    {
        Destroy(gameObject);
    }


}
