using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if HBDOTween
using DG.Tweening;
#endif

[System.Serializable]
public class BloxSixbySix
{
    public List<string> nameblock = new List<string>();
    public List<GameObject> nameblockGameObject = new List<GameObject>();
}
[System.Serializable]
public class Move_block_data
{
    public List<GameObject> LatestMove_blocks = new List<GameObject>();
}
public class GameBoardGenerator : Singleton<GameBoardGenerator>
{
    /// Total Rows, Configurable from inspector.
    public int TotalRows;

    /// Total Column Count, Configurable from inspector.
    public int TotalColumns;

    /// Space between each blocks, Configurable from inspector.
    public int blockSpace = 5;

    /// The content of the board.
    public GameObject BoardContent;

    /// The empty block template.
    public GameObject emptyBlockTemplate;

    int startPosx = 0;
    int startPosy = 0;

    int blockWidth = 56;
    int blockHeight = 56;

    int cellIndex = 0;

    public float BlockSize = 1F;

    public PreviousSessionData previousSessionData;

    public bool DestoryOneBlock = false;
    public bool ChangeDragBlock = false;
    public bool RotateDragBlock = false;

    public List<Move_block_data> move_Block_Data = new List<Move_block_data>();
    public List<Block> AllBlock = new List<Block>();
    public List<BloxSixbySix> bloxSixbySixes = new List<BloxSixbySix>();
    void Start()
    {
        ///checks if level needs to start from previos session or start new session.
       // previousSessionData = GetComponent<GameProgressManager>().GetPreviousSessionData();
        for (int i = 3; i < 7; i++)
        {
            UpdateFeaturesPenddingText(i);
        }
    }

    /// <summary>
    /// Generates the board.
    /// </summary>
    public void GenerateBoard()
    {
        if (previousSessionData != null)
        {
            if (previousSessionData.blockGridInfo.Count > 0)
            {
                TotalRows = previousSessionData.blockGridInfo.Count;
                TotalColumns = previousSessionData.blockGridInfo[0].Split(',').Length;
            }
        }

        // blockHeight = (int)emptyBlockTemplate.GetComponent<RectTransform>().sizeDelta.x;
        // blockWidth = (int)emptyBlockTemplate.GetComponent<RectTransform>().sizeDelta.y;

        blockHeight = (int)(blockHeight * BlockSize);
        blockWidth = (int)(blockWidth * BlockSize);

        startPosx = -(((TotalColumns - 1) * (blockHeight + blockSpace)) / 2);
        startPosy = (((TotalRows - 1) * (blockWidth + blockSpace)) / 2);

        int newPosX = startPosx;
        int newPosY = startPosy;

        for (int row = 0; row < TotalRows; row++)
        {
            List<Block> thisRowCells = new List<Block>();
            for (int column = 0; column < TotalColumns; column++)
            {

                GameObject newCell = GenerateNewBlock(row, column);
                newCell.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(newPosX, (newPosY), 0);
                newCell.GetComponent<BoxCollider2D>().size = new Vector2(blockWidth, blockHeight);
                newPosX += (blockWidth + blockSpace);
                Block thisCellInfo = newCell.GetComponent<Block>();
                thisCellInfo.blockImage = newCell.transform.GetChild(0).GetComponent<Image>();
                thisCellInfo.rowID = row;
                thisCellInfo.columnID = column;
                thisRowCells.Add(thisCellInfo);
                cellIndex++;
            }
            GamePlay.Instance.blockGrid.AddRange(thisRowCells);
            newPosX = startPosx;
            newPosY -= (blockHeight + blockSpace);
        }

        // SetupPreviousSessionBoard();
    }

