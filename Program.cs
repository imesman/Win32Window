using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;

#pragma warning disable CA1416

bool success = false;

string name = "Win32Window";

if (RegisterClass(name))
    if (CreateWindow(name))
        success = true;

MSG msg = new MSG();
if (success)
{
    int rv;
    while ((rv = PInvoke.GetMessage(out msg, HWND.Null, 0, 0)) > 0)
    {
        unsafe
        {
            PInvoke.TranslateMessage(msg);
            PInvoke.DispatchMessage(msg);
        }
    }
}


bool RegisterClass(string classname)
{
    WNDCLASSEXW wcex = new WNDCLASSEXW();
    wcex.style = WNDCLASS_STYLES.CS_DBLCLKS;
    wcex.cbSize = (uint)Marshal.SizeOf(wcex);
    wcex.lpfnWndProc = wndproc;
    wcex.cbClsExtra = 0; wcex.cbWndExtra = 0;

    unsafe
    {
        fixed (char* p = "32512") // IDI_APPLICATION & IDC_ARROW
        {
            wcex.hIcon = PInvoke.LoadIcon(HINSTANCE.Null, new PCWSTR(p));
            wcex.hCursor = PInvoke.LoadCursor(HINSTANCE.Null, new PCWSTR(p));
        }
    }

    wcex.hIconSm = HICON.Null;
    wcex.hbrBackground = new HBRUSH(6);
    
    unsafe
    {
        fixed (char* p = classname)
        {
            wcex.lpszClassName = new PCWSTR(p);
        }
    }

    if (PInvoke.RegisterClassEx(wcex) == 0)
    {
        MessageBox("RegisterClassEx failed", classname);
        return false;
    }

    return true;
}

bool CreateWindow(string classname)
{
    HWND hwnd;
    unsafe
    {
        fixed (char* c = classname)
        {
            hwnd = PInvoke.CreateWindowEx(WINDOW_EX_STYLE.WS_EX_OVERLAPPEDWINDOW, new PCWSTR(c), 
                new PCWSTR(c), WINDOW_STYLE.WS_OVERLAPPEDWINDOW, 300, 300, 640, 
                480, HWND.Null, HMENU.Null, HINSTANCE.Null);
        }
    }


    if (hwnd != HWND.Null)
    {
        MessageBox("SUCCESS", classname);
        PInvoke.ShowWindow(hwnd, SHOW_WINDOW_CMD.SW_SHOW);
        PInvoke.UpdateWindow(hwnd);
        return true;
    }
    else
    {
        MessageBox("CreateWindowEx failed", classname);
        return false;
    }
}

void MessageBox(string text, string header)
{
    PInvoke.MessageBox(HWND.Null, text, header, MESSAGEBOX_STYLE.MB_OK | MESSAGEBOX_STYLE.MB_SETFOREGROUND 
                                                                       | MESSAGEBOX_STYLE.MB_ICONEXCLAMATION);
}

LRESULT wndproc(HWND hwnd, uint message, WPARAM wParam, LPARAM lparam)
{
    switch (message)
    {
        case 0x0010: // WM_CLOSE
        case 0x0012: // WM_QUIT
            PInvoke.PostQuitMessage(0);
            return new LRESULT();
        default:
            return PInvoke.DefWindowProc(hwnd, message, wParam, lparam);
    }
}