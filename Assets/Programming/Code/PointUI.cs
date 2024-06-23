using UnityEngine;

public class PointUI : MonoBehaviour
{
    public void Activate()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}