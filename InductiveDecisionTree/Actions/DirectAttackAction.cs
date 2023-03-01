using System;
using UnityEngine;
using System.Collections;

class DirectAttackAction : AbstractAction
{
    private GameObject bulletPrefab;
    float duration = 0.2f;

    public DirectAttackAction(GameObject bulletPrefab) { this.bulletPrefab = bulletPrefab; }

    public override IEnumerator Execute()
    {
        while (true)
        {
            EnemyProperties enemyProperties = MonoBehaviour.FindObjectOfType<EnemyProperties>();
            enemyProperties.SetColor(Color.red);

            GameObject bulletPrefabGameObject = MonoBehaviour.Instantiate(bulletPrefab);
            BulletControl bulletControl = bulletPrefabGameObject.GetComponent<BulletControl>();
            bulletControl.SetSource("Enemy");
            bulletControl.SetTarget("Player");

            yield return new WaitForSecondsRealtime(duration);
        }
    }
}
