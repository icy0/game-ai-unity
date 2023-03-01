using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProperties : MonoBehaviour, IProperty
{
    [SerializeField]
    private Transform               playerTransform;
    [SerializeField]
    private Transform               enemyTransform;
    private List<AbstractAttribute> allAttributes;
    private PlayerHealthAttribute   playerHealthAttribute;
    private PlayerAmmoAttribute     playerAmmoAttribute;
    private PlayerCoverAttribute    playerCoverAttribute;
    private GameObject              playerAmmoText;
    private GameObject              healthText;
    private GameObject              gameOverText;
    private GameObject              inCoverText;

    public List<AbstractAttribute>  GetAllAttributes()                              { return allAttributes; }
    public void                     AddAttribute(AbstractAttribute newAttribute)    { allAttributes.Add(newAttribute); }
    public void                     Damage(int damage)                              { playerHealthAttribute.changeBy(-damage); }

    public void Awake()
    {
        allAttributes = new List<AbstractAttribute>();

        playerHealthAttribute = new PlayerHealthAttribute();
        playerAmmoAttribute = new PlayerAmmoAttribute();
        playerCoverAttribute = new PlayerCoverAttribute();

        AddAttribute(playerHealthAttribute);
        AddAttribute(playerAmmoAttribute);
        AddAttribute(playerCoverAttribute);

        playerAmmoText =    GameObject.Find("PlayerAmmo");
        healthText =        GameObject.Find("PlayerHP");
        gameOverText =      GameObject.Find("GameOverBanner");
        inCoverText =       GameObject.Find("PlayerCover");
        gameOverText.SetActive(false);
    }
    
    public void Update()
    {
        // check, if player is in cover
        Vector3 raycastDirection = enemyTransform.position - playerTransform.position;

        RaycastHit hit;
        if (Physics.Raycast(playerTransform.position, raycastDirection, out hit, raycastDirection.magnitude, LayerMask.GetMask("Obstacle")))
        {
            playerCoverAttribute.change(true);
            inCoverText.GetComponent<Text>().text = "True";
        }
        else
        {
            playerCoverAttribute.change(false);
            inCoverText.GetComponent<Text>().text = "False";
        }

        // update labels
        playerAmmoText.GetComponent<Text>().text = playerAmmoAttribute.GetValue().ToString();
        healthText.GetComponent<Text>().text = playerHealthAttribute.GetValue().ToString();

        // print game over, if player's health is 0
        if(playerHealthAttribute.GetValue() == 0)
        {
            gameOverText.SetActive(true);
            Destroy(this.gameObject);
        }
    }

    public void ChangeAmmo(int change)
    {
        playerAmmoAttribute.changeBy(change);
    }

    public bool CanShoot()
    {
        return playerAmmoAttribute.GetValue() > 0;
    }
}
