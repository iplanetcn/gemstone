using UnityEngine;
/// <summary>
/// Gemstone信息
/// </summary>
public struct GemstoneInfo
{
    /// <summary>
    /// 位置
    /// </summary>
    public readonly Vector3 Position;
    /// <summary>
    /// 类型
    /// </summary>
    public readonly int GemstoneType;

    public GemstoneInfo(Vector3 position, int gemstoneType)
    {
        Position = position;
        GemstoneType = gemstoneType;
    }
}