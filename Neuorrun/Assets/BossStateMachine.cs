using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BossState
{
    Start,
    Idle,
    BeamAttack,
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
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private int damageAmount;
    [SerializeField] private int healAmount;
    [SerializeField] private int maxConsecutiveAttacks=2;
    [SerializeField] private GameObject spikesAttackTelegraph;
    [SerializeField] private GameObject beamLeftAttackTelegraph;
    [SerializeField] private GameObject beamRightAttackTelegraph;
    [SerializeField] private GameObject enemySpawnAttackTelegraph;
    [SerializeField] private WaveSpawner waveSpawner;
    [SerializeField] private Transform playerRespawn;

    [SerializeField] private float changeStateTime;
    [SerializeField] private float changeStateTimeVariation;

    private Dictionary<BossState, List<BossState>> possibleTransitions;
    private Dictionary<BossState, Action> stateActions;

    private bool isTakingAction;
    private bool isHealing =false;
    private bool isDamaged =false;
    private int consecutiveAttacks = 0;
    private BossState lastState=BossState.Start;
    private DeathManager deathManager;

    private void Awake()
    {
        deathManager = FindObjectOfType<DeathManager>();
        deathManager.setLastGroundPosition(playerRespawn.position);
        currentState = BossState.Start;
        InitializeTransitions();
        InitializeStateActions();
    }

    private void InitializeTransitions()
    {
        // Initialize the dictionary and add possible transitions for each state
        possibleTransitions = new Dictionary<BossState, List<BossState>>();

        AddTransition(BossState.Start, BossState.Idle);

        AddTransition(BossState.Idle, BossState.BeamAttack);
        AddTransition(BossState.Idle, BossState.EnemySpawnAttack);
        AddTransition(BossState.Idle, BossState.SpikeAttack);



        AddTransition(BossState.LeftBeamAttack, BossState.Idle);
        AddTransition(BossState.RightBeamAttack, BossState.Idle);
        AddTransition(BossState.DoubleBeamAttack, BossState.Idle);
        AddTransition(BossState.EnemySpawnAttack, BossState.Idle);
        AddTransition(BossState.SpikeAttack, BossState.Idle);
        AddTransition(BossState.Damaged, BossState.Idle);
        AddTransition(BossState.Heal, BossState.Idle);

        AddTransition(BossState.Damaged, BossState.LeftBeamAttack);
        AddTransition(BossState.Damaged, BossState.RightBeamAttack);
        AddTransition(BossState.Damaged, BossState.DoubleBeamAttack);
        AddTransition(BossState.Damaged, BossState.EnemySpawnAttack);
        AddTransition(BossState.Damaged, BossState.SpikeAttack);



        AddTransition(BossState.Heal, BossState.Idle);
        AddTransition(BossState.Heal, BossState.LeftBeamAttack);
        AddTransition(BossState.Heal, BossState.RightBeamAttack);
        AddTransition(BossState.Heal, BossState.DoubleBeamAttack);
        AddTransition(BossState.Heal, BossState.EnemySpawnAttack);
        AddTransition(BossState.Heal, BossState.Damaged);

        AddTransition(BossState.BeamAttack, BossState.LeftBeamAttack);
        AddTransition(BossState.BeamAttack, BossState.RightBeamAttack);
        AddTransition(BossState.BeamAttack, BossState.DoubleBeamAttack);

    }

    private void InitializeStateActions()
    {
        // Initialize the dictionary and add actions for each state
        stateActions = new Dictionary<BossState, Action>();

        stateActions[BossState.SpikeAttack] = () => { spikeAttack(1.5f); };
        stateActions[BossState.LeftBeamAttack] = () => { leftBeamAttack(3.0f); };
        stateActions[BossState.RightBeamAttack] = () => { rightBeamAttack(3.0f); };
        stateActions[BossState.DoubleBeamAttack] = () => { doubleBeamAttack(1.5f); };
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
        if (health <= 0)
        {
            changeState(BossState.Death);
        }

        if(deathManager.isKilled)
        {
            changeState(BossState.Heal);
        }

        if (currentState!=BossState.Damaged && BossState.Heal!=currentState && currentState!=BossState.Death)
        {
            if (!isTakingAction)
            {
                StartCoroutine(ChangeStateAfterDelay());
            }
        }
  

    }

    private IEnumerator ChangeStateAfterDelay()
    {
        isTakingAction = true;

        if (currentState != BossState.BeamAttack)
        {
            float randomDelay = UnityEngine.Random.Range(changeStateTime - changeStateTimeVariation, changeStateTime + changeStateTimeVariation);
            yield return new WaitForSeconds(randomDelay);
        }
       
        if(currentState!= BossState.Idle)
        {
            lastState = currentState;
        }

        BossState nextState = GetRandomTransition(currentState);

        if (lastState == nextState)
        {
            consecutiveAttacks++;
            if (consecutiveAttacks >= maxConsecutiveAttacks)
            {
                while (nextState == lastState)
                {
                    nextState = GetRandomTransition(currentState);
                }
            }
        }
        else
        {
            consecutiveAttacks = 0;
        }

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

    private void changeState(BossState state)
    {
        currentState = state;
    }

    private IEnumerator SpikeAttackCoroutine(float time)
    {
        yield return telegraphAttack(spikesAttackTelegraph, time);

        Vector3 initialPosition = spikes.transform.position;
        Vector3 targetPosition = new Vector3(spikes.transform.position.x, spikes.transform.position.y+spikesOffset, spikes.transform.position.z);

        while (spikes.transform.position.y < targetPosition.y)
        {
            spikes.transform.position = Vector3.MoveTowards(spikes.transform.position, targetPosition, time * Time.deltaTime);
            yield return null;
        }

        yield return clearSpikes(initialPosition);
    }

    private IEnumerator clearSpikes(Vector3 initialPosition)
    {

        yield return new WaitForSeconds(0.2f);
        spikes.transform.position = initialPosition;
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
        StartCoroutine(beamAttackCoroutine(beamLeftAttackTelegraph, leftBeam, time));
    }

    private void rightBeamAttack(float time)
    {
        StartCoroutine(beamAttackCoroutine(beamRightAttackTelegraph, rightBeam, time));
    }

    private void doubleBeamAttack(float time)
    {
        StartCoroutine(beamAttackCoroutine(beamLeftAttackTelegraph,leftBeam, time));
        StartCoroutine(beamAttackCoroutine(beamRightAttackTelegraph, rightBeam, time));
    }

    private IEnumerator beamAttackCoroutine(GameObject beamTelegraph, GameObject beam, float time)
    {
        yield return telegraphAttack(beamTelegraph, 0.5f);

        beam.SetActive(true);
        
        yield return new WaitForSeconds(time);

        beam.GetComponent<Animator>().SetTrigger("Despawned");
        beam.SetActive(false);
        
    }


    private void heal(int healAmount)
    {
        // Implementation for healing
        // Use healAmount to determine the amount of healing
    }

    private void spawnEnemy()
    {
        StartCoroutine(spawnEnemyCoroutine());
    }

    private IEnumerator spawnEnemyCoroutine()
    {
        yield return telegraphAttack(enemySpawnAttackTelegraph, 0.5f);

        waveSpawner.SpawnRandomWave();
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
