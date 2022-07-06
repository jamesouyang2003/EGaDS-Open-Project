using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AbilityHolder : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Ability _ability;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _ability.AbilitySprite;
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
