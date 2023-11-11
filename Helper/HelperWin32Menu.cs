using LeafFallEngine.Forms;
using ScnScript;
using System.Runtime.InteropServices;
using static LeafFallEngine.Helper.HelperWin32Menu;

namespace LeafFallEngine.Helper
{
    /// <summary>
    /// 包含创建和管理菜单的Win32 API函数声明的辅助类。
    /// </summary>
    public static class HelperWin32Menu
    {
        // 定义Win32 API函数
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateMenu();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool AppendMenu(IntPtr hMenu, uint uFlags, uint uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetMenu(IntPtr hWnd, IntPtr hMenu);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ModifyMenu(IntPtr hMnu, uint uPosition, uint uFlags, IntPtr uIDNewItem, string lpNewItem);

        // 定义菜单项标识符(样式)
        public enum MenuItemFlags : uint
        {
            MF_BYCOMMAND = 0x00000000,
            MF_POPUP = 0x00000010,
            MF_SEPARATOR = 0x00000800,
            MF_STRING = 0x00000000,
            MIIM_FTYPE = 0x00000100,
            MF_RIGHTJUSTIFY = 0x00004000,
            MF_CHECKED = 0x00000008,
            MF_RADIOCHECK= 0x00000200,
            MF_MENUBREAK= 0x00000040,
            MF_MENUBARBREAK= 0x00000020,
            MF_RIGHTORDER=0x00002000,
            MFS_DISABLED=0x00000003,
            MFS_DEFAULT=0x00001000,
            MFS_ENABLED=0x00000000,
            MFS_HILITE=0x00000080
        }
    }

    /// <summary>
    /// 使用Win32 API创建和管理菜单的测试类。
    /// </summary>
    public static class Win32MenuTest
    {
        /// <summary>
        /// 为给定窗体创建菜单和子菜单。
        /// </summary>
        /// <param name="form">将添加菜单的窗体。</param>
        /// 使用示例：
        /// <code>
        /// Form form = new Form();
        /// Win32MenuTest.TestMenu(form);
        /// </code>
        public static void TestMenu(Form form)
        {

            WindowsPlus.EnableDarkModeForWindow(form.Handle, true);
            FlowLayoutPanel panel = new FlowLayoutPanel() { Dock=DockStyle.Fill};
            Button bt = new Button() { Text = "Logger" };
            Button bt2 = new Button() { Text = "Scn" };

            ScnLexer lexer = new ScnLexer();
            lexer.InputData(Resources.test);
            bt.Click += (obj, e) =>
            {
                new LoggerUI().Show();
            };
            bt2.Click += (obj, e) =>
            {
                lexer.LexerAllString();
            };
            panel.Controls.Add(bt);
            panel.Controls.Add(bt2);
            form.Controls.Add(panel);

            Win32Menu MainMenu = new("MainMenu");

            Win32Menu FileMenu = new("Sample");
            MainMenu.AddSubMenu(FileMenu, 0);
            Win32Menu hMenu = new("Checked");
            FileMenu.AddMenuItem(hMenu, MenuItemFlags.MF_CHECKED);
            Win32Menu CMenu = new("-");
            FileMenu.AddMenuItem(CMenu, MenuItemFlags.MF_SEPARATOR);
            Win32Menu RMenu = new("Radio Checked!");
            FileMenu.AddMenuItem(RMenu, MenuItemFlags.MF_RADIOCHECK | MenuItemFlags.MF_CHECKED | MenuItemFlags.MFS_DISABLED);
            Win32Menu ExitMenu = new("Exit");
            FileMenu.AddMenuItem(ExitMenu, 0);
            Win32Menu XMenu = new("x");
            MainMenu.AddSubMenu(XMenu, MenuItemFlags.MF_RIGHTJUSTIFY);
            MainMenu.SetMenuForWindow(form);


            Application.Run(form);
        }
    }

    /// <summary>
    /// 表示菜单或菜单项的类。
    /// </summary>
    public class Win32Menu
    {
        private List<Win32Menu> _SubMenus = new();
        private List<Win32Menu> _MenuItems = new();
        private string _MenuText = "Menu";
        private IntPtr _Id = CreateMenu();

        /// <summary>
        /// 初始化Win32Menu类的新实例。
        /// </summary>
        /// <param name="MenuText">此菜单显示的文本。</param>
        public Win32Menu(string MenuText)
        {
            _MenuText = MenuText;
        }

        /// <summary>
        /// 向此菜单添加子菜单。
        /// </summary>
        /// <param name="menu">要添加的子菜单。</param>
        /// <param name="style">子菜单的样式。</param>
        public void AddSubMenu(Win32Menu menu, MenuItemFlags style)
        {
            AppendMenu(_Id, (uint)MenuItemFlags.MF_POPUP | (uint)style, (uint)menu._Id, menu._MenuText);
            _SubMenus.Add(menu);
        }

        /// <summary>
        /// 向此菜单添加菜单项。
        /// </summary>
        /// <param name="menu">要添加的菜单项。</param>
        /// <param name="style">菜单项的样式。</param>
        public void AddMenuItem(Win32Menu menu, MenuItemFlags style)
        {
            AppendMenu(_Id, 0 | (uint)style, (uint)menu._Id, menu._MenuText);
            _MenuItems.Add(menu);
        }

        /// <summary>
        /// 为给定窗体设置此菜单。
        /// </summary>
        /// <param name="form">将设置菜单的窗体。</param>
        public void SetMenuForWindow(Form form)
        {
            SetMenu(form.Handle, _Id);
        }
    }
}
