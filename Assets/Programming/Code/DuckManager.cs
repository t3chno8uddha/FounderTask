using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class DuckManager : MonoBehaviour
{
    public Basket[] baskets;
    public Duck[] ducks;
    public SplineContainer[] splines;
    
    public GameObject basketParticle;
    public GameObject dropParicle;
    public GameObject triumphParticle;

    public Duck heldDuck;

    public PointUI[] pUI;
    int pointIndex = 0;

    bool inactive;
    public float inactiveTimer = 8f;

    public GameObject gameOverScreen;
    List<Duck> activeDucks = new List<Duck>();

    public Transform handObject;

    void Start()
    {
        StartCoroutine(StartGame(true));
    }

    IEnumerator StartGame(bool firstTime)
    {
        if (firstTime) { BasketCtl(false); }
        DucksRollOut();

        yield return new WaitForSeconds(1f);
    
        if (firstTime) { BasketCtl(true); }
    }

    void BasketCtl(bool enabled)
    {
        GameObject bObject;
        Transform bTransform;

        foreach(Basket basket in baskets)
        {
            bObject = basket.gameObject;
            bTransform = basket.transform;

            bObject.SetActive(enabled);
            if (enabled) { Instantiate (basketParticle, bTransform.position, bTransform.rotation); }
        }
    }

    void DucksRollOut()
    {
        // Shuffle the splines array
        ShuffleSplines();

        for (int i = 0; i < ducks.Length; i++)
        {
            // Instantiate the duck at a position, you might want to set a specific position or use a prefab's position
            Duck duckInstance = Instantiate(ducks[i], Vector3.zero, Quaternion.identity);
            activeDucks.Add(duckInstance);

            // Get the SplineAnimate component from the instantiated duck
            SplineAnimate splineAnimator = duckInstance.splineAnimator;
            
            // Set the spline for the duck
            if (splineAnimator != null && i < splines.Length)
            {
                splineAnimator.Container = splines[i];
            }
        }
    }
    
    void ShuffleSplines()
    {
        System.Random rng = new System.Random();
        int n = splines.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            SplineContainer value = splines[k];
            splines[k] = splines[n];
            splines[n] = value;
        }
    }

    public void AddDuck(Duck duck)
    {
        heldDuck = duck;
        inactive = false;
        handObject.gameObject.SetActive(false);
    }

    public void RemoveDuck(Duck duck)
    {
        activeDucks.Remove(duck);
        if (activeDucks.Count == 0)
        {
            FinishSequence();
        }
    }

    void FinishSequence()
    {
        Instantiate(triumphParticle);

        pUI[pointIndex].Activate();
        pointIndex++;       

        if (pointIndex < pUI.Length)
        {
            StartCoroutine(StartGame(false));
        }
        else
        {
            FinishGame();
        }
    }

    void FinishGame()
    {
        gameOverScreen.SetActive(true);
    }

    void Update()
    {
        if (!heldDuck)
        {
            if (!inactive)
            {
                StartCoroutine(InactiveCooldown());
            }
        }
    }

    IEnumerator InactiveCooldown()
    {
        inactive = true;

        yield return new WaitForSeconds (inactiveTimer);

        if (inactive)
        {
            Vector3 randomDuckPos = activeDucks[Random.Range(0, activeDucks.Count)].transform.position;

            handObject.transform.position = randomDuckPos;

            handObject.gameObject.SetActive(true);
        }
    }
}