using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPoliceAttack : MonoBehaviour
{
    public List<BotProperty> playersInTrigger = new List<BotProperty>();

    private void OnTriggerEnter(Collider other)
    {
        BotProperty botProperty = other.GetComponent<BotProperty>();
        if (botProperty)
        {
            playersInTrigger.Add(botProperty);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BotProperty botProperty = other.GetComponent<BotProperty>();
        if (botProperty)
        {
            playersInTrigger.Remove(botProperty);
        }
    }
    public void AttackPlayer()
    {
        foreach(BotProperty botProperty in playersInTrigger)
        {
            if (botProperty.isAIBot)
            {
                botProperty.gameObject.GetComponent<AIBot>().Dead();
            }
            else
            {
                botProperty.gameObject.GetComponent<PlayerBot>().Dead();
            }
        }
        playersInTrigger.Clear();
    }
}
