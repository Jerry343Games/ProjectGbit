using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SceneManager : Singleton<SceneManager>
{
    public List<AIBot> AIBotList = new List<AIBot>();

    public List<PlayerBot> PlayerBotList = new List<PlayerBot>();

    public List<Part> AllParts = new List<Part>();
    public void RegisterAIBot(AIBot bot)
    {
        if (!AIBotList.Contains(bot))
        {
            AIBotList.Add(bot);
        }
    }

    public void RemoveAIBot(AIBot bot)
    {
        if (AIBotList.Contains(bot))
        {
            AIBotList.Remove(bot);
        }
    }

    public void RegisterPlayerBot(PlayerBot bot)
    {
        if (!PlayerBotList.Contains(bot))
        {
            PlayerBotList.Add(bot);
        }
    }

    public void RemovePlayerBot(PlayerBot bot)
    {
        if (PlayerBotList.Contains(bot))
        {
            PlayerBotList.Remove(bot);
        }
    }

    public void RegisterPart(Part part)
    {
        if(!AllParts.Contains(part))
        {
            AllParts.Add(part);
        }
    }

    public void RemovePart(Part part)
    {
        if (AllParts.Contains(part))
        {
            AllParts.Remove(part);
        }
    }

    public void CallHook()
    {

    }

}
