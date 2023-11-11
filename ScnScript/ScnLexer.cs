using LeafFallEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ScnScript;

public interface ScnLaxerEx
{

}

public class ScnLexer
{
    private List<string> CodeLines=new();
    private int index;
    private List<Token> tokens=new();

    /// <summary>
    /// 输入数据
    /// </summary>
    /// <param name="input">数据字符串</param>
    public void InputData(string input)
    {
        CodeLines=input.Split(Environment.NewLine.ToCharArray()).ToList();
    }

    /// <summary>
    /// 解析所有文本
    /// </summary>
    public void LexerAllString()
    {
        foreach (string key in CodeLines)
        {
            tokens.Add(LexerString(key));
        }
    }

    /// <summary>
    /// 获取下一个Token数据
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">超出索引范围</exception>
    public Token getNextToken()
    {
        if (index <= CodeLines.Count)
        {
            index++;
            Logger.WriteLine($"Index: {index}");
            return LexerString(CodeLines[index]);
        }
        throw new ArgumentOutOfRangeException(nameof(index));
    }
    /// <summary>
    /// 解析行语法
    /// </summary>
    public Token LexerString(string s)
    {
        Token tok = new Token();

        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        Logger.WriteLine($"LexerString: {s}");

        Logger.WriteLine($"TokenType: {tok.TokenType} , TokenKey:{tok.Key} , TokenValue: {tok.Value}");

        return tok;
    }

    public Token getToken(int index)
    {
        return tokens[index];
    }
}

public enum TokenType
{
    //节点
    nodeSelect,
    //场景
    sceneSelect,
    //选中对象
    objSelect,
    //对象命令
    objCommand,
    //取消选中对象
    objClr,
    //本地命令
    localCommand,
    //本地配置
    localConfig,
    //解析配置
    scnSetting,
    //注释
    scnComments,
    //全局命令
    globalCommand,
    //字符串值
    stringKey,
    //布尔值
    boolKey,
    //运算符号
    calcKey,
    //条件符号
    isKay,
    //元数据表达式
    metaExpression,
    //变量与数据
    Value,
    //未知
    Unk
}

public class Token
{
    public TokenType TokenType=TokenType.Unk;
    public string Key="Unk";
    public string Value="Unk";
}
