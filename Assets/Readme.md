Multiple Screen Dialogue

This project offers the tools to create a Dialogue Window that can be dragged across multiple screens.

Requirements:
=
- Window must support Header (text), Button, and Toggle elements
- Window can be dragged from one screen to another
- Window's Button and Toggle elements are responsive, both functionally and visibly, at all times.

Bonus Features:

- Multiple, looping displays: the solution works for more than 2 screens, and allows for the final display to loop back
  on the 1st one, achieving a surround system, and allowing the user to continuously drag the Window across displays to
  their heart's content
- Different Display sizes: the solution works very well with different display **width**, and works acceptable with
  different display **heights** (some scaling should apply)
- Different Window anchors (except stretching), pivots, and scale
- Dragged Window should always be fully visible on top of other elements, and there should be no clipping/overlapping/Z-fights for elements displayed on both screens, regardless of their order in their respective hierarchies

Prerequisites:
-
- Use multiple ScreenSpace Canvases, with multiple Cameras
- Each Camera displays one Canvas on one Display
- All Canvases use the same scaling and rendering parameters
- All Displays are aligned horizontally (no vertical transitions are supported at this time)
- All Windows should be parented under the same\* common Transform under their respective Canvases *(the root transforms need to have the same component values)* 
- Windows **do not** use a stretch anchor


Solution:
=
 The provided solution offers 2 scripts that the user can integrate to achieve the requirements:
 > MultiScreenCanvas
 >  - Add this component to your Canvas. 
 >  - Specify which Canvas should be to the Right and Left of the current canvas respectively, or leave null for none (this will behave like a solid screen edge)
 >  - Specify the Root for your Windows, or leave blank (this will use the Canvas as the Root).

> MultiScreenWindow
>  - Add this script to your Window
>  - This window can now be dragged across the MultiScreenCanvases, consistently responding to user input
>  - Enjoy :)

---
The solution creates a Copy of the Window when the Window is dragged out of the edges of its canvas. While both the original and the copy are active, any events occuring on one must be mirrored (visually, but not functionally) by the other. After the Window completely moves into a canvas, the copy (or the original, which is seen as the copy of the copy) is disabled, and any drag event is passed to the remaining Window.

The MultiScreenWindow hooks to its Canvas, and responds to Drag events:
- when dragged, it will check for clipping with the left and right edges of its parent canvas
- if the limits are breached, it creates a Copy of itself
- - both the original and the copy will add MultiScreenComponents to their responsive elements (Button, Toggle)
- - these components are responsible for passing user event parameters between the copy and the original, ensuring both Windows "respond" to user input, even if one only handles the visual part.

Check code comments for additional explanations.

Notes:
=
- Toggle Groups get broken if an element is dragged to a new canvas: this basically introduces a new element in the group, which, if active (isOn = true) will disable the original one.
- A performance spike can be noticed when a Copy is created. To make the scripts very easy to use, a lot of components are added dynamically, at a great cost. Suggestions for improving this are provided in the comments
- Changing the size and/or position of the devices after startup is not support, but can be added