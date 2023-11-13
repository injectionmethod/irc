Module GUIHandler

    Public RainbowThread As Threading.Thread = Nothing
    Function ShowHelp()
        Console.WriteLine("Connected " + Console.Title.ToString)
        Console.WriteLine("methods: host | connect")
        Console.WriteLine(vbNewLine)
        Console.WriteLine("ciphers:")
        Console.WriteLine("bobo | ascii | ctx | raw | dbl | cust | des | des-padding")
        Console.WriteLine("yab  |  kyt  | occ | jmk | ccs | qea  | aes-padding")
        Console.WriteLine(vbNewLine + "note: when using des with padding, use this format")
        Console.WriteLine("format example: des-padding ctx password" + vbNewLine + vbNewLine)
        Console.WriteLine("note: when using custom chars, make sure to set the file location")
        Console.WriteLine("format example: cust C:/chars.txt")
        Console.WriteLine(vbNewLine + vbNewLine + "- host usage")
        Console.WriteLine("irc host nickname cipher_choice")
        Console.WriteLine(vbNewLine + "- connection usage")
        Console.WriteLine("irc connect nickname ip port cipher_choice")
        Console.WriteLine(vbNewLine + vbNewLine + "- commands")
        Console.WriteLine("/method > change method > example: method ctx")
        Console.WriteLine("/padding > change padding > example: padding utf")
        Console.WriteLine("/password > change password > example: password testing123")
        Console.WriteLine("/color > set text color > example: color red")
        Console.WriteLine("/max > max clients connected at one time > example: max 10")
        Console.WriteLine("/rules > read server rules")
    End Function
    Function ShowRules()
        Console.WriteLine("General IRC rules for this program:")
        Console.WriteLine("1. No referencing real events with full names")
        Console.WriteLine("2. Quite a few people use this, no snitching on fellow users")
        Console.WriteLine("3. This is an underground communication network, exposing it is uncouth and won't end well")
        Console.WriteLine("4. Admins can access only the ip that you provide in terms of checks, it doesnt mean they can't demask you by the way you type or your nickname")
        Console.WriteLine("5. Don't associate us as a group, you connect to this service as an individual")
        Console.WriteLine("6. Talk about the sale of narcotics,firearms and software are permitted, being stupid however will result in blacklisting")
        Console.WriteLine("7. Under no circumstances will innapropriate content/talk regarding minors of any sort be tolerated")
        Console.WriteLine("8. The application does work its just complex to use, its not our fault you can't set it up, be patient you will get there!")
        Console.WriteLine("- Not a rule but some good information, never share your personal information or your location. as secure as this is, people could still get in via a weak link if all criteria was met")
    End Function
    Function SetTitle(var1, var2)
        Console.Title = "@irc | " + var1 + " | Method {" + var2 + "}"
        Return Nothing
    End Function
    Function checkColor(st As String) As ConsoleColor
        Dim _Color As ConsoleColor = ConsoleColor.Gray
        If st.ToLower.Contains("red") Or st = "1" Then
            _Color = ConsoleColor.Red
        ElseIf st.ToLower.Contains("green") Or st = "2" Then
            _Color = ConsoleColor.Green
        ElseIf st.ToLower.Contains("blue") Or st = "3" Then
            _Color = ConsoleColor.Blue
        ElseIf st.ToLower.Contains("cyan") Or st = "4" Then
            _Color = ConsoleColor.Cyan
        ElseIf st.ToLower.Contains("purple") Or st = "5" Then
            _Color = ConsoleColor.Magenta
        ElseIf st.ToLower.Contains("gold") Or st = "6" Then
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
                If Not f = ConsoleColor.Black And Not f = ConsoleColor.DarkGray And Not f = ConsoleColor.Gray And Not f = ConsoleColor.White Then
                    Console.ForegroundColor = f
                    Threading.Thread.SpinWait(300)
                End If
            Next
        End While
    End Sub

    Dim GetUserTagRandom As New Random()

    Function GetUserTagForThisSession() As String
        Dim randomNumber As Integer = GetUserTagRandom.Next(1000, 10000)
        Dim result As String = "#" & randomNumber.ToString()
        Return result
    End Function

End Module
