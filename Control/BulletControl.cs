using System;
using UnityEngine;

class BulletControl : MonoBehaviour
{
    private Vector3     direction;
    private Vector3     sourcePosition;
    private Transform   playerTransform;
    private Transform   enemyTransform;
    private Transform   bulletTransform;
    private bool        targetSet = false;
    private bool        sourceSet = false;
    private float       speed = 50.0f;
    private String      target;
    private String      source;

    public void         SetSpeed(float newSpeed) { this.speed = newSpeed; }

    public void Start()
    {
        bulletTransform = GetComponent<Transform>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        enemyTransform = GameObject.FindGameObjectWithTag("Enemy").transform;
    }

    public void FixedUpdate()
    {
        if(sourceSet && targetSet)
            Move(Time.deltaTime);
    }

    /*
     * The Bullet-Movement.
     */
    private void Move(float deltaTime)
    {
        Vector3 velocity = direction.normalized * speed;
        bulletTransform.position = bulletTransform.position + velocity * Time.fixedDeltaTime;
    }


    public void SetSource(String source)
    {
        Start();
        this.source = source;

        if(source == "Player")
        {
            bulletTransform.position = playerTransform.position;
            bulletTransform.position += new Vector3(0, 5, 0);
        }
        else if (source == "Enemy")
        {
            bulletTransform.position = enemyTransform.position;
            bulletTransform.position += new Vector3(0, 10, 0);
        }
        else if (source == "Sky")
        {
            bulletTransform.position = playerTransform.position;
            bulletTransform.position += new Vector3(0, 50, 0);
        }

        sourceSet = true;
    }

    public void SetTarget(String target)
    {
        this.target = target;
        if (target == "Player")
        {
            direction = playerTransform.position - bulletTransform.position;
        }
        else if (target == "Enemy")
        {
            direction = enemyTransform.position - bulletTransform.position;
        }
        targetSet = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if      (collision.collider.transform.tag == "Player" && target == "Player")   { playerTransform.GetComponent<PlayerProperties>().Damage(1); }
        else if (collision.collider.transform.tag == "Enemy" && target == "Enemy")    { enemyTransform.GetComponent<EnemyProperties>().Damage(1); }
        Destroy(this.gameObject);
    }
}
