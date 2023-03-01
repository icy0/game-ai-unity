using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyProperties : MonoBehaviour, IProperty
{
    private List<AbstractAttribute> allAttributes;
    private EnemyHealthAttribute    enemyHealthAttribute;
    private Renderer                renderer;
    private GameObject              healthText;
    private GameObject              winText;

    public List<AbstractAttribute>  GetAllAttributes()                              { return allAttributes; }
    public void                     AddAttribute(AbstractAttribute newAttribute)    { allAttributes.Add(newAttribute); }
    public void                     Damage(int damage)                              { enemyHealthAttribute.changeBy(-damage); }
    public void                     Heal(int heal)                                  { enemyHealthAttribute.changeBy(heal); }
    public void                     SetColor(Color color)                           { renderer.material.color = color; }

    void Awake ()
    {
        allAttributes = new List<AbstractAttribute>();
        enemyHealthAttribute = new EnemyHealthAttribute();
        AddAttribute(enemyHealthAttribute);
        renderer = GetComponent<Renderer>();

        healthText = GameObject.Find("EnemyHP");
        winText = GameObject.Find("WinBanner");
        winText.SetActive(false);
    }

    public void Update()
    {
        healthText.GetComponent<Text>().text = enemyHealthAttribute.GetValue().ToString();
        if (enemyHealthAttribute.GetValue() == 0)
        {
            winText.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}
