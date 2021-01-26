using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour, ISelectListener
{
    public Gemstone gemstone;

    [SerializeField] public int rows;

    [SerializeField] public int columns;

    public List<List<Gemstone>> gemstoneList;

    public List<Sprite> spriteList;

    private Gemstone _currentGemstone;

    private Camera _camera;

    private void Awake()
    {
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Start()
    {
        // 设置camera
        var offsetX = columns / 2f - 0.5f;
        var offsetY = rows / 2f - 0.5f;
        var cameraTransform = _camera.transform;
        var oldCameraPosition = cameraTransform.position;
        var newCameraPosition =
            new Vector3(oldCameraPosition.x + offsetX, oldCameraPosition.y + offsetY, oldCameraPosition.z);
        cameraTransform.position = newCameraPosition;
        _camera.orthographicSize = columns < 6 ? 5 : columns + 1;

        // 设置gemstone
        gemstoneList = new List<List<Gemstone>>();

        for (var row = 0; row < rows; row++)
        {
            var temp = new List<Gemstone>();
            for (var column = 0; column < columns; column++)
            {
                var g = CreateGemstone(row, column);
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
    private Gemstone CreateGemstone(int row, int column)
    {
        var item = Instantiate(gemstone);
        item.rowIndex = row;
        item.columnIndex = column;
        item.SetSpriteList(spriteList);
        return item;
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
        Match(g1);
        Match(g2);
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
    private void Match(Gemstone g)
    {
        // 判断非空
        if (g == null)
        {
            return;
        }

        // 判断横排
        var rowGemstones = gemstoneList[g.rowIndex];
        var tempHorizontal = new List<Gemstone> {g};
        // 左边
        AppendLess(rowGemstones, tempHorizontal, g.columnIndex);
        // 右边
        AppendMore(rowGemstones, tempHorizontal, g.columnIndex);
        CheckAndCrush(tempHorizontal);

        // 判断竖排
        var columnGemstones = gemstoneList.Select(t => t[g.columnIndex]).ToList();
        var tempVertical = new List<Gemstone> {g};
        // 上边
        AppendLess(columnGemstones, tempVertical, g.rowIndex);
        // 右边
        AppendMore(columnGemstones, tempVertical, g.rowIndex);
        CheckAndCrush(tempVertical);
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

    /// <summary>
    /// 过滤列表中小于position且与position的gemstoneType一致的数据
    /// </summary>
    /// <param name="from">源列表</param>
    /// <param name="dist">目标列表</param>
    /// <param name="position">位置</param>
    private static void AppendLess(IReadOnlyList<Gemstone> from, IList<Gemstone> dist, int position)
    {
        for (var i = position; i > 0; i--)
        {
            // 如果为当前位置，则继续
            if (i == position)
            {
                continue;
            }

            // 将相邻且相同类型的数据加入到列表，否则直接退出循环
            if (from[i].GemstoneType() == dist[0].GemstoneType())
            {
                dist.Add(from[i]);
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 过滤列表中大于position且与position的gemstoneType一致的数据
    /// </summary>
    /// <param name="from">源列表</param>
    /// <param name="dist">目标列表</param>
    /// <param name="position">位置</param>
    private static void AppendMore(IReadOnlyList<Gemstone> from, IList<Gemstone> dist, int position)
    {
        for (var i = position; i < from.Count; i++)
        {
            // 如果为当前位置，则继续
            if (i == position)
            {
                continue;
            }

            // 将相邻且相同类型的数据加入到列表，否则直接退出循环
            if (from[i].GemstoneType() == dist[0].GemstoneType())
            {
                dist.Add(from[i]);
            }
            else
            {
                break;
            }
        }
    }

    private static void CheckAndCrush(IReadOnlyCollection<Gemstone> gl)
    {
        if (gl.Count < 3)
        {
            return;
        }

        foreach (var g in gl)
        {
            g.CrushGemstone();
        }
    }
}