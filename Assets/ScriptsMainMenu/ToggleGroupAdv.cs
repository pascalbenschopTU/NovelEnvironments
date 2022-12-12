using System.Collections.Generic;
using UnityEngine.UI;

public class ToggleGroupAdv : ToggleGroup
{
    public List<Toggle> Toggles;

    public ToggleGroupAdv()
    {
        Toggles = m_Toggles;
    }

    public int GetFirstOnIndex()
    {
        for (int i = 0; i < Toggles.Count; i++)
        {
            if (Toggles[i].isOn)
            {
                return i;
            }
        }

        return 0;
    }
}