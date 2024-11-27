using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    public Dictionary<STORYFLAG, bool> flags = new Dictionary<STORYFLAG, bool>();

    public void SetFlag(STORYFLAG flag, bool value = true)
    {
        if (flags.ContainsKey(flag))
            flags[flag] = value;
        else
            flags.Add(flag, value);
    }

    public bool CheckFlag(STORYFLAG flag)
    {
        if (!flags.ContainsKey(flag)) return false;
        return flags.TryGetValue(flag, out bool value) ? value : false;
    }

}

public enum STORYFLAG
{
    NONE,
    MetCharacterA,
    DidntKillYourself,
    DoesEatAss
}