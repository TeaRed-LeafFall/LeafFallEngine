using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeafFallEngine.Configs;

public static class EngineData
{
    public static string Engine { get; } = "LeafFall Engine";
    public static string Version { get; } = "0.0.2.0";
    public static string Author { get; } = "TeaRed LeafFall";
    public static string Copyright { get; } = "LeafFall Engine (c) 2023 TeaRed LeafFall&ADVN Project.";

    public static class Program
    {
        public static string Name { get; } = "LeafFall Engine Application";
        public static string Author { get; } = "TeaRed LeafFall";
        public static string MainEncode { get; } = "UTF-8";
        public static string BaseLanguage { get; } = "zh-CN";
        public static string[] Runtime { get; } = { ".net7.0", "DirectX11" };
        public static string programPath = AppDomain.CurrentDomain.BaseDirectory;
        public static bool LoadRess { get; } = true;
        public static bool FindRessParts { get; } = true;
        public static bool Global { get; } = true;
    }

    public static class Paths
    {
        /// <summary>
        /// 日志目录
        /// </summary>
        public static string Logs { get; } = Path.Combine(Program.programPath, "Logs");
        /// <summary>
        /// 插件目录
        /// </summary>
        public static string Plugins { get; } = Path.Combine(Program.programPath, "Plugins");
        /// <summary>
        /// 崩溃数据
        /// </summary>
        public static string Crash { get; } = Path.Combine(Program.programPath, "Logs\\Crash");
    }

    public static class File
    {
        public static string Log { get; } = $"log_{DateTime.Now:yyyyMMdd_HHmmss}.log";
        public static string[] Startup { get; } = { "startup", "main" };
        public static string[] LoadRess { get; } = {
            "root","data", "system","face","uipsd", "emote", "bgm", "sound", "voice", "fgimage", "bgimage",
            "evimage", "image", "img", "video", "any", "page", "part", "music","rule","scenario", "char", "patch"
        };
    }
}
