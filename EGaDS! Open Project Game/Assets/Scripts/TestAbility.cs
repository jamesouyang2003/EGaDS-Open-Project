using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/TestAbility")]
public class TestAbility : TriggeredAbility
{
    [SerializeField] private float speed;

    private Rigidbody2D rb;

    public override void AbilityStart(PlayerComponents player)
    {
        rb = player.rigidbody;
    }

    public override void AbilityUpdate(PlayerComponents player)
    {
        if (GetKeyDown())
        {
            rb.velocity = new Vector2(rb.velocity.x, speed);
        }

        if (Input.GetKeyDown(KeyCode.A))
            Debug.Log("HEARD A KEYPRESS");
    }
}
