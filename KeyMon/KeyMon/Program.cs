using System;

using System.Diagnostics;

using System.Windows.Forms;

using System.Runtime.InteropServices;

using System.Collections;

class InterceptKeys

{

    private const int WH_KEYBOARD_LL = 13;

    private const int WM_KEYDOWN = 0x0100;

    private static LowLevelKeyboardProc _proc = HookCallback;

    private static IntPtr _hookID = IntPtr.Zero;

    private static Queue inputQueue = new Queue();

    private static bool shiftState;


    public static void Main()

    {

        _hookID = SetHook(_proc);

        Application.Run();

        UnhookWindowsHookEx(_hookID);

    }


    private static IntPtr SetHook(LowLevelKeyboardProc proc)

    {

        using (Process curProcess = Process.GetCurrentProcess())

        using (ProcessModule curModule = curProcess.MainModule)

        {

            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,

                GetModuleHandle(curModule.ModuleName), 0);

        }

    }


    private delegate IntPtr LowLevelKeyboardProc(

        int nCode, IntPtr wParam, IntPtr lParam);


    private static IntPtr HookCallback(

        int nCode, IntPtr wParam, IntPtr lParam)

    {

        // if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        if (nCode >= 0)

        {

            int vkCode = Marshal.ReadInt32(lParam);

            if ((Keys)vkCode == Keys.LShiftKey || (Keys)vkCode == Keys.RShiftKey)
            {
                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    shiftState = true;
                }
                else
                {
                    shiftState = false;
                }

            }

            if (wParam == (IntPtr)WM_KEYDOWN || (Keys)vkCode == Keys.LShiftKey || (Keys)vkCode == Keys.RShiftKey)
            {
                addToQueue((Keys)vkCode);
            }

        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);

    }

    private static void addToQueue(Keys key)
    {
        if (key == Keys.Enter)
        {
            printText();
        }
        else
        {
            inputQueue.Enqueue(getString(key));
        }
    }

    private static void printText()
    {
        string text = "";
        while (inputQueue.Count > 0)
        {
            text += inputQueue.Dequeue();
        }
        Console.WriteLine(text);
    }

    private static string getString(Keys key)
    {
        string result = "";
        switch (key)
        {
            case Keys.Space:
                result = " ";
                break;

            case Keys.Tab:
            case Keys.LWin:
                break;

            case Keys.LControlKey:
            case Keys.RControlKey:
                result = " Ctrl+";
                break;

            case Keys.LShiftKey:
            case Keys.RShiftKey:
                {
                    result = "^";
                }
                break;

            case Keys.Oem5:
                result = "\\";
                break;
            case Keys.Oemplus:
                result = "+";
                break;
            case Keys.OemMinus:
                result = "-";
                break;
            case Keys.OemOpenBrackets:
                result = "[";
                break;
            case Keys.OemCloseBrackets:
                result = "]";
                break;
            case Keys.Oem7:
                result = "'";
                break;
            case Keys.OemQuestion:
                result = "/";
                break;
            case Keys.Oem1:
                result = ";";
                break;
            case Keys.OemPeriod:
                result = ".";
                break;
            case Keys.Oemcomma:
                result = ",";
                break;

            case Keys.Capital:
                result = " CAPS ";
                break;
            case Keys.Back:
                result = "<";
                break;


            case Keys.D0:
            case Keys.D1:
            case Keys.D2:
            case Keys.D3:
            case Keys.D4:
            case Keys.D5:
            case Keys.D6:
            case Keys.D7:
            case Keys.D8:
            case Keys.D9:
                result = ((char)key).ToString();
                break;



            default:
                //result = (char)key+" ";
                if (shiftState == true)
                {
                    result = key.ToString().ToUpper();
                }
                else
                {
                    result = key.ToString().ToLower();
                }
                break;
        }

        return result;

    }






    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

    private static extern IntPtr SetWindowsHookEx(int idHook,

        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

    [return: MarshalAs(UnmanagedType.Bool)]

    private static extern bool UnhookWindowsHookEx(IntPtr hhk);


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,

        IntPtr wParam, IntPtr lParam);


    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]

    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
