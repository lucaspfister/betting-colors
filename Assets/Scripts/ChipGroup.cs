using System.Collections.Generic;
using UnityEngine;

public class ChipGroup
{
    private Transform m_Transform;
    private float m_SpaceBetweenChips;
    private Stack<GameObject> m_Chips = new Stack<GameObject>();

    public int ChipCount => m_Chips.Count;

    public ChipGroup(Transform transform, float spaceBetweenChips)
    {
        m_Transform = transform;
        m_SpaceBetweenChips = spaceBetweenChips;
    }

    public void AddChip(GameObject chip)
    {
        chip.transform.position = m_Transform.position + new Vector3(0, m_SpaceBetweenChips * m_Chips.Count, 0);
        chip.transform.SetParent(m_Transform);
        m_Chips.Push(chip);
    }

    public void AddNewChip(int amount, Material chipMaterial)
    {
        GameObject chip = null;

        for (int i = 0; i < amount; i++)
        {
            chip = Pool.Instance.GetChip();
            chip.GetComponent<Renderer>().material = chipMaterial;
            AddChip(chip);
        }
    }

    public GameObject RemoveChip()
    {
        if (m_Chips.Count == 0) return null;

        return m_Chips.Pop();
    }

    public void Clear()
    {
        while (m_Chips.Count > 0)
        {
            Pool.Instance.ReturnChip(m_Chips.Pop());
        }
    }
}
