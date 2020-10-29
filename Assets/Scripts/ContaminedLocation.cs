using UnityEngine;

public class ContaminedLocation : MonoBehaviour
{
    public int infectionStrength;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<PlayerVitals>().InfectionStrength = infectionStrength;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<PlayerVitals>().InfectionStrength = 0;
    }
}
