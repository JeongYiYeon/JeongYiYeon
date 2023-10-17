using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMoveCharacter : BaseCharacter
{
    public void InitMoveCharacter(CharacterData characterData)
    {
        InitStat(characterData);
        InitCharacterInfo();
    }

    private void InitCharacterInfo()
    {
        CharacterAnimation.SetAtlas(Character.CharacterAtlasName);

        fsmManager.SetFSM(AnimationType.Run.ToString(), new StateMove(this));
    }

    public override void FixedUpdate()
    {
        SetState(AnimationType.Run.ToString());
    }
}
