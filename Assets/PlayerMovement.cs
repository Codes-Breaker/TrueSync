using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed = 1;
    private Vector3 moveDirection;
    private CharacterController controller;
    private TSTransform tsTransform;
    private TSRigidBody tsRigidbody;
    // Start is called before the first frame update
    void Start()
    {
        controller = this.gameObject.GetComponent<CharacterController>();
        tsTransform = this.gameObject.GetComponent<TSTransform>();
        tsRigidbody = this.gameObject.GetComponent<TSRigidBody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        var vector3Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        tsTransform.Translate(vector3Move.ToTSVector() * 1 * Time.deltaTime, Space.World);
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            return;
        //tsTransform.rotation = TSQuaternion.Slerp(tsTransform.rotation, TSQuaternion.LookRotation(vector3Move.ToTSVector()), Time.deltaTime * 40f);
        tsTransform.rotation = TSQuaternion.LookRotation(vector3Move.ToTSVector());
    }
}
