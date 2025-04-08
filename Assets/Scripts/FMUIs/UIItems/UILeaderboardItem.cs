using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting; //TODO : Naftali, it is need?
using UnityEngine;
using UnityEngine.UI;

public class UILeaderboardItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerRank;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI playerScore;
    [SerializeField] Image flagIcon;
    [SerializeField] Image rankIcon;
    [SerializeField] Image imageFrame;

    public void SetPlayerData(int rank,string flag, string name, float score, bool isCurrent)
    {
        playerRank.text = rank.ToString();
        playerName.text = name;
        playerScore.text = score.ToString("N0");

        //Rank Icon
        Sprite spriteRank;
        switch (rank)
        {
            case 1:
                spriteRank = FMAssetFactory.GetLeaderboardIcon("RankGold");
                rankIcon.sprite = spriteRank;
                break;
            case 2:
                spriteRank = FMAssetFactory.GetLeaderboardIcon("RankSilver");
                rankIcon.sprite = spriteRank;
                break;
            case 3:
                spriteRank = FMAssetFactory.GetLeaderboardIcon("RankBronze");
                rankIcon.sprite = spriteRank;
                break;
            default:
                spriteRank = FMAssetFactory.GetLeaderboardIcon("RankNormal_Off");
                rankIcon.sprite = spriteRank;
                break;
        }

        if (isCurrent)
        {
            spriteRank = FMAssetFactory.GetLeaderboardIcon("RankNormal_On");
            rankIcon.sprite = spriteRank;
        }

        //Flag Icon
        Sprite spriteFlag;
        spriteFlag = FMAssetFactory.GetFlag(flag);
        flagIcon.sprite = spriteFlag;
    }

    public void SetFrame(bool isCurrent)
    {
        Sprite spriteFrame;
        spriteFrame = FMAssetFactory.GetLeaderboardItemFrame(isCurrent);
        imageFrame.sprite = spriteFrame;
    }

    public void SetParent(RectTransform root)
    {
        transform.SetParent(root);
    }
}
