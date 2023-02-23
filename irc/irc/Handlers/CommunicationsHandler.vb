Imports System.IO
Imports System.Net
Imports System.Net.Sockets

Module CommunicationsHandler
    Dim EstablishedToServer As New TcpClient

    Dim ListenerServer As TcpListener
    Dim ClientsList As New List(Of TcpClient)
    Dim LastClient As TcpClient
    Dim _max_Clients As Integer = 10

    'Client
    Function EstablishConnection(irc_address As String, irc_port As Integer)
        Dim irc_server As New TcpClient
        Console.WriteLine("Establishing Conection to " + irc_address + "...")
        Threading.Thread.SpinWait(300)
        irc_server.Connect(irc_address, irc_port)
        If irc_server.Connected Then
            Console.WriteLine("Connected to " + irc_address)
            Console.Title = "@irc | " + irc_address + " | Method {" + method + "}"
            EstablishedToServer = irc_server
            Dim th As New Threading.Thread(AddressOf HandleMessages)
            th.Start()
            Dim t2 As New Threading.Thread(AddressOf SendMessages)
            t2.Start()
        End If
    End Function
    Sub HandleMessages()
        If EstablishedToServer IsNot Nothing Then
            Dim sr = New StreamReader(EstablishedToServer.GetStream)
            While True
                Try
                    Dim resp = DecideMethod(sr.ReadLine, False, method)
                    If resp.Contains("@" + Name) Then
                        resp = "INCLUDED YOU |" + resp
                        Beep()
                    End If
                    If resp.Length > 0 Then
                        Console.WriteLine(DateTime.Now.ToShortTimeString + " |" + resp)
                    End If
                Catch ex As Exception
                    If EstablishedToServer.Connected = False Then
                        Console.WriteLine("{+} Server Exception, You Have Been Disconnected | _0xHR:" + ex.HResult)
                    End If
                    Exit While
                End Try
            End While
        End If
    End Sub
    Sub SendMessages()
        Dim SW As New StreamWriter(EstablishedToServer.GetStream) : SW.AutoFlush = True
        While EstablishedToServer IsNot Nothing
            Dim resp = Console.ReadLine
            If resp.StartsWith("/") Then
                If resp.Contains("/method") Then
                    Dim TempMeth = resp.Split(" ")(1)
                    method = TempMeth
                    Console.WriteLine("Method Changed To " + TempMeth)
                End If

                If resp.Contains("/padding") Then
                    Dim TempPadding = resp.Split(" ")(1)
                    padding = TempPadding
                    Console.WriteLine("Padding Changed To " + TempPadding)
                End If

                If resp.Contains("/color") Then
                    Console.ForegroundColor = checkColor(resp.Split(" ")(1))
                    Console.WriteLine("{+} Color Changed")
                End If

            Else
                SW.WriteLine(DecideMethod(" " + Name + " | " + resp, True, method))
            End If

            SetTitle(EstablishedToServer.Client.RemoteEndPoint.ToString.Split(":")(0), method)
        End While
    End Sub

    Function SendHeaders(cli As TcpClient)
        Dim Writer As New StreamWriter(cli.GetStream) : Writer.AutoFlush = True
        Writer.WriteLine(DecideMethod(" Users Online {" + ClientsList.Count.ToString + "}", True, method))
    End Function
    ' Admin
    Sub HostConnection()
        Dim irc_server As New TcpListener(IPAddress.Any, 7832)
        Console.WriteLine("server | binded @ " + DateTime.Now.ToShortTimeString)
        irc_server.Start()
        Console.WriteLine("server | started @ " + DateTime.Now.ToShortTimeString)
        If crypto = Nothing Then
            Console.WriteLine(DecideMethod(DecideMethod("server | using cipher-" + method, True, method), False, method))
        Else
            Dim d = DecideMethod("server | using cipher-" + method, True, method)
            Console.WriteLine(DecideMethod(d, False, method))
            Dim e = DecideMethod("server | using padding-" + padding, True, method)
            Console.WriteLine(DecideMethod(e, False, method))
        End If

        Dim AdminType As New Threading.Thread(AddressOf AdminCanType)
        AdminType.Start()
        ListenerServer = irc_server
        Console.WriteLine("server | listening for connections @ " + DateTime.Now.ToShortTimeString)
        Console.Title = "@irc | Hosting Server | Method {" + method + "}"
        While True
            If irc_server.Pending = True Then
                If ClientsList.Count < _max_Clients Then
                    Dim Clive = irc_server.AcceptTcpClient
                    SendHeaders(Clive)
                    ClientsList.Add(Clive)
                    LastClient = Clive
                    Dim th As New Threading.Thread(AddressOf HandleConnections)
                    th.Start()
                End If
            End If

            SetTitle("Hosting Server", method)
        End While
    End Sub
    Sub AdminCanType()
        While True
            Dim resp = Console.ReadLine
            If resp.StartsWith("/") Then

                If resp.Contains("/color") Then
                    Console.ForegroundColor = checkColor(resp.Split(" ")(1))
                    Console.WriteLine("{+} Color Changed")
                End If

                If resp.Contains("/method") Then
                    Dim TempMeth = resp.Split(" ")(1)
                    SendAdminMsg("Server | Method Updated, Please Set Or Re-Join And Use The " + TempMeth + " Method")
                    method = TempMeth
                    Console.WriteLine("Method Changed To " + TempMeth)
                End If

                If resp.Contains("/padding") Then
                    Dim TempPadding = resp.Split(" ")(1)
                    SendAdminMsg("Server | Padding Updated, Please Set Or Re-Join And Use The " + TempPadding + " Choice")
                    padding = TempPadding
                    Console.WriteLine("Padding Changed To " + padding)
                End If

                If resp.Contains("/max") Then
                    _max_Clients = Convert.ToInt32(resp.Split(" ")(1))
                    Console.WriteLine("{+} Client Capacity Set To " + resp.Split(" ")(1))
                End If
                If resp.Contains("/clients") Then
                    Console.WriteLine("{" + ClientsList.Count.ToString + "} Clients:")
                    For Each c In ClientsList
                        Console.WriteLine(c.Client.RemoteEndPoint.ToString.Split(":")(0))
                    Next
                End If

            Else
                If resp.Length > 0 Then
                    SendAdminMsg(resp)
                End If
            End If
        End While
    End Sub
    Sub SendAdminMsg(resp As String)
        For Each cl As TcpClient In ClientsList
            Dim SW As New StreamWriter(cl.GetStream) : SW.AutoFlush = True
            SW.WriteLine(DecideMethod(" " + Name + " | " + resp, True, method))
        Next
    End Sub
    Sub HandleConnections()
        Dim Clive = LastClient
        Dim sr As New StreamReader(Clive.GetStream)
        Console.WriteLine(DateTime.Now.ToShortTimeString + " | Client Connected " + Clive.Client.RemoteEndPoint.ToString)
        While True
            Try
                Dim resp = DecideMethod(sr.ReadLine, False, method)
                If resp.Length > 0 Then
                    Console.WriteLine(DateTime.Now.ToShortTimeString + " |" + resp)
                    For Each cl As TcpClient In ClientsList
                        If cl IsNot Clive Then
                            Dim SW As New StreamWriter(cl.GetStream) : SW.AutoFlush = True
                            SW.WriteLine(DecideMethod(resp, True, method))
                        End If
                    Next
                End If
            Catch ex As Exception
                Console.WriteLine(DateTime.Now.ToShortTimeString + " | Client Disconnected " + Clive.Client.RemoteEndPoint.ToString)
                ClientsList.Remove(Clive)
                Exit While
            End Try
        End While
    End Sub
End Module