    /// <summary>
    /// Setups the previous session board.
    /// </summary>
    void SetupPreviousSessionBoard()
    {
        if (previousSessionData != null)
        {
            #region normal blocks setup on board
            if (previousSessionData.blockGridInfo.Count > 0)
            {
                int rowIndex = 0;
                foreach (string gridRow in previousSessionData.blockGridInfo)
                {
                    int columnIndex = 0;
                    foreach (string blockID in gridRow.Split(','))
                    {
                        int thisBlockID = blockID.TryParseInt();
                        if (thisBlockID >= 0)
                        {
                            Block thisBlock = GamePlay.Instance.blockGrid.Find(o => o.rowID == rowIndex && o.columnID == columnIndex);
                            if (thisBlock != null)
                            {
                                thisBlock.ConvertToFilledBlock(thisBlockID);
                            }
                        }
                        columnIndex++;
                    }
                    rowIndex++;
                }
            }
            #endregion

            #region place previous session bombs
            if (GameController.gameMode == GameMode.BLAST || GameController.gameMode == GameMode.CHALLENGE)
            {
                if (previousSessionData.placedBombInfo != null)
                {
                    foreach (PlacedBomb bomb in previousSessionData.placedBombInfo)
                    {
                        Block thisBlock = GamePlay.Instance.blockGrid.Find(o => o.rowID == bomb.rowID && o.columnID == bomb.columnID);
                        thisBlock.ConvertToBomb(bomb.bombCounter);
                    }
                }
            }
            #endregion
            /*
                        #region set score
                        ScoreManager.Instance.AddScore(previousSessionData.score, false);
                        #endregion*/

            #region moves count
            GamePlay.Instance.MoveCount = previousSessionData.movesCount;
            #endregion

            #region set timer for time and challenge mode
            if (GameController.gameMode == GameMode.TIMED || GameController.gameMode == GameMode.CHALLENGE)
            {
                GamePlay.Instance.timeSlider.SetTime(previousSessionData.remainingTime);
            }
            #endregion
        }
        GetComponent<GameProgressManager>().ClearProgress();
    }

