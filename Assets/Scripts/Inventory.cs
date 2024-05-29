using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, IGatherable
{
    // 0 = not unlocked
    // 1 = unlocked
    // 2 = equipped
    public int Wand = 0;
    public int CultRobe = 0;
    public int InvisibilityRobe = 0;
    public int HeroGarb = 2;

    // 0 = not unlocked
    // 1 = level 1
    // 2 = level 2
    public int Sword = 0;
    public int Boots = 0;
    
    public string gather_name => "Inventory";

    public void FillQuery(string parent_name, Dictionary<string, object> query)
    {
        if (parent_name != null)
        {
            parent_name = parent_name + ".";
        }
        query.Add(parent_name + gather_name + "." + nameof(Wand), Wand);
        query.Add(parent_name + gather_name + "." + nameof(CultRobe), CultRobe);
        query.Add(parent_name + gather_name + "." + nameof(InvisibilityRobe), InvisibilityRobe);
        query.Add(parent_name + gather_name + "." + nameof(Sword), Sword);
        query.Add(parent_name + gather_name + "." + nameof(Boots), Boots);
        query.Add(parent_name + gather_name + "." + nameof(HeroGarb), HeroGarb);
    }

    public void UnEquipAll()
    {
        if (CultRobe > 0)
        {
            CultRobe = 1;
        }
        if (InvisibilityRobe > 0)
        {
            InvisibilityRobe = 1;
        }
        if (HeroGarb > 0)
        {
            HeroGarb = 1;
        }
    }
}
