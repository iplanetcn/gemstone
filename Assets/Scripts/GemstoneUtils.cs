using System.Collections.Generic;

public static class GemstoneUtils
{
    /// <summary>
    /// 过滤列表中小于position且与position的gemstoneType一致的数据
    /// </summary>
    /// <param name="src">源列表</param>
    /// <param name="dist">目标列表</param>
    /// <param name="position">位置</param>
    public static void AppendLess(IReadOnlyList<Gemstone> src, IList<Gemstone> dist, int position)
    {
        for (var i = position; i >= 0; i--)
        {
            // 如果为当前位置，则继续
            if (i == position)
            {
                continue;
            }

            // 将相邻未消除且相同类型的数据加入到列表，否则直接退出循环
            if (!src[i].isCrushed && src[i].gemstoneType == dist[0].gemstoneType)
            {
                dist.Add(src[i]);
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
    /// <param name="src">源列表</param>
    /// <param name="dist">目标列表</param>
    /// <param name="position">位置</param>
    public static void AppendMore(IReadOnlyList<Gemstone> src, IList<Gemstone> dist, int position)
    {
        for (var i = position; i < src.Count; i++)
        {
            // 如果为当前位置，则继续
            if (i == position)
            {
                continue;
            }

            // 将相邻未消除且相同类型的数据加入到列表，否则直接退出循环
            if (!src[i].isCrushed && src[i].gemstoneType == dist[0].gemstoneType)
            {
                dist.Add(src[i]);
            }
            else
            {
                break;
            }
        }
    }

    public static bool IsSame(Gemstone one, Gemstone another)
    {
        if (one is null || another is null)
        {
            return false;
        }

        return one.GetGuid().Equals(another.GetGuid());
    }
}