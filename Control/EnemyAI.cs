using UnityEngine;

/*
 * This class implements the AI of the Enemy.
 * It has an ExternalState-instance to track
 * possible changes to the environment and to
 * adapt by traversing its inductive decision tree.
 */
public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private GameObject              bulletPrefab;
    private ExternalStateTracker    externalStateTracker = new ExternalStateTracker();
    private DataBase                database;
    private float                   checkInterval = 1.0f;
    private AbstractAction          currentAction;

    public void Start ()
    {
        externalStateTracker.AddProperty(FindObjectOfType<PlayerProperties>());
        externalStateTracker.AddProperty(FindObjectOfType<EnemyProperties>());

        database = new DataBase(externalStateTracker.GetAllAttributes(), bulletPrefab);
        currentAction = database.GetInductiveDecisionTree().traverse(externalStateTracker.TrackCurrentSituation());

        InvokeRepeating("LoadDecision", 1.0f, checkInterval);        
    }

    private void LoadDecision()
    {
        StopAllCoroutines();

        if (externalStateTracker.HasChanged())
        {
            // Traverse the inductive decision tree with 
            // the new situation.
            currentAction = database.GetInductiveDecisionTree()
                .traverse(externalStateTracker.GetSituation());
        }

        if (currentAction != null)
        {
            StartCoroutine(currentAction.Execute());
        }
    }
}
