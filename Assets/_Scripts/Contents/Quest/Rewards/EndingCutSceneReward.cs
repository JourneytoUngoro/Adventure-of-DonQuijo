using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCutSceneReward : Reward
{
    // TODO : GameObject -> EndingController ending 변경한다

    public override void GiveRewardTo()
    {
        if (! canGiveReward) return;

        // endingController.ExecuteBeforeEnding()
        Debug.Log("ending controller에 컷씬 이벤트 등록 완료!");
    }
}
