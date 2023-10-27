using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static LeafFallEngine.Helper.HelperWin32Menu;

namespace LeafFallEngine.Helper
{
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

        // 定义菜单项标识符
        public enum MenuItemFlags : uint
        {
            MF_BYCOMMAND = 0x00000000,
            MF_POPUP = 0x00000010,
            MF_SEPARATOR = 0x00000800,
            MF_STRING = 0x00000000,
            MIIM_FTYPE = 0x00000100,
            MFT_RIGHTJUSTIFY = 0x00004000
        }

        [Flags]
        public enum TrackPopupMenuFlags : uint
        {
            TPM_LEFTALIGN = 0x0000,
            TPM_LEFTBUTTON = 0x0000,
            TPM_RIGHTALIGN = 0x0008,
            TPM_RETURNCMD = 0x0100
        }
    }
    public class Win32Menu
    {
        public void TestMenu()
        {
            Form form = new Form() { 
                Width=800,
                Height=600
            };
            
            // 创建菜单栏和子菜单
            IntPtr menuBar = CreateMenu();
            IntPtr fileMenu = CreateMenu();
            IntPtr systemMenu = CreateMenu();

            // 添加菜单项
            AppendMenu(fileMenu, 0, 1000, "New");
            AppendMenu(fileMenu, 0, 1001, "Open");
            AppendMenu(fileMenu, (uint)MenuItemFlags.MF_SEPARATOR, 0, "");
            AppendMenu(fileMenu, 0, (uint)MenuItemFlags.MF_POPUP, "Exit");

            // 将子菜单添加到菜单栏
            AppendMenu(menuBar, (uint)MenuItemFlags.MF_POPUP, (uint)fileMenu, "File");
            int scClose = 0xF060;
            AppendMenu(menuBar, (uint)MenuItemFlags.MF_POPUP | (uint)TrackPopupMenuFlags.TPM_RIGHTALIGN, 0, $"x");
            // 将菜单栏设置为窗口菜单
            //SetMenu(form.Handle, menuBar);

            MenuStrip ms=new MenuStrip();
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem() { Text = "x",Alignment=ToolStripItemAlignment.Right,ToolTipText="关闭菜单栏"};

            ms.Items.AddRange(new ToolStripItem[] {toolStripMenuItem});
            form.Controls.Add(ms);
            form.MainMenuStrip = ms;

            Application.Run(form);
        }
    }
}

//namespace Win32MenuExample
//{
//    public partial class Form1 : Form
//    {
//        // 定义Win32 API函数
//        [DllImport("user32.dll", SetLastError = true)]
//        static extern IntPtr CreateMenu();

//        [DllImport("user32.dll", SetLastError = true)]
//        static extern bool AppendMenu(IntPtr hMenu, uint uFlags, uint uIDNewItem, string lpNewItem);

//        [DllImport("user32.dll", SetLastError = true)]
//        static extern bool TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        

//        public Form1()
//        {
//            InitializeComponent();
//        }

//        protected override void OnLoad(EventArgs e)
//        {
//            base.OnLoad(e);

            
//        }

//        protected override void OnMouseDown(MouseEventArgs e)
//        {
//            base.OnMouseDown(e);

//            if (e.Button == MouseButtons.Right)
//            {
//                // 显示菜单
//                TrackPopupMenu(GetSubMenu(GetMenu(Handle), 0), (uint)TrackPopupMenuFlags.TPM_LEFTALIGN | (uint)TrackPopupMenuFlags.TPM_LEFTBUTTON, 0, 0, 0, Handle, IntPtr.Zero);
//            }
//        }

//        protected override void WndProc(ref Message m)
//        {
//            const int WM_COMMAND = 0x0111;
//            const int IDM_NEW = 1000;
//            const int IDM_OPEN = 1001;
//            const int IDM_EXIT = 3;

//            switch (m.Msg)
//            {
//                case WM_COMMAND:
//                    int id = m.WParam.ToInt32() & 0xffff;
//                    switch (id)
//                    {
//                        case IDM_NEW:
//                            MessageBox.Show("New clicked");
//                            break;
//                        case IDM_OPEN:
//                            MessageBox.Show("Open clicked");
//                            break;
//                        case IDM_EXIT:
//                            Application.Exit();
//                            break;
//                    }
//                    break;
//                default:
//                    base.WndProc(ref m);
//                    break;
//            }
//        }
//    }
//}
