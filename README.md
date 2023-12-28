# VTools
 **Victor Toolkit** for Unity Game And Editor Development

<p align="center"><img src="ReadMeImages/VTLogo.jpg" alt="VTLogo" height="20%" width="30%" align="center" /></p>

## VTCustomPreview 
**VTCustomPreview** enhances your scene workflow by providing a **dynamic** **preview** directly within the hierarchy. Enjoy fluid navigation with rotation, scrolling, and panning using WASD/Arrow Keys to view objects from various angles. You can also adjust the lighting in the preview scene. The camera position and distance are automatically optimized to encompass all objects.

<p align="center"><img src="ReadMeImages/VTCustomPreview Demo.gif" alt="VTCustomPreview" title="VTCustomPreview" width="40%" /></p>

## VTRevealer 
**VTRevealer** simplifies the process of selecting a specific object from a group of overlapping ones. Pressing Cmd/Ctrl + Shift + Right Mouse Button on the line of sight in the scene view, and all the overlapping objects in that line will be listed in the window, allowing you to select and focus on the one you want. This tool can pick any objects that have a **concrete** bound (with Renderer or Collider) or have a logo (Camera, Directional Light, etc.). UI elements are also listed and if they are on a Canvas with **Overlay** render mode, they are also being sorted based on the **rendering precedence** (order in the hierarchy).

<img src="ReadMeImages/VTRevealerWindow Demo.gif" alt="VTRevealerWindow" title="VTRevealerWindow"/>

<img src="ReadMeImages/VTRevealerWindow Demo 2D.gif" alt="VTRevealerWindow" title="VTRevealerWindow" />

## VTHierarchy
Elevate Unity hierarchy clarity with alternating row shading, intuitive foldouts, and clear object indentations for a more organized and efficient workflow. Selecting objects at the same hierarchy level under the same parent can also be done through the right-click menu.
<p align="center"><img src="ReadMeImages/VTHierarchy Indentation Line.png" alt="VTHierarchy Indentation Line" height="30%" width="30%" /></p>

## VTSceneLoader 
**VTSceneLoader** allows you to manage and load various scenes in one place. You can assign each scene in the custom scene list a **tag** with an **icon** and **color**. To add a scene to the custom scene list, you can **choose** one from the selection popup or **drag** the scene directly to the “Custom Scene List” title. More functionalities can be accessed by pressing the “Three Dot” icon on the top right of the window. The button to the right of the **Load button** specifies whether the current scene should be added to build settings if **ApplyToBuildSettings** is pressed. To **open** this window, naviagte to Victor/Tools/Dev Window on the top tool bar. You can add custom icons to this folder **relative** to your project: Victor/VTools/Editor/VTUtilities/VTSceneLoader/Resources/VTSceneLoader Tag Icons. You can also customize the order of the icons displayed when adding them and more in ProjectSettings/VTSceneLoader tab. The custom scene list is being saved after assembly reload, but it's nice to save it manually by clicking the **Save** button.

<p align="center"><img src="ReadMeImages/VTSceneLoader Demo.gif" alt="VTSceneLoader Demo" height="30%" width="30%" /></p>

## VTPrefabLoader
**VTPrefabLoader** provides you with a place to hold and manage the prefabs that you care about the most. You can add them by choosing one from the selection popup or by dragging a bunch of them to the title of the secondary reorderable list directly. You can also drag from the preview icon of the prefab to the scene to add them quickly.

<p align="center"><img src="ReadMeImages/VTPrefabLoader Demo.gif" alt="VTPrefabLoader Demo" height="30%" width="30%"/></p>

## SnapToSurface
Allowing you to snap a 3D object with a renderer to the surface of other objects, making them touch each other. You can perform snap based on either the world axis or the local axis of the object. 

<p align="center"><img src="ReadMeImages/Snap To Surface World Axis Mode.png" alt="Snap To Surface World Axis Mode" height="30%" width="30%" /></p>

<p align="center"><img src="ReadMeImages/Snap To Surface Local Axis Mode.png" alt="Snap To Surface Local Axis Mode" height="30%" width="30%" /></p>

## VTEditorTween
An Editor tween library built for **simplicity**, **flexibility** and **extensibility**. Use static and extension methods for tweening various variable types like float, int, Vector2, and more. Customize tweens with ease by chaining everything and defining your unique way of tweening a variable.

