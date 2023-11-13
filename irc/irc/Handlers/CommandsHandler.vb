Module CommandsHandler
    Public Function CheckMethod(type As String)
        If type = "@irc-host" Then

            If method = "cust" Then
                CustomSchema = Environment.GetCommandLineArgs(4)
            ElseIf method.ToString.Contains("des") Then
                If method = "des-padding" Then
                    padding = Environment.GetCommandLineArgs(4)
                    password = Environment.GetCommandLineArgs(5)
                    crypto = "des"
                Else
                    padding = "none"
                    password = Environment.GetCommandLineArgs(4)
                    crypto = "des"
                End If
            ElseIf method.ToString.Contains("aes") Then
                If method = "aes-padding" Then
                    padding = Environment.GetCommandLineArgs(4)
                    password = Environment.GetCommandLineArgs(5)
                    crypto = "aes"
                End If
            End If
        End If

        If type = "@irc-client" Then
            If method = "cust" Then
                CustomSchema = Environment.GetCommandLineArgs(6)
            ElseIf method.ToString.Contains("des") Then
                If method = "des-padding" Then
                    padding = Environment.GetCommandLineArgs(6)
                    password = Environment.GetCommandLineArgs(7)
                    crypto = "des"
                Else
                    padding = "none"
                    password = Environment.GetCommandLineArgs(6)
                    crypto = "des"
                End If
            ElseIf method.ToString.Contains("aes") Then
                If method = "aes-padding" Then
                    padding = Environment.GetCommandLineArgs(6)
                    password = Environment.GetCommandLineArgs(7)
                    crypto = "aes"
                End If
            End If
        End If
        Return type
    End Function
End Module
