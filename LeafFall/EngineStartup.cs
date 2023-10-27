using LeafFallEngine.Configs;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LeafFallEngine;

public static class LFEngine
{
    public static Mutex? mutex;
    public static string logPath = Path.Combine(EngineData.Program.programPath, EngineData.Paths.Logs);
    public static LFApp? LFApp;

    public static void EngineInit()
    {
        // 初始化路径
        DirectoryInit();
        // 加载英文文本（程序内）
        StringMgr.LoadData(Loader.GetInternalResourceLanguage());
        StringMgr.LoadData(Encoding.UTF8.GetString(Resources.zh_cn));

        // 初始化
        DateTime beginTime = DateTime.Now;
        Logger.SetLogPath(logPath);
        Logger.DeleteLogBefore();
        // 启用视觉样式（否则会出现奇怪的问题还有老旧的win32界面）
        Application.EnableVisualStyles();
        // 加载外部json语言
        //StringMgr.LoadFile(EngineData.Program.programPath + "\\res\\cute_catgirl.json");
        // 日志记录器输出已完成操作的日志
        Logger.WriteLine(StringMgr.GetString("System.logger"), LogLevel.Debug);
        Logger.WriteLine(StringMgr.GetString("msg.language-done"));
    }

    private static void DirectoryInit()
    {
        Directory.CreateDirectory(EngineData.Paths.Logs);
        Directory.CreateDirectory(EngineData.Paths.Plugins);
        Directory.CreateDirectory(EngineData.Paths.Crash);
    }

    public static void Startup(StartupMessage sm, string[] CommandLine)
    {
        string MutexID = $"{sm.Author}_{sm.AppName}";

        Logger.WriteLine($"======= {MutexID} =======");

        mutex = new Mutex(true, MutexID);
        if (!mutex.WaitOne(TimeSpan.Zero, true))
        {
            Logger.WriteLine($"{MutexID} :The program is already running!", LogLevel.Error);
            return;
        }

        Logger.WriteLine(StringMgr.GetString("System.startup"), LogLevel.Load);

        switch (ParseCommandLine(CommandLine))
        {
            case 0:
                break;
            case 1:
                Environment.Exit(0);
                break;

        }

        try
        {
            EngineMain();
            //Stop();
        }
        catch (Exception e)
        {
            Logger.WriteLine(StringMgr.GetString("msg.crash", Environment.NewLine + e.Message), LogLevel.Fatal);
            // 运行结束(崩溃下)
            Logger.WriteLine(StringMgr.GetString("msg.stop"), LogLevel.Fatal);
            Logger.SetLogPath(EngineData.Paths.Crash);
            Stop();

            throw new Exception(e.Message);
        }
    }

    public static int ParseCommandLine(string[] CommandLine)
    {
        if (CommandLine == null)
        {
            return -1;
        }

        foreach (string arg in CommandLine)
        {
            Logger.WriteLine($"解析到参数{arg}");
            switch (arg)
            {
                case "--openlogpath":
                    Process.Start("explorer.exe", EngineData.Paths.Logs);
                    return 1;
            }
        }
        return 0;
    }

    /// <summary>
    /// 主函数
    /// </summary>
    /// <exception cref="FileLoadException">插件加载失败</exception>
    private static void EngineMain()
    {
        Logger.WriteLine(StringMgr.GetString("msg.about"));
        
        try
        {
            PluginsMgr.LoadPluginsAuto();
        }
        catch (Exception e)
        {
            Logger.WriteLine(e.Message, LogLevel.Fatal);
            throw new FileLoadException(e.Message);
        }

        // 插件加载完成的重新调用所有插件的事件处理
        if (PluginsMgr.IsHavePlugins())
        {
            PluginsMgr.PluginLoadDone();
        };
    }

    /// <summary>
    /// 结束
    /// </summary>
    public static void Stop()
    {
        PluginsMgr.UnloadPlugins();
        Logger.WriteLine(StringMgr.GetString("System.logger-save"), LogLevel.Info);


        // 必须提前创建目录，不然报错
        if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

        Logger.WriteLine(StringMgr.GetString("System.save-log-path", logPath), LogLevel.Load);
        Logger.SaveLogData();

        mutex?.Dispose();

        // 运行结束
        Logger.WriteLine(StringMgr.GetString("msg.stop"), LogLevel.Load);
    }
}


public class StartupMessage
{
    public string AppName=EngineData.Program.Name;
    public string Author=EngineData.Program.Author;
}