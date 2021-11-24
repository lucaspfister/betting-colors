using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] Animator m_BoxAnimator;
    [SerializeField] Renderer m_BetObject;
    [SerializeField] Material[] m_Materials;

    private BetColor m_CurrentBetColor;

    public System.Action<BetColor> OnRoundEnd;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        NewRound();
    }

    private void NewRound()
    {
        m_CurrentBetColor = Random.Range(0, 1f) > 0.5f ? BetColor.Red : BetColor.Green;
        m_BetObject.material = m_Materials[(int)m_CurrentBetColor];
    }

    private IEnumerator RoundEnd()
    {
        WaitForSeconds wait = new WaitForSeconds(1.5f);
        m_BoxAnimator.SetBool("reveal", true);
        yield return wait;
        m_BoxAnimator.SetBool("reveal", false);
        yield return wait;
        OnRoundEnd?.Invoke(m_CurrentBetColor);
        NewRound();
    }

    public void RevealObject()
    {
        StartCoroutine(RoundEnd());
    }
}
