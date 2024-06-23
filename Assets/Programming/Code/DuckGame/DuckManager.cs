using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class DuckManager : MonoBehaviour
{
    public Basket[] baskets; // Array of baskets
    public Duck[] ducks; // Array of ducks
    public SplineContainer[] splines; // Array of splines

    public GameObject basketParticle; // Particle effect for basket appearance
    public GameObject dropParicle; // Particle effect for duck dropping
    public GameObject triumphParticle; // Particle effect for level completion

    public Duck heldDuck; // Currently held duck

    public PointUI[] pUI; // Array of UI points
    int pointIndex = 0; // Index for the current point

    bool inactive; // Flag to check if the game is inactive
    public float inactiveTimer = 8f; // Timer for inactivity

    public GameObject gameOverScreen; // Game over screen
    List<Duck> activeDucks = new List<Duck>(); // List of active ducks

    public Transform handObject; // Transform of the hand object

    private Coroutine inactiveCoroutine; // Handle for the InactiveCooldown coroutine

    void Start()
    {
        StartCoroutine(StartGame(true)); // Start the game
    }

    IEnumerator StartGame(bool firstTime)
    {
        if (firstTime)
        {
            BasketCtl(false); // Disable baskets at the start
        }

        DucksRollOut(); // Initialize ducks

        yield return new WaitForSeconds(1f); // Wait for a second

        if (firstTime)
        {
            BasketCtl(true); // Enable baskets after initialization
        }
    }

    void BasketCtl(bool enabled)
    {
        // Control the visibility of baskets and instantiate particles
        foreach (Basket basket in baskets)
        {
            GameObject bObject = basket.gameObject;
            Transform bTransform = basket.transform;

            bObject.SetActive(enabled); // Set basket active/inactive
            if (enabled)
            {
                Instantiate(basketParticle, bTransform.position, bTransform.rotation); // Instantiate particle effect
            }
        }
    }

    void DucksRollOut()
    {
        ShuffleSplines(); // Shuffle the splines array

        for (int i = 0; i < ducks.Length; i++)
        {
            Duck duckInstance = Instantiate(ducks[i], Vector3.zero, Quaternion.identity); // Instantiate duck at the origin
            activeDucks.Add(duckInstance); // Add duck to the active list

            SplineAnimate splineAnimator = duckInstance.splineAnimator; // Get the SplineAnimate component
            
            if (splineAnimator != null && i < splines.Length)
            {
                splineAnimator.Container = splines[i]; // Assign spline to the duck
            }
        }
    }

    void ShuffleSplines()
    {
        // Shuffle the splines array algorithm
        System.Random rng = new System.Random();
        int count = splines.Length;
        while (count > 1)
        {
            count--;
            int randomIndex = rng.Next(count + 1);
            SplineContainer value = splines[randomIndex];
            splines[randomIndex] = splines[count];
            splines[count] = value;
        }
    }

    public void AddDuck(Duck duck)
    {
        heldDuck = duck; // Set the currently held duck
        inactive = false; // Reset inactivity flag
        handObject.gameObject.SetActive(false); // Hide the hand object

        if (inactiveCoroutine != null)
        {
            StopCoroutine(inactiveCoroutine); // Stop the inactivity cooldown coroutine
            inactiveCoroutine = null;
        }
    }

    public void RemoveDuck(Duck duck)
    {
        activeDucks.Remove(duck); // Remove duck from the active list
        if (activeDucks.Count == 0)
        {
            FinishSequence(); // Finish the sequence if no active ducks remain
        }
    }

    void FinishSequence()
    {
        Instantiate(triumphParticle); // Instantiate triumph particle effect

        pUI[pointIndex].Activate(); // Activate the current point UI
        pointIndex++; // Move to the next point

        if (pointIndex < pUI.Length)
        {
            StartCoroutine(StartGame(false)); // Start the next game round
        }
        else
        {
            FinishGame(); // Finish the game if all points are activated
        }
    }

    void FinishGame()
    {
        gameOverScreen.SetActive(true); // Display the game over screen
    }

    void Update()
    {
        if (!heldDuck)
        {
            if (!inactive)
            {
                inactiveCoroutine = StartCoroutine(InactiveCooldown()); // Start the inactivity cooldown
            }
        }
    }

    IEnumerator InactiveCooldown()
    {
        inactive = true; // Set inactivity flag

        yield return new WaitForSeconds(inactiveTimer); // Wait for the inactivity timer

        if (inactive)
        {
            // Move the hand object to a random active duck position
            Vector3 randomDuckPos = activeDucks[Random.Range(0, activeDucks.Count)].transform.position;
            handObject.transform.position = randomDuckPos;

            handObject.gameObject.SetActive(true); // Show the hand object
        }
    }
}
