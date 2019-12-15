# UltraDMD

FlexDMD was created since UltraDMD was not updated any more. Therefore, it is designed to easily replace UltraDMD on existing tables.

## Converting a table from UltraDMD to FlexDMD

To convert a table to FlexDMD, load the table in VPX, open the script, then find a line containing `Set UltraDMD = CreateObject("UltraDMD.DMDObject")` and replace it with `Set UltraDMD = CreateObject("FlexDMD.DMDObject")`.

Additionally, you should share the game name with the render device (This allows virtual DMD to adapt their rendering to the played games with contextual frame,...). To do so, simply replace the `UltraDMD.Init` by `UltraDMD.Init cGameName`.

As an example, here is the modification for the [great Diablo III table](https://www.vpforums.org/index.php?app=downloads&showfile=12750) from JPSalas:
```VBScript
Sub DMD_Init
    Set UltraDMD = CreateObject("FlexDMD.DMDObject")
    'Set UltraDMD = CreateObject("UltraDMD.DMDObject")
    If UltraDMD is Nothing Then
        MsgBox "No UltraDMD found.  This table will NOT run without it."
        Exit Sub
    End If

    UltraDMD.Init cGameName
    If Not UltraDMD.GetMajorVersion = 1 Then
        MsgBox "Incompatible Version of UltraDMD found."
        Exit Sub
    End If
	
    ...
	
End Sub
```

## Differences between UltraDMD and FlexDMD
Since the license from UltraDMD was unclear, FlexDMD was developed from scratch, without reusing any code or assets from UltraDMD. The only common part is the API. Therefore, you can expect a few differences when switching from one to the other. The main one are listed below;
* FlexDMD uses its own fonts, which are not the one from UltraDMD. We are working to find the best matching ones (you can help !) but there are still differences.
* UltraDMD uses only fixed width fonts whereas FlexDMD uses fixed or variable width fonts, hence leading to glyph layout differences.
* FlexDMD animates background of the scoreboard where UltraDMD wont, even if the background is a video.
* By default, FlexDMD renders score of the scoreboard with a comma separator (1000000 renders as 1,000,000). UltraDMD does not render with comma separator.
* FlexDMD is an "in process COM object". UltraDMD is an "out of process COM object". Not sure if this will cause any difference but it might impact the object life cycle.
