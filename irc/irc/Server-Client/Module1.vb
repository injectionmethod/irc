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
            Console.WriteLine("methods: host | connect")
            Console.WriteLine("ciphers: bobo | ascii | ctx | stx | raw | dbl | cust | des | des-padding")
            Console.WriteLine(vbNewLine + "note: when using des with padding, use this format")
            Console.WriteLine("format example: des-padding ctx password" + vbNewLine + vbNewLine)
            Console.WriteLine("note: when using custom chars, make sure to set the file location")
            Console.WriteLine("format example: cust C:/chars.txt")
            Console.WriteLine(vbNewLine + vbNewLine + "- host usage")
            Console.WriteLine("irc host nickname cipher_choice")
            Console.WriteLine(vbNewLine + "- connection usage")
            Console.WriteLine("irc connect nickname ip port cipher_choice")
        End If
    End Sub
End Module
