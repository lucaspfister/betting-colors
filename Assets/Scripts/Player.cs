using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform[] m_ChipGroupObjs;
    [SerializeField] private Transform m_BetChipGroupObj;
    [Header("UI")]
    [SerializeField] private CanvasGroup m_BetPanel;
    [SerializeField] private Button m_ButtonRed;
    [SerializeField] private Button m_ButtonGreen;
    [SerializeField] private Button m_ButtonFinishBet;
    [SerializeField] private CanvasGroup m_BottomPanel;
    [SerializeField] private Transform m_BetButtonsParent;
    [SerializeField] private TextMeshProUGUI m_TxtAvailable;
    [SerializeField] private TextMeshProUGUI m_TxtBet;

    private ChipSettings m_ChipSettings;
    private ChipGroup[] m_ChipGroups;
    private ChipGroup m_BetChipGroup;
    private List<int> m_CurrentBetChips = new List<int>();
    private BetColor m_CurrentBetColor;

    public int TotalAvailable { get; private set; }
    public int CurrentBet { get; private set; }

    private void Start()
    {
        m_ChipSettings = Resources.Load<ChipSettings>(Paths.CHIP_SETTINGS);
        m_ChipGroups = new ChipGroup[m_ChipGroupObjs.Length];
        m_BetChipGroup = new ChipGroup(m_BetChipGroupObj, m_ChipSettings.spaceBetweenChips);
        Button betButtonPrefab = Resources.Load<Button>(Paths.BUTTON_BET);
        m_ButtonRed.onClick.AddListener(() => SendBet(BetColor.Red));
        m_ButtonGreen.onClick.AddListener(() => SendBet(BetColor.Green));
        m_ButtonFinishBet.onClick.AddListener(FinishBet);
        Button betButton;

        for (int i = 0; i < m_ChipSettings.chips.Length; i++)
        {
            m_ChipGroups[i] = new ChipGroup(m_ChipGroupObjs[i], m_ChipSettings.spaceBetweenChips);
            betButton = Instantiate(betButtonPrefab, m_BetButtonsParent);
            int index = i;
            betButton.onClick.AddListener(() => AddBet(index));
            betButton.GetComponent<RawImage>().texture = m_ChipSettings.chips[i].texture;
            betButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = m_ChipSettings.chips[i].value.ToString();
        }

        AddInitialChips();
        GameManager.Instance.OnRoundEnd += RoundEnd;
    }

    private void AddBet(int chipIndex)
    {
        GameObject chipObj = m_ChipGroups[chipIndex].RemoveChip();

        if (chipObj == null) return;

        m_BetChipGroup.AddChip(chipObj);
        m_CurrentBetChips.Add(chipIndex);
        int value = m_ChipSettings.chips[chipIndex].value;
        TotalAvailable -= value;
        CurrentBet += value;

        if (TotalAvailable == 0)
        {
            AddInitialChips();
        }

        if (m_BetChipGroup.ChipCount == 10)
        {
            FinishBet();
        }

        RefreshTexts();
    }

    private void RefreshTexts()
    {
        m_TxtAvailable.text = $"Available: {TotalAvailable}";
        m_TxtBet.text = $"Bet: {CurrentBet}";
    }

    private void FinishBet()
    {
        if (m_BetChipGroup.ChipCount == 0) return;

        m_BottomPanel.interactable = false;
        m_BetPanel.alpha = 1;
        m_BetPanel.interactable = true;
    }

    private void SendBet(BetColor betColor)
    {
        m_CurrentBetColor = betColor;
        m_BetPanel.alpha = 0;
        m_BetPanel.interactable = false;

        GameManager.Instance.RevealObject();
    }

    private void AddInitialChips()
    {
        Chip chip;

        for (int i = 0; i < m_ChipSettings.chips.Length; i++)
        {
            chip = m_ChipSettings.chips[i];
            m_ChipGroups[i].AddNewChip(chip.initialAmount, chip.material);
            TotalAvailable += chip.value * chip.initialAmount;
        }

        RefreshTexts();
    }

    private void RoundEnd(BetColor result)
    {
        CurrentBet = 0;
        m_BetChipGroup.Clear();
        m_BottomPanel.interactable = true;

        if (m_CurrentBetColor == result)
        {
            int index;

            for (int i = 0; i < m_CurrentBetChips.Count; i++)
            {
                index = m_CurrentBetChips[i];
                m_ChipGroups[index].AddNewChip(2, m_ChipSettings.chips[index].material);
                TotalAvailable += m_ChipSettings.chips[index].value * 2;
            }
        }

        m_CurrentBetChips.Clear();
        RefreshTexts();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnRoundEnd -= RoundEnd;
    }
}
