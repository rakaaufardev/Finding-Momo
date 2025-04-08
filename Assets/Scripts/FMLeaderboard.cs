using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMLeaderboard
{
    public void AddLeaderboardData(float playerScore, UserInfo userInfo)
    {
        string playerFlag = userInfo.currentFlag;
        string playerName = userInfo.currentPlayerName;

        userInfo.currentPlayerScore = playerScore;

        LocalLeaderboardData newEntry = new LocalLeaderboardData(playerFlag, playerName, playerScore);
        List<LocalLeaderboardData> leaderboardData = FMUserDataService.Get().GetUserInfo().localLeaderboardDataList;
        leaderboardData.Add(newEntry);

        //sort leaderboard score
        leaderboardData.Sort((entry1, entry2) => entry2.playerScore.CompareTo(entry1.playerScore));
        for (int i = 0; i < leaderboardData.Count; i++)
        {
            leaderboardData[i].playerRank = i + 1;
        }

        FMUserDataService.Get().SaveLeaderboardData(leaderboardData);
    }

    public int GetPlayerRankIndex()
    {
        int result = -1;

        UserInfo userInfo = FMUserDataService.Get().GetUserInfo();

        string playerFlag = userInfo.currentFlag;
        string playerName = userInfo.currentPlayerName;
        float score = userInfo.currentPlayerScore;

        List<LocalLeaderboardData> localLeaderboardDatas = userInfo.localLeaderboardDataList;
        int count = localLeaderboardDatas.Count;
        for (int i = 0; i < count; i++)
        {
            LocalLeaderboardData data = localLeaderboardDatas[i];
            bool valid = data.playerFlag == playerFlag && data.playerName == playerName && data.playerScore == score;
            if (valid)
            {
                result = i;
            }
        }

        return result;
    }

    public LocalLeaderboardData GetNextRank(float score)
    {
        List<LocalLeaderboardData> leaderboardDataList = FMUserDataService.Get().GetUserInfo().localLeaderboardDataList;
        float playerScore = score;

        int low = 0;
        int high = leaderboardDataList.Count - 1;
        int index = 0;

        while (low <= high)
        {
            int mid = low + (high - low) / 2;

            if (leaderboardDataList[mid].playerScore > playerScore)
            {
                low = mid + 1;
                index = mid;
            }
            else
            {
                high = mid - 1;
            }
        }

        LocalLeaderboardData result = leaderboardDataList[index];
        return result;
    }

    public bool IsRankOne()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        List<LocalLeaderboardData> leaderboardDataList = FMUserDataService.Get().GetUserInfo().localLeaderboardDataList;

        if (leaderboardDataList.Count > 0)
        {
            float rankOneScore = leaderboardDataList[0].playerScore;
            float playerScore = mainScene.GetScoreController().GetTotalScore();

            return playerScore >= rankOneScore;
        }
        else
        {
            return true;
        }
    }
}
