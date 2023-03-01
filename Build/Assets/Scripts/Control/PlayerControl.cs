using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{

    private Rigidbody playerRigidbody;    
    private float speed = 15.0f;
    [SerializeField]
    private GameObject bulletPrefab;
    private PlayerProperties playerProperties;

    public void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerProperties = GetComponent<PlayerProperties>();
    }
    public void Update()        { Shoot(); }
    public void FixedUpdate()   { Move(Time.deltaTime); }

    /*
     * The Player-Movement. 
     * Horizontal movement is bound to A and D,
     * vertical movement is bound to W and S.
     */
    private void Move(float deltaTime)
    {
        Vector3 movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 velocity = movementInput.normalized * speed;

        playerRigidbody.MovePosition(playerRigidbody.position + velocity * Time.fixedDeltaTime);
    }

    /*
     * Shoot a bullet from the player to the enemy.
     */
    private void Shoot()
    {
        if(Input.GetKeyDown(KeyCode.Space)) { StartCoroutine("SpawnBullets"); }
        if(Input.GetKeyUp(KeyCode.Space))   { StopAllCoroutines(); }
    }

    private IEnumerator SpawnBullets()
    {
        while (true)
        {
            if(playerProperties.CanShoot())
            {
                GameObject bulletPrefabGameObject = MonoBehaviour.Instantiate(bulletPrefab, playerRigidbody.position, Quaternion.identity);
                BulletControl bulletControl = bulletPrefabGameObject.GetComponent<BulletControl>();
                bulletControl.SetSource("Player");
                bulletControl.SetTarget("Enemy");

                playerProperties.ChangeAmmo(-1);
            }

            yield return new WaitForSecondsRealtime(0.2f);
        }
    }
}
