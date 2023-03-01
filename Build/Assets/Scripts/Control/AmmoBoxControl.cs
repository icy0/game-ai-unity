using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBoxControl : MonoBehaviour
{
    GameObject player;

    public void Start()
    {
        player = GameObject.Find("Player");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.tag == "Player")
        {
            player.GetComponent<PlayerProperties>().ChangeAmmo(20);
            Destroy(this.gameObject);
        }
    }
}
