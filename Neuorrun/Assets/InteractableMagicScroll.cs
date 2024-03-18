using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableMagicScroll : MonoBehaviour, Interactable.InteractableObject
{
    [SerializeField] private float timeToUse = 2f;
    [SerializeField] private GameObject spellCast;
    private bool isSpellUsed = false;

    private float interactionProgress = 0f;


    private float timeToUseLeft;

    private void Start()
    {
        timeToUseLeft = timeToUse;
    }

    public float getProgress()
    {
        return 1f-interactionProgress;
    }

    public void interact(float interactionProgress)
    {
        this.interactionProgress = interactionProgress;
        if (interactionProgress >= 1)
        {
            if (timeToUseLeft > 0)
            {
                timeToUseLeft -= Time.deltaTime;
            }
            else
            {
                if (!isSpellUsed)
                {
                    StartCoroutine(waitForSpell());
                    isSpellUsed = true;
                }


            }
        }
        else
        {
            timeToUseLeft = timeToUse;
        }
    }

    IEnumerator waitForSpell()
    {
        Instantiate(spellCast, spellCast.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.4f);
        FindObjectOfType<BossStateMachine>().damage();
        Destroy(GetComponentInParent<Interactable>().gameObject);
    }

    public void reset()
    {
        interactionProgress = 0f;
    }
}
