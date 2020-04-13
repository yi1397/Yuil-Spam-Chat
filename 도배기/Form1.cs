using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace 도배기
{
    public partial class Form1 : Form
    {

        static string txt;

        static Thread rTh;

        static bool isRunning = false;

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;

        private LowLevelKeyboardProc _proc = hookProc;

        private static IntPtr hhook = IntPtr.Zero;


        public void SetHook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        }


        public static void UnHook()
        {
            UnhookWindowsHookEx(hhook);
        }


        public static IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (vkCode.ToString() == "114")
                {
                    if (isRunning == false)
                    {
                        rTh = new Thread(send_txt);
                        rTh.Start();
                        isRunning = true;
                    }
                    return (IntPtr)1;
                }
                if (vkCode.ToString() == "115")
                {
                    rTh.Abort();
                    isRunning = false;
                    return (IntPtr)1;
                }
                return CallNextHookEx(hhook, code, (int)wParam, lParam);
            }
            else
                return CallNextHookEx(hhook, code, (int)wParam, lParam);
        }


        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            rTh = new Thread(send_txt);
            SetHook();
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnHook();
            rTh.Abort();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            txt = richTextBox1.Text;
        }

        static void send_txt()
        {
            while (true)
            {
                Thread.Sleep(80);
                SendKeys.SendWait(txt);
                SendKeys.SendWait("{ENTER}");
            }
        }
    }
    
}
