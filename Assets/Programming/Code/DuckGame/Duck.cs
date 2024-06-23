using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class Duck : ColorBehaviour
{
    public bool isHeld = false; // Flag to check if the duck is currently held
    bool wasHeld = false; // Flag to check if the duck was held at least once
    bool isFinished = false; // Flag to check if the duck has finished its task
    public SplineAnimate splineAnimator; // Reference to the SplineAnimate component
    Vector3 destination; // The final destination of the duck
    public float recallSpeed = 25f; // Speed at which the duck returns to its destination
    Animator duckAnimator; // Reference to the Animator component
    DuckManager dManager; // Reference to the DuckManager
    Basket basket; // Reference to the Basket
    public AudioClip spawnClip, grabClip; // Audio clips for spawning and grabbing
    AudioSource source; // AudioSource component for playing sounds

    void Start()
    {
        // Initialize references
        dManager = FindObjectOfType<DuckManager>();
        source = GetComponent<AudioSource>();
        splineAnimator = GetComponent<SplineAnimate>();
        duckAnimator = GetComponent<Animator>();

        // Set the destination to the last point on the spline
        destination = splineAnimator.Container.Spline[splineAnimator.Container.Spline.Count - 1].Position;
        
        // Start the quack timer
        StartCoroutine(QuackTimer(splineAnimator.Duration));
    }

    IEnumerator QuackTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (!wasHeld)
        {
            wasHeld = true; // Mark as held
            Destroy(splineAnimator); // Remove the spline animator
            source.PlayOneShot(spawnClip); // Play spawn sound
        }
    }

    void Update()
    {
        if (!isFinished)
        {
            if (isHeld)
            {
                // Move the duck to the cursor position
                Vector3 newDestination = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
                newDestination.z = transform.position.z; // Keep the z position unchanged
                transform.position = newDestination; // Update position
            }
            else if (wasHeld)
            {
                // Move the duck towards its destination
                transform.position = Vector3.MoveTowards(transform.position, destination, recallSpeed * Time.deltaTime);
            }
        }
    }

    void OnMouseDown()
    {
        if (!isFinished)
        {
            ToggleHeld(true); // Set the duck as held

            if (!wasHeld)
            {
                Destroy(splineAnimator); // Remove the spline animator
                wasHeld = true; // Mark as held
            }

            source.PlayOneShot(grabClip); // Play grab sound
        }
    }

    void OnMouseUp()
    {
        if (!isFinished)
        {
            if (basket && basket.objectColor == objectColor)
            {
                StartCoroutine(FinishDuck()); // Finish the duck's task
            }

            ToggleHeld(false); // Set the duck as not held
        }
    }

    void ToggleHeld(bool held)
    {
        isHeld = held; // Update the held state
        duckAnimator.SetBool("Is Held", held); // Update the animator

        if (held)
        {
            dManager.AddDuck(this); // Add this duck to the manager
        }
        else
        {
            dManager.AddDuck(null); // Remove this duck from the manager
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Basket"))
        {
            basket = col.GetComponent<Basket>(); // Set the basket reference
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Basket"))
        {
            basket = null; // Clear the basket reference
        }
    }

    IEnumerator FinishDuck()
    {
        isFinished = true; // Mark as finished
        duckAnimator.SetBool("Finished", isFinished); // Update the animator

        // Create particle effect at the basket position
        Instantiate(dManager.dropParicle, basket.transform.position, basket.transform.rotation);

        dManager.RemoveDuck(this); // Remove this duck from the manager
        Destroy(GetComponent<Collider2D>()); // Remove the collider

        yield return new WaitForSeconds(1f); // Wait for 1 second

        Destroy(gameObject); // Destroy the duck object
    }
}
