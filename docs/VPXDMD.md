# Embedding the DMD inside VPX

Recent versions of Visual Pinball allow to direclty render the DMD on the playfield output, for example for FSS or desktop layouts, allowing to run in exclusive fullscreen mode without problems.  A great example is the [Cirqus Voltaire table](https://vpinball.com/VPBdownloads/cirqus-voltaire-bally-1997/) by Knorr, Clark Kent, Randr, Dark.

FlexDMD also supports this feature. In this situation, FlexDMD renders the DMD and sends back the dot matrix to Visual Pinball for final rendering.

To add an embedded DMD to your table, follow these steps;
* You should start with a working table, that uses FlexDMD and successfully renders through dmddevice.dll to a virtual or real DMD device
* Add a Flasher object, and check its 'Use Script DMD' checkbox
* Add a Timer object, name it FlexDMDTimer, enable it, set its Timer Interval to -1
* In the script, after FlexDMD object creation, add either of `UseDMD = true` (for monochrome) or `UseColoredDMD = true` (for color)
* In the script add the following snippet:
```VBScript
' Update the embedded DMD inside VPX. This requires the table to have a timer object, named 'FlexDMDTimer', enabled, with a timer interval of -1 (VPX > 10.2)
Sub FlexDMDTimer_Timer
	Dim DMDp
	If UseDMD Then
		DMDp = FlexDMD.DmdPixels
		If Not IsEmpty(DMDp) Then
			DMDWidth = FlexDMD.Width
			DMDHeight = FlexDMD.Height
			DMDPixels = DMDp
		End If
	ElseIf UseColoredDMD Then
		DMDp = FlexDMD.DmdColoredPixels
		If Not IsEmpty(DMDp) Then
			DMDWidth = FlexDMD.Width
			DMDHeight = FlexDMD.Height
			DMDColoredPixels = DMDp
		End If
	End If
End Sub
```

<br></br>As an example, applying these changes to JPSalas' Diablo III table, gives the following result (top left DMD is rendered through FlexDMD/dmddevice.dll, bottom one is rendered by VPX):
<br></br>![Visual Pinball Architecture](./media/diablo-embedded.png)
