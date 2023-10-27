using LeafFallEngine;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace LeafFallEngine.Helper;

public class WindowsPlus
{
    /// <summary>
    /// 把byte[]数据转换成Icon数据
    /// </summary>
    public static Icon ByteToIcon(byte[] byteData) => new Icon(new MemoryStream(byteData));
    /// <summary>
    /// 把byte[]数据转换成Image数据
    /// </summary>
    public static Image ByteToImage(byte[] byteData) => Image.FromStream(new MemoryStream(byteData));
    /// <summary>
    /// 把byte[]的png数据转换成Icon数据
    /// </summary>
    public static Icon BytePngToIcon(byte[] pngData)
    {
        using (MemoryStream stream = new MemoryStream(pngData))
        {
            using (Bitmap bitmap = new Bitmap(stream))
            {
                IntPtr hIcon = bitmap.GetHicon();
                Icon icon = Icon.FromHandle(hIcon);
                return icon;
            }
        }
    }

    /// <summary>
    /// DWM设置窗口特性
    /// </summary>
    [DllImport("dwmapi.dll", PreserveSig = true)]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute attr, ref int attrValue, int attrSize);
    /// <summary>
    /// 设置窗口深色模式(Win10+)
    /// </summary>
    /// <param name="Handle">句柄</param>
    /// <param name="enable">启用状态</param>
    /// <returns></returns>
    public static bool EnableDarkModeForWindow(IntPtr Handle, bool enable)
    {
        int darkMode = enable ? 1 : 0;
        int hr = DwmSetWindowAttribute(Handle, DwmWindowAttribute.UseImmersiveDarkMode, ref darkMode, sizeof(int));
        return hr >= 0;
    }
    /// <summary>
    /// 枚举的Dwm窗口特性
    /// </summary>
    public enum DwmWindowAttribute : uint
    {
        NCRenderingEnabled = 1,
        NCRenderingPolicy,
        TransitionsForceDisabled,
        AllowNCPaint,
        CaptionButtonBounds,
        NonClientRtlLayout,
        ForceIconicRepresentation,
        Flip3DPolicy,
        ExtendedFrameBounds,
        HasIconicBitmap,
        DisallowPeek,
        ExcludedFromPeek,
        Cloak,
        Cloaked,
        FreezeRepresentation,
        PassiveUpdateMode,
        UseHostBackdropBrush,
        UseImmersiveDarkMode = 20,
        WindowCornerPreference = 33,
        BorderColor,
        CaptionColor,
        TextColor,
        VisibleFrameBorderThickness,
        SystemBackdropType,
        Last
    }
    /// <summary>
    /// 是否处于深色模式(注册表查询)
    /// </summary>
    public static bool IsDarkMode()
    {
        bool isDark = false;

        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize")!)
        {
            if (key != null)
            {
                int appsUseLightTheme = (int)key.GetValue("AppsUseLightTheme", -1);
                if (appsUseLightTheme == 0)
                {
                    // 当前使用暗色主题
                    isDark = true;
                }
                else if (appsUseLightTheme == 1)
                {
                    // 当前使用亮色主题
                    isDark = false;
                }
            }
        }

        return isDark;
    }

}
#if WINFORM
public class ButtonMenuPanel : Panel
{
    /// <summary>
    /// 横与列
    /// </summary>
    public enum Horizontal_Or_Column
    {
        ///<summary>横 ==</summary>
        Horizontal = 0,
        ///<summary>列 ||</summary>
        Column = 1
    }
    public ButtonMenuPanel(int NumberOfItem = 2, int sWidth = 200, int sHeight = 32, int Spacing = 0, Horizontal_Or_Column hc = Horizontal_Or_Column.Column)
    {
        // 这里是 2023/8/17 1:05:52 我正在熬夜改bug ,这里的 Width一段时间没有发现写成了with,没有被函数使用... 这tm谁看的出来！
        Logger.WriteLine($"按钮菜单容器 元素数量:{NumberOfItem} 元素宽度:{sWidth} 元素高度:{sHeight} 元素间距:{Spacing} 菜单方向:{hc}");
        if (NumberOfItem <= 0) NumberOfItem = 2;
        // 创建按钮组
        Button[] but = new Button[NumberOfItem];
        for (int i = 0; i < but.Length; i++)
        {
            but[i] = new();
            // 设置按钮
            but[i].TabIndex = i;
            but[i].FlatStyle = FlatStyle.System;
            //but[i].UseVisualStyleBackColor = true;
            but[i].Size = new Size(sWidth, sHeight);
            but[i].Text = $"Item {i}";
            // 列 ||
            if (hc == Horizontal_Or_Column.Column)
            {
                but[i].Location = new Point(0, i * (sHeight + Spacing));
            }
            // 横 ==
            if (hc == Horizontal_Or_Column.Horizontal)
            {
                but[i].Location = new Point(i * (sWidth + Spacing), 0);

            }

            Logger.WriteLine($"按钮 {i} X:{but[i].Location.X}  Y:{but[i].Location.Y}");

            // 添加按钮
            this.Controls.Add(but[i]);
        }

        // 调整面板大小

        if (hc == Horizontal_Or_Column.Column)
        {
            this.Height = NumberOfItem * (sHeight + Spacing);
            this.Width = sWidth;
        }
        if (hc == Horizontal_Or_Column.Horizontal)
        {
            this.Width = NumberOfItem * (sWidth + Spacing);
            this.Height = sHeight;
        }
        Logger.WriteLine($"按钮菜单容器 高度:{this.Height} 宽度:{this.Width}");

    }
}
#endif