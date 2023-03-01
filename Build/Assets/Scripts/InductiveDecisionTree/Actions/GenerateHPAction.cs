using System.Collections;
using UnityEngine;

class GenerateHPAction : AbstractAction
{
    public override IEnumerator Execute()
    {
        while (true)
        {
            EnemyProperties enemyProperties = MonoBehaviour.FindObjectOfType<EnemyProperties>();
            enemyProperties.SetColor(Color.blue);

            enemyProperties.Heal(1);

            yield return new WaitForSecondsRealtime(0.3f);
        }
    }
}
