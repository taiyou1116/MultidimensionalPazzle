using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class EditStageManager : MonoBehaviour
{
    [SerializeField] PlayerEdit edit;
    [SerializeField] GameObject[] stagePrefabs;
    private enum TILE_TYPE
    {
        WALL,BLOCK,NONE,PLAYER,GOAL,TRAP,FALLOBJ,OTHER2,OTHER,GRASS,SOIL
    }
    private TILE_TYPE wall = TILE_TYPE.WALL;
    private TILE_TYPE block = TILE_TYPE.BLOCK;
    private TILE_TYPE none = TILE_TYPE.NONE;
    private TILE_TYPE player = TILE_TYPE.PLAYER;
    private TILE_TYPE goal = TILE_TYPE.GOAL;
    private TILE_TYPE trap = TILE_TYPE.TRAP;
    private TILE_TYPE fall = TILE_TYPE.FALLOBJ;
    private TILE_TYPE grass = TILE_TYPE.GRASS;
    private TILE_TYPE soil = TILE_TYPE.SOIL;
    TILE_TYPE[,,] tileAll;
    private string[] high;

    void Start()
    {
        if(MainForOnline.Instance.stageData == "") return;
        LoadStageData();
        CreateStage();
    }

    public void LoadStageData()
    {
        string path = Application.dataPath + "/test.txt";
        string a = File.ReadAllText(path);
        high = a.Split(new[] {'.'},System.StringSplitOptions.RemoveEmptyEntries);

        string[] lines = high[0].Split(new[] {'\n','\r'},System.StringSplitOptions.RemoveEmptyEntries);
        int colums = lines[0].Split(new[] {','}).Length;
        int rows = lines.Length;
        int h = high.Length;
        
        tileAll = new TILE_TYPE[colums,h,rows];

        for(int y = 0; y < h; y++)
        {
            string[] value = high[y].Split(new[] {'.'});
            lines = high[y].Split(new[] {'\n','\r'},System.StringSplitOptions.RemoveEmptyEntries);
            for(int z = 0; z < rows; z++)
            {
                string[] values = lines[z].Split(new[] {','});
                for(int x = 0; x < colums; x++)
                {
                    tileAll[x,y,z] = (TILE_TYPE)int.Parse(values[x]);
                }
            }
        }
    }

    public void CreateStage()
    {
        for (int y = 0; y < tileAll.GetLength(1); y++)
        {
            for (int z = 0; z < tileAll.GetLength(2); z++)
            {
                for (int x = 0; x < tileAll.GetLength(0); x++)
                {
                    Vector3Int position = new Vector3Int(x,y,z);
                    TILE_TYPE tileType = tileAll[x,y,z];
                    GameObject obj = Instantiate(stagePrefabs[(int)tileType]);
                    obj.transform.position = new Vector3(x,y,z);
                    
                    // 他のも追加 NONEの場合は何も配置しないようにする
                    if (GetTILE_TYPE(tileType, new TILE_TYPE[]{wall, player, goal, block, trap, fall}))
                    {
                        edit.objDic.Add(obj,position);
                    }
                }
            }
        }
    }

    private bool GetTILE_TYPE(TILE_TYPE tileType, TILE_TYPE[] tYPEs)
    {
        foreach (var value in tYPEs) {
            if (value == tileType) {
                return true;
            }
        }
        return false;
    }
}
