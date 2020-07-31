# UltraDMD

Among other needs, FlexDMD was created since the great UltraDMD sadely has a few bugs and it was not updated anymore by its author. Therefore, it is designed to easily replace UltraDMD on existing tables.

## Running an UltraDMD table

The easiest way it to choose to register FlexDMD as UltraDMD when installing it (using the companion application). In this case, **you do not need to do anything else; all UltraDMD tables will just work.**

When running like this;
- to position the DMD, if you are using Freezy's DMD, simply run the table, right click on the DMD and select the option to save its position,
- to select wether to run in full color or in monochrome, simply run FlexDMD's companion application and use the UltraDMD configuration tab,
- to select the text and monochrome color, simply run FlexDMD's companion application and use the UltraDMD configuration tab.

That's all !


## Converting a table from UltraDMD to FlexDMD

The following instruction are for advanced users who want to try FlexDMD features on existing UltraDMD tables. Note that the script folder of this repository contains scripts which includes the changes below for a few popular tables.

To convert a table to FlexDMD, load the table in VPX, open the script, then find a line containing `Set UltraDMD = CreateObject("UltraDMD.DMDObject")` and replace it with `Set FlexDMD = CreateObject("FlexDMD.FlexDMD")`.

You should share the game name with the render device (This allows virtual DMD to adapt their rendering to the played table with per table layout, contextual frame,...). To do so, simply add `FlexDMD.GameName = cGameName` before `Init`.

UltraDMD does not define if the table is monochrome or color from the script but from its UI. FlexDMD lets this choice to the table designer. So, to enable colors, you have to add `FlexDMD.RenderMode = 2` before `Init`.

Finally, create the UltraDMD's API object by calling `NewUltraDMD()` and call `Init`.

As an example, here is the modification for the [great Diablo III table](https://www.vpforums.org/index.php?app=downloads&showfile=12750) from JPSalas:
```VBScript
Sub DMD_Init
    'Set UltraDMD = CreateObject("UltraDMD.DMDObject")

    Dim FlexDMD
    Set FlexDMD = CreateObject("FlexDMD.FlexDMD")
    If FlexDMD is Nothing Then
        MsgBox "No UltraDMD found.  This table will NOT run without it."
        Exit Sub
    End If
    FlexDMD.GameName = cGameName
    FlexDMD.RenderMode = 2
    Set UltraDMD = FlexDMD.NewUltraDMD()
    UltraDMD.Init
    
    If Not UltraDMD.GetMajorVersion = 1 Then
        MsgBox "Incompatible Version of UltraDMD found."
        Exit Sub
    End If
	
    ...
	
End Sub
```

If the table is missing UltraDMD Uninit call on exit, you should also add it in `table1_Exit` like this:
```VBScript
Sub table1_Exit
    If Not UltraDMD is Nothing Then
        If UltraDMD.IsRendering Then
            UltraDMD.CancelRendering
        End If
        UltraDMD.Uninit
        UltraDMD = NULL
    End If
    ...
End Sub
```

