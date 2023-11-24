using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BossState
{
    Start,
    Idle,
    LeftBeamAttack,
    RightBeamAttack,
    DoubleBeamAttack,
    EnemySpawnAttack,
    SpikeAttack,
    Damaged,
    Heal,
    Death
}

public class BossStateMachine : MonoBehaviour
{
    [SerializeField] private BossState currentState;
    [SerializeField] private GameObject leftBeam;
    [SerializeField] private GameObject rightBeam;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject spikes;
    [SerializeField] private float spikesOffset;
    [SerializeField] private GameObject healParticles;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private GameObject bossHealthBar;
    [SerializeField] private Transform enemyLeftSpawnPoint;
    [SerializeField] private Transform enemyRightSpawnPoint;
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private int damageAmount;
    [SerializeField] private int healAmount;
    [SerializeField] private GameObject spikesAttackTelegraph;
    [SerializeField] private GameObject beamLeftAttackTelegraph;
    [SerializeField] private GameObject beamRightAttackTelegraph;
    [SerializeField] private GameObject enemySpawnAttackTelegraph;

    [SerializeField] private float changeStateTime;
    [SerializeField] private float changeStateTimeVariation;

    private Dictionary<BossState, List<BossState>> possibleTransitions;
    private Dictionary<BossState, Action> stateActions;

    private bool isTakingAction;
    private bool isHealing;
    private bool isDamaged;
    private int consecutiveAttacks = 0;

    private void Awake()
    {
        currentState = BossState.Start;
        InitializeTransitions();
        InitializeStateActions();
    }

    private void InitializeTransitions()
    {
        // Initialize the dictionary and add possible transitions for each state
        possibleTransitions = new Dictionary<BossState, List<BossState>>();

        // Example: AddTransition(BossState.Start, BossState.Idle);
        // Example: AddTransition(BossState.Idle, BossState.LeftBeamAttack);
        // ... add more transitions
    }

    private void InitializeStateActions()
    {
        // Initialize the dictionary and add actions for each state
        stateActions = new Dictionary<BossState, Action>();

        stateActions[BossState.SpikeAttack] = () => { spikeAttack(1.0f); };
        stateActions[BossState.LeftBeamAttack] = () => { leftBeamAttack(1.0f); };
        stateActions[BossState.RightBeamAttack] = () => { rightBeamAttack(1.0f); };
        stateActions[BossState.DoubleBeamAttack] = () => { doubleBeamAttack(1.0f); };
        stateActions[BossState.Heal] = () => { heal(healAmount); };
        stateActions[BossState.EnemySpawnAttack] = () => { spawnEnemy(); };
        stateActions[BossState.Damaged] = () => { damage(); };
        stateActions[BossState.Death] = () => { death(); };
    }

    private void AddTransition(BossState fromState, BossState toState)
    {
        if (!possibleTransitions.ContainsKey(fromState))
        {
            possibleTransitions[fromState] = new List<BossState>();
        }

        possibleTransitions[fromState].Add(toState);
    }

    private void PerformStateAction(BossState state)
    {
        if (stateActions.ContainsKey(state))
        {
            stateActions[state].Invoke();
        }
    }

    private void Update()
    {
        if (!isTakingAction)
        {
            StartCoroutine(ChangeStateAfterDelay());
        }
    }

    private IEnumerator ChangeStateAfterDelay()
    {
        isTakingAction = true;

        float randomDelay = UnityEngine.Random.Range(changeStateTime - changeStateTimeVariation, changeStateTime + changeStateTimeVariation);
        yield return new WaitForSeconds(randomDelay);

        BossState nextState = GetRandomTransition(currentState);
        PerformStateAction(nextState);

        currentState = nextState;
        isTakingAction = false;
    }

    private BossState GetRandomTransition(BossState currentState)
    {
        if (possibleTransitions.ContainsKey(currentState))
        {
            List<BossState> transitions = possibleTransitions[currentState];
            return transitions[UnityEngine.Random.Range(0, transitions.Count)];
        }

        return currentState;
    }

    private IEnumerator SpikeAttackCoroutine(float time)
    {
        yield return telegraphAttack(spikesAttackTelegraph, time);

        Vector3 targetPosition = new Vector3(spikes.transform.position.x, spikesOffset, spikes.transform.position.z);

        while (spikes.transform.position.y < spikesOffset)
        {
            spikes.transform.position = Vector3.MoveTowards(spikes.transform.position, targetPosition, time * Time.deltaTime);
            yield return null;
        }
    }

    private void spikeAttack(float time)
    {
        StartCoroutine(SpikeAttackCoroutine(time));
    }

    private IEnumerator telegraphAttack(GameObject telegraph, float time)
    {
        telegraph.SetActive(true);
        yield return new WaitForSeconds(time);
        telegraph.SetActive(false);
    }






    private void leftBeamAttack(float time)
    {
        // Implementation for the left beam attack
    }

    private void rightBeamAttack(float time)
    {
        // Implementation for the right beam attack
    }

    private void doubleBeamAttack(float time)
    {
        // Implementation for the double beam attack
    }

    private void heal(int healAmount)
    {
        // Implementation for healing
        // Use healAmount to determine the amount of healing
    }

    private void spawnEnemy()
    {
        // Implementation for spawning an enemy
    }

    private void damage()
    {
        // Implementation for taking damage
    }

    private void death()
    {
        // Implementation for death
    }
}
