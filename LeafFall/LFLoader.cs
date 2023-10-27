using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace LeafFallEngine;

public class Loader
{
    public static void LoadFile(string path)
    {

    }
    /// <summary>
    /// 获取程序内部语言文件数据
    /// </summary>
    public static string GetInternalResourceLanguage() => Encoding.UTF8.GetString(Resources.en_us);
}
