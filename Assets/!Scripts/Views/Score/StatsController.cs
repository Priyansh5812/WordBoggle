using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsController
{

    public bool IsGamePaused
    {
        get;
        set;
    } = false;

    public bool IsGameOver
    {
        get;
        set;
    } = false;

    GameConfig m_config;
    StatsView m_view;
    WaitForSeconds m_delay;

    int m_score = 0;
    int m_totalBonusCount = 0;
    int totalWordsFound = 0;

    public StatsController(StatsView view, GameConfig config)
    { 
        m_config = config;
        m_view = view;
        m_delay = new WaitForSeconds(1.0f);
    }
    public IEnumerator TimerRoutine()
    {
        int mins = m_config.minutes;
        int secs = m_config.seconds;

        m_view?.UpdateTimerUI(mins, secs);

        yield return m_delay;

        while (secs > 0 || mins > 0)
        {
            while (IsGamePaused)
            {
                yield return null;
            }

            if (mins >= 0)
            {

                if (secs <= 0)
                {
                    secs = 59;
                    mins--;
                }
                else
                {
                    secs--;
                }

                m_view?.UpdateTimerUI(mins, secs);   

                yield return m_delay;
            }
        }
        IsGameOver = true;
        m_view.TriggerGameOver((m_score , m_totalBonusCount, totalWordsFound));

    }

    public void ProcessValidWord(List<LetterTile> tiles)
    {
        int bonusLetterTiles = 0;
        int totalLetterCount = tiles.Count;
        string str = string.Empty;

        foreach (var i in tiles)
        {
            str += i.GetText();
            if (i.IsBonus)
            {
                FreeBonusTile(i);
                i.PlayBonusParticles();
                bonusLetterTiles++;
            }
        }

        if (bonusLetterTiles != 0)
            m_totalBonusCount++;

        totalWordsFound++;

        m_score += (bonusLetterTiles * m_config.scorePerBonus) + (totalLetterCount * m_config.scorePerLetter);

        m_view.UpdateScoreUI(m_score);
        m_view.UpdateExistingWordsUI(str);
    }

    public void FreeBonusTile(LetterTile tile) => tile.IsBonus = false;

    public bool GetIsGameOver() => IsGameOver;

    public void ResetStats()
    {
        m_score = 0;
        m_totalBonusCount = 0;
        totalWordsFound = 0;
        IsGamePaused = false;
        IsGameOver = false;
        m_view.UpdateScoreUI(m_score);
        m_view.UpdateTimerUI(m_config.minutes, m_config.seconds);
    }

}
