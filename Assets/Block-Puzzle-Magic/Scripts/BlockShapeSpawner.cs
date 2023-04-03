using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using UnityEngine.EventSystems;

#if HBDOTween
using DG.Tweening;
#endif

[System.Serializable]
public class FinalClass_Block_Shape
{
    public List<Block_Shape> block_Shapes = new List<Block_Shape>();
}
[System.Serializable]
public class Block_Shape
{
    public List<Block_Shape_subclass> block_Shape_subclass = new List<Block_Shape_subclass>();
    public List<string> BlockNameForRotate = new List<string>();
}
[System.Serializable]
public class Block_Shape_subclass
{
    public List<string> BlockName = new List<string>();
}
public class BlockShapeSpawner : Singleton<BlockShapeSpawner>
{
    [Tooltip("Setting this true means placing a block will add new block instantly, false means new shape blocks will be added only once all three are placed on the board.")]
    public bool keepFilledAlways = false;

    [SerializeField] ShapeBlockList shapeBlockList;

    [HideInInspector] public ShapeBlockList ActiveShapeBlockModule;

    public Transform[] ShapeContainers;

    List<int> shapeBlockProbabilityPool;

    int shapeBlockPoolCount = 1;
    
    public FinalClass_Block_Shape finalClass_Block_Shape;        

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
        if (GameController.gameMode == GameMode.ADVANCE || GameController.gameMode == GameMode.CHALLENGE)
        {
        }
        else
        {
            ActiveShapeBlockModule = shapeBlockList;
        }

