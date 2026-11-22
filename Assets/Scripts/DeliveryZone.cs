using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    [SerializeField] private string acceptedTag = "CardboardBox";
    [SerializeField] private int points = 10;
    [SerializeField] private int penalty = 5;
    [SerializeField] public ParticleSystem confettiPrefab;
    [SerializeField] public ParticleSystem wrongPuffPrefab;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if(rb == null) return;

        GameObject box = rb.gameObject;
        Vector3 spawnPos = GetComponent<Collider>().bounds.center + Vector3.up *1f;

        if(box.CompareTag(acceptedTag))
        {
            GameManager.Instance.AddScore(points);
            Destroy(box);
            if(confettiPrefab != null)
                Instantiate(confettiPrefab, spawnPos, Quaternion.identity);

        }
        else if(box.CompareTag("JunkBox"))
        {
            GameManager.Instance.AddScore(-penalty);
            Destroy(box);
            if(wrongPuffPrefab != null)
                Instantiate(wrongPuffPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            GameManager.Instance.AddScore(-penalty);
            Destroy(box);
            if(wrongPuffPrefab != null)
                Instantiate(wrongPuffPrefab, spawnPos, Quaternion.identity);
        }

    }
}