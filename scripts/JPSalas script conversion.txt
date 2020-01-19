To add FlexDMD to a recent (late 2019, beginning of 2020) JPSalas table, follow these 3 simple steps ;

1. Replace DMD_Init method's beginning with the following
=========================================================

Dim FlexDMD
Dim DMDScene

Sub DMD_Init() 'default/startup values
	Set FlexDMD = CreateObject("FlexDMD.FlexDMD")
	If Not FlexDMD is Nothing Then
		FlexDMD.TableFile = Table1.Filename & ".vpx"
		FlexDMD.RenderMode = 2
		FlexDMD.Width = 128
		FlexDMD.Height = 36
		FlexDMD.GameName = cGameName
		FlexDMD.Init
		Set DMDScene = FlexDMD.NewGroup("Scene")
		DMDScene.AddActor FlexDMD.NewImage("Back", "VPX.bkempty")
		DMDScene.GetImage("Back").SetSize FlexDMD.Width, FlexDMD.Height
		DigitsBack(0).Visible = False
		For i = 0 to 35
			DMDScene.AddActor FlexDMD.NewImage("Dig" & i, "VPX.dempty&dmd=2")
			Digits(i).Visible = False
		Next
		For i = 0 to 19 ' Bottom
			DMDScene.GetImage("Dig" & i).SetBounds 4 + i * 6, 3 + 16 + 2, 8, 8
		Next
		For i = 20 to 35 ' Top
			DMDScene.GetImage("Dig" & i).SetBounds (i - 20) * 8, 3, 8, 16
		Next
		FlexDMD.LockRenderThread
		FlexDMD.Stage.AddActor DMDScene
		FlexDMD.UnlockRenderThread
	End If

2. Replace the end of DMDUpdate and DMDDisplayChar with the following
=====================================================================

Sub DMDUpdate(id)
    Dim digit, value
	If Not FlexDMD is Nothing Then FlexDMD.LockRenderThread
    Select Case id
        Case 0 'top text line
            For digit = 20 to 35
                DMDDisplayChar mid(dLine(0), digit-19, 1), digit
            Next
        Case 1 'bottom text line
            For digit = 0 to 19
                DMDDisplayChar mid(dLine(1), digit + 1, 1), digit
            Next
        Case 2 ' back image - back animations
            If dLine(2) = "" OR dLine(2) = " " Then dLine(2) = "bkempty"
            DigitsBack(0).ImageA = dLine(2)
			If Not FlexDMD is Nothing Then DMDScene.GetImage("Back").Bitmap = FlexDMD.NewImage("", "VPX." & dLine(2) & "&dmd=2").Bitmap
    End Select
	If Not FlexDMD is Nothing Then FlexDMD.UnlockRenderThread
End Sub

Sub DMDDisplayChar(achar, adigit)
    If achar = "" Then achar = " "
    achar = ASC(achar)
    Digits(adigit).ImageA = Chars(achar)
	If Not FlexDMD is Nothing Then DMDScene.GetImage("Dig" & adigit).Bitmap = FlexDMD.NewImage("", "VPX." & Chars(achar) & "&dmd=2").Bitmap
End Sub

3. In Table1_Exit add the FlexDMD uninit line
=============================================

Sub Table1_Exit
    Savehs
	If Not FlexDMD is Nothing Then FlexDMD.Uninit
    If B2SOn = true Then Controller.Stop
End Sub
