using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveGimic : MonoBehaviour
{
    public StageManager stage;
    private ChangeStage change;
    private PlayerManager playerManager;
    private MainUI mainUI;
    public GameObject fallOBj{get; set;}
    public bool isRiding{get; set;}
    private Vector3Int pos;
    private Vector3Int fallObjPos = new Vector3Int(100,100,100);
    public Vector3Int Pos {get{return pos;}}
    public Vector3Int FallObjPos {get{return fallObjPos;} set{fallObjPos = value;}}
    public List<GameObject> oldObjs = new List<GameObject>();
    public List<GameObject> oldDownObjs = new List<GameObject>();
    private List<Vector3Int> oldPickaxePos = new List<Vector3Int>();
    private List<Vector3Int> oldWallPos = new List<Vector3Int>();
    private List<Vector3Int> oldPutWallPos = new List<Vector3Int>();
    private List<int> itemCount = new List<int>();
    private List<int> destroyWallCount = new List<int>();
    private List<int> putWallCount = new List<int>();
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject itemPrefab;

    // ブロック破壊処理
    public int destroyCount{get; set;}
    public GameObject wallobj{get; private set;}
    private GameObject oldwallobj;

    #region タイルタイプを簡略化
    public StageManager.TILE_TYPE wall {get{return StageManager.TILE_TYPE.WALL;}}
    public StageManager.TILE_TYPE block {get{return StageManager.TILE_TYPE.BLOCK;}}
    public StageManager.TILE_TYPE none {get{return StageManager.TILE_TYPE.NONE;}}
    public StageManager.TILE_TYPE player {get{return StageManager.TILE_TYPE.PLAYER;}}
    public StageManager.TILE_TYPE goal {get{return StageManager.TILE_TYPE.GOAL;}}
    public StageManager.TILE_TYPE trap {get{return StageManager.TILE_TYPE.TRAP;}}
    public StageManager.TILE_TYPE item {get{return StageManager.TILE_TYPE.ITEM;}}
    public StageManager.TILE_TYPE fall {get{return StageManager.TILE_TYPE.FALLOBJ;}}
    public StageManager.TILE_TYPE grass {get{return StageManager.TILE_TYPE.GRASS;}}
    public StageManager.TILE_TYPE soil {get{return StageManager.TILE_TYPE.SOIL;}}
    #endregion
    
    void Start()
    {
        change = stage.changeStage;
        mainUI = stage.mainUI;
    }
    
    // 位置からオブジェクトを特定
    private GameObject GetBlockObjAt(Vector3Int position)
    {
        foreach(var pair in stage.moveObjPositionOnTile)
        {
            if(pair.Value == position) {
                return pair.Key;
            }
        }
        return null;
    }

    // タイルタイプを一括で検証
    private bool ConfirmTileType(Vector3Int position, StageManager.TILE_TYPE[] tileType)
    {
        foreach (var value in tileType)
        {
            if (stage.tileAll[position.x,position.y,position.z] == value) {
                return true;
            }
        }
        return false;
    }

    // ステージ外の場合TRUE
    public bool None(Vector3Int position)
    {
        if(position.x == 9 || position.x == -1 || position.z == 9 || position.z == -1 || position.y < 0) {
            Sounds.instance.se[10].Play();
            return true;
        }
        return false;
    }

    // 自分の真下にブロックが存在するか
    public bool NoneDown(Vector3Int downPos)
    {
        if (ConfirmTileType(downPos, new StageManager.TILE_TYPE[]{none, trap, goal})) {
            Sounds.instance.se[10].Play();
            return true;
        }
        return false;
    }


    // 障害物の場合TRUE
    public bool IsWall(Vector3Int position)
    {
        if (ConfirmTileType(position, new StageManager.TILE_TYPE[]{wall, trap, grass, fall})) {
            Sounds.instance.se[10].Play();
            return true;
        }
        return false;
    }

    // ブロックの場合TRUE
    public bool IsBlock(Vector3Int position)
    {
        if (ConfirmTileType(position, new StageManager.TILE_TYPE[]{block})) return true;
        return false;
    }

    // BLOCK処理で進めない場合にTRUE
    public bool IsStop(Vector3Int nextBlockPos, Vector3Int downPos, Vector3Int upPos)
    {
        // 範囲外
        if (!InTheRange(nextBlockPos)){
            Sounds.instance.se[10].Play(); 
            return true;
        }

        // BLOCKのNEXT
        if (ConfirmTileType(nextBlockPos, new StageManager.TILE_TYPE[]{block, wall, trap, fall})) {
            Sounds.instance.se[10].Play();
            return true;
        }
        // PLAYERのNEXT + DOWN
        if (ConfirmTileType(downPos, new StageManager.TILE_TYPE[]{trap})) {
            Sounds.instance.se[10].Play();
            return true;
        }
        // PLAYERのNEXT + UP
        if (ConfirmTileType(upPos, new StageManager.TILE_TYPE[]{block})) {
            Sounds.instance.se[10].Play();
            return true;
        }
        return false;
    }

    // PLAYERのNEXT + DOWNが障害物の場合TRUE
    public bool IsNone(Vector3Int position)
    {
        if (ConfirmTileType(position, new StageManager.TILE_TYPE[]{none, trap})) {
            Sounds.instance.se[10].Play();
            return true;
        }
        return false;
    }
    
    // GOALの場合
    public bool Goal(Vector3Int position)
    {
        if (ConfirmTileType(position, new StageManager.TILE_TYPE[]{goal})) return true;
        return false;
    }

    // BLOCKを落とせる場合TRUE
    public bool BlockDownPos(Vector3Int downPos)//ここboolにする
    {
        if (!InTheRange(downPos)) return false;
        if (ConfirmTileType(downPos, new StageManager.TILE_TYPE[]{wall, block, trap, grass, fall})) {
            pos = downPos;
            return true;
        }
        return false;
    }
    
    // POSの初期化処理
    public void ResetPosData()
    {
        pos = new Vector3Int(0,-1,0);
    }

    // TRAPがある場合はTRUE
    public bool HighPosTrap(Vector3Int upPos)
    {
        if (ConfirmTileType(upPos, new StageManager.TILE_TYPE[]{trap})) {
            pos = upPos;
            Sounds.instance.se[10].Play();
            return true;
        }
        return false;
    }

    // 一番高い場所を取得
    public bool CheckMaxHigh(Vector3Int upPos)
    {
        if (ConfirmTileType(upPos, new StageManager.TILE_TYPE[]{wall, block, grass, fall})) {
            pos = upPos;
            return true;
        }
        return false;
    }

    public bool CheckMaxPut(Vector3Int upPos)
    {
        if (ConfirmTileType(upPos, new StageManager.TILE_TYPE[]{wall, block, grass, fall, trap})) {
            pos = upPos;
            return true;
        }
        return false;
    }

    // FALLOBJを落とせる場合TRUE
    public bool ReadyFallObj(Vector3Int downPos)
    {
        if (ConfirmTileType(downPos, new StageManager.TILE_TYPE[]{fall})  && !isRiding) {
            isRiding = true;
            fallObjPos = new Vector3Int(downPos.x,downPos.y,downPos.z);
            return true;
        }
        return false;
    }

    // FALLOBJをLISTに
    public void GetFallObj(Vector3Int downPos)
    {
        if (ConfirmTileType(downPos, new StageManager.TILE_TYPE[]{fall})) {
            fallOBj = GetBlockObjAt(new Vector3Int(downPos.x,downPos.y,downPos.z));
            oldDownObjs.Add(fallOBj);
        }
    }
    
    // BLOCKを落とす
    public void BlockDown(Vector3Int currentPos, Vector3Int nextPos, Vector3Int fallPos)
    {
        GameObject block = GetBlockObjAt(currentPos);

        oldObjs.Add(block);
        block.transform.DOPath(new Vector3[]{block.transform.position,nextPos,fallPos},0.5f,PathType.Linear);
        stage.moveObjPositionOnTile[block] = fallPos;
        stage.tileAll[fallPos.x,fallPos.y,fallPos.z] = StageManager.TILE_TYPE.BLOCK;

        // 高さが違う場合
        if(fallPos.y != block.transform.position.y) {
            StartCoroutine(BlockTimer(fallPos));
        }
    }

    //タイルタイプ取得
    public StageManager.TILE_TYPE GetTileType(Vector3Int oldPos)
    {
        return stage.tileAll[oldPos.x,oldPos.y,oldPos.z];
    }
    
    // PLAYER情報の書き換え
    public void UpdateTileTableForPlayer(Vector3Int currentPos, Vector3Int nextPos)
    {
        stage.tileAll[nextPos.x,nextPos.y,nextPos.z] = player;
        stage.tileAll[currentPos.x,currentPos.y,currentPos.z] = none;
    }

    //巻き戻し機能 (BLOCK)
    public bool BackBlock(StageManager.TILE_TYPE type)
    {
        if (type == block) {
            return true;
        }
        return false;
    }

    //巻き戻し機能 (FALL)
    public bool BackFallObj(StageManager.TILE_TYPE type, Vector3Int currentPlayerPos)
    {
        if (type != fall) {
            return false;
        }
        oldDownObjs[oldDownObjs.Count-1].transform.DOMove(currentPlayerPos + Vector3Int.down,0.5f);
        oldDownObjs[oldDownObjs.Count-1].GetComponentInChildren<MeshRenderer>().material.DOFade(1,0.5f);
        stage.moveObjPositionOnTile[oldDownObjs[oldDownObjs.Count-1]] = currentPlayerPos + Vector3Int.down;
        stage.tileAll[(currentPlayerPos + Vector3Int.down).x,(currentPlayerPos + Vector3Int.down).y,(currentPlayerPos + Vector3Int.down).z] = fall;
        return true;
    }

    public bool BackItem(int count)
    {
        if (itemCount.Count == 0) {
            return false;
        }
        if (count == itemCount[itemCount.Count-1]) {
            // 見た目
            GameObject itemObj = Instantiate(itemPrefab, oldPickaxePos[oldPickaxePos.Count-1], Quaternion.identity);
            change.pickaxeObj.Add(itemObj.transform.GetChild(0));
            change.pickaxeImage.Add(itemObj.transform.GetChild(1));
            itemObj.transform.GetChild(1).gameObject.SetActive(false);
            // タイル情報を格納
            stage.moveObjPositionOnTile.Add(itemObj, oldPickaxePos[oldPickaxePos.Count-1]);
            stage.tileAll[oldPickaxePos[oldPickaxePos.Count-1].x,oldPickaxePos[oldPickaxePos.Count-1].y,oldPickaxePos[oldPickaxePos.Count-1].z] = item;

            playerManager.pickaxeCount--;
            mainUI.pickaxeText.text = playerManager.pickaxeCount.ToString();

            itemCount.RemoveAt(itemCount.Count-1);
            oldPickaxePos.RemoveAt(oldPickaxePos.Count-1);
            return true;
        }
        return false;
    }

    // PICKAXE
    public bool GetItem(Vector3Int position, int count)
    {
        if (ConfirmTileType(position, new StageManager.TILE_TYPE[]{item})) {
            // ITEMを消す
            if (playerManager == null) {
                playerManager = stage.Player;
            }
            playerManager.pickaxe.SetActive(true);
            playerManager.pickaxeCount++;
            mainUI.pickaxeText.text = playerManager.pickaxeCount.ToString();
            GameObject item = GetBlockObjAt(position);
            change.pickaxes.Remove(item);
            change.pickaxeObj.Remove(item.transform.GetChild(0));
            change.pickaxeImage.Remove(item.transform.GetChild(1));
            stage.moveObjPositionOnTile.Remove(item);
            Sounds.instance.se[14].Play();
            Destroy(item);
            oldPickaxePos.Add(position);
            itemCount.Add(count);
            return true;
        }
        return false;
    }

    // DestroyしたWallを戻す
    public bool BackDestroyWall(int count)
    {
        if (destroyWallCount.Count == 0) {
            return false;
        }
        if (count + 1 == destroyWallCount[destroyWallCount.Count-1]) {
            // 見た目
            GameObject wallObj = Instantiate(wallPrefab, oldWallPos[oldWallPos.Count-1], Quaternion.identity);
            // タイル情報を格納
            stage.moveObjPositionOnTile.Add(wallObj, oldWallPos[oldWallPos.Count-1]);
            stage.tileAll[oldWallPos[oldWallPos.Count-1].x,oldWallPos[oldWallPos.Count-1].y,oldWallPos[oldWallPos.Count-1].z] = wall;

            playerManager.pickaxeCount++;
            playerManager.stoneCount--;
            mainUI.pickaxeText.text = playerManager.pickaxeCount.ToString();
            mainUI.stoneText.text = playerManager.stoneCount.ToString();

            destroyWallCount.RemoveAt(destroyWallCount.Count-1);
            oldWallPos.RemoveAt(oldWallPos.Count-1);
            return true;
        }
        return false;
    }

    public bool BackPutWall(int count)
    {
        if (wallobj != null) {
                wallobj.GetComponentInChildren<MeshRenderer>().material.DOColor(Color.white,0);
            }
        if (playerManager.pickaxeCount == 0) {
            playerManager.pickaxe.SetActive(false);
        }
        playerManager.Anim.SetBool("dig", false);

        if (putWallCount.Count == 0) {
            return false;
        }
        
        if (count + 1 == putWallCount[putWallCount.Count-1]) {
            // 見た目
            GameObject putwallObj = GetBlockObjAt(oldPutWallPos[oldPutWallPos.Count - 1]);
            // タイル情報を格納
            stage.tileAll[oldPutWallPos[oldPutWallPos.Count-1].x,oldPutWallPos[oldPutWallPos.Count-1].y,oldPutWallPos[oldPutWallPos.Count-1].z] = none;

            playerManager.stoneCount++;
            mainUI.stoneText.text = playerManager.stoneCount.ToString();
            Destroy(putwallObj);
            putWallCount.RemoveAt(putWallCount.Count-1);
            oldPutWallPos.RemoveAt(oldPutWallPos.Count-1);
            return true;
        }
        return false;
    }

    public bool DestroyWall(Vector3Int next, int count)
    {
        if (playerManager == null) {
            playerManager = stage.Player;
        }
        if (ConfirmTileType(next + Vector3Int.up, new StageManager.TILE_TYPE[]{block})) {
            return false;
        }
        if (!ConfirmTileType(next, new StageManager.TILE_TYPE[]{wall})) {
            destroyCount = 0;
            if (wallobj != null) {
                wallobj.GetComponentInChildren<MeshRenderer>().material.DOColor(Color.white,0);
            }
            playerManager.Anim.SetBool("dig", false);
            return false;
        } 
        if (playerManager.pickaxeCount == 0) {
            destroyCount = 0;
            return false;
        }
        if (stage.tileAll[next.x,next.y,next.z] == wall) {
            destroyCount++;
            wallobj = GetBlockObjAt(new Vector3Int(next.x,next.y,next.z));
            // 3回目からmissing
            switch (destroyCount)
            {
                case 1:
                    wallobj.GetComponentInChildren<MeshRenderer>().material.DOColor(Color.red,0);
                    playerManager.Anim.SetBool("dig", true);
                break;
                case 2:
                    if (wallobj != oldwallobj) {
                        destroyCount = 0;
                        oldwallobj.GetComponentInChildren<MeshRenderer>().material.DOColor(Color.white,0);
                        playerManager.Anim.SetBool("dig", false);
                        return false;
                    }
                    // 本処理
                    stage.tileAll[next.x,next.y,next.z] = none;
                    stage.moveObjPositionOnTile.Remove(wallobj);
                    Destroy(wallobj);
                    destroyCount = 0;
                    playerManager.pickaxeCount--;
                    playerManager.stoneCount++;
                    mainUI.pickaxeText.text = playerManager.pickaxeCount.ToString();
                    mainUI.stoneText.text = playerManager.stoneCount.ToString();
                    Sounds.instance.se[15].Play();
                    oldWallPos.Add(next);
                    destroyWallCount.Add(count);
                    playerManager.Anim.SetBool("dig", false);

                    if (playerManager.pickaxeCount == 0) {
                        playerManager.pickaxe.SetActive(false);
                    }
                break;
            }
            oldwallobj = wallobj;
            return true;
        }
        playerManager.Anim.SetBool("dig", false);
        return false;
    }
    
    // wallObjを置く
    public bool PutWall(Vector3Int position, int count)
    {
        if (playerManager == null) {
            playerManager = stage.Player;
        }
        if (playerManager.stoneCount == 0) return false;

        // 見た目
        GameObject wallObj = Instantiate(wallPrefab, position, Quaternion.identity);
        GameObject smoke = Instantiate(Sounds.instance.effects[0],wallObj.transform.position + new Vector3(0,-0.5f,0),Quaternion.identity);
        playerManager.stoneCount--;
        mainUI.stoneText.text = playerManager.stoneCount.ToString();
        // タイル情報を格納
        stage.moveObjPositionOnTile.Add(wallObj, position);
        stage.tileAll[position.x,position.y,position.z] = wall;

        putWallCount.Add(count);
        oldPutWallPos.Add(position);
        Sounds.instance.se[3].Play();
        StartCoroutine(DestroyTimer(smoke));
        return true;
    }

    // 範囲外
    private bool InTheRange(Vector3Int position)
    {
        if(position.x == 9 || position.x == -1 || position.z == 9 || position.z == -1)
        {
            return false;
        }
        return true;
    }

    public void RemoveList(List<Vector3Int>[] posLists, List<StageManager.TILE_TYPE>[] typeLists)
    {
        foreach(var value in posLists) {
            value.RemoveAt(value.Count-1);
        }
        foreach(var value in typeLists) {
            value.RemoveAt(value.Count-1);
        }
    }

    // 煙エフェクト
    private IEnumerator BlockTimer(Vector3 pos)
    {
        yield return new WaitForSeconds(0.5f);
        GameObject effect = Instantiate(Sounds.instance.effects[0],pos + new Vector3(0,-0.5f,0),Quaternion.identity); 
        Sounds.instance.se[3].Play();

        StartCoroutine(DestroyTimer(effect));
    }

    private IEnumerator DestroyTimer(GameObject effect)
    {
        yield return new WaitForSeconds(1f);
        Destroy(effect);
    }
}