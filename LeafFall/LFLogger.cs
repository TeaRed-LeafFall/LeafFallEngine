using LeafFallEngine.Configs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeafFallEngine;
/// <summary>
/// 日志数据模型类
/// </summary>
/// <value>
/// 类型, 发送者, 内容
/// </value>
public class LogData
{
    public string Type = "[Null]";
    public string Header = "[unk]";
    public string Context = "";
}
/// <summary>
/// 日志级别
/// </summary>
public enum LogLevel
{
    Info = 1,
    Warning = 2,
    Error = 3,
    Fatal = 4,
    Load = 5,
    Debug = 6
}
/// <summary>
/// 日志记录器
/// </summary>
public static class Logger
{
    private static string logPath = ".\\logs\\";

    /// <summary>
    /// 日志记录数据列表
    /// </summary>
    public static List<LogData> LogRegMsg = new();
    /// <summary>
    /// 设置日志保存位置
    /// </summary>
    /// <param name="path">保存位置</param>
    public static void SetLogPath(string path)
    {
        logPath = path;
    }

    /// <summary>
    /// 输出日志（只有以行为单位的）
    /// </summary>
    /// <remarks>
    /// <para>输出日志信息(logtype)可以填的数字对应类型</para>
    /// 1:[info]
    /// 2:[warn]
    /// 3:[error]
    /// 4:[fatal]
    /// 5:[load]
    /// 6:[debug]
    /// <para>将会记录在 <see cref="LogRegMsg"/></para>
    /// </remarks>
    /// <value>
    /// 输出日志信息, 输出日志类型(可空)
    /// </value>
    /// <param name="message">输出日志信息</param>
    /// <param name="logtype">输出日志类型</param>
    public static void WriteLine(string message, LogLevel logtype = LogLevel.Info)
    {
        if (message == String.Empty) throw new Exception("输出日志内容不能为空！");
        /* ★★★★★ 任务准备 ★★★★★ */

        // 初始化需要的变量以及类
        string Outmessage;
        string Msgtype;
        LogData Data = new();

        /* ★★★★★ 任务开始 ★★★★★ */

        // 获取调用栈信息
        StackTrace trace = new();
        StackFrame frame = trace.GetFrame(1)!;
        MethodBase method = frame.GetMethod()!;
        // 获取调用类
        String CallclassName = method.ReflectedType!.Name!;
        // 获取调用方法
        String CallmethodName = method.Name!;
        // 获取调用命名空间
        String CallnameSpace = method.DeclaringType!.Namespace!;

        // 判断日志类型
        switch (logtype)
        {
            case LogLevel.Info: Msgtype = "[info]"; break;
            case LogLevel.Warning: Msgtype = "[warn]"; break;
            case LogLevel.Error: Msgtype = "[error]"; break;
            case LogLevel.Fatal: Msgtype = "[fatal]"; break;
            case LogLevel.Load: Msgtype = "[load]"; break;
            case LogLevel.Debug: Msgtype = "[debug]"; break;
            default: Msgtype = "[info]"; break;
        }

        // 日志数据填写
        Data.Type = Msgtype;
        Data.Header = $"[{CallnameSpace}][{CallclassName}][{CallmethodName}]";
        Data.Context = message;

        // 输出日志信息的准备
        Outmessage = Data.Type + Data.Header + Data.Context;

        /* ★★★★★ 任务结束 ★★★★★ */

        // 记录日志信息
        LogRegMsg.Add(Data);

        // 输出控制台日志
        Console.WriteLine(Outmessage);

        // 调试输出日志信息
        Debug.WriteLine(Outmessage);
    }

    /// <summary>
    /// 保存日志到指定目录
    /// </summary>
    /// <param name="path">目录</param>
    public static void SaveLogData()
    {
        WriteLine("正在保存日志哦...");
        string logfilename = Path.Combine(logPath, EngineData.File.Log);

        using StreamWriter sw = new(logfilename, true, Encoding.UTF8);
        WriteLine("日志保存完成啦.");
        foreach (LogData Data in LogRegMsg) 
        {
            // 保存日志信息的准备
            string logmessage = Data.Type + Data.Header + Data.Context;
            sw.WriteLine(logmessage);
        }
    }
    public static void DeleteLogBefore()
    {
        WriteLine("正在判断是否需要删除旧日志文件...");
        string[] filePathArr = Directory.GetFiles(logPath, "*.log", SearchOption.TopDirectoryOnly);
        if (filePathArr.Length > 5)
        {
            WriteLine("旧日志文件数量大于5个,满足清理条件");
            for (int filenumber = 0; filePathArr.Length > 5; filenumber++)
            {
                filePathArr = Directory.GetFiles(logPath, "*.log", SearchOption.TopDirectoryOnly);

                Dictionary<string, DateTime> fileCreateDate = new Dictionary<string, DateTime>();

                for (int i = 0; i < filePathArr.Length; i++)
                {
                    FileInfo fi = new FileInfo(filePathArr[i]);
                    fileCreateDate[filePathArr[i]] = fi.CreationTime;
                }
                fileCreateDate = fileCreateDate.OrderBy(f => f.Value).ToDictionary(f => f.Key, f => f.Value);
                foreach (KeyValuePair<string, DateTime> item in fileCreateDate)
                {
                    WriteLine("旧日志文件[日期]:" + item.Value + "[文件名]:" + item.Key, LogLevel.Warning);
                    File.Delete(fileCreateDate.First().Key);
                    WriteLine("已删除旧日志文件 [文件名]:" + item.Key, LogLevel.Warning);
                }
            }

            WriteLine("清理完成");
        }
        else
        {
            WriteLine("旧日志数量小于5个,不满足清理条件");
        }
    }
}