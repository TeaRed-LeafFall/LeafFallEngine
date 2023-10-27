using LeafFallEngine.Configs;
using LeafFallEngine.Helper;

namespace LeafFallEngine;

/// <summary>
/// 这里是引擎应用程序的主程序 类 
/// </summary>

// 具体的功能将会在tiny system插件里面实现 
public abstract class LFApp
{
    /// <summary>
    /// 启动应用
    /// </summary>
    public abstract void Startup();

    

#if WINFORM
    /// <summary>
    /// 设置主窗口
    /// </summary>
    /// <param name="MainWindow"></param>
    public static void SetMainWindow()
    {
        // 获取屏幕大小
        var screen = Screen.PrimaryScreen;
        if (screen != null)
        {
            var screenWidth = screen.Bounds.Width;
            var screenHeight = screen.Bounds.Height;

            // 计算窗口居中位置
            var windowWidth = MainWindow.Width;
            var windowHeight = MainWindow.Height;
            var centerX = (screenWidth - windowWidth) / 2;
            var centerY = (screenHeight - windowHeight) / 2;

            // 设置窗口在屏幕中居中
            MainWindow.StartPosition = FormStartPosition.Manual;
            MainWindow.Location = new Point(centerX, centerY);
        }


        // 创建按钮菜单项
        ButtonMenuPanel buttonMenu = new(7, 120, 32, 1);

        // 插件子菜单效果
        ButtonMenuPanel PluginButtonMenu = new(2, 120, 31, 1);
        PluginButtonMenu.Visible = false;

        // 设置主菜单
        MainWindow.GetPanel().Controls.Add(buttonMenu);
        buttonMenu.Controls[0].Text = "退出程序";
        buttonMenu.Controls[1].Text = "关于窗口";
        buttonMenu.Controls[0].Click += Exit_Click;
        buttonMenu.Controls[1].Click += Button_AboutWindow_Click;

        buttonMenu.Controls[3].Text = "测试Toast";
        buttonMenu.Controls[3].Click += But_Toast_Click;

        buttonMenu.Controls[4].Text = "测试LFWindow";
        buttonMenu.Controls[4].Click += But_WindowTest_Click;
        buttonMenu.Controls[5].Text = "Move 1";
        buttonMenu.Controls[5].Click += (obj, e) =>
        {
            WindowsPlus.AlignMouseToControl(buttonMenu.Controls[0]);
        };
        buttonMenu.Controls[6].Text = "Move 2";
        buttonMenu.Controls[6].Click += (obj, e) =>
        {
            WindowsPlus.AlignMouseToControl(buttonMenu.Controls[1]);
        };

        // 设置主菜单定位
        buttonMenu.Top = (MainWindow.ClientSize.Height - buttonMenu.Height) / 2;
        buttonMenu.Left = (MainWindow.ClientSize.Width - buttonMenu.Width) / 2;

        // 设置子菜单
        MainWindow.GetPanel().Controls.Add(PluginButtonMenu);
        buttonMenu.Controls[2].Text = "插件菜单 ◀";
        buttonMenu.Controls[2].Click += Plugins_MenuBut_Click;



        // 设置主窗口事件
        MainWindow.SizeChanged += MainWindow_SizeChanged;


        /*  ***********  */
        /*    函数处理    */
        /*  ***********  */
        void MainWindow_SizeChanged(object? sender, EventArgs e)
        {
            // 更新主菜单位置
            buttonMenu.Top = (MainWindow.ClientSize.Height - buttonMenu.Height) / 2;
            buttonMenu.Left = (MainWindow.ClientSize.Width - buttonMenu.Width) / 2;
            // 更新子菜单位置
            PluginButtonMenu.Top = buttonMenu.Controls[2].Top + buttonMenu.Top;
            PluginButtonMenu.Left = buttonMenu.Controls[2].Left + buttonMenu.Controls[2].Width + buttonMenu.Left;
        }

        void But_Toast_Click(object? sender, EventArgs e)
        {
            WindowsPlus.ShowToast("测试");
        }
        void But_WindowTest_Click(object? sender, EventArgs e)
        {
            LFWindows test = WindowsPlus.CreateWindowEx(600, 400, "Unsupport Window", null, FormBorderStyle.FixedToolWindow);
            test.GetPanel().Hide();
            test.ShowDialog();
        }
        void Plugins_MenuBut_Click(object? sender, EventArgs e)
        {
            // 设置子菜单位置
            PluginButtonMenu.Top = buttonMenu.Controls[2].Top + buttonMenu.Top;
            PluginButtonMenu.Left = buttonMenu.Controls[2].Left + buttonMenu.Controls[2].Width + buttonMenu.Left;
            // 显示与隐藏
            if (PluginButtonMenu.Visible == true)
            {
                PluginButtonMenu.Visible = false;
                buttonMenu.Controls[2].Text = "插件菜单 ◀";
            }
            else
            {
                PluginButtonMenu.Visible = true;
                buttonMenu.Controls[2].Text = "插件菜单 ▶";
            }
        }
        void Exit_Click(object? sender, EventArgs e)
        {
            MainWindow.Close();
        }
        void Button_AboutWindow_Click(object? sender, EventArgs e)
        {
            EngineSystem.CreateAboutWindow().ShowDialog();
        }
    }
#endif
}

