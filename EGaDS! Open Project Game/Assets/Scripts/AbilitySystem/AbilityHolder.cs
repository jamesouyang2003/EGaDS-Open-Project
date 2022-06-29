using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AbilityHolder : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Ability _ability;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Called whenever this ability is dropped with the ability 
    /// that this ability holder represents.
    /// </summary>
    /// <param name="ability">The ability which this ability
    /// holder represents.</param>
    public void DropAbility(Ability ability)
    {
        _ability = ability;
        _spriteRenderer.sprite = ability.AbilitySprite;
    }

    /// <summary>
    /// Called whenever this ability is picked up. 
    /// </summary>
    /// <returns>Returns the ability which this ability holder
    /// holds.</returns>
    public Ability PickUpAbility()
    {
        return _ability;
    }

}
