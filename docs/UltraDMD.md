# UltraDMD

FlexDMD was created since UltraDMD was not updated any more. Therefore, it is designed to easily replace UltraDMD on existing tables.

## Converting a table from UltraDMD to FlexDMD

To convert a table to FlexDMD, load the table in VPX, open the script, then find a line containing `Set UltraDMD = CreateObject("UltraDMD.DMDObject")` and replace it with `Set UltraDMD = CreateObject("FlexDMD.DMDObject")`.

Additionally, you should share the game name with the render device (This allows virtual DMD to adapt their rendering to the played games with contextual frame,...). To do so, simply add `UltraDMD.GameName = cGameName` before `Init`.

Finally, UltraDMD does not define if the table is monochrome or color from the script but from its UI. FlexDMD lets this choice to the table designer. So, to enable colors, you have to add `UltraDMD.RenderMode = 2` before `Init`.

As an example, here is the modification for the [great Diablo III table](https://www.vpforums.org/index.php?app=downloads&showfile=12750) from JPSalas:
```VBScript
Sub DMD_Init
    Set UltraDMD = CreateObject("FlexDMD.DMDObject")
    'Set UltraDMD = CreateObject("UltraDMD.DMDObject")
    If UltraDMD is Nothing Then
        MsgBox "No UltraDMD found.  This table will NOT run without it."
        Exit Sub
    End If

    UltraDMD.GameName = cGameName
    UltraDMD.RenderMode = 2
    UltraDMD.Init
    If Not UltraDMD.GetMajorVersion = 1 Then
        MsgBox "Incompatible Version of UltraDMD found."
        Exit Sub
    End If
	
    ...
	
End Sub
```
