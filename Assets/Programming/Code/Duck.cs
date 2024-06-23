using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class Duck : ColorBehaviour
{
    public bool isHeld = false;

    bool wasHeld = false;
    bool isFinished = false;
    public SplineAnimate splineAnimator;
    Vector3 destination;

    public float recallSpeed = 25f;

    Animator duckAnimator;

    DuckManager dManager;

    Basket basket;

    public AudioClip spawnClip, grabClip;

    AudioSource source;

    void Start()
    {
        dManager = FindObjectOfType<DuckManager>();
        
        source = GetComponent<AudioSource>();

        splineAnimator = GetComponent<SplineAnimate>();
        destination = splineAnimator.Container.Spline[splineAnimator.Container.Spline.Count - 1].Position;
    
        duckAnimator = GetComponent<Animator>();
    
        StartCoroutine(QuackTimer(splineAnimator.Duration));
    }

    IEnumerator QuackTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (!wasHeld)
        {
            wasHeld = true;
            Destroy(splineAnimator);

            source.PlayOneShot(spawnClip);
        }
    }

    void Update()
    {
        if (!isFinished)
        {
            if (isHeld)
            {
                // Get the cursor position
                Vector3 newDestination = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 1));
                newDestination.z = transform.position.z;

                transform.position = newDestination;
            }
            else
            {
                if (wasHeld)
                {
                    transform.position = Vector3.MoveTowards(transform.position, destination, recallSpeed * Time.deltaTime);
                }
            }
        }
    }

    void OnMouseDown()
    {
        if (!isFinished)
        {
            ToggleHeld(true);

            if (!wasHeld)
            {
                Destroy(splineAnimator);
                wasHeld = true;
            }

            source.PlayOneShot(grabClip);
        }
    }

    void OnMouseUp()
    {
        if (!isFinished)
        {
            if (basket && basket.objectColor == objectColor)
            {
                StartCoroutine(FinishDuck());
            }

            ToggleHeld(false);
        }
    }

    void ToggleHeld(bool held)
    {
        isHeld = held;
        duckAnimator.SetBool("Is Held", held);
        
        if (held) { dManager.AddDuck(this); }
        else { dManager.AddDuck(null); }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Basket")
        {
            basket = col.GetComponent<Basket>();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        basket = null;
    }

    IEnumerator FinishDuck()
    {
        isFinished = true;
        duckAnimator.SetBool("Finished", isFinished);

        Instantiate(dManager.dropParicle, basket.transform.position, basket.transform.rotation);

        dManager.RemoveDuck(this);
    
        Destroy(GetComponent<Collider2D>());

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}