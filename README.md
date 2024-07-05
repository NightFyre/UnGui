# UnGui
UnGui is a lightweight, Dear ImGui-inspired GUI toolkit for Unity, designed to be used with BepInEx plugins. It simplifies the creation and management of UI elements in a window-based system, making it easy to build custom interfaces for your Unity projects.

## Features
- Simple and intuitive API for creating and managing windows and UI elements.
- Automatic layout management for sequentially placed elements.
- Customizable button sizes and positions.
- Draggable windows.

## Installation
- Ensure you have BepInEx installed in your Unity project.
- Create a new BepInEx plugin and add the UnGui class to your project.

## Usage
In your BepInEx plugin, use the UnGui class to create and manage your GUI elements. Below is an example plugin implementation:  

```c#
using BepInEx;
using UnityEngine;
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        //  Hook Component
        Hook.AddComponent<YourModMan>();
        DontDestroyOnLoad(Hook);

        //  Log information
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }
}

public class YourModMan : MonoBehaviour
{
    private bool showMenu = true; // Toggle this based on your conditions
    private Rect windowRect = new Rect(20, 20, 300, 200);   //  Set this based on your conditions

    private void OnGUI()
    {
        if (showMenu)
        {

            UnGui.Window pWindow = UnGui.GetCurrentWindow();
            if (pWindow != null)
            {
                windowRect.size = pWindow.windowRect.size;

                //  update Window class variable
                UnGui.SetWindowPos(windowRect.position);

                Vector2 pos = pWindow.windowRect.position;
                if (UnGui.ClipWindowToScreenBounds(pWindow, ref pos))
                    windowRect.position = pos;
            }

            windowRect = GUI.Window(0, windowRect, DrawMenuWindow, PluginInfo.PLUGIN_NAME);
        }
    }

    private bool bTestToggle = false;
    private void DrawMenuWindow(int windowID)
    {
        //  Early exit if window creation / find fails
        if (!UnGui.Begin(windowID, PluginInfo.PLUGIN_NAME))
        {
            UnGui.End();
            return;
        }

        UnGui.Window pWindow = UnGui.GetCurrentWindow();
    
        UnGui.Text("This is text inside the window.");
        UnGui.Text("This is centered text inside the window.", true);
        UnGui.Text("This is colored text inside the window.", Color.green);

        UnGui.Text(windowRect.ToString());

        UnGui.Text(UnGui.GetCursorPos().ToString());

        // Add your GUI elements here
        if (UnGui.Button("Button 1"))
            Console.WriteLine("Button 1 clicked!");

        if (UnGui.Button("Large Button", new Vector2(UnGui.GetContentRegionAvail().x, UnGui.CalcTextHeight("Large Button"))))
            Console.WriteLine("Large Button clicked!");

        string label = "Color Button";
        Vector2 szText = UnGui.CalcTextSize(label); //  calculate text size
        Vector2 szBtn = new Vector2(szText.x + szText.x * .33f, szText.y);  //  set button size
        UnGui.SetCursorPos(new Vector2(UnGui.GetContentRegionAvail().x - (szBtn.x - pWindow.padding), UnGui.GetCursorPos().y));
        if (UnGui.Button("Color Button", Color.cyan))
            Console.WriteLine("Color 3 clicked!");
    
        Menu.bTestToggle = UnGui.Toggle("Toggle", Menu.bTestToggle);
            
        //  
        UnGui.End();

        // Ensure the window is draggable
        GUI.DragWindow(new Rect(0, 0, pWindow.windowRect.width, pWindow.windowRect.height));
    }
}
```

## Resources
- [BepinEx - Writing a basic plugin](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/index.html)
- [SPTarkov - Intro to Modding](https://hub.sp-tarkov.com/doc/entry/54-tutorial-intro-to-client-modding-and-mod-examples/)
- [SPTarkov - Client Mod Examples](https://github.com/kobrakon/ClientModdingExamples)

## License
This project is licensed under the MIT License.