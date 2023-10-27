using System.Diagnostics;

namespace LeafFallEngine.Helper;

/// <summary>
/// 继承<see cref="Stopwatch"/>实现计时器
/// </summary>
/// <remarks>
/// <para>特性1.声明就开始计时</para>
/// <para>特性2.GetTime获取字符串时间</para>
/// </remarks>
public class TaskTimer : Stopwatch
{
    public TaskTimer()
    {
        Start();
    }
    /// <summary>
    /// 获取字符串任务耗时时间(自动暂停计时器)
    /// </summary>
    /// <returns></returns>
    public string GetTime()
    {
        Stop();

        var ms = ElapsedMilliseconds;
        var s = Elapsed.TotalSeconds;
        var m = Elapsed.TotalMinutes;

        if (ms < 1000)
        {
            return ms.ToString() + "ms";
        }
        else
        {
            if (s < 60)
            {
                return s.ToString("F2") + "s";
            }
            else
            {
                return m.ToString("F2") + "m";
            }
        }
    }
}
