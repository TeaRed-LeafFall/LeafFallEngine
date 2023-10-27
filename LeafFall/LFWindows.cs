using LeafFallEngine.Helper;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LeafFallEngine;

#if WINFORM
public class LFWindows : Form
{
    // 绘画对象 
    private Graphics g;

    // 面板 也就是画板 
    private Panel pl;

    public LFWindows()
    {
        // 窗口设置
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = SystemColors.Control;

        // 错误提示标签
        Label Error = new()
        {
            AutoSize = true,
            ForeColor = Color.DarkRed,
            Text = " Error: The base display panel is missing.\n Please contact support for assistance or reach out to the developer. "
        };

        Controls.Add(Error);

        // 添加面板
        pl = new()
        {
            Top = 0,
            Left = 0,
            // 这里解决了窗口被拉大无法缩小的问题
            Dock = DockStyle.Fill,
        };

        Controls.Add(pl);

        Controls.SetChildIndex(pl, Controls.IndexOf(Error));

        // 画布
        g = pl.CreateGraphics();

        SizeChanged += PeWindows_SizeChanged;
        WindowsPlus.EnableDarkModeForWindow(Handle, WindowsPlus.IsDarkMode());
    }

    private void PeWindows_SizeChanged(object? sender, EventArgs e)
    {
        Logger.WriteLine($"窗口大小改变 宽度:{Width} 高度:{Height}");
    }

    public Graphics GetGraphics()
    {
        return g;
    }

    public Panel GetPanel()
    {
        return pl;
    }
}
#endif