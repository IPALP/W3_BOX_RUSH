using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    [SerializeField] private string acceptedTag = "CardboardBox";
    [SerializeField] private int points = 10;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if(rb == null) return;

        GameObject box = rb.gameObject;

        if(box.CompareTag(acceptedTag))
        {
            GameManager.Instance.AddScore(points);
            Destroy(box);
        }
        else if(box.CompareTag("JunkBox"))
        {
            Destroy(box);
        }
        else
        {
            Destroy(box);
        }
    }
}
