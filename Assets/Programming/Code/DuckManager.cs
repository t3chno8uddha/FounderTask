using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class DuckManager : MonoBehaviour
{
    public Basket[] baskets;
    public Duck[] ducks;
    public SplineContainer[] splines;
    
    public GameObject basketParticle;

    public Duck heldDuck;

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
}