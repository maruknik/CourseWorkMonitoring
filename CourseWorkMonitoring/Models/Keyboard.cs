using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CourseWorkMonitoring.Models
{
    public class Keyboard
    {
        const int WH_KEYBOARD_LL = 13;
        const int VK_F = 0x46;

        public Keyboard()
        {
            handler = KeyboardHandler;
        }
        KeyboardHookDelegate handler;
        delegate IntPtr KeyboardHookDelegate(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int hookId, KeyboardHookDelegate del, IntPtr hmod, uint idThread);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr pHook);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr pHook, int code, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        private IntPtr _setHook(KeyboardHookDelegate del)
        {
            var proccess = Process.GetCurrentProcess();
            var module = proccess.MainModule;
            var moduleName = module.ModuleName;

            return SetWindowsHookEx(WH_KEYBOARD_LL, del, GetModuleHandle(moduleName), 0);
        }
        private IntPtr _hookPtr = IntPtr.Zero;

        public void SetHook()
        {
            _hookPtr = _setHook(handler);
        }

        public void UnsetHook()
        {

            UnhookWindowsHookEx(_hookPtr);
        }

        IntPtr KeyboardHandler(int code, IntPtr wParam, IntPtr lParam)
        {
            var keyCode = Marshal.ReadInt32(lParam);
            throw new NotImplementedException("Test");
            //Console.WriteLine("Press " + keyCode.ToString());

            if (keyCode == VK_F)
            {
                return IntPtr.Zero;
            }


            return CallNextHookEx(_hookPtr, code, wParam, lParam); // Передати далі
                                                                   //return IntPtr.Zero; // подія оброблена - далі передавати її не треба

        }
    }
}
