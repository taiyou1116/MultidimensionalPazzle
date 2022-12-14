using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.IO;
public class PlayerEdit : MonoBehaviour
{
    [SerializeField] EditOperate edit;
    [SerializeField] EditUI ui;
    private EditSound sound;
    [SerializeField] GameObject currentBlock;
    [SerializeField] GameObject[] frames;
    public GameObject[] prefabs;
    [SerializeField] GameObject[] itemImage;

    Vector3Int currentPosition = new Vector3Int(0, 0, 0);
    private int layerCount = 0;
    private int itemCount = 2;
    private int playerCount;
    private int goalCount;
    public Dictionary<GameObject, Vector3Int> objDic = new Dictionary<GameObject, Vector3Int>();
    enum DIRECTION
    {
        UP,DOWN,LEFT,RIGHT
    }

    void Start()
    {
        sound = edit.sound;
    }
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
    private void MoveToNext(DIRECTION direction)
    {
        Vector3Int nextPosition = GetNextPlayerPositionOnTile(currentPosition,direction);
        if (nextPosition.x == 9 || nextPosition.x == -1 || nextPosition.z == 9 || nextPosition.z == -1) {
            return;
        }
        currentBlock.transform.position = nextPosition;
        currentPosition = nextPosition;
        sound.editAudios[0].Play();
    }
    private void UpLayer()
    {
        if(layerCount == 4) return;
        layerCount++;
        foreach(var value in frames) value.SetActive(false);
        frames[layerCount].SetActive(true);
        currentBlock.transform.position = currentPosition + Vector3Int.up;
        currentPosition = currentPosition + Vector3Int.up;
        sound.editAudios[2].Play();
    }
    private void DownLayer()
    {
        if(layerCount == 0) return;
        layerCount--;
        foreach(var value in frames) value.SetActive(false);
        frames[layerCount].SetActive(true);
        currentBlock.transform.position = currentPosition + Vector3Int.down;
        currentPosition = currentPosition + Vector3Int.down;
        sound.editAudios[2].Play();
    }
    private void SetPrefab()
    {
        if(GetBlockObjAt(currentPosition) != null) return;
        GameObject prefab = Instantiate(prefabs[itemCount]);
        prefab.transform.position = currentPosition;
        objDic.Add(prefab,currentPosition);
        sound.editAudios[1].Play();
    }
    private void RemoveObj()
    {
        if(GetBlockObjAt(currentPosition) == null) return;
        GameObject removeObj = GetBlockObjAt(currentPosition);
        objDic.Remove(removeObj);
        Destroy(removeObj);
        sound.editAudios[8].Play();
    }
    private GameObject GetBlockObjAt(Vector3Int position)
    {
        foreach(var pair in objDic)
        {
            if(pair.Value == position)
            {
                return pair.Key;
            }
        }
        return null;
    }
    
    void Update()//??????????????????????????????
    {
        // ??????
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (edit.cameraNumber == 0) {
                MoveToNext(DIRECTION.UP);
            }
            if (edit.cameraNumber == 1) {
                MoveToNext(DIRECTION.LEFT);
            }
            if (edit.cameraNumber == 2) {
                MoveToNext(DIRECTION.DOWN);
            }
            if (edit.cameraNumber == 3) {
                MoveToNext(DIRECTION.RIGHT);
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (edit.cameraNumber == 0) {
                MoveToNext(DIRECTION.LEFT);
            }
            if (edit.cameraNumber == 1) {
                MoveToNext(DIRECTION.DOWN);
            }
            if (edit.cameraNumber == 2) {
                MoveToNext(DIRECTION.RIGHT);
            }
            if (edit.cameraNumber == 3) {
                MoveToNext(DIRECTION.UP);
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (edit.cameraNumber == 0) {
                MoveToNext(DIRECTION.DOWN);
            }
            if (edit.cameraNumber == 1) {
                MoveToNext(DIRECTION.RIGHT);
            }
            if (edit.cameraNumber == 2) {
                MoveToNext(DIRECTION.UP);
            }
            if (edit.cameraNumber == 3) {
                MoveToNext(DIRECTION.LEFT);
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (edit.cameraNumber == 0) {
                MoveToNext(DIRECTION.RIGHT);
            }
            if (edit.cameraNumber == 1) {
                MoveToNext(DIRECTION.UP);
            }
            if (edit.cameraNumber == 2) {
                MoveToNext(DIRECTION.LEFT);
            }
            if (edit.cameraNumber == 3) {
                MoveToNext(DIRECTION.DOWN);
            }
        }
        
        // ????????????
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            UpLayer();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DownLayer();
        }

        // ????????????
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            itemCount = 0;
            SetItem(itemCount);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            itemCount = 1;
            SetItem(itemCount);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            itemCount = 2;
            SetItem(itemCount);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            itemCount = 3;
            SetItem(itemCount);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            itemCount = 4;
            SetItem(itemCount);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            itemCount = 5;
            SetItem(itemCount);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            itemCount = 6;
            SetItem(itemCount);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            itemCount = 7;
            SetItem(itemCount);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            itemCount = 8;
            SetItem(itemCount);
        }

        // ????????????
        if (Input.GetMouseButtonDown(0))
        {
            SetPrefab();
        }
        if (Input.GetMouseButtonDown(1))
        {
            RemoveObj();
        }

        // ?????????
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            edit.ChangeCameraRight();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            edit.ChangeCameraLeft();
        }

