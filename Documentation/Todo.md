# Todo List

### General Engine

    [4] [1] File -> New
    [4] [1] File -> Save As
    [4] [3] Build new File Browser in ImGui
    [2] [5] Undo
    [2] [2] Export UV Overlay PNG 
    [4] [2] Warning if Sector Size increases

### Animated Meshes

    [2] [8] Investigate Format
    [2] [2] Isolation Mode
    [2] [5] Rendering Animation
    [2] [2] Saving

### Resource Management

    [1] [3] Adding / Removing states
        Write GNS when saving
        Figure out how to number XFiles
            Maybe manually define XFile?
    [1] [4] Copy / Paste
    [1] [1] Add Has Invisibility Angles to the Resource Management Window
    [1] [1] Reformat so it can scroll horizontally better

### File Menu / Windows

    [4] [1] Help -> About Window
    [4] [2] Help -> Tips Window

### Rendering

    [5] [2] Game View shadow overlay
    [2] [3] Accurate rendering of Invisibility Angles when using camera controls
    [3] [2] Build Number-Border for Terrain
    [2] [2] Uv Animations Wrap Around (Map 101)
    [5] [2] Compass Rose with N-E-S-W on it

### Terrain

    [1] [1] Allow Alt-Click to change First Terrain Tile
    [2] [2] Render non-red tiles on top when both are the same

### Polygons

    [3] [1] Warn the user when more polys than the max are in the scene
    [4] [1] Snap Poly to Poly
    [4] [1] Snap Vertex to Vertex
    [5] [3] Using the widget with Alt+drag the panel will drift away from the mouse
    [5] [2] Import from OBJ
    [3] [2] Copy and Paste Polygon Selection 

### Bugs

    [1] [1] Terrain, resizing after adding terrain to another state
    [1] [1] Turn off Resize Terrain Mode on tab switch / state switch / map load
    [1] [1] Copying a Triangle's UV to a Quad's UV causes a crash

## Investigation

    - Map 41. Overridden Mesh File possibly linked to Removeable Objects
    - Figure out polygon max count and add it to the UI
    - Map 92 & 104. Palette Animation only plays once in game