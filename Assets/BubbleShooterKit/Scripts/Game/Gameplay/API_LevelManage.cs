using BubbleShooterKit;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class API_LevelManage : MonoBehaviour
{
    private LevelInfo levelInfo;

    private void Awake()
    {
        GenerateLevel();
    }
    void GenerateLevel()
    {
        levelInfo = null;
        levelInfo = FileUtils.LoadLevel(1);
        if (levelInfo == null)
        {
            Debug.LogError("levelInfo null");
            return;
        }
        if(DataHandler.inst == null)
        {
            return;
        }
        if (DataHandler.inst.apiLevelData == null)
        {
            Debug.LogError("API level data null");
            return;
        }
        Debug.LogError("Start generating level from API");
        levelInfo.Rows = DataHandler.inst.apiLevelData.puzzle.Count;
        levelInfo.Tiles = new List<LevelRow>(levelInfo.Rows);
        for (int i = 0; i < levelInfo.Rows; i++)
        {
            LevelRow row = new LevelRow();
            row.Tiles = new List<TileInfo>(DataHandler.inst.apiLevelData.puzzle[i].Count);
            for (int j = 0; j < DataHandler.inst.apiLevelData.puzzle[i].Count; j++)
            {
                if (DataHandler.inst.apiLevelData.puzzle[i][j] == -1)
                {
                    BubbleTileInfo ti = null;
                    row.Tiles.Add(ti);
                }
                else
                {
                    BubbleTileInfo ti = new BubbleTileInfo();
                    ti.Type = (ColorBubbleType)DataHandler.inst.apiLevelData.puzzle[i][j];
                    ti.CoverType = CoverType.None;
                    row.Tiles.Add(ti);
                }
            }
            if (DataHandler.inst.apiLevelData.puzzle[i].Count == 9)
            {
                BubbleTileInfo ti = null;
                row.Tiles.Add(ti);
            }
            //Debug.LogError(row.Tiles.Count);
            levelInfo.Tiles.Add(row);
        }
        levelInfo.NumBubbles = 50000;
    } 
} 