using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScoreCategoryItem : MonoBehaviour
{
    [Header("Garbage")]
    [SerializeField] private TextMeshProUGUI scoreNameText;
    [SerializeField] private TextMeshProUGUI scoreAmountText;
    [SerializeField] private TextMeshProUGUI textMissionRepeatCount;
    [SerializeField] private RectTransform rootMultipleScore;

    float tickeringTime = 1.5f;

    public void SetTextScore(string scoreName, float scoreAmount)
    {
        scoreNameText.text = scoreName;
        scoreAmountText.text = ((int)scoreAmount).ToString();
    }

    public void SetMissionText(string missionDesc, float rewardAmount, int missionRepeatCount)
    {
        rootMultipleScore.gameObject.SetActive(true);
        scoreNameText.SetText(missionDesc);
        scoreAmountText.SetText(((int)rewardAmount).ToString());
        textMissionRepeatCount.SetText("x" + missionRepeatCount.ToString());
    }

    public void TickeringTextReward(float targetReward)
    {
        int currentReward = 0;
        DOTween.To(() => currentReward, value =>
        {
            currentReward = (int)value;
            scoreAmountText.SetText(currentReward.ToString());
        }, (int)targetReward, tickeringTime).SetEase(Ease.Linear);
    }

}
