using System;
using System.Collections.Generic;
[Serializable]
public class PlayerSaveData
{
    //背包资源
    public List<ResourceData> resources;
    //玩家位置
    public PlayerPosition playerPosition;
    //已经打开的宝箱ID
    public List<string> openedChestIds;
    //已经击败的敌人ID
    public List<string> defeatedEnemyIds;
    //是否已经通关
    public bool completed;
}
[Serializable]
public class PlayerPosition
{
    public float x;
    public float y;
}