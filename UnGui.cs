using System.Collections.Generic;
using UnityEngine;

internal class UnGui
{
    private static Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);       //  monitor screen client area
    private static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);       //  obtains a default GUIStyle instance based on the current GUI.skin.label
    private static Color Color { get { return GUI.color; } set { GUI.color = value; } }     //  

    public class Window
    {
        public Rect windowRect;         //  this window size
        public string title;            //  name identifier
        public int id;                  // array index
        public float padding;           //  general padding for element spacing , 
        public float lineHeight;        //  standard height of each element.
        public Vector2 cursorPos;       //  last element position
        public Vector2 cursorMax;       //  max cursor dimensions obtained at imgui end call [ width , height ]

        //  Creates a default window with 300x , 200y dimensions
        public Window(string name, int index)
        {
            id = index;
            title = name;
            padding = 10f;
            lineHeight = 10f;

            cursorPos = new Vector2(padding, padding);
            cursorMax = cursorPos;
            Vector2 szWindow = new Vector2(300f, 200f);
            Vector2 posWindow = new Vector2(screenRect.width / 2 - szWindow.x / 2, screenRect.height / 2 - szWindow.y / 2);
            windowRect = new Rect(posWindow, szWindow);
        }
    }
    private static List<Window> gWindows = new List<Window>();
    private static Window gCurrentWindow = null;

    private static Window CreateNewWindow(string name, int index)
    {
        // push new window to window list
        gWindows.Add(new Window(name, index));

        //  Adjust initial Cursor Position as its not reflected in the class initializer
        Window pWindow = GetWindowByName(name);
        pWindow.cursorPos.y += pWindow.lineHeight;

        //  return newly created window
        return pWindow;
    }

    private static Window GetWindowByID(int id)
    {
        Window result = null;

        // get size of window array
        int szWindow = gWindows.Count;
        if (szWindow <= 0)
        {// no windows have been created
            return result;
        }

        if (id > szWindow)
        {// window has not been created yet
            return result;
        }

        return gWindows[id];
    }

    private static Window GetWindowByName(string name)
    {
        Window result = null;
        foreach (Window w in gWindows)
        {
            if (w.title != name)
                continue;

            return w;
        }

        return result;
    }

    public static Window GetCurrentWindow() {  return gCurrentWindow; }

    public static void SetWindowWidth(float width) { gCurrentWindow.windowRect.width = width; }

    public static void SetWindowHeight(float height) { gCurrentWindow.windowRect.height = height; }

    public static void SetWindowSize(Vector2 size) { gCurrentWindow.windowRect.size = size; }

    public static float GetWindowWidth() { return gCurrentWindow.windowRect.width; }

    public static float GetWindowHeight() { return gCurrentWindow.windowRect.height; }

    public static Vector2 GetWindowSize() { return gCurrentWindow.windowRect.size; }

    public static void SetWindowPos(Vector2 pos) { gCurrentWindow.windowRect.position = pos; }

    public static Vector2 GetWindowPos() { return gCurrentWindow.windowRect.position; }

    public static void SetCursorPos(Vector2 pos) { gCurrentWindow.cursorPos = pos; }

    public static Vector2 GetCursorPos() { return gCurrentWindow.cursorPos; }

    public static void SetNextItemPos(Vector2 pos) { SetCursorPos(pos); }

    public static Vector2 GetNextItemPos() { return GetCursorPos(); }
    
    public static Vector2 GetContentRegionAvail()
    {
        return new Vector2((gCurrentWindow.windowRect.width - gCurrentWindow.padding) - gCurrentWindow.cursorPos.x, (gCurrentWindow.windowRect.height - gCurrentWindow.padding) - gCurrentWindow.cursorPos.y);
    }

    public static float CalcTextWidth(string fmt) { return CalcTextSize(fmt).x; }

    public static float CalcTextHeight(string fmt) { return CalcTextSize(fmt).y; }

    public static Vector2 CalcTextSize(string fmt) { return StringStyle.CalcSize(new GUIContent(fmt)); }

    private static bool IsDragging()
    {
        return (Event.current.type == EventType.MouseDrag && GUIUtility.hotControl != 0);
    }

    private static void AutoResizeWindow(Window pWindow)
    {
        var szScreen = screenRect.size;
        float maxWidth = szScreen.x;
        float maxHeight = szScreen.y;

        // Set new window height
        Vector2 szWindowNew = pWindow.cursorMax;
        float newWindowWidth = Mathf.Min(szWindowNew.x, maxWidth);
        float newWindowHeight = Mathf.Min(szWindowNew.y, maxHeight);
        SetWindowWidth(newWindowWidth);
        SetWindowHeight(newWindowHeight);
    }

    private static void ClipWindowToScreen(Window pWindow)
    {
        var szWindow = GetWindowSize();
        var posWindow = GetWindowPos();
        var szScreen = screenRect.size;
        var maxWidth = szScreen.x;
        var maxHeight = szScreen.y;

        Vector2 posWindowNew = posWindow;

        // Clip new window Width
        float cPos = posWindow.x + szWindow.x;
        if (cPos > maxWidth)
        {
            posWindowNew.x -= (cPos - maxWidth) + pWindow.padding;
        }
        if (posWindow.x < 0)
        {
            posWindowNew.x = 0 + pWindow.padding;
        }

        // Clip new Window Height
        cPos = posWindow.y + szWindow.y;
        if (cPos > maxHeight)
        {
            posWindowNew.y -= (cPos - maxHeight) + pWindow.padding;
        }
        if (posWindow.y < 0)
        {
            posWindowNew.y = 0 + pWindow.padding;
        }

        // Set Window Position
        if (posWindowNew != posWindow)
        {
            SetWindowPos(posWindowNew);
        }
    }

    public static bool ClipWindowToScreenBounds(Window pWindow, ref Vector2 pos)
    {
        var szWindow = GetWindowSize();
        var posWindow = GetWindowPos();
        var szScreen = screenRect.size;
        var maxWidth = szScreen.x;
        var maxHeight = szScreen.y;

        Vector2 posWindowNew = posWindow;

        // Clip new window Width
        float cPos = posWindow.x + szWindow.x;
        if (cPos > maxWidth)
        {
            posWindowNew.x -= (cPos - maxWidth) + pWindow.padding;
        }
        if (posWindow.x < 0)
        {
            posWindowNew.x = 0 + pWindow.padding;
        }

        // Clip new Window Height
        cPos = posWindow.y + szWindow.y;
        if (cPos > maxHeight)
        {
            posWindowNew.y -= (cPos - maxHeight) + pWindow.padding;
        }
        if (posWindow.y < 0)
        {
            posWindowNew.y = 0 + pWindow.padding;
        }

        // Set Window Position
        if (posWindowNew != posWindow)
        {
            pos = posWindowNew;
            return true;
        }

        return false;
    }

    //  Should be called once before any window / widget / text events.
    public static bool Begin(int id, string title, GUI.WindowFunction fn = null)
    {
        //  Find or Create Window
        Window pWindow = GetWindowByName(title);
        if (pWindow == null)
            pWindow = CreateNewWindow(title, id);

        if (pWindow == null)
            return false;

        //  Set Current Window
        gCurrentWindow = pWindow;

        // Set Cursor Position
        gCurrentWindow.cursorPos = new Vector2(gCurrentWindow.padding, gCurrentWindow.padding * 2);

        //  Render
        if (fn != null)
            gCurrentWindow.windowRect = GUI.Window(id, gCurrentWindow.windowRect, fn, gCurrentWindow.title);

        return true;
    }

    // Called between UnGui.Begin & UnGui.End
    public static void Draw(GUI.WindowFunction fn)
    {
        gCurrentWindow.windowRect = GUI.Window(gCurrentWindow.id, gCurrentWindow.windowRect, fn, gCurrentWindow.title);
    }

    //  should be called after any window / widget / text events.
    public static void End()
    {
        Window pWindow = gCurrentWindow;
        if (pWindow == null)
            return;

        // Update window height if not dragging
        if (!IsDragging())
            AutoResizeWindow(pWindow);

        // Clip window position to screen bounds
        ClipWindowToScreen(pWindow);

        // release current window context
        //  gCurrentWindow = null;
    }

    public static void Text(string fmt, bool centered = false)
    {
        //  Get Current Window
        Window pWindow = gCurrentWindow;
        if (pWindow == null)
            return;

        // Get label size
        Vector2 size = CalcTextSize(fmt);
        if (centered)
            pWindow.cursorPos.x = (pWindow.windowRect.width - pWindow.padding) / 2 - size.x / 2;

        //  render label
        GUI.Label(new Rect(pWindow.cursorPos, size), fmt);

        //  adjust cursor position for next element
        pWindow.cursorPos.x = pWindow.padding;
        pWindow.cursorPos.y += size.y + pWindow.lineHeight;

        //  Update Max Cursor
        pWindow.cursorMax.y = pWindow.cursorPos.y;
        if (size.x > pWindow.cursorMax.x)
            pWindow.cursorMax.x = size.x + pWindow.padding;
    }

    public static void Text(string fmt, Color color, bool centered = false)
    {
        Color = color;
        Text(fmt, centered);
        Color = Color.white;    //  reset color
    }

    public static bool Button(string fmt, Vector2? size = null) //, Vector2? position = null)
    {
        //  Get Current Window
        Window pWindow = gCurrentWindow;
        if (pWindow == null) 
            return false;

        // Determine the position of the button
        Vector2 buttonPos = pWindow.cursorPos;
        //  Vector2 buttonPos = position ?? pWindow.cursorPos;

        // Determine the size of the button
        Vector2 szText = CalcTextSize(fmt);
        Vector2 buttonSize = size ?? szText;
        if (!size.HasValue)
        {
            if (buttonSize.y < pWindow.lineHeight)
                buttonSize.y = pWindow.lineHeight;

            buttonSize.x += buttonSize.x * 0.33f;
        } 
        else if (buttonSize.y < szText.y)
            buttonSize.y = szText.y;

        //  Get result from gui button event
        bool result = GUI.Button(new Rect(buttonPos, buttonSize), fmt);

        //  adjust cursor position for next element
        pWindow.cursorPos.x = pWindow.padding;
        pWindow.cursorPos.y += buttonSize.y + pWindow.lineHeight;

        //  Update Max Cursor
        pWindow.cursorMax.y = pWindow.cursorPos.y;
        if (buttonSize.x > pWindow.cursorMax.x)
            pWindow.cursorMax.x = buttonSize.x + pWindow.padding;

        // return result
        return result;
    }

    public static bool Button(string fmt, Color color, Vector2? size = null) //, Vector2? position = null)
    {
        Color = color;

        bool result = Button(fmt, size);
        //  bool result = Button(fmt, size, position);
        
        Color = Color.white;
        
        return result;
    }

    public static bool Toggle(string fmt, bool p)
    {
        //  Get Current Window
        Window pWindow = gCurrentWindow;
        if (pWindow == null)
            return false;

        Vector2 size = CalcTextSize(fmt);
        if (size.y < pWindow.lineHeight)
            size.y = pWindow.lineHeight;
        size.x += size.x * .5f;

        bool result = GUI.Toggle(new Rect(pWindow.cursorPos, size), p, fmt);


        //  adjust cursor position for next element
        pWindow.cursorPos.x = pWindow.padding;
        pWindow.cursorPos.y += size.y + pWindow.lineHeight;

        //  Update Max Cursor
        pWindow.cursorMax.y = pWindow.cursorPos.y;
        if (size.x > pWindow.cursorMax.x)
            pWindow.cursorMax.x = size.x + pWindow.padding;

        // return result
        return result;
    }

    public static bool Toggle(string fmt, bool p , Color color)
    {
        Color = color;

        bool result = Toggle(fmt, p);
        
        Color = Color.white;
        
        return result;
    }
}