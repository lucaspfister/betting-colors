using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    public static readonly Pool Instance = new Pool();

    private GameObject m_ChipPrefab;
    private Queue<GameObject> m_Chips = new Queue<GameObject>();

    private Pool()
    {
        m_ChipPrefab = Resources.Load<GameObject>(Paths.CHIP);
        GameObject chip;

        for (int i = 0; i < 100; i++)
        {
            chip = Object.Instantiate(m_ChipPrefab);
            chip.SetActive(false);
            m_Chips.Enqueue(chip);
        }
    }

    public GameObject GetChip()
    {
        GameObject chip = m_Chips.Count > 0 ? m_Chips.Dequeue() : Object.Instantiate(m_ChipPrefab);
        chip.SetActive(true);
        return chip;
    }

    public void ReturnChip(GameObject chip)
    {
        chip.SetActive(false);
        chip.transform.SetParent(null);
        m_Chips.Enqueue(chip);
    }
}