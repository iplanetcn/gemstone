using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public Gemstone gemstone;

    [SerializeField] public int rows;

    [SerializeField] public int columns;

    public List<List<Gemstone>> gemstoneList;

    public List<Sprite> spriteList;

    private Gemstone _currentGemstone;

    private Camera _camera;
    
    private Transform _spriteMaskTransform;

    private void Awake()
    {
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        _spriteMaskTransform = GameObject.FindWithTag("SpriteMask").GetComponent<Transform>();
    }

    private void Start()
    {
        // 设置camera
        // var offsetX = columns / 2f - 0.5f;
        // var offsetY = rows / 2f - 0.5f;
        // var cameraTransform = _camera.transform;
        // var oldCameraPosition = cameraTransform.position;
        // var newCameraPosition = new Vector3(oldCameraPosition.x + offsetX, oldCameraPosition.y + offsetY, oldCameraPosition.z);
        // cameraTransform.position = newCameraPosition;
        _camera.orthographicSize = columns < 6 ? 5 : columns + 1;
        _spriteMaskTransform.localScale = new Vector3(columns, rows, 1);

        // 设置gemstone
        gemstoneList = new List<List<Gemstone>>();

        for (var row = 0; row < rows; row++)
        {
            var temp = new List<Gemstone>();
            for (var column = 0; column < columns; column++)
            {
                var g = SpawnGemstone(row, column);
                temp.Add(g);
            }

            gemstoneList.Add(temp);
        }
    }

    /// <summary>
    /// 创建gemstone
    /// </summary>
    /// <param name="row">行</param>
    /// <param name="column">列</param>
    /// <returns></returns>
    private Gemstone SpawnGemstone(int row, int column)
    {
        var item = Instantiate(gemstone);
        item.rowIndex = row;
        item.columnIndex = column;
        item.gemstoneType = Random.Range(0, spriteList.Count);
        item.SetOffset(new Vector2(GetOffsetX(), GetOffsetY()));
        return item;
    }

    private float GetOffsetX()
    {
        return columns / 2f - 0.5f;
    }
    
    private float GetOffsetY()
    {
        return rows / 2f - 0.5f;
    }

   


    /// <summary>
    /// <para>1. 检查当前选择的gemstone是否为空</para>
    /// <para>2. gemstone为空，则保存当前gemstone，并修改isSelect为true</para>
    /// <para>3. gemstone为非空，则判断是否可以交换位置</para>
    /// <para>4. 如果可以交换，则交换gemstone的位置</para>
    /// <para>5. 判断消除逻辑，满足消除逻辑，则消除，否则还原之前的位置，并重置状态</para>
    /// </summary>
    /// <param name="g">被选gemstone</param>
    public void Select(Gemstone g)
    {
        if (_currentGemstone == null)
        {
            _currentGemstone = g;
            g.isSelect = true;
        }
        else
        {
            // 判断两个gemstone是否相邻
            if (Math.Abs(_currentGemstone.rowIndex - g.rowIndex) +
                Math.Abs(_currentGemstone.columnIndex - g.columnIndex) == 1)
            {
                ExchangeAndMatch(_currentGemstone, g);
            }

            _currentGemstone.isSelect = false;
            _currentGemstone = null;
        }
    }

    /// <summary>
    /// 交换gemstone位置并判断是否匹配
    /// </summary>
    /// <param name="g1">gemstone1</param>
    /// <param name="g2">gemstone2</param>
    private void ExchangeAndMatch(Gemstone g1, Gemstone g2)
    {
        Exchange(g1, g2);
        if (!Match(g1) && !Match(g2))
        {
            // 还原
            StartCoroutine(Restore(g1, g2));
        }
        else
        {
            //  产生新的gemstone，并播放位移动画
            StartCoroutine(SpawnNewSpriteForGemstones());
        }
    }

    private IEnumerator Restore(Gemstone g1, Gemstone g2)
    {
        yield return new WaitForSeconds(0.6f);
        Exchange(g1, g2);
    }

    /// <summary>
    /// 交换两个gemstone位置
    /// </summary>
    /// <param name="g1">gemstone1</param>
    /// <param name="g2">gemstone2</param>
    private void Exchange(Gemstone g1, Gemstone g2)
    {
        // 交换列表数据中的位置
        var tempG1ColumnIndex = g1.columnIndex;
        var tempG1RowIndex = g1.rowIndex;
        var tempG2ColumnIndex = g2.columnIndex;
        var tempG2RowIndex = g2.rowIndex;

        g1.columnIndex = tempG2ColumnIndex;
        g1.rowIndex = tempG2RowIndex;
        g2.columnIndex = tempG1ColumnIndex;
        g2.rowIndex = tempG1RowIndex;

        var temp = g1;
        gemstoneList[tempG1RowIndex][tempG1ColumnIndex] = g2;
        gemstoneList[tempG2RowIndex][tempG2ColumnIndex] = temp;
        // 更新视图中的位置
        g1.Move();
        g2.Move();
    }

    /// <summary>
    /// 在gemstone周围按照规则匹配
    /// </summary>
    /// <param name="g">指定gemstone</param>
    private bool Match(Gemstone g)
    {
        // 判断非空
        if (g == null || g.isCrushed)
        {
            return false;
        }

        var isMatch = false;

        // 判断横排
        var rowGemstones = gemstoneList[g.rowIndex];
        var tempHorizontal = new List<Gemstone> {g};
        // 左边
        GemstoneUtils.AppendLess(rowGemstones, tempHorizontal, g.columnIndex);
        // 右边
        GemstoneUtils.AppendMore(rowGemstones, tempHorizontal, g.columnIndex);

        // 判断竖排
        var columnGemstones = gemstoneList.Select(t => t[g.columnIndex]).ToList();
        var tempVertical = new List<Gemstone> {g};
        // 上边
        GemstoneUtils.AppendLess(columnGemstones, tempVertical, g.rowIndex);
        // 右边
        GemstoneUtils.AppendMore(columnGemstones, tempVertical, g.rowIndex);

        // 判断是否匹配三消规则
        if (tempHorizontal.Count >= 3)
        {
            tempHorizontal.ForEach(gt => gt.CrushGemstone());
            isMatch = true;
        }
        else if (tempVertical.Count >= 3)
        {
            tempVertical.ForEach(gv => gv.CrushGemstone());
            isMatch = true;
        }

        return isMatch;
    }

    /// <summary>
    /// 按照规则逐行匹配
    /// </summary>
    private void MatchAllRow()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 按照规则逐列匹配
    /// </summary>
    private void MatchAllColumn()
    {
        throw new NotImplementedException();
    }

    private IEnumerator SpawnNewSpriteForGemstones()
    {
        yield return new WaitForSeconds(0.55f);
        List<List<Gemstone>> shouldArrangeGemstones = new List<List<Gemstone>>();
        for (int i = 0; i < columns; i++)
        {
            bool needArrange = false;
            List<Gemstone> gemstones = gemstoneList.Select(t =>
            {
                if (t[i].isCrushed)
                {
                    needArrange = true;
                }
                return t[i];
            }).ToList();

            if (needArrange)
            {
                shouldArrangeGemstones.Add(gemstones);
            }
        }

        if (shouldArrangeGemstones.Count > 0)
        {
            int maxShift = 0;
            foreach (var colList in shouldArrangeGemstones)
            {
                List<GemstoneInfo> gemstoneInfos = new List<GemstoneInfo>();
                Vector3 p = colList[0].transform.position;
                // 提取同一列中未被消除的Gemstone信息
                foreach (Gemstone g in colList)
                {
                    if (!g.isCrushed)
                    {
                        gemstoneInfos.Add(new GemstoneInfo(g.transform.position, g.gemstoneType));
                    }
                }

                // 从顶端生成新的Gemstone信息
                var shiftCount = rows - gemstoneInfos.Count;
                for (int i = 0; i < shiftCount; i++)
                {
                    var position = new Vector3(p.x,  rows + i + 1 - GetOffsetY(), p.z);
                    gemstoneInfos.Add(new GemstoneInfo(position,Random.Range(0, spriteList.Count)));
                }

                // 更新列表信息及状态
                for (var i = 0; i < colList.Count; i++)
                {
                    colList[i].ActiveWithGemstoneInfo(gemstoneInfos[i]);
                }

                // 计算最大偏移量
                if (shiftCount > maxShift)
                {
                    maxShift = shiftCount;
                }
            }

            float waitSeconds = maxShift / Time.deltaTime;
            Debug.Log($"waitSeconds: {waitSeconds}");
        }
    }
}