using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private enum JumpState
    {
        Grounded,
        Ascending,
        Descending
    }

    public float HorizontalVelocityMax = 5;
    public float HorizontalAcceleration = 5;
    public float VerticalVelocityMax = 5;
    public float VerticalAcceleration = 5;
    public float JumpTreshold = 0.05f;
    public float GroundDetectionRadius = 0.01f;

    Rigidbody2D player;

    private Vector2 GroundDetectionRayCastDirection = new Vector2(0, -1);
    private ContactFilter2D GroundDetectionFilter;
    private bool dashed = true;

    // Start is called before the first frame update
    void Start()
    {
        player = this.GetComponent<Rigidbody2D>();
        GroundDetectionFilter = new ContactFilter2D()
        {
            layerMask = Physics2D.GetLayerCollisionMask(3),
            useLayerMask = true,
            useTriggers = false
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool CheckIfGrounded()
    {
        RaycastHit2D[] hits = new RaycastHit2D[1];
        player.Cast(GroundDetectionRayCastDirection, GroundDetectionFilter, hits, (player.GetComponent<Collider2D>().bounds.size.y / 2) + GroundDetectionRadius);
        return hits[0] != default(RaycastHit2D);
    }


    void FixedUpdate()
    {
        var gamepad = Gamepad.current;

        if (gamepad == null)
        {
            Debug.Log("No controller found");
            return;
        }

        var grounded = CheckIfGrounded();

        Debug.Log($"Current velocity: {player.velocity}");
        if (gamepad.aButton.IsPressed() && grounded)
        {
            player.AddForce(new Vector2(0, VerticalAcceleration), ForceMode2D.Impulse);
        }

        var horizontalVelocity = player.velocity.x;
        if(math.abs(horizontalVelocity) < HorizontalVelocityMax)
        {
            var torque = gamepad.leftStick.ReadValue().x * HorizontalAcceleration;
            player.AddForce(new Vector2(torque, 0));
            if(horizontalVelocity > HorizontalVelocityMax)
            {
                player.velocity = new Vector2(HorizontalVelocityMax, player.velocity.y);
            }
            else if (horizontalVelocity < -HorizontalVelocityMax)
            {
                player.velocity = new Vector2(-HorizontalVelocityMax, player.velocity.y);
            }
        }
    }
}
