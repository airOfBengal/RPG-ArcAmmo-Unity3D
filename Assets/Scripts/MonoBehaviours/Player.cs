using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public HitPoints hitPoints;
    public HealthBar healthBarPrefab;
    HealthBar healthBar;
    public Inventory inventoryPrefab;
    Inventory inventory;

    private void Start()
    {
        hitPoints.value = startingHitPoints;
        ResetCharacter();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("CanBePickedUp"))
        {
            Item hitObject = other.gameObject.GetComponent<Consumable>().item;

            if(hitObject != null)
            {
                bool shouldDisappear = false;
                switch (hitObject.itemType)
                {
                    case Item.ItemType.COIN:
                        shouldDisappear = inventory.AddItem(hitObject);
                        break;
                    case Item.ItemType.HEALTH:
                        shouldDisappear = AdjustHitPoints(hitObject.quantity);
                        break;
                    default:
                        break;
                }
                if (shouldDisappear)
                {
                    other.gameObject.SetActive(false);
                }
                
            }
        }
    }

    private bool AdjustHitPoints(int quantity)
    {
        if(hitPoints.value <= maxHitPoints+quantity)
        {
            hitPoints.value += quantity;
            Debug.Log("Adjusted hitpoints by: " + quantity + ". New value: " + hitPoints);
            return true;
        }
        return false;
    }

    public override void ResetCharacter()
    {
        healthBar = Instantiate(healthBarPrefab);
        healthBar.character = this;

        inventory = Instantiate(inventoryPrefab);

        hitPoints.value = startingHitPoints;
    }

    public override IEnumerator DamageCharacter(int damage, float interval)
    {
        while (true)
        {
            StartCoroutine(FlickerCharacter());

            hitPoints.value = hitPoints.value - damage;
            if(hitPoints.value <= float.Epsilon)
            {
                KillCharacter();
                break;
            }

            if(interval > float.Epsilon)
            {
                yield return new WaitForSeconds(interval);
            }
            else
            {
                break;
            }
        }
    }

    public override void KillCharacter()
    {
        base.KillCharacter();

        Destroy(healthBar.gameObject);
        Destroy(inventory.gameObject);
    }
}