        // ??????
        if (Input.GetKeyDown(KeyCode.H))
        {
            sound.bgm.DOFade(0,1);
            FadeManager.Instance.LoadScene("TitleScene", 1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            sound.bgm.DOFade(0,0.5f);
            FadeManager.Instance.LoadScene("EditScene", 0.5f);
        }
        
        // ?????????
        if (Input.GetKeyDown(KeyCode.Space))
        {
            edit.Change();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            string data = WriteInStageData();
            if (playerCount != 1 || goalCount != 1) {
                ui.objErrorPanel.SetActive(true);
                sound.editAudios[7].Play();
                return;
            } 
            ReadInStageData(data);
            if (PlayerPrefs.GetString("MODE") != "REEDIT") {
                PlayerPrefs.SetString("MODE", "EDIT");
            }
            MainForOnline.Instance.stageData = data;
            FadeManager.Instance.LoadScene("MainScene",1);
        }
    }

    private void SetItem(int iCount)
    {
        foreach(var value in itemImage) {
            value.transform.DOLocalMoveX(10,0.3f);
        }
        itemImage[iCount].transform.DOLocalMoveX(-10,0.3f);
        sound.editAudios[3].Play();
    }

    public string WriteInStageData()
    {
        playerCount = 0;
        goalCount = 0;
        string rows = "";
        for (int high = 0; high < 6; high++)
        {
            for (int culm = 0; culm < 9; culm++)
            {
                for (int row = 0; row < 9; row++)
                {
                    GameObject obj = GetBlockObjAt(new Vector3Int(row, high, culm));
                    
                    if (obj == null) {
                        rows += "2";
                        continue;
                    }

                    switch (obj.tag) {
                        case "box":
                            rows += "1";
                        break;
                        case "cube":
                            rows += "0";
                        break;
                        case "playerObj":
                            rows += "3";
                            playerCount++;
                        break;
                        case "trap":
                            rows += "5";
                        break;
                        case "fall":
                            rows += "6";
                        break;
                        case "goal":
                            rows += "4";
                            goalCount++;
                        break;
                        case "pickaxe":
                            rows += "7";
                        break;
                        case "dont":
                                rows += "8";
                        break;
                        case "glass":
                            rows += "9";
                        break;
                    }
                }
            }
        }
        return rows;
    }

    private void ReadInStageData(string stageData)
    {
        string path = Application.dataPath + "/test.txt";
        StreamWriter sw = new StreamWriter(path,false);
        string rows = "";
        int count = 0;
        int layerCount = 0;
        int fiishCount = 0;

        for(int i = 0; i < stageData.Length; i++)
        {
            rows += stageData[i] + ",";
            count++;
            if (count == 9 && layerCount == 8 && fiishCount == 5)
            {
                string result = rows.Substring(0, rows.Length - 1);
                sw.Write(result + "\n");
            }
            else if (layerCount == 8 && count == 9) //i % 8 == 0 && i != 0
            {
                string result = rows.Substring(0, rows.Length - 1) + ".";
                sw.Write(result + "\n");// ??????????????????????????????????????????
                rows = "";

                count = 0;
                layerCount = 0;
                fiishCount++;
            }
            else if (count == 9)
            {
                string result = rows.Substring(0, rows.Length - 1);
                sw.WriteLine(result);// ??????????????????????????????????????????
                rows = "";

                count = 0;
                layerCount++;
            }
        }
        sw.Close();
    }
}
