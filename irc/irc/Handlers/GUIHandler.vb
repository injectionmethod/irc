Module GUIHandler

    Public RainbowThread As Threading.Thread = Nothing
    Function SetTitle(var1, var2)
        Console.Title = "@irc | " + var1 + " | Method {" + var2 + "}"
        Return Nothing
    End Function
    Function checkColor(st As String) As ConsoleColor
        Dim _Color As ConsoleColor = ConsoleColor.Gray
        If st.ToLower.Contains("red") Or st = "12" Then
            _Color = ConsoleColor.Red
        ElseIf st.ToLower.Contains("green") Or st = "10" Then
            _Color = ConsoleColor.Green
        ElseIf st.ToLower.Contains("blue") Or st = "9" Then
            _Color = ConsoleColor.Blue
        ElseIf st.ToLower.Contains("cyan") Or st = "11" Then
            _Color = ConsoleColor.Cyan
        ElseIf st.ToLower.Contains("purple") Or st = "13" Then
            _Color = ConsoleColor.Magenta
        ElseIf st.ToLower.Contains("gold") Or st = "14" Then
            _Color = ConsoleColor.Yellow
        ElseIf st.ToLower.Contains("rainbow") Then
            Dim th2 As New Threading.Thread(AddressOf DoRainbow)
            RainbowThread = th2
            RainbowThread.Start()
        End If
        If Not st = "rainbow" Then
            If RainbowThread IsNot Nothing Then
                RainbowThread.Abort()
            End If
        End If
        If Not _Color = Nothing Then
            Return _Color
        Else
            _Color = ConsoleColor.Gray : Return _Color
        End If
    End Function

    Sub DoRainbow()
        While True
            Dim colors As ConsoleColor() = CType(ConsoleColor.GetValues(GetType(ConsoleColor)), ConsoleColor())
            For Each f As ConsoleColor In colors
                If Not f = ConsoleColor.Black And Not f = ConsoleColor.DarkGray Then
                    Console.ForegroundColor = f
                    Threading.Thread.SpinWait(300)
                End If
            Next
        End While
    End Sub
End Module
