using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class StageManager : MonoBehaviour
{
    public MainUI mainUI;
    public ChangeStage changeStage;
    [SerializeField] TextAsset[] stageFiles;
    [SerializeField] GameObject[] prefabs;
    public enum TILE_TYPE
    {
        WALL,
        BLOCK,
        NONE,
        PLAYER,
        GOAL,
        TRAP,
        FALLOBJ,
        ITEM,
        DONTDESTROY,
        GRASS,
        SOIL,
    }
    public TILE_TYPE[,,] tileAll;
    private int playerHierarchy;
    private int maxHierarchy;
    public int PlayerHierarchy {
        get {return playerHierarchy;}
        set {playerHierarchy = value;}
    }
    public int MaxHierarchy {
        get {return maxHierarchy;}
    }
    
    private PlayerManager player;
    public PlayerManager Player {
        get {return player;}
    }
    public GameObject fireWork;
    public GameObject key;
    public int stageNumber;
    public List<GameObject> objList = new List<GameObject>();
    public Dictionary<GameObject, Vector3Int> moveObjPositionOnTile = new Dictionary<GameObject, Vector3Int>();

    private string SelectMode()
    {
        return PlayerPrefs.GetString("MODE", "DEFAULT");
    }
    private string[] high;
    public void LoadStageData()
    {
        // DEFAULTMODE
        if (SelectMode() == "DEFAULT") {
            stageNumber = PlayerPrefs.GetInt("STAGENUMBER", 0);
            mainUI.ShowStageNumber(stageNumber);
            high = stageFiles[stageNumber].text.Split(new[] {'.'},System.StringSplitOptions.RemoveEmptyEntries);
        } 
        // EDITMODE
        if (SelectMode() == "EDIT") {
            string path = Application.dataPath + "/test.txt";
            string a = File.ReadAllText(path);
            high = a.Split(new[] {'.'},System.StringSplitOptions.RemoveEmptyEntries);
        }
        // ONLINEMODE
        if (SelectMode() == "ONLINE") {
            string path = Application.dataPath + "/test.txt";
            string a = File.ReadAllText(path);
            high = a.Split(new[] {'.'},System.StringSplitOptions.RemoveEmptyEntries);
        }

        // TEXTFILEの情報を読み込む
        string[] lines = high[0].Split(new[] {'\n','\r'},System.StringSplitOptions.RemoveEmptyEntries);
        int colums = lines[0].Split(new[] {','}).Length;
        int rows = lines.Length;
        int h = high.Length;
        
        // X,Y,Z　情報から　TILE＿TYPEへ変換
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
                    // その地点のVector3Int 情報から TILETYPE へ変換
                    Vector3Int position = new Vector3Int(x,y,z);
                    TILE_TYPE tileType = tileAll[x,y,z];

                    // TILETYPEに合ったオブジェクトをインスタンス化
                    if ((int)tileType != 9)
                    {
                        GameObject obj = Instantiate(prefabs[(int)tileType]);
                        obj.transform.position = new Vector3(x,y,z);
                        objList.Add(obj);

                        if (tileType == TILE_TYPE.PLAYER)
                        {
                            player = obj.GetComponent<PlayerManager>();
                            moveObjPositionOnTile.Add(obj,position);
                            PlayerHierarchy = y;
                        }
                        if (tileType == TILE_TYPE.BLOCK || tileType == TILE_TYPE.FALLOBJ || tileType == TILE_TYPE.ITEM || tileType == TILE_TYPE.WALL)
                        {
                            moveObjPositionOnTile.Add(obj,position);
                        }
                        if (tileType == TILE_TYPE.GOAL)
                        {
                            key = obj;
                        }
                        maxHierarchy = y;
                    }
                    // GRASSの場合
                    else
                    {
                        int randomCount = Random.Range(9,13);
                        GameObject obj = Instantiate(prefabs[randomCount]);
                        obj.transform.position = new Vector3(x,y,z);
                        objList.Add(obj);
                        
                        maxHierarchy = y;
                    }
                }
            }
        }
    }
    public void DestroyStage()
    {
        foreach(var value in objList)
        {
            Destroy(value.gameObject);
        }
        moveObjPositionOnTile = new Dictionary<GameObject, Vector3Int>();
        objList = new List<GameObject>();
        changeStage.meshs = new List<MeshRenderer>();
        changeStage.objs = new List<GameObject>();
        fireWork.SetActive(false);
        Sounds.instance.bgm[0].Play();
    }
}
