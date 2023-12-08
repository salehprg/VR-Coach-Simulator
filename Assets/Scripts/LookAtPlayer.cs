using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 5f;
    public float damping = 0.1f;

    void Update()
    {
        if (player != null)
        {
            Vector3 playerPosition = player.position;
            playerPosition.y = transform.position.y; // Ensure the object doesn't tilt up or down

            // Look at the player's forward direction
            transform.LookAt(playerPosition + player.forward);

            // Optional smoothing
            Quaternion targetRotation = Quaternion.LookRotation(playerPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, damping * Time.deltaTime);
        }
    }
}
