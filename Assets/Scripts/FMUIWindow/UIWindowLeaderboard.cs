using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VD;

public class UIWindowLeaderboard : VDUIWindow
{
    [SerializeField] private RectTransform rootRankContent;
    [SerializeField] private RectTransform rootPlayerRankContent;

    private UITopFrameHUD uiTopFrameHUD;

    public override void Show(object[] dataContainer)
    {
        uiTopFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        uiTopFrameHUD.ShowInputName(false);

        FMLeaderboard leaderboard = (FMLeaderboard)dataContainer[0];
        List<LocalLeaderboardData> leaderboardDatas = FMUserDataService.Get().GetUserInfo().localLeaderboardDataList;
        int playerRankIndex = leaderboard.GetPlayerRankIndex();

        bool dataExist = leaderboardDatas.Count > 0;
        if (!dataExist)
        {
            return;
        }

        //Top 10 Rank
        int showCount = VDParameter.LEADERBOARD_MAX_RANK_SHOW_COUNT;
        int dataCount = leaderboardDatas.Count;
        for (int i = 0; i < showCount; i++)
        {
            if (i >= dataCount)
            {
                break;
            }

            LocalLeaderboardData data = leaderboardDatas[i];
            SetItem(i, playerRankIndex, data, rootRankContent);
        }

        //Player rank
        showCount = VDParameter.LEADERBOARD_MAX_PLAYER_RANK_SHOW_COUNT;
        int rankIndexOffset = VDParameter.LEADERBOARD_MAX_PLAYER_RANK_SHOW_COUNT / 2;
        bool isLastRank = playerRankIndex >= (dataCount - rankIndexOffset);
        int startRankIndex = isLastRank ? playerRankIndex - (showCount - (dataCount - playerRankIndex)) : playerRankIndex - rankIndexOffset;

        if (startRankIndex < 0)
        {
            startRankIndex = 0;
        }

        int totalShow = 0;
        for (int i = startRankIndex; i < dataCount; i++)
        {
            LocalLeaderboardData data = leaderboardDatas[i];
            SetItem(i, playerRankIndex, data, rootPlayerRankContent);

            totalShow++;
            if (totalShow >= showCount)
            {
                break;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rootRankContent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rootPlayerRankContent);
    }
    public override void Hide()
    {
        uiTopFrameHUD.ShowInputName(true);
    }

    public override void DoUpdate()
    {

    }

    public override void OnRefresh()
    {

    }

    private void SetItem(int index, int playerRankIndex, LocalLeaderboardData data, RectTransform root)
    {
        bool isCurrent = index == playerRankIndex;
        UILeaderboardItem item = FMAssetFactory.GetUILeaderboardItem();
        item.SetParent(root);
        item.SetPlayerData(data.playerRank, data.playerFlag, data.playerName, data.playerScore, isCurrent);
        item.SetFrame(isCurrent);
        item.GetComponent<RectTransform>().localScale = Vector3.one;
    }
}
