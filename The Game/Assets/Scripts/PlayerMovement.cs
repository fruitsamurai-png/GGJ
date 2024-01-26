using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private float moveSpd = 5.0f;
    private CharacterController cc;
    public bool interact=false;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 movement = v * Vector3.forward + h * Vector3.right;
        if (movement.sqrMagnitude > 0)
            transform.rotation = Quaternion.LookRotation(movement, Vector3.up);

        cc.SimpleMove(movement * moveSpd);
        interact = false;
    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.tag=="Interactable")
        {
            interact = true;
        }
    }
}