        //string json = JsonUtility.ToJson(finalClass_Block_Shape);        
        //File.WriteAllText("gamedata.json",json);        
        
    }

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start()
    {
        #region blast mode
        if (GameController.gameMode == GameMode.BLAST || GameController.gameMode == GameMode.CHALLENGE)
        {
            keepFilledAlways = true;
        }
        #endregion

        Invoke("SetupPreviousSessionShapes", 0.2F);
        Invoke("createShapeBlockProbabilityList", 0.5F);
        Invoke("FillShapeContainer", 0.5F);
    }

    /// <summary>
    /// Setups the previous session shapes.
    /// </summary>
    void SetupPreviousSessionShapes()
    {
        if (GameBoardGenerator.Instance.previousSessionData != null)
        {
            List<int> shapes = GameBoardGenerator.Instance.previousSessionData.shapeInfo.Split(',').Select(Int32.Parse).ToList();

            int shapeIndex = 0;
            foreach (int shapeID in shapes)
            {
                if (shapeID >= 0)
                {
                    CreateShapeWithID(ShapeContainers[shapeIndex], shapeID);
                }
                shapeIndex += 1;
            }
        }
    }

    /// <summary>
    /// Creates the shape block probability list.
    /// </summary>
    void createShapeBlockProbabilityList()
    {
        shapeBlockProbabilityPool = new List<int>();
        if (ActiveShapeBlockModule != null)
        {
            foreach (ShapeBlockSpawn shapeBlock in ActiveShapeBlockModule.ShapeBlocks)
            {
                AddShapeInProbabilityPool(shapeBlock.BlockID, shapeBlock.spawnProbability);
            }
        }
        shapeBlockProbabilityPool.Shuffle();
    }

    /// <summary>
    /// Adds the shape in probability pool.
    /// </summary>
    /// <param name="blockID">Block I.</param>
    /// <param name="probability">Probability.</param>
    void AddShapeInProbabilityPool(int blockID, int probability)
    {
        int probabiltyTimesToAdd = shapeBlockPoolCount * probability;

        for (int index = 0; index < probabiltyTimesToAdd; index++)
        {
            shapeBlockProbabilityPool.Add(blockID);
        }
    }

    /// <summary>
    /// Fills the shape container.
    /// </summary>
    public void FillShapeContainer()
    {
        ReorderShapes();

        if (!keepFilledAlways)
        {
            bool isAllEmpty = true;
            foreach (Transform shapeContainer in ShapeContainers)
            {
                if (shapeContainer.childCount > 0)
                {
                    isAllEmpty = false;
                }
            }

            if (isAllEmpty)
            {
                foreach (Transform shapeContainer in ShapeContainers)
                {
                    AddRandomShapeToContainer(shapeContainer);
                }
            }
        }
        else
        {
            foreach (Transform shapeContainer in ShapeContainers)
            {
                if (shapeContainer.childCount <= 0)
                {
                    AddRandomShapeToContainer(shapeContainer);
                }
            }
        }

        Invoke("CheckOnBoardShapeStatus", 0.2F);
    }

    /// <summary>
    /// Adds the random shape to container.
    /// </summary>
    /// <param name="shapeContainer">Shape container.</param>
    public void AddRandomShapeToContainer(Transform shapeContainer)
    {
        if (shapeBlockProbabilityPool == null || shapeBlockProbabilityPool.Count <= 0)
        {
            createShapeBlockProbabilityList();
        }

        int RandomShape = shapeBlockProbabilityPool[0];
        shapeBlockProbabilityPool.RemoveAt(0);

        GameObject newShapeBlock = ActiveShapeBlockModule.ShapeBlocks.Find(o => o.BlockID == RandomShape).shapeBlock;
        GameObject spawningShapeBlock = (GameObject)(Instantiate(newShapeBlock)) as GameObject;
        spawningShapeBlock.transform.SetParent(shapeContainer);
        spawningShapeBlock.transform.localScale = Vector3.one * 0.6F;
        spawningShapeBlock.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(800F, 0, 0);
#if HBDOTween
        spawningShapeBlock.transform.DOLocalMove(Vector3.zero, 0.3F);
#endif

    }

    /// <summary>
    /// Creates the shape with I.
    /// </summary>
    /// <param name="shapeContainer">Shape container.</param>
    /// <param name="shapeID">Shape I.</param>
    void CreateShapeWithID(Transform shapeContainer, int shapeID)
    {
        GameObject newShapeBlock = ActiveShapeBlockModule.ShapeBlocks.Find(o => o.BlockID == shapeID).shapeBlock;
        GameObject spawningShapeBlock = (GameObject)(Instantiate(newShapeBlock)) as GameObject;
        spawningShapeBlock.transform.SetParent(shapeContainer);
        spawningShapeBlock.transform.localScale = Vector3.one * 0.6F;
        spawningShapeBlock.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(800F, 0, 0);
#if HBDOTween
        spawningShapeBlock.transform.DOLocalMove(Vector3.zero, 0.3F);
#endif
    }
    public void CheckOnBoardShapeStatus()
    {
        List<ShapeInfo> OnBoardBlockShapes = new List<ShapeInfo>();
        foreach (Transform shapeContainer in ShapeContainers)
        {
            if (shapeContainer.childCount > 0)
            {
                OnBoardBlockShapes.Add(shapeContainer.GetChild(0).GetComponent<ShapeInfo>());
            }
        }

        bool canExistingBlocksPlaced = GamePlay.Instance.CanExistingBlocksPlaced(OnBoardBlockShapes);

        if (canExistingBlocksPlaced == false)
        {
            GamePlay.Instance.OnUnableToPlaceShape();
        }
    }
    void ReorderShapes()
    {
        List<Transform> EmptyShapes = new List<Transform>();

        foreach (Transform shapeContainer in ShapeContainers)
        {
            if (shapeContainer.childCount == 0)
            {
                EmptyShapes.Add(shapeContainer);
            }
            else
            {
                if (EmptyShapes.Count > 0)
                {
                    Transform emptyContainer = EmptyShapes[0];
                    shapeContainer.GetChild(0).SetParent(emptyContainer);
                    EmptyShapes.RemoveAt(0);
#if HBDOTween
                    emptyContainer.GetChild(0).DOLocalMove(Vector3.zero, 0.3F);
#endif
                    EmptyShapes.Add(shapeContainer);
                }
            }
        }
    }
    public string GetAllOnBoardShapeNames()
    {
        string shapeNames = "";
        foreach (Transform shapeContainer in ShapeContainers)
        {
            if (shapeContainer.childCount > 0)
            {
                shapeNames = shapeNames + shapeContainer.GetChild(0).GetComponent<ShapeInfo>().ShapeID + ",";
            }
            else
            {
                shapeNames = shapeNames + "-1,";
            }
        }

        shapeNames = shapeNames.Remove(shapeNames.Length - 1);
        return shapeNames;
    }

    public void AddEventTriggerCommponent()
    {
        for (int i = 0; i < ShapeContainers.Length; i++)
        {
            int currentIndex = i;
            EventTrigger eventTrigger = ShapeContainers[currentIndex].gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { Block_Change_Or_Rotate(currentIndex); }); // add the callback to the entry
            eventTrigger.triggers.Add(entry);
        }
    }
    public void Block_Change_Or_Rotate(int Index)
    {
        if (GameBoardGenerator.Instance.ChangeDragBlock && ShapeContainers[Index].childCount != 0)
        {
            GameBoardGenerator.Instance.ChangeDragBlock = false;
            GamePlayUI.Instance.FeaturesPopupSAnimator.Play("LeftSide");
            Destroy(ShapeContainers[Index].transform.GetChild(0).gameObject);
            int RandomShape = shapeBlockProbabilityPool[0];
            shapeBlockProbabilityPool.RemoveAt(0);
            GameObject newShapeBlock = ActiveShapeBlockModule.ShapeBlocks.Find(o => o.BlockID == RandomShape).shapeBlock;
            GameObject spawningShapeBlock = (GameObject)(Instantiate(newShapeBlock)) as GameObject;
            spawningShapeBlock.transform.SetParent(ShapeContainers[Index].transform);
            spawningShapeBlock.transform.localScale = Vector3.one * 0.6F;
            spawningShapeBlock.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(800F, 0, 0);
#if HBDOTween
            spawningShapeBlock.transform.DOLocalMove(Vector3.zero, 0.3F);
#endif
        }
        if (GameBoardGenerator.Instance.RotateDragBlock && ShapeContainers[Index].childCount != 0)
        {
            GamePlayUI.Instance.FeaturesPopupSAnimator.Play("LeftSide");
            Transform Rotate_Object = ShapeContainers[Index].transform.GetChild(0).transform;
            Rotate_Object.DORotate(new Vector3(0f, 0f, Rotate_Object.eulerAngles.z + 90f), .3f).SetEase(Ease.OutQuad)
                .OnComplete(() => { BlockShapePosSet(Rotate_Object); });
        }
        for (int i = 0; i < ShapeContainers.Length; i++)
        {
            int currentIndex = i;
            Destroy(ShapeContainers[currentIndex].GetComponent<EventTrigger>());
        }
    }

    void BlockShapePosSet(Transform Rotate_Object)
    {
        int Rotate_Index = 0;
        int Rotate_SubIndex = 0;
        int Rotate_Angel = 0;
        Rotate_Index = Rotate_Object.gameObject.GetComponent<ShapeInfo>().ShapeID;
        Rotate_Angel = Mathf.RoundToInt(Rotate_Object.eulerAngles.z);
        if (Rotate_Angel == 0) { Rotate_SubIndex = 0; }
        if (Rotate_Angel == 90) { Rotate_SubIndex = 1; }
        if (Rotate_Angel == 180) { Rotate_SubIndex = 2; }
        if (Rotate_Angel == 270) { Rotate_SubIndex = 3; }
        //print(Rotate_Angel +" = " + Rotate_Index + " = " + Rotate_SubIndex);               
        for (int i = 0; i < Rotate_Object.childCount; i++)
        {
            if(finalClass_Block_Shape.block_Shapes[Rotate_Index].block_Shape_subclass.Count != 0)
            Rotate_Object.GetChild(i).name = finalClass_Block_Shape.block_Shapes[Rotate_Index].block_Shape_subclass[Rotate_SubIndex].BlockName[i];
        }
        foreach (var item in Rotate_Object.GetComponent<ShapeInfo>().ShapeBlocks)
        {
            //print(item.block.gameObject.name + " :: " + finalClass_Block_Shape.block_Shapes[Rotate_Index].BlockNameForRotate[Rotate_SubIndex]);
            if (finalClass_Block_Shape.block_Shapes[Rotate_Index].BlockNameForRotate.Count != 0 &&
                finalClass_Block_Shape.block_Shapes[Rotate_Index].BlockNameForRotate[Rotate_SubIndex] != "" &&
                item.block.gameObject.name == finalClass_Block_Shape.block_Shapes[Rotate_Index].BlockNameForRotate[Rotate_SubIndex])
            {
                Rotate_Object.GetComponent<ShapeInfo>().firstBlock.block = item.block;
            }
            else if (finalClass_Block_Shape.block_Shapes[Rotate_Index].BlockNameForRotate.Count != 0 &&
              finalClass_Block_Shape.block_Shapes[Rotate_Index].BlockNameForRotate[Rotate_SubIndex] == "")
            {
                Rotate_Object.GetComponent<ShapeInfo>().firstBlock.block = Rotate_Object.transform;
            }
        }
        Rotate_Object.GetComponent<ShapeInfo>().CreateBlockList();
        GameBoardGenerator.Instance.RotateDragBlock = false;
    }
}

