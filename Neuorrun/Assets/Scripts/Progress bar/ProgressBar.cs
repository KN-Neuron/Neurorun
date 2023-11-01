using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    public Image progressBar;
    [SerializeField] RectTransform fxHolder;
    [SerializeField] GameObject interactable;
    private float lerpSpeed = 2f ;


    float progress, goal = 1f;


    private void Start()
    {
        progress = 0;
    }

    private void Update()
    {
        
        if (progress > goal) progress = goal;

     

        progress = goal - interactable.GetComponent<Interactable.InteractableObject>().getProgress();
        
        progressBarFill();
        ColorChanger();
        fxHolderUpdate();
        progressBarEmpty();
    }

    void progressBarFill()
    {
        float targetFillAmount = progress;
        float currentFillAmount = progressBar.fillAmount;



        float calculatedFillAmount = Mathf.MoveTowards(currentFillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);


        if (currentFillAmount < targetFillAmount)
        progressBar.fillAmount = calculatedFillAmount;
    }

    void progressBarEmpty()
    {
   

        float targetFillAmount = progress;
        float currentFillAmount = progressBar.fillAmount;
        float calculatedFillAmount = Mathf.MoveTowards(currentFillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);

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
        fxHolder.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    


    }

    public void setProgress(float progress)
    {
      this.progress = progress;
    }


}
