Module irc
    Public Name = "Admin/"
    Public method = StrReverse("war")
    Public padding = Nothing
    Public crypto = Nothing
    Public password As String = StrReverse("DROWSSAP_KROWTEN_TLUAFED")
    Public CustomSchema
    Public Port As Integer = 0
    Public RunUpToSpeed = False ' Keep disabled, used to store server logs and send them to new clients so they can view old chat, not recomended for security
    Public L As New List(Of String)
    Public ServerTag As String = StrReverse("EDON_PANZ_GAT_FED")
    Public SessionBan As New List(Of String)

    Sub Main()
        If Not Environment.GetCommandLineArgs(2).Contains("/") Or Environment.GetCommandLineArgs(2).Contains("\") Then
            If Environment.GetCommandLineArgs(1).ToLower.Contains("connect") Then
                Name = Environment.GetCommandLineArgs(2) + " " + GetUserTagForThisSession()
                method = Environment.GetCommandLineArgs(5)

                CommandsHandler.CheckMethod("@irc-client")

                EstablishConnection(Environment.GetCommandLineArgs(3), Convert.ToInt32(Environment.GetCommandLineArgs(4)))
            ElseIf Environment.GetCommandLineArgs(1).ToLower.Contains("host") Then
                Console.Write("Enter Host Port: ")
                Port = Convert.ToInt32(Console.ReadLine)
                Name += Environment.GetCommandLineArgs(2) + " " + GetUserTagForThisSession()
                method = Environment.GetCommandLineArgs(3).ToLower

                CommandsHandler.CheckMethod("@irc-host")

                Dim th As New Threading.Thread(AddressOf HostConnection)
                th.Start()
            ElseIf Environment.GetCommandLineArgs(1).ToLower.Contains("help") Or Environment.GetCommandLineArgs(1).ToLower.StartsWith("h") Or Environment.GetCommandLineArgs(1).ToLower.StartsWith("-h") Then
                ShowHelp()
            End If
        End If
    End Sub

End Module
