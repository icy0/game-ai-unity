using System.Collections;
using UnityEngine;

class AttackFromAboveAction : AbstractAction
{
    private GameObject bulletPrefab;
    float duration = 2.0f;

    public AttackFromAboveAction(GameObject bulletPrefab) { this.bulletPrefab = bulletPrefab; }

    public override IEnumerator Execute()
    {
        while (true)
        {
            EnemyProperties enemyProperties = MonoBehaviour.FindObjectOfType<EnemyProperties>();
            enemyProperties.SetColor(Color.green);

            GameObject bulletPrefabGameObject = MonoBehaviour.Instantiate(bulletPrefab);
            bulletPrefabGameObject.transform.localScale *= 1.5f;
            BulletControl bulletControl = bulletPrefabGameObject.GetComponent<BulletControl>();
            bulletControl.SetSpeed(25.0f);
            bulletControl.SetSource("Sky");
            bulletControl.SetTarget("Player");

            yield return new WaitForSecondsRealtime(duration);
        }
    }
}
