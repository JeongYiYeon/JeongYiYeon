using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstHelper
{    
    public const string TAG_PLAYER = "PLAYER";
    public const string TAG_ENEMY = "ENEMY";
    public const string TAG_PROJECTILE = "PROJECTILE";
    public const string TAG_NPC = "NPC";
    public const string TAG_SKILL = "SKILL";

    public const string LAYER_UI = "UI";
    public const string LAYER_CHARACTER = "CHARACTER";
    public const string LAYER_DONTSHOW = "DONTSHOW";
    public const string LAYER_SLOT = "SLOT";
    public const string LAYER_SLOTITEM = "SLOTITEM";

    public class MessengerString
    {
        public const string MSG_GAMESPEED = "GameSpeed";
        public const string MSG_HEROLISTITEM_RESET = "HeroListItemReset";
        public const string MSG_STATUP_RESET = "StatUpReset";
        public const string MSG_STATUP = "StatUp";
        public const string MSG_ACTIVESKILL = "ActiveSkill";
        public const string MSG_GAMEAUTO = "GameAuto";
        public const string MSG_TOTAL_COMBAT_POWER = "TotalCombatPower";
    }

}
