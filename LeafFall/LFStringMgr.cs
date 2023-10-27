using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LeafFallEngine;
/// <summary>
/// 字符串管理器
/// </summary>
public static class StringMgr
{
    // 字典 （key,value）
    private static Dictionary<string, string> dataDict = new Dictionary<string, string>();

    /// <summary>
    /// 加载Json文件(后来者替换值)
    /// </summary>
    /// <param name="path">路径</param>
    public static void LoadFile(string path) => LoadData(File.ReadAllText(path));

    /// <summary>
    /// 加载Json数据
    /// </summary>
    /// <param name="data">数据</param>
    public static void LoadData(string data) => ParseJsonAndAddToDict(JsonDocument.Parse(data).RootElement, "");
    /// <summary>
    /// 解析json对象
    /// </summary>
    /// <param name="element">输入对象</param>
    /// <param name="parentKey">输入键</param>
    private static void ParseJsonAndAddToDict(JsonElement element, string parentKey)
    {
        foreach (JsonProperty prop in element.EnumerateObject())
        {
            // 添加父级前缀
            string key = parentKey + (string.IsNullOrEmpty(parentKey) ? "" : ".") + prop.Name;

            // 是对象还是字符串键值
            if (prop.Value.ValueKind == JsonValueKind.Object)
            {
                // 添加父对象键到字典中
                if (!dataDict.ContainsKey(key))
                {
                    dataDict.Add(key, "");
                }

                // 递归解析嵌套的对象并添加到字典中
                ParseJsonAndAddToDict(prop.Value, key);
            }
            else if (prop.Value.ValueKind == JsonValueKind.String)
            {
                string value = prop.Value.GetString()!;

                if (value.Contains("{NewLine}"))
                {
                    // 替换"{NewLine}"为系统的换行符
                    value = value.Replace("{NewLine}", Environment.NewLine);
                }
                if (value.Contains("\\n"))
                {
                    value = value.Replace("\\n", Environment.NewLine);
                }

                // 添加到字典
                if (dataDict.ContainsKey(key))
                {
                    // 重复覆盖(汉化及多语言用)
                    dataDict[key] = value;
                }
                else
                {
                    // 添加(键,值)
                    dataDict.Add(key, value);
                }
            }
        }
    }


    /// <summary>
    /// 获取文本
    /// </summary>
    /// <remarks>
    /// <para>这里是很常见的bug:</para>
    /// <para>因为你可能把值所属的对象搞错了，或者新旧文件冲突，或者是大小写错误!以及没有该值</para>
    /// </remarks>
    /// <param name="key">消息键名</param>
    /// <param name="replacements">替换{0}等项</param>
    /// <exception cref="Exception">这里是很常见的bug,因为你可能把值所属的对象搞错了，或者新旧文件冲突，或者是大小写错误!</exception>
    /// <returns>字符串</returns>
    public static string GetString(string key, params object[] replacements)
    {
        if (dataDict.ContainsKey(key))
        {
            string text = dataDict[key];

            if (replacements != null && replacements.Length > 0)
            {
                text = string.Format(text, replacements);
            }

            return text;
        }

        // 如果字符串管理器加载了unk的值就获取输出
        if (dataDict.ContainsKey("unk"))
        {
#if DEBUG
            // DEBUG： 这里是很常见的bug,因为你可能把值所属的对象搞错了，或者新旧文件冲突，或者是大小写错误!
            // 在调试的时候需要注意
            throw new Exception("未能发现值:" + key);
#endif
#if !DEBUG
                //NOT DEBUG: 这个错误在发布版本可以忽略。
                string unk = GetString("unk", key);
                return unk;
#endif
        }

        return key; // 未找到指定的字符串内容
                    // throw new ArgumentNullException(nameof(key));
    }
}
