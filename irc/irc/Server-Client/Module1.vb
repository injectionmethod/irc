Module Module1
    Public Name = "Admin/"
    Public method = "raw"
    Public padding = Nothing
    Public crypto = Nothing
    Public password As String = "DEFAULT_NETWORK_PASSWORD"
    Public CustomSchema
    Sub Main()
        If Environment.GetCommandLineArgs(1).ToLower.Contains("connect") Then
            Name = Environment.GetCommandLineArgs(2)
            method = Environment.GetCommandLineArgs(5)

            CheckMethod("@irc-client")

            EstablishConnection(Environment.GetCommandLineArgs(3), Convert.ToInt32(Environment.GetCommandLineArgs(4)))
        ElseIf Environment.GetCommandLineArgs(1).ToLower.Contains("host") Then
            Name += Environment.GetCommandLineArgs(2)
            method = Environment.GetCommandLineArgs(3).ToLower
            CheckMethod("@irc-host")

            Dim th As New Threading.Thread(AddressOf HostConnection)
            th.Start()
        ElseIf Environment.GetCommandLineArgs(1).ToLower.Contains("help") Then
            ShowHelp()
        End If
    End Sub
End Module