    /// <summary>
    /// Generates the new block.
    /// </summary>
    /// <returns>The new block.</returns>
    /// <param name="rowIndex">Row index.</param>
    /// <param name="columnIndex">Column index.</param>
    bool IsFirstBlock = false;
    GameObject GenerateNewBlock(int rowIndex, int columnIndex)
    {
        GameObject newBlock = (GameObject)Instantiate(emptyBlockTemplate);
        newBlock.GetComponent<RectTransform>().sizeDelta = new Vector2((blockWidth), (blockHeight));
        newBlock.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2((blockWidth), (blockHeight));
        newBlock.transform.SetParent(BoardContent.transform);
        newBlock.transform.localScale = Vector3.one;
        newBlock.transform.SetAsLastSibling();
        newBlock.name = "Block-" + rowIndex.ToString() + "-" + columnIndex.ToString();
        if (!IsFirstBlock)
        {
            if (DDOL.Theme == 0) { newBlock.GetComponent<Image>().sprite = GamePlayUI.Instance.Light_Theme[7]; }
            else { newBlock.GetComponent<Image>().sprite = GamePlayUI.Instance.Dark_Theme[7]; }
            IsFirstBlock = true;
        }
        else
        {
            if (DDOL.Theme == 0) { newBlock.GetComponent<Image>().sprite = GamePlayUI.Instance.Light_Theme[8]; }
            else { newBlock.GetComponent<Image>().sprite = GamePlayUI.Instance.Dark_Theme[8]; }
            IsFirstBlock = false;
        }
        AllBlock.Add(newBlock.GetComponent<Block>());
        Addnameintolist(rowIndex, columnIndex, newBlock);
        return newBlock;
    }
    public void FeaturesButtonClick(string Features)
    {
        switch (Features)
        {
            case "Undo":
                if (move_Block_Data.Count != 0 && DDOL.Undo != 0 && !DestoryOneBlock && !ChangeDragBlock && !RotateDragBlock)
                {
                    DDOL.Undo--;
                    UpdateFeaturesPenddingText(3);
                    if (move_Block_Data[move_Block_Data.Count - 1].LatestMove_blocks.Count == 0)
                    {
                        move_Block_Data.RemoveAt((move_Block_Data.Count - 1));
                    }
                    foreach (GameObject item in move_Block_Data[move_Block_Data.Count - 1].LatestMove_blocks)
                    {
                        item.GetComponent<Block>().ClearBlock();
                    }
                    if (move_Block_Data.Count != 0)
                        move_Block_Data.RemoveAt(move_Block_Data.Count - 1);
                }
                break;
            case "Destory":
                int Check = 0;
                foreach (var item in AllBlock)
                {
                    if (item.GetComponent<Block>().isFilled)
                    {
                        Check++;
                        if (Check == 1 && DDOL.destroy != 0 && !DestoryOneBlock && !ChangeDragBlock && !RotateDragBlock)
                        {
                            DDOL.destroy--;
                            UpdateFeaturesPenddingText(4);
                            DestoryOneBlock = true;
                            GamePlayUI.Instance.FeaturesPopupSAnimator.Play("RightSide");
                            GamePlayUI.Instance.FeaturesPopupSAnimator.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Destory";
                            GamePlayUI.Instance.FeaturesPopupSAnimator.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "Tap On Block Which One You Like To Destroy!";
                            return;
                        }
                    }
                }
                break;
            case "Rotate":
                if (DDOL.Rotate != 0 && !DestoryOneBlock && !ChangeDragBlock && !RotateDragBlock)
                {
                    DDOL.Rotate--;
                    UpdateFeaturesPenddingText(5);
                    GamePlayUI.Instance.FeaturesPopupSAnimator.Play("RightSide");
                    GamePlayUI.Instance.FeaturesPopupSAnimator.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Rotate";
                    GamePlayUI.Instance.FeaturesPopupSAnimator.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "Tap Drag Block Which One You Like To Rotate Their Position!";
                    RotateDragBlock = true;
                    BlockShapeSpawner.Instance.AddEventTriggerCommponent();
                }
                break;
            case "Change":
                if (DDOL.Change != 0 && !DestoryOneBlock && !ChangeDragBlock && !RotateDragBlock)
                {
                    DDOL.Change--;
                    UpdateFeaturesPenddingText(6);
                    GamePlayUI.Instance.FeaturesPopupSAnimator.Play("RightSide");
                    GamePlayUI.Instance.FeaturesPopupSAnimator.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Change";
                    GamePlayUI.Instance.FeaturesPopupSAnimator.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "Tap Drag Block Which One You Like To Change It!";
                    ChangeDragBlock = true;
                    BlockShapeSpawner.Instance.AddEventTriggerCommponent();
                }
                break;
        }
    }
    void UpdateFeaturesPenddingText(int Index)
    {
        int FinalPenddingvalue = 3;
        if (Index == 3) { FinalPenddingvalue = DDOL.Undo; }
        if (Index == 4) { FinalPenddingvalue = DDOL.destroy; }
        if (Index == 5) { FinalPenddingvalue = DDOL.Rotate; }
        if (Index == 6) { FinalPenddingvalue = DDOL.Change; }
        GamePlayUI.Instance.Change_Theme_Object[Index].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + FinalPenddingvalue;
    }
    public void DestoryOneBlock_feature(Block block)
    {
        if (DestoryOneBlock && block.isFilled)
        {
            DestoryOneBlock = false;
            GamePlayUI.Instance.FeaturesPopupSAnimator.Play("LeftSide");
            for (int i = 0; i < move_Block_Data.Count; i++)
            {
                for (int j = 0; j < move_Block_Data[i].LatestMove_blocks.Count; j++)
                {
                    if (move_Block_Data[i].LatestMove_blocks.Contains(block.gameObject))
                    {
                        move_Block_Data[i].LatestMove_blocks.Remove(block.gameObject);
                    }
                }
            }
            block.ClearBlock();
        }
    }
    void Addnameintolist(int rowIndex, int columnIndex, GameObject newBlock)
    {
        int First = 0; int Secound = 1; int Third = 2;
        int Fourth = 3; int Five = 4; int Six = 5;
        int Seven = 6; int Eight = 7; int Nine = 8;

        //First        
        if (rowIndex == 0 && columnIndex <= 2) { bloxSixbySixes[First].nameblock.Add(newBlock.name); }
        if (rowIndex == 1 && columnIndex <= 2) { bloxSixbySixes[First].nameblock.Add(newBlock.name); }
        if (rowIndex == 2 && columnIndex <= 2) { bloxSixbySixes[First].nameblock.Add(newBlock.name); }
        //Second
        if (rowIndex == 0 && columnIndex >= 3 && columnIndex <= 5) { bloxSixbySixes[Secound].nameblock.Add(newBlock.name); }
        if (rowIndex == 1 && columnIndex >= 3 && columnIndex <= 5) { bloxSixbySixes[Secound].nameblock.Add(newBlock.name); }
        if (rowIndex == 2 && columnIndex >= 3 && columnIndex <= 5) { bloxSixbySixes[Secound].nameblock.Add(newBlock.name); }
        //Third
        if (rowIndex == 0 && columnIndex >= 6 && columnIndex <= 8) { bloxSixbySixes[Third].nameblock.Add(newBlock.name); }
        if (rowIndex == 1 && columnIndex >= 6 && columnIndex <= 8) { bloxSixbySixes[Third].nameblock.Add(newBlock.name); }
        if (rowIndex == 2 && columnIndex >= 6 && columnIndex <= 8) { bloxSixbySixes[Third].nameblock.Add(newBlock.name); }


        //Fourth
        if (rowIndex == 3 && columnIndex <= 2) { bloxSixbySixes[Fourth].nameblock.Add(newBlock.name); }
        if (rowIndex == 4 && columnIndex <= 2) { bloxSixbySixes[Fourth].nameblock.Add(newBlock.name); }
        if (rowIndex == 5 && columnIndex <= 2) { bloxSixbySixes[Fourth].nameblock.Add(newBlock.name); }
        //Five
        if (rowIndex == 3 && columnIndex >= 3 && columnIndex <= 5) { bloxSixbySixes[Five].nameblock.Add(newBlock.name); }
        if (rowIndex == 4 && columnIndex >= 3 && columnIndex <= 5) { bloxSixbySixes[Five].nameblock.Add(newBlock.name); }
        if (rowIndex == 5 && columnIndex >= 3 && columnIndex <= 5) { bloxSixbySixes[Five].nameblock.Add(newBlock.name); }
        //Six
        if (rowIndex == 3 && columnIndex >= 6 && columnIndex <= 8) { bloxSixbySixes[Six].nameblock.Add(newBlock.name); }
        if (rowIndex == 4 && columnIndex >= 6 && columnIndex <= 8) { bloxSixbySixes[Six].nameblock.Add(newBlock.name); }
        if (rowIndex == 5 && columnIndex >= 6 && columnIndex <= 8) { bloxSixbySixes[Six].nameblock.Add(newBlock.name); }


        //Seven
        if (rowIndex == 6 && columnIndex <= 2) { bloxSixbySixes[Seven].nameblock.Add(newBlock.name); }
        if (rowIndex == 7 && columnIndex <= 2) { bloxSixbySixes[Seven].nameblock.Add(newBlock.name); }
        if (rowIndex == 8 && columnIndex <= 2) { bloxSixbySixes[Seven].nameblock.Add(newBlock.name); }
        //Eight
        if (rowIndex == 6 && columnIndex >= 3 && columnIndex <= 5) { bloxSixbySixes[Eight].nameblock.Add(newBlock.name); }
        if (rowIndex == 7 && columnIndex >= 3 && columnIndex <= 5) { bloxSixbySixes[Eight].nameblock.Add(newBlock.name); }
        if (rowIndex == 8 && columnIndex >= 3 && columnIndex <= 5) { bloxSixbySixes[Eight].nameblock.Add(newBlock.name); }
        //Nine
        if (rowIndex == 6 && columnIndex >= 6 && columnIndex <= 8) { bloxSixbySixes[Nine].nameblock.Add(newBlock.name); }
        if (rowIndex == 7 && columnIndex >= 6 && columnIndex <= 8) { bloxSixbySixes[Nine].nameblock.Add(newBlock.name); }
        if (rowIndex == 8 && columnIndex >= 6 && columnIndex <= 8) { bloxSixbySixes[Nine].nameblock.Add(newBlock.name); }

        //CheckAllandAdd
        CheckandAddObject(First, newBlock);
        CheckandAddObject(Secound, newBlock);
        CheckandAddObject(Third, newBlock);
        CheckandAddObject(Fourth, newBlock);
        CheckandAddObject(Five, newBlock);
        CheckandAddObject(Six, newBlock);
        CheckandAddObject(Seven, newBlock);
        CheckandAddObject(Eight, newBlock);
        CheckandAddObject(Nine, newBlock);
    }
    void CheckandAddObject(int Index, GameObject newBlock)
    {
        if (bloxSixbySixes[Index].nameblock.Contains(newBlock.name) && !bloxSixbySixes[Index].nameblockGameObject.Contains(newBlock)) { bloxSixbySixes[Index].nameblockGameObject.Add(newBlock); }
    }
    public void CheckBoxIsFilled()
    {
        for (int i = 0; i < 9; i++)
        {
            int Check = 0;
            foreach (var item in bloxSixbySixes[i].nameblockGameObject)
            {
                if (item.GetComponent<Block>().isFilled)
                {
                    Check++;
                    if (Check == 9)
                    {
                        DDOL.Instance.LineDestoryClick();
                        ScoreManager.Instance.AddScore(9 * 20);
                        foreach (GameObject b in bloxSixbySixes[i].nameblockGameObject)
                        {
                            b.GetComponent<Block>().ClearBlock();
                            for (int j = 0; j < move_Block_Data.Count; j++)
                            {
                                for (int k = 0; k < move_Block_Data[j].LatestMove_blocks.Count; k++)
                                {
                                    if (move_Block_Data[j].LatestMove_blocks[k].gameObject == b)
                                    {
                                        move_Block_Data[j].LatestMove_blocks.Remove(b);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}