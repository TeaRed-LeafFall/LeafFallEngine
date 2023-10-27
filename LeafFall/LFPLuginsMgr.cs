using LeafFallEngine.Configs;
using LeafFallEngine.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeafFallEngine;
/// <summary>
/// 插件数据信息
/// </summary>
/// <value>
/// 名称， 作者， 描述， 版本(参见<see cref="LFVersion"/>)
/// </value>
public class PluginMsg
{
    public string Name = "Unk.";
    public string Author = "Unk.";
    public string Desc = "Unk.";
    public LFVersion Version = new LFVersion();
}

/// <summary>
/// 插件接口
/// <remark>
/// <para>插件必须继承以下部分实现</para>
/// <para>必须返回的插件信息（验证身份）：<see cref="GetLFPluginMsg"/></para>
/// <para>插件初始化(不是插件加载时操作 <see cref="LFPluginOnload"/> )：<see cref="LFPluginInit"/></para>
/// </remark>
/// </summary>
public interface ILFPlugin : IDisposable
{
    /// <summary>
    /// 获取插件信息
    /// </summary>
    /// <returns></returns>
    public PluginMsg GetPluginMsg();
    /// <summary>
    /// 插件初始化
    /// </summary>
    public void PluginInit();
    /// <summary>
    /// 开放接口: 加载插件时操作(初始化之前的)
    /// </summary>
    public void PluginOnload() { }
    /// <summary>
    /// 开放接口: 所有插件加载完成之后的命令
    /// </summary>
    public void PluginsLoadDone() { }
}

/// <summary>
/// 插件管理器
/// </summary>
public static class PluginsMgr
{
    //不同系统换行符不同(获取系统换行符装换成字符串数据=不用每次动态获取)
    private static readonly string NewLine = Environment.NewLine;
    public static string auto_load_plugins_path = EngineData.Paths.Plugins;

    //剩下是代码主要部分
    /// <summary>
    /// 当前拥有的插件
    /// </summary>
    static List<ILFPlugin> _Plugins = new();
    /// <summary>
    /// 当前拥有的插件信息
    /// </summary>
    static List<PluginMsg> _PluginsInfo = new();

    /// <summary>
    /// 是否存在插件
    /// </summary>
    /// <returns>布尔值</returns>
    public static bool IsHavePlugins() => _Plugins.Count != 0 || _PluginsInfo.Count != 0;
    /// <summary>
    /// 自动加载plugins目录下插件
    /// </summary>
    public static bool LoadPluginsAuto()
    {
        Logger.WriteLine(StringMgr.GetString("PluginsMgr.LoadPluginsAuto"), LogLevel.Load);
        //获取程序当前目录
        var program_dic = Directory.GetCurrentDirectory();
        //处理子文件夹
        Logger.WriteLine(StringMgr.GetString("PluginsMgr.FindDir", auto_load_plugins_path), LogLevel.Warning);
        //加载/搜索插件
        TaskTimer SreachPluginsUseTime = new();
        string[] plugin_files = GetPluginPaths(auto_load_plugins_path);
#if !DEBUG
            //如果没有插件
            if (plugin_files.Length == 0) throw new FileNotFoundException(StringMgr.GetString("PluginsMgr.PluginsNotFound"));
#endif
        //添加插件
        foreach (string path in plugin_files)
        {
            Logger.WriteLine(StringMgr.GetString("PluginsMgr.CallLoad", path), LogLevel.Load);
            LoadPlugin(path);
        }


        Logger.WriteLine(StringMgr.GetString("PluginsMgr.UseTime", SreachPluginsUseTime.GetTime()));

        return true;
    }

    /// <summary>
    /// 获取所有插件文件路径，包括子目录
    /// </summary>
    /// <param name="directory">目录</param>
    /// <returns>所有插件的路径</returns>
    private static string[] GetPluginPaths(string directory)
    {
        List<string> filesList = new List<string>();

        string[] files = Directory.GetFiles(directory);
        foreach (string file in files)
        {
            if (file.ToLower().EndsWith(".dll"))
            {
                Logger.WriteLine(StringMgr.GetString("PluginsMgr.FindDLL", file));
                filesList.Add(file);
            }
        }

        string[] directories = Directory.GetDirectories(directory);
        foreach (string subDirectory in directories) filesList.AddRange(GetPluginPaths(subDirectory));

        return filesList.ToArray();
    }
    /// <summary>
    /// 获取已加载插件列表
    /// </summary>
    public static void GetPluginsList()
    {
        int pluginID = 0;
        var str = String.Empty;
        //已加载插件列表
        string pluginsmsg = $"{NewLine}======= 已加载插件列表 ======={NewLine}";

        foreach (PluginMsg msg in _PluginsInfo)
        {
            pluginID++;
            str += StringMgr.GetString("PluginsMgr.PluginMsg", pluginID, msg.Name, msg.Author, msg.Version.GetVersion(), msg.Desc);
        }
        pluginsmsg = $"{pluginsmsg}{str}{NewLine}======= 插件列表结束啦 ======={NewLine}";
        Logger.WriteLine($"{pluginsmsg}");
    }
    /// <summary>
    /// 初始化插件(调用所有插件的Init()函数)
    /// </summary>
    public static void RunPluginsInit()
    {
        TaskTimer tt = new();
        foreach (ILFPlugin instance in _Plugins)
        {
            instance.PluginInit(); Logger.WriteLine(StringMgr.GetString("PluginsMgr.plugin-init-success", instance.GetPluginMsg().Name), LogLevel.Load);
        }
        Logger.WriteLine(StringMgr.GetString("PluginsMgr.plugin-init-complete", _Plugins.Count, tt.GetTime()));
    }
    /// <summary>
    /// 运行加载完成之后的函数(对所有插件)
    /// </summary>
    public static void RunPluginLoadDone()
    {
        TaskTimer tt = new();
        foreach (ILFPlugin instance in _Plugins)
        {
            // 加载完成调用
            instance.PluginsLoadDone();
            Logger.WriteLine(StringMgr.GetString("PluginsMgr.WhoReadyDone", instance.GetPluginMsg().Name), LogLevel.Load);
        }
        Logger.WriteLine(StringMgr.GetString("PluginsMgr.ReadyPlugins", _Plugins.Count, tt.GetTime()));
    }

    /// <summary>
    /// 加载插件
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>加载是否成功</returns>
    /// <exception cref="FileLoadException">文件不存在</exception>
    /// <exception cref="Exception">插件加载异常</exception>
    public static bool LoadPlugin(string path)
    {
        TaskTimer tt = new();

        Logger.WriteLine(StringMgr.GetString("PluginsMgr.loading-plugin", path), LogLevel.Load);

        // 路径是否存在
        if (!Path.Exists(path)) Logger.WriteLine(StringMgr.GetString("problem.nonexistent-path", path), LogLevel.Error);

        // 获取程序集
        var fileData = File.ReadAllBytes(path);
        Assembly asm = Assembly.Load(fileData);
        var manifestModuleName = asm.ManifestModule.ScopeName;
        Logger.WriteLine(StringMgr.GetString("PluginsMgr.read-module-name", manifestModuleName));

        // 加载dll
        try
        {
            // 加载dll到程序集命名空间
            var classLibrayName = manifestModuleName.Remove(manifestModuleName.LastIndexOf("."), manifestModuleName.Length - manifestModuleName.LastIndexOf("."));
            var namespacename = "LFPluginsN";
            var needtype = namespacename + "." + classLibrayName;
            Type? type = asm.GetType(needtype);
            Logger.WriteLine(StringMgr.GetString("PluginsMgr.module-requirement-message", manifestModuleName, needtype), LogLevel.Warning);
            // 是否包含格式声明
            if (!typeof(ILFPlugin).IsAssignableFrom(type))
            {
                var e = StringMgr.GetString("PluginsMgr.invalid-plugin-message", manifestModuleName, needtype);
                Logger.WriteLine(e, LogLevel.Error);
                throw new FileLoadException(e);
            }
            Logger.WriteLine(StringMgr.GetString("PluginsMgr.already-declared-message", manifestModuleName, needtype), LogLevel.Load);
            Logger.WriteLine(StringMgr.GetString("PluginsMgr.instantiating-message", manifestModuleName), LogLevel.Load);

            // dll实例化
            var instance = Activator.CreateInstance(type) as ILFPlugin;

            // 获取插件信息
            var Plugin = instance!.GetPluginMsg();

            // 判断是否重复
            bool isDuplicate = _PluginsInfo.Any(p => p.Name == Plugin.Name);
            string errorMessage = StringMgr.GetString("PluginsMgr.duplicate-plugin-name-message", manifestModuleName, Plugin.Name);
            if (isDuplicate) throw new Exception(errorMessage);

            // 插件加载时需要干什么
            instance.PluginOnload();

            // 插件加载后事
            Logger.WriteLine(StringMgr.GetString("PluginsMgr.plugin-info-message", manifestModuleName, Plugin.Name, Plugin.Version.GetVersion(), Plugin.Author, Plugin.Desc));

            // 插件列表加入
            _Plugins.Add(instance);
            _PluginsInfo.Add(Plugin);

            // 完成
            Logger.WriteLine(StringMgr.GetString("PluginsMgr.add-module-success-message", manifestModuleName), LogLevel.Load);
        }
        catch (Exception ex)
        {
            //这个错误在发布时应该报错：（不应该加载无用的其它插件），调试时这个错误是可以忽略的
#if DEBUG
            Logger.WriteLine(StringMgr.GetString("problem.debug-mode-failure-message", ex.Message), LogLevel.Debug);
#endif
#if RELEASE
                throw new Exception(StringMgr.GetString("PluginsMgr.failure-message",path,ex));
#endif
        }

        Logger.WriteLine(StringMgr.GetString("PluginsMgr.load-module-time-message", manifestModuleName, tt.GetTime()), 0);

        return true;
    }

    /// <summary>
    /// 所有插件加载完成启用
    /// </summary>
    /// <returns>是否成功</returns>
    public static bool PluginLoadDone()
    {
        TaskTimer tt = new();
        GetPluginsList();
        RunPluginsInit();
        RunPluginLoadDone();

        Logger.WriteLine(StringMgr.GetString("PluginsMgr.startup-plugin-time-message", tt.GetTime()));
        return true;
    }
    /// <summary>
    /// 卸载插件
    /// </summary>
    /// <returns>是否成功</returns>
    public static bool UnloadPlugins()
    {
        Logger.WriteLine(StringMgr.GetString("PluginsMgr.unload-plugins"), LogLevel.Warning);

        _Plugins.ForEach(PluginI => PluginI.Dispose());
        _Plugins.Clear();
        return true;
    }

}

/// <summary>
/// 版本数据
/// </summary>
/// <remarks>
/// <para>Vtype 整数类型 0~6</para>
/// <para>0 alpha：内部版本</para>
/// <para>1 beta：测试版</para>
/// <para>2 demo：演示版</para>
/// <para>4 rc：即将作为正式版发布</para>
/// <para>5 release：发行版</para>
/// <para>6 full version：完整版，即正式版</para>
/// </remarks>
/// <value>
/// Major, Minor, Build, Revision, Vtype
/// </value>
public class VersionData
{
    public int Major = 0;
    public int Minor = 0;
    public int Build = 0;
    public int Revision = 1;
    public int Vtype = 0;
}
/// <summary>
/// 版本数据结构体
/// </summary>
/// <value>
/// 参见<see cref="VersionData"/>
/// </value>
public class LFVersion
{
    /// <summary>
    /// 创建数据
    /// </summary>
    VersionData v = new VersionData();
    /// <summary>
    /// 设置版本号
    /// </summary>
    /// <value>
    /// 主版本号, 次版本号, 构建版本号, 修改号, 版本类型
    /// </value>
    /// <param name="major">主版本号</param>
    /// <param name="minor">次版本号</param>
    /// <param name="build">构建版本号</param>
    /// <param name="revision">修改号</param>
    /// <param name="vtype">版本类型</param>
    /// <returns name="PeVersionData">版本数据</returns>
    /// <exception cref="Exception">版本数据不符合格式</exception>
    public LFVersion(int major = 0, int minor = 0, int build = 0, int revision = 1, int vtype = 0)
    {
        if (major + minor + build + revision <= 0 || vtype > 6 || vtype < 0)
        {
            throw new Exception("版本号格式错误");
        }
        v.Major = major;
        v.Minor = minor;
        v.Build = build;
        v.Revision = revision;
        v.Vtype = vtype;
    }

    public string GetVersion()
    {
        string str = $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
        return str;
    }
}