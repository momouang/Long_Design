using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 100f;
    public float time = 3f;
    public float bulletDestroyTime = 3f;


    // Update is called once per frame
    void FixedUpdate()
    {
        if(time >= 0)
        {
            time -= Time.deltaTime;
            if(time <= 0)
            {
                Destroy(gameObject);
                time = bulletDestroyTime;
            }

        }
        rb.AddForce(gameObject.transform.right * speed * Time.deltaTime);     
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Cowboy")
        {
            Destroy(collision.gameObject);
            gGlobal.fShoot = true;
        }
    }
}