### Usage
```C#
// Create a tween with static method and chain modifications one after another. The first parameter is the variable to tween, second is the setter you construct for that variable and the third is the target value of the tween
VTweenCreator.TweenVector3(m_WindowSize, newSize => m_WindowSize = newSize, targetSize).SetDuration(0.65f).SetEaseType(EaseType.EaseOutBack).SetOvershootOrAmplitude(0.65f).OnValueChanged(editorWindow.Repaint);

// Store tween reference, you can only chain applier settings right after the tween is created, this rounds float tween result to a multiple of 0.1f
VTweenCore tween = VTweenCreator.TweenFloat(m_TagsSpacing, newSpacing => m_TagsSpacing = newSpacing, 0f).SetFloatApplierSettings(0.1f).SetDuration(0.75f).SetInitialDelay(0.15f).SetEaseType(EaseType.EaseInOutQuart);

// Create a tween config object, chain settings after it and attach it to multiple tweens to apply those settings without the extra work of copying and pasting
VTweenConfig hueTweenConfig = new VTweenConfig();
hueTweenConfig.SetPlayStyle(PlayStyle.Normal).SetInfinite(true);

m_ObjTitleColorHueTween = VTweenCreator.TweenFloat(m_ObjTitleColorHue, newH => m_ObjTitleColorHue = newH, 0f).SetConfig(hueTweenConfig).SetDuration(5f).SetEaseType(EaseType.EaseInOutCirc);
m_UITitleColorHueTween = VTweenCreator.TweenFloat(m_UITitleColorHue, newHue => m_UITitleColorHue = newHue, 1f).SetConfig(hueTweenConfig).SetDuration(7f).SetEaseType(EaseType.Linear);

// This is a working example used for reset the camera and lighting in VTCustomPreview
// Note: you may want to remove previous tween of a variable before starting a new one for it to avoid conflict
if (m_HasModelPreview)
{
    m_PanTween?.Remove();
    m_PanTween = VTweenCreator.TweenVector3(m_CurrentPanDelta, pan => m_CurrentPanDelta = pan, Vector3.zero).SetDuration(VTVector3.Approximately(m_CurrentPanDelta, Vector3.zero, 0.001f) ? 0f : 0.5f).OnComplete(() =>
    {
        m_DistanceTween?.Remove();
        m_DistanceTween = VTweenCreator.TweenFloat(m_CurrentZoomLevel, zoom => m_CurrentZoomLevel = zoom, 1).SetDuration(0.75f).OnValueChanged(() =>
        {
            m_TargetZoomLevel = m_CurrentZoomLevel;
        });

        m_RotTween?.Remove();
        m_RotTween = VTweenCreator.TweenQuaternion(m_CurrentRot, rot => m_CurrentRot = rot, new Vector3(30, 50, 0)).SetDuration(0.75f).OnValueChanged(() =>
        {
            m_RotAroundX = m_CurrentRot.eulerAngles.x;
            m_RotAroundY = m_CurrentRot.eulerAngles.y;
        });

        m_LightTween?.Remove();
        m_LightTween = VTweenCreator.TweenFloat(lightIntensity, intensity => lightIntensity = intensity, 0.8f).SetDuration(0.5f).OnValueChanged(Repaint);
    });
}
```

### Extension
``` C#
// VTEditorTween comes with a generic method to tween a string, allowing you to provide custom tween applier without the need to create multiple functions like TweenString, TweenStringAdvanced, TweenStringNiceLooking etc.
public static VTween<string, string, TApplier> TweenString<TApplier>(string stringToTween, Action<string> setter, string target) where TApplier : TweenApplier
{
    VTween<string, string, TApplier> tween = new VTween<string, string, TApplier>(setter, stringToTween, target);
    return tween;
}

// Here is another non-generic built-in method that tweens a quaternion with custom settings
public static VTween<Quaternion, Vector3, QuaternionV3Applier> TweenQuaternion(Quaternion quaternionToTween, Action<Quaternion> setter, Vector3 target)
{
    VTween<Quaternion, Vector3, QuaternionV3Applier> tween = new VTween<Quaternion, Vector3, QuaternionV3Applier>(setter, quaternionToTween.eulerAngles, target);
    tween.m_ApplierSettings = new QuaternionV3ApplierSettings();
    return tween;
}
```
