using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] MoveGimic gimic;
    [SerializeField] TextAnim textAnim;
    [SerializeField] TextAnim textAnimonline;
    [SerializeField] BasicOperation operation;
    private ChangeStage changeStage;
    private MainUI mainUI;
    private StageManager stage;
    private SaveData save;
    private PlayerManager player;
    private bool goal;
    private Vector3Int currentPlayerPositionOnTile;
    private Vector3Int nextPlayerPositionOnTile;
    private Vector3Int nextPutPos3D;
    private Vector3Int nextPutPos2D;
    #region 巻き戻し処理リスト
    private List<int> oldHierarchy = new List<int>();
    private List<Vector3Int> oldPos = new List<Vector3Int>();
    private List<Vector3Int> nextPos = new List<Vector3Int>();
    private List<Vector3Int> nextBlockPos = new List<Vector3Int>();
    private List<StageManager.TILE_TYPE> oldTileType = new List<StageManager.TILE_TYPE>();
    private List<StageManager.TILE_TYPE> oldDownTileType = new List<StageManager.TILE_TYPE>();
    private List<StageManager.TILE_TYPE> oldNextDownTileType = new List<StageManager.TILE_TYPE>();
    #endregion
    private enum DIRECTION
    {
        UP,DOWN,LEFT,RIGHT
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        
        Initialize();
        MainForOnline.Instance.Initialize();
        if (SelectMode() == "EDIT") {
            mainUI.stageNumberText.text = "TEST MODE";
        }
        if (SelectMode() == "ONLINE") {
            mainUI.stageNumberText.text = "EVERYONE STAGE";
        }

        mainUI.pickaxeText.text = player.pickaxeCount.ToString();
        mainUI.stoneText.text = player.stoneCount.ToString();
    }
    private void Initialize()
    {
        stage = gimic.stage;
        stage.LoadStageData();
        stage.CreateStage();
        player = stage.Player;
        changeStage = stage.changeStage;
        mainUI = stage.mainUI;
        changeStage.GetObj();
    }
    private void Update()
    {
        PlayerOperation();
    }
    private string SelectMode()
    {
        return PlayerPrefs.GetString("MODE", "DEFAULT");
    }
    private void MoveTo(DIRECTION direction)
    {
        //anim初期化
        player.Anim.ResetTrigger("Push");
        player.Anim.ResetTrigger("Walk");

        //現在の位置と次の位置を取得
        currentPlayerPositionOnTile = stage.moveObjPositionOnTile[player.gameObject];
        nextPlayerPositionOnTile = GetNextPlayerPositionOnTile(currentPlayerPositionOnTile,direction);

        //これやらないとfallObjPosが上手く書き換わらない。バグる
        gimic.FallObjPos = currentPlayerPositionOnTile + Vector3Int.down;

        //MOVE処理
        Action<DIRECTION> func = MoveInThreedImensions;
        if (!changeStage.Stage2D) {
            func(direction);
        } else {
            func = MoveInTwoImensions;
            func(direction);
        }

        // nextの次が範囲外ならreturn
        if (nextPlayerPositionOnTile.x == 8 || nextPlayerPositionOnTile.x == 0 || nextPlayerPositionOnTile.z == 8 || nextPlayerPositionOnTile.z == 0) {
            return;
        }
        nextPutPos3D = GetNextPlayerPositionOnTile(nextPlayerPositionOnTile,direction);
        
        for(int i = stage.MaxHierarchy-1; i >= 0; i--) {
            if (!gimic.CheckMaxPut(new Vector3Int(nextPutPos3D.x,i,nextPutPos3D.z))) continue;

            nextPutPos2D = new Vector3Int(nextPutPos3D.x,i + 1,nextPutPos3D.z);
            return;
        }
    }

    //2次元処理
    private void MoveInTwoImensions(DIRECTION direction)
    {
        //posの位置を初期化
        gimic.ResetPosData();

        //NEXTPOSITIONび移動できるか判別
        for(int i = stage.MaxHierarchy-1; i >= 0; i--)
        {
            // 範囲外
            Func<Vector3Int, bool> func = gimic.None;
            if (func(new Vector3Int(nextPlayerPositionOnTile.x,0,nextPlayerPositionOnTile.z) + (new Vector3Int(0,i,0)))) {
                nextPlayerPositionOnTile = currentPlayerPositionOnTile;
                return;
            }

            // トラップが上部に存在するか
            func = gimic.HighPosTrap;
            if (func(new Vector3Int(nextPlayerPositionOnTile.x,0,nextPlayerPositionOnTile.z) + (new Vector3Int(0,i,0)))) {
                nextPlayerPositionOnTile = currentPlayerPositionOnTile;
                return;
            }

            // 最大の高さを取得
            func = gimic.CheckMaxHigh;
            if (!func(new Vector3Int(nextPlayerPositionOnTile.x,i,nextPlayerPositionOnTile.z))) continue;
            
            // itemget
            func = gimic.GetItem;
            func(gimic.Pos + Vector3Int.up);

            // ゴール
            func = gimic.Goal;
            if (func(gimic.Pos + Vector3Int.up)) Goal();

            //巻き戻しリストにADD
            AddListsAll(gimic.Pos + Vector3Int.up, currentPlayerPositionOnTile + Vector3Int.down);

            //FALLOBJの処理
            FallProcess(gimic.Pos);

            //プレイヤー処理
            PlayerProcess(gimic.Pos + Vector3Int.up);

            //プレイヤーの高さを更新
            stage.PlayerHierarchy = gimic.Pos.y + Vector3Int.up.y;
            return;
        }
    }

    //3次元処理
    private void MoveInThreedImensions(DIRECTION direction)
    {
        //移動先が範囲外か確認
        Func<Vector3Int, bool> func = gimic.None;
        if (func(nextPlayerPositionOnTile)) {
            nextPlayerPositionOnTile = currentPlayerPositionOnTile;
            return;
        }

        func = gimic.DestroyBlock;
        if (func(nextPlayerPositionOnTile)) {
            nextPlayerPositionOnTile = currentPlayerPositionOnTile;
            return;
        }

        // プレイヤー下にブロックが存在するか (エディットのエラーを防ぐ)
        func = gimic.NoneDown;
        if (func(nextPlayerPositionOnTile + Vector3Int.down)) {
            nextPlayerPositionOnTile = currentPlayerPositionOnTile;
            return;
        }

        //移動先に障害物があるか確認
        func = gimic.IsWall;
        if (func(nextPlayerPositionOnTile)) {
            nextPlayerPositionOnTile = currentPlayerPositionOnTile;
            return;
        }

        func = gimic.GetItem;
        func(nextPlayerPositionOnTile);

        //移動先がブロックの場合(木箱)
        func = gimic.IsBlock;
        if (func(nextPlayerPositionOnTile))
        {
            //ブロックの位置を取得
            Vector3Int nextBlockPositionOnTile = GetNextPlayerPositionOnTile(nextPlayerPositionOnTile,direction);

            //ブロックを落とせるか確認
            if (gimic.IsStop(nextBlockPositionOnTile, nextPlayerPositionOnTile + Vector3Int.down, nextPlayerPositionOnTile + Vector3Int.up)) return;

            //ここにブロックを落とす処理
            BlockFallDown(nextBlockPositionOnTile);
            return;
        }
        
        //プレイヤーが進めるか
        func = gimic.IsNone;
        if (func(nextPlayerPositionOnTile + Vector3Int.down)) {
            nextPlayerPositionOnTile = currentPlayerPositionOnTile;
            return;
        }

        //クリア処理
        func = gimic.Goal;
        if (func(nextPlayerPositionOnTile)) Goal();

        //通常の移動
        player.Anim.SetTrigger("Walk");

        //巻き戻しリストにADD
        AddListsAll(nextPlayerPositionOnTile, currentPlayerPositionOnTile + Vector3Int.down);

        //FALLOBJの処理
        FallProcess(nextPlayerPositionOnTile + Vector3Int.down);

        //プレイヤーの処理
        PlayerProcess(nextPlayerPositionOnTile);
    }

    #region GOAL関連

    // 通常モードのゴール
    private void Goal()
    {
        goal = true;
        Sounds.instance.se[7].Play();
        Sounds.instance.bgm[0].Stop();
        stage.fireWork.SetActive(true);
        
        player.key.SetActive(true);
        stage.key.SetActive(false);
        player.Anim.SetBool("Goal", gimic.Goal(nextPlayerPositionOnTile));

        if (SelectMode() == "EDIT") {
            EditGoal();
            return;
        }
        if (SelectMode() == "ONLINE") {
            OnlineGoal();
            return;
        }
        StoryGoal();
    }

    // ストーリーモードのゴール処理
    private void StoryGoal()
    {
        //save.ClearStage(stage.stageNumber);
        mainUI.goalPanel.SetActive(true);
        stage.stageNumber++;
        player.vc.Priority = 10;
        textAnim.Initialize();
        textAnim.Play(2f);
    }

    // EDITモードのゴール処理
    private void EditGoal()
    {
        stage.fireWork.SetActive(true);
        mainUI.editClearPanel.SetActive(true);
    }

    // ONLINEモードのゴール処理
    private void OnlineGoal()
    {
        stage.fireWork.SetActive(true);
        mainUI.onlinegoalPanel.SetActive(true);
        player.vc.Priority = 10;
        textAnimonline.Initialize();
        textAnimonline.Play(2f);
    }
    #endregion

    // PLAYERの初期化処理
    private void PlayerProcess(Vector3Int nextPos)
    {
        gimic.UpdateTileTableForPlayer(currentPlayerPositionOnTile, nextPos);
        player.Move(nextPos);
        stage.moveObjPositionOnTile[player.gameObject] = nextPos;
    }

    // BLOCKを落とす処理
    private void BlockFallDown(Vector3Int nextBlockPositionOnTile)
    {
        int confirmCount = stage.PlayerHierarchy + 1;

        for (int i = 0; i < confirmCount; i++)
        {
            // BLOCKが落とせるか判別
            if (!gimic.BlockDownPos(nextBlockPositionOnTile - (new Vector3Int(0,i,0)))) continue;

            for (int j = nextBlockPositionOnTile.y; j >= (gimic.Pos + Vector3Int.up).y; j--) {
                gimic.GetItem(new Vector3Int(nextBlockPositionOnTile.x,j,nextBlockPositionOnTile.z));
            }

            //巻き戻しリストにADD
            AddListsAll(nextPlayerPositionOnTile, currentPlayerPositionOnTile + Vector3Int.down);

            //ブロックの巻き戻しリストにADD
            nextBlockPos.Add(gimic.Pos + Vector3Int.up);

            //FALLOBJの処理
            FallProcess(nextPlayerPositionOnTile + Vector3Int.down);

            // BLOCKを落とす
            gimic.BlockDown(nextPlayerPositionOnTile,nextBlockPositionOnTile,gimic.Pos + Vector3Int.up);

            // PLAYERを更新
            PlayerProcess(nextPlayerPositionOnTile);
                    
            player.Anim.SetTrigger("Push");
            Sounds.instance.se[1].Play();
            return;
        }
        Sounds.instance.se[10].Play();
    }
    
    #region FALL関連
    // FALLの処理
    private void FallProcess(Vector3Int downPos)
    {
        // 落とす
        FallObj();
        // 準備
        gimic.ReadyFallObj(downPos);
    }

    //フォールオブジェを落とす処理
    public Material transparentMaterial;
    public Material opaqueMaterial;
    private void FallObj()
    {
        //CURRENT + DOWN に FALLOBJ がなかったら RETURN
        if (gimic.fallOBj == null) return;

        //FALLOBJに乗っている状態に FALSE
        gimic.isRiding = false;

        // FALLOBJのあった場所はNONEにする
        Vector3Int fallPos = gimic.FallObjPos;
        stage.tileAll[fallPos.x,fallPos.y,fallPos.z] = gimic.none;
        
        // FALLOBJの位置を他のOBJECTと被らないようにDICに登録
        GameObject obj = gimic.fallOBj;
        
        // MATERIALをTRANSPARENTへ
        MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
        Material materialTrans = new Material(transparentMaterial);
        renderer.sharedMaterial = materialTrans;
        
        stage.moveObjPositionOnTile[obj] = new Vector3Int((int)obj.transform.position.x,0,(int)obj.transform.position.z);

        // 落とす
        Sequence sequence = DOTween.Sequence();
        sequence.Append(obj.transform.DOMoveY(0,0.5f))
                .Join(obj.GetComponentInChildren<MeshRenderer>().material.DOFade(0,0.5f));
        
        // 乗っている状態を解除
        gimic.fallOBj = null;
        Sounds.instance.se[8].Pause();
        Sounds.instance.se[9].Play();
    }
    #endregion

    #region LIST関連

    // ADDを一括
    private void AddListsAll(Vector3Int addPos, Vector3Int fallPos)
    {
        //巻き戻しリストにADD
        AddList(addPos);

        //フォールオブジェリストにADD
        gimic.GetFallObj(fallPos);
    }

    // BACK処理に必要なLISTに格納
    private void AddList(Vector3Int next)
    {
        oldHierarchy.Add(stage.PlayerHierarchy);
        oldPos.Add(currentPlayerPositionOnTile);
        nextPos.Add(next);
        oldTileType.Add(gimic.GetTileType(next));
        oldDownTileType.Add(gimic.GetTileType(currentPlayerPositionOnTile + Vector3Int.down));
        oldNextDownTileType.Add(gimic.GetTileType(nextPlayerPositionOnTile + Vector3Int.down));
    }
    #endregion

    // BACK処理
    private void GoBackOne()
    {
        // BACKできない場合
        if(oldPos.Count == 0) return;

        // 現在の位置を一個前の状態に更新
        currentPlayerPositionOnTile = oldPos[oldPos.Count-1];
        // 次の位置を一個前の状態に更新
        nextPlayerPositionOnTile = nextPos[nextPos.Count-1];
        //  ???
        gimic.FallObjPos = currentPlayerPositionOnTile + Vector3Int.down;
        
        // ひとつ前のNEXTがBLOCKの場合
        if (gimic.BackBlock(oldTileType[oldTileType.Count-1])) {

            // 現在のBLOCKの位置にNONE
            stage.tileAll[nextBlockPos[nextBlockPos.Count-1].x,nextBlockPos[nextBlockPos.Count-1].y,nextBlockPos[nextBlockPos.Count-1].z] = gimic.none;

            // plusYに現在のPLAYERの位置のY座標を入れる
            int plusY = nextPlayerPositionOnTile.y - nextBlockPos[nextBlockPos.Count-1].y;

            // BLOCKを移動
            gimic.OldObjs[gimic.OldObjs.Count-1].transform.DOPath(new Vector3[]{gimic.OldObjs[gimic.OldObjs.Count-1].transform.position + new Vector3Int(0,plusY,0),nextPlayerPositionOnTile},0.5f,PathType.Linear);

            // BLOCKのTILE情報を更新
            stage.moveObjPositionOnTile[gimic.OldObjs[gimic.OldObjs.Count-1]] = nextPlayerPositionOnTile;
            stage.tileAll[nextPlayerPositionOnTile.x,nextPlayerPositionOnTile.y,nextPlayerPositionOnTile.z] = gimic.block;

            // FALLOBJのBACK処理
            if (gimic.BackFallObj(oldDownTileType[oldDownTileType.Count-1], currentPlayerPositionOnTile)) {
                MeshRenderer renderer = gimic.OldDownObjs[gimic.OldDownObjs.Count-1].GetComponentInChildren<MeshRenderer>();
                Material materialTrans = new Material(opaqueMaterial);
                renderer.sharedMaterial = materialTrans;
                gimic.OldDownObjs.RemoveAt(gimic.OldDownObjs.Count-1);
            }
            
            gimic.OldObjs.RemoveAt(gimic.OldObjs.Count-1);
            nextBlockPos.RemoveAt(nextBlockPos.Count-1);
        }
        // それ以外の場合
        else {

            // FALLOBJのBACK処理
            if (gimic.BackFallObj(oldDownTileType[oldDownTileType.Count-1], currentPlayerPositionOnTile)) {
                MeshRenderer renderer = gimic.OldDownObjs[gimic.OldDownObjs.Count-1].GetComponentInChildren<MeshRenderer>();
                Material materialTrans = new Material(opaqueMaterial);
                renderer.sharedMaterial = materialTrans;
                gimic.OldDownObjs.RemoveAt(gimic.OldDownObjs.Count-1);
            }

            // NEXTにNONE
            stage.tileAll[nextPlayerPositionOnTile.x,nextPlayerPositionOnTile.y,nextPlayerPositionOnTile.z] = gimic.none;
        }

        // BACKした現在の位置にPLAYERを入れる
        stage.moveObjPositionOnTile[player.gameObject] = currentPlayerPositionOnTile;
        stage.tileAll[oldPos[oldPos.Count-1].x,oldPos[oldPos.Count-1].y,oldPos[oldPos.Count-1].z] = gimic.player;
        
        // PLAYER処理
        player.Move(currentPlayerPositionOnTile);

        // LIsTの一括削除
        stage.PlayerHierarchy = oldHierarchy[oldHierarchy.Count-1];
        gimic.RemoveList(new List<Vector3Int>[]{oldPos, nextPos}, new List<StageManager.TILE_TYPE>[]{oldTileType, oldDownTileType, oldNextDownTileType});
        oldHierarchy.RemoveAt(oldHierarchy.Count-1);

        Sounds.instance.se[12].Play();
    }
    
    // 移動方向をわかりやすく
    private Vector3Int GetNextPlayerPositionOnTile(Vector3Int currentPlayerPosition,DIRECTION direction)
    {
        switch(direction)
        {
            case DIRECTION.UP:
            return currentPlayerPosition + Vector3Int.forward;
            case DIRECTION.DOWN:
            return currentPlayerPosition + Vector3Int.back;
            case DIRECTION.LEFT:
            return currentPlayerPosition + Vector3Int.left;
            case DIRECTION.RIGHT:
            return currentPlayerPosition + Vector3Int.right;
            
        }
        return currentPlayerPosition;
    }
    
    private void PlayerOperation()
    {
        operation.PageUpdate();
        if (goal || operation.Operation) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (changeStage.Stage2D) {
                gimic.PutWall(nextPutPos2D);
            }
            else {
                gimic.PutWall(nextPutPos3D);
            }
        }

        // MOVE
        if (changeStage.CameraNumber == 0)
        {
            if(Input.GetKeyDown(KeyCode.W))
            {
                MoveTo(DIRECTION.UP);
                player.transform.DORotate(new Vector3(0,0,0),0.1f);
                Sounds.instance.se[0].Play();
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                MoveTo(DIRECTION.DOWN);
                player.transform.DORotate(new Vector3(0,180,0),0.1f);
                Sounds.instance.se[0].Play();
            }
            if(Input.GetKeyDown(KeyCode.A))
            {
                MoveTo(DIRECTION.LEFT);
                player.transform.DORotate(new Vector3(0,270,0),0.1f);
                Sounds.instance.se[0].Play();
            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                MoveTo(DIRECTION.RIGHT);
                player.transform.DORotate(new Vector3(0,90,0),0.1f);
                Sounds.instance.se[0].Play();
            }
        }
        if (changeStage.CameraNumber == 1)//right
        {
            if(Input.GetKeyDown(KeyCode.W))
            {
                MoveTo(DIRECTION.RIGHT);
                player.transform.DORotate(new Vector3(0,90,0),0.1f);
                Sounds.instance.se[0].Play();
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                MoveTo(DIRECTION.LEFT);
                player.transform.DORotate(new Vector3(0,270,0),0.1f);
                Sounds.instance.se[0].Play();
            }
            if(Input.GetKeyDown(KeyCode.A))
            {
                MoveTo(DIRECTION.UP);
                player.transform.DORotate(new Vector3(0,0,0),0.1f);
                Sounds.instance.se[0].Play();
            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                MoveTo(DIRECTION.DOWN);
                player.transform.DORotate(new Vector3(0,180,0),0.1f);
                Sounds.instance.se[0].Play();
            }
        }
        if (changeStage.CameraNumber == 2)//up
        {
            if(Input.GetKeyDown(KeyCode.W))
            {
                MoveTo(DIRECTION.DOWN);
                player.transform.DORotate(new Vector3(0,180,0),0.1f);
                Sounds.instance.se[0].Play();
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                MoveTo(DIRECTION.UP);
                player.transform.DORotate(new Vector3(0,0,0),0.1f);
                Sounds.instance.se[0].Play();
            }
            if(Input.GetKeyDown(KeyCode.A))
            {
                MoveTo(DIRECTION.RIGHT);
                player.transform.DORotate(new Vector3(0,90,0),0.1f);
                Sounds.instance.se[0].Play();
            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                MoveTo(DIRECTION.LEFT);
                player.transform.DORotate(new Vector3(0,270,0),0.1f);
                Sounds.instance.se[0].Play();
            }
        }
        if (changeStage.CameraNumber == 3)//right
        {
            if(Input.GetKeyDown(KeyCode.W))
            {
                MoveTo(DIRECTION.LEFT);
                player.transform.DORotate(new Vector3(0,270,0),0.1f);
                Sounds.instance.se[0].Play();
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                MoveTo(DIRECTION.RIGHT);
                player.transform.DORotate(new Vector3(0,90,0),0.1f);
                Sounds.instance.se[0].Play();
            }
            if(Input.GetKeyDown(KeyCode.A))
            {
                MoveTo(DIRECTION.DOWN);
                player.transform.DORotate(new Vector3(0,180,0),0.1f);
                Sounds.instance.se[0].Play();
            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                MoveTo(DIRECTION.UP);
                player.transform.DORotate(new Vector3(0,0,0),0.1f);
                Sounds.instance.se[0].Play();
            }
        }

        // カメラ
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            changeStage.ChangeCameraLeft();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            changeStage.ChangeCameraRight();
        }
        
        // 次元
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            changeStage.Change();
        }
        
        // 遷移
        if (Input.GetKeyDown(KeyCode.R))
        {
            Sounds.instance.bgm[0].DOFade(0,0.5f);
            FadeManager.Instance.LoadScene("MainScene",0.5f);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Sounds.instance.bgm[0].DOFade(0,1);
            if (SelectMode() == "EDIT") {
                FadeManager.Instance.LoadScene("EditScene",1f);
                return;
            }
            FadeManager.Instance.LoadScene("TitleScene",1f);
        }
        
        // その他
        if (Input.GetKeyDown(KeyCode.B))
        {
            GoBackOne();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            operation.Operation = true;
            operation.ShowOperationPanel();
        }
    }
    
    #region ステージ関連
    
    // 次のステージへ
    public void ShowStageNumber()
    {
        if(stage.stageNumber == 23)
        {
            mainUI.EndStage();
            return;
        }
        StartCoroutine(StageUpdate());
    }
    // ステージの更新
    private IEnumerator StageUpdate() //ステージの更新
    {
        mainUI.ShowBlackPanel();
        // 暗転時間1秒
        yield return new WaitForSeconds(1f);
        // ステージの初期化
        yield return new WaitUntil(() => StageInitialize());
        // MESHを受け取る
        changeStage.GetObj();
        goal = false;
    }
    private bool StageInitialize()
    {
        stage.DestroyStage();
        PlayerPrefs.SetInt("STAGENUMBER",stage.stageNumber);
        stage.LoadStageData();
        stage.CreateStage();
        mainUI.StageTextAnim(stage.stageNumber);
        player = stage.Player;
        return true;
    }
    public void ChangeScene(string name)
    {
        Sounds.instance.bgm[0].DOFade(0,1);
        FadeManager.Instance.LoadScene(name,1);
    }
    #endregion
}