using UnityEngine;

public class ConePhysics : MonoBehaviour
{
    public float hitForce = 8f;
    public float upwardForce = 2f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponentInParent<PrometeoCarController>() != null)
        {
            Vector3 direction = (transform.position - collision.transform.position).normalized;
            direction.y = upwardForce;

            rb.AddForce(direction * hitForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * hitForce, ForceMode.Impulse);
        }
    }
}