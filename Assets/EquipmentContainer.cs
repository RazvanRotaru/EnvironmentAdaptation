using System.Collections.Generic;

[System.Serializable]
public class EquipmentContainer {
    public Item head;
    public Item body;
    public Item pants;
    public Item weapon;
    public Item armor;

    public List<Item> runes;

    public GeneContainer ApplyBuffs(GeneContainer genes) {
        GeneContainer ans = new GeneContainer(genes);

        List<string> keys = new List<string>(genes.map.Keys);
        foreach (var key in keys) {
            float buff = 0f;

            if (head != null && head.buffs.HasBuff(key))       buff += head.buffs.GetBuff(key);
            if (body != null && body.buffs.HasBuff(key))       buff += body.buffs.GetBuff(key);
            if (pants != null && pants.buffs.HasBuff(key))     buff += pants.buffs.GetBuff(key);
            if (weapon != null && weapon.buffs.HasBuff(key))   buff += weapon.buffs.GetBuff(key);
            if (armor != null && armor.buffs.HasBuff(key))     buff += armor.buffs.GetBuff(key);

            foreach (var rune in runes)
               if (rune != null && rune.buffs.HasBuff(key))    buff += rune.buffs.GetBuff(key);

            ans.map[key] = genes.map[key] + buff;
        }

        return ans;
    }

    public GeneContainer RemoveBuffs(GeneContainer genes) {
        // TODO implement
        throw new System.NotImplementedException();
    }
}
