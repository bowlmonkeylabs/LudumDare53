using UnityEngine.AI;
using UnityEngine;
using BML.ScriptableObjectCore.Scripts.SceneReferences;

public class Patrol : MonoBehaviour
{
    [SerializeField] private TransformSceneReference _patrolPointsContainer;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private bool userRandomOrder = true;

    private int nextPointIndex = 0;

    void Awake() {

    }

    void Start () {
        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        // agent.autoBraking = false;

        GotoNextPoint();
    }


    void GotoNextPoint() {
        // Set the agent to go to the currently selected destination.
        _agent.destination = _patrolPointsContainer.Value.GetChild(nextPointIndex).position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        nextPointIndex = userRandomOrder ? Random.Range(0, _patrolPointsContainer.Value.childCount)
            : (nextPointIndex + 1) % _patrolPointsContainer.Value.childCount;
    }


    void Update () {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            GotoNextPoint();
    }
}
