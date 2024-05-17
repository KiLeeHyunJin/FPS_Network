using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearcher : MonoBehaviourPun
{
    [SerializeField] float range;
    [SerializeField] Collider[] colliders;

    [SerializeField] public GameObject enemyIcon;
    Vector3 dir;
    RaycastHit hit;
    private void Update()
    {
        CheckEnemy();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, hit.point);
    }
    public void CheckEnemy()
    {
        int enemyLayerMask = LayerMask.GetMask("SearchEnemyCam");
        int wallLayerMask = LayerMask.GetMask("Wall");
        int combinedLayerMask = enemyLayerMask | wallLayerMask;
        colliders = Physics.OverlapSphere(transform.position, range, enemyLayerMask);

        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                EnemyIcon enemy = collider.GetComponent<EnemyIcon>();
                if (enemy == null)
                    return;

                dir = (collider.transform.position - transform.position).normalized;
                if (Physics.Raycast(transform.position, dir, out hit, range, combinedLayerMask))
                {
                    Debug.Log($"hit : {hit.collider.gameObject.name}");
                    EnemyIcon hitIcon = hit.collider.GetComponent<EnemyIcon>();
                    if (hitIcon == null)
                    {
                        enemy.enemyIcon.SetActive(false);
                            return;
                    }
                        
                    hitIcon.enemyIcon.SetActive(true);
                }
                else
                    enemy.enemyIcon.SetActive(false);
            }

        }
    }
}