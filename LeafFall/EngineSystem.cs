using LeafFallEngine.Configs;
using LeafFallEngine.Helper;
using System.Text;

namespace LeafFallEngine;

/// <summary>
/// 系统
/// </summary>
public static class EngineSystem
{
#if WINFORM
    /// <summary>
    /// 创建关于窗口
    /// </summary>
    public static LFWindows CreateAboutWindow()
    {
        LFWindows AboutWindow = WindowsPlus.CreateWindow(400, 300, $"About \" {EngineData.Engine} \"");
        AboutWindow.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        AboutWindow.StartPosition = FormStartPosition.CenterParent;
        AboutWindow.GetPanel().BackColor = Color.FromArgb(26, 178, 103);

        // 创建文本标签
        Label label = new Label();

        label.Location = new Point(20, 20);

        label.ForeColor = Color.White;
        // 左右上下间隔 20
        label.Width = AboutWindow.Width - 20;
        label.Height = AboutWindow.Height - 20;
        AboutWindow.Height = label.Height + 2 * 20;
        label.AutoEllipsis = true;

        // 添加配置信息
        label.Text += $"{EngineData.Engine} v{EngineData.Version}\n";
        label.Text += $"Author: {EngineData.Author}\n";
        label.Text += $"Copyright: {EngineData.Copyright}\n";
        label.Text += $"-----------------------\n";
        label.Text += "Program:\n";
        label.Text += $"Name: {EngineData.Program.Name}\n";
        label.Text += $"Author: {EngineData.Program.Author}\n";
        label.Text += $"Main Encode: {EngineData.Program.MainEncode}\n";
        label.Text += $"Base Language: {EngineData.Program.BaseLanguage}\n";
        label.Text += $"Runtime: {string.Join(", ", EngineData.Program.Runtime)}\n";
        label.Text += $"Path: {EngineData.Program.programPath}\n";
        label.Text += $"Load Ress: {EngineData.Program.LoadRess}\n";
        label.Text += $"Find Ress Parts: {EngineData.Program.FindRessParts}\n";
        label.Text += $"Global: {EngineData.Program.Global}\n";


        // 将文本标签添加到窗口中
        AboutWindow.GetPanel().Controls.Add(label);

        return AboutWindow;
    }
#endif
}
