using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    public Image progressBar;
    [SerializeField] RectTransform fxHolder;
    [SerializeField] GameObject interactable;

    float progress, goal = 1f;
    float lerpSpeed;

    private void Start()
    {
        progress = 0;
    }

    private void Update()
    {
        
        if (progress > goal) progress = goal;

        lerpSpeed = 2f * Time.deltaTime;

        progress = goal - interactable.GetComponent<Interactable.InteractableObject>().getProgress();
        Debug.Log(progress);
        HealthBarFiller();
        ColorChanger();
        fxHolderUpdate();
        HealthBarDecrease();
    }

    void HealthBarFiller()
    {
        float targetFillAmount = progress;
        float currentFillAmount = progressBar.fillAmount;

    
      

        

        float calculatedFillAmount = Mathf.MoveTowards(currentFillAmount, targetFillAmount, 2*lerpSpeed);

       
 


        if (currentFillAmount < targetFillAmount)
        progressBar.fillAmount = calculatedFillAmount;
    }

    void HealthBarDecrease()
    {
       /* progress = Mathf.MoveTowards(progress, 0, 1.5f * Time.deltaTime);*/

        float targetFillAmount = progress;
        float currentFillAmount = progressBar.fillAmount;
        float calculatedFillAmount = Mathf.MoveTowards(currentFillAmount, targetFillAmount, lerpSpeed);

        progressBar.fillAmount = calculatedFillAmount;
    }

    void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (progress));

        progressBar.color = healthColor;

    }

    void fxHolderUpdate()
    {

        float targetAngle = -(progress) * 360;
        float currentAngle = fxHolder.rotation.eulerAngles.z;
        float angle = Mathf.LerpAngle(currentAngle, targetAngle, lerpSpeed*4);
/*        float angle2 = Mathf.MoveTowards(currentAngle, targetAngle, lerpSpeed);*/
        /*        fxHolder.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle2));*/
        fxHolder.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        /*Debug.Log(angle + "/" + angle2);*/
        /*        float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, 2f * Time.deltaTime);
                fxHolder.rotation = Quaternion.Euler(fxHolder.rotation.x, fxHolder.rotation.y, angle);*/
    


    }

    public void setProgress(float progress)
    {
      /*  this.progress = progress;*/
    }


}
