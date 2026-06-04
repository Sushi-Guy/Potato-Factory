using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Conveyor Settings")]
    [SerializeField] private float speed = 2.5f;

    void OnCollisionEnter(Collision collision)
    {
        // Check if the object we hit is on the Player layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) return;

        SnapToCenter(collision.rigidbody);
    }

    void OnCollisionStay(Collision collision)
    {
        // Check if the object we are touching is on the Player layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) return;

        Rigidbody rb = collision.rigidbody;
        
        if (rb != null)
        {
            rb.WakeUp();

            // Kill any sideways rolling velocity immediately to stop drift
            Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
            localVelocity.x = 0f; 
            rb.linearVelocity = transform.TransformDirection(localVelocity);

            // Move down the belt's center track
            Vector3 forwardMovement = transform.forward * speed * Time.deltaTime;
            
            Vector3 localPoint = transform.InverseTransformPoint(rb.position);
            localPoint.x = 0f; // HARD LOCK to the absolute center line
            
            Vector3 centeredWorldPosition = transform.TransformPoint(localPoint);
            
            rb.MovePosition(centeredWorldPosition + forwardMovement);
        }
    }

    private void SnapToCenter(Rigidbody rb)
    {
        if (rb == null) return;

        Vector3 localPoint = transform.InverseTransformPoint(rb.position);
        localPoint.x = 0f; 
        
        rb.position = transform.TransformPoint(localPoint);
    }
}