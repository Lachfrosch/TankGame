using UnityEngine;

public class TankController : MonoBehaviour
{
    public float moveSpeed = 10.0f; // speed of tank movement
    public float rotateSpeed = 10.0f; // speed of tank rotation
    public Transform[] leftWheels; // array of left wheels
    public Transform[] rightWheels; // array of right wheels
    public Transform[] leftTracks; // array of left tracks
    public Transform[] rightTracks; // array of right tracks
    public float trackScrollSpeed = 0.5f; // speed of track scrolling

    private Rigidbody rb; // rigidbody of the tank
    private float movementInput = 0.0f; // current input for tank movement
    private float rotationInput = 0.0f; // current input for tank rotation

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        movementInput = Input.GetAxis("Vertical");
        rotationInput = Input.GetAxis("Horizontal");

        // move the tank based on input
        Vector3 movement = transform.forward * movementInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        // rotate the tank based on input
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, rotationInput * rotateSpeed * Time.deltaTime);
        rb.MoveRotation(rb.rotation * rotation);

        // update wheel rotations based on tank movement
        float leftWheelRotation = movementInput * moveSpeed * Time.deltaTime * 360.0f / (2.0f * Mathf.PI * 0.5f) * -1.0f;
        float rightWheelRotation = leftWheelRotation * -1.0f;
        for (int i = 0; i < leftWheels.Length; i++)
        {
            Quaternion leftRotation = Quaternion.Euler(0.0f, 0.0f, leftWheelRotation);
            Quaternion rightRotation = Quaternion.Euler(0.0f, 0.0f, rightWheelRotation);
            leftWheels[i].rotation *= leftRotation;
            rightWheels[i].rotation *= rightRotation;
        }

        // update track scrolling based on tank movement
        float trackOffset = movementInput * trackScrollSpeed * Time.deltaTime;
        for (int i = 0; i < leftTracks.Length; i++)
        {
            Vector2 leftOffset = new Vector2(leftTracks[i].GetComponent<Renderer>().material.mainTextureOffset.x + trackOffset, 0.0f);
            Vector2 rightOffset = new Vector2(rightTracks[i].GetComponent<Renderer>().material.mainTextureOffset.x + trackOffset, 0.0f);
            leftTracks[i].GetComponent<Renderer>().material.mainTextureOffset = leftOffset;
            rightTracks[i].GetComponent<Renderer>().material.mainTextureOffset = rightOffset;
        }
    }
}
