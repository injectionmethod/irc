Module Cryptography
    Public Function DoCrypto(st As String, DoEncrypt As Boolean, method As String) As String
        Dim bb As String = st
        If method = "des" Then
            If DoEncrypt = True Then
                bb = Convert.ToBase64String(EncryptDES_CBC(bb))
            Else
                bb = (System.Text.Encoding.UTF8.GetString(DecryptDES_CBC(Convert.FromBase64String(bb))))
            End If
        End If

        If method = "des-padding" Then
            If DoEncrypt = True Then
                bb = Convert.ToBase64String(EncryptDES_CBC(DecideMethod(DecideMethod(bb, True, "bobo"), True, padding)))
            Else
                bb = System.Text.Encoding.UTF8.GetString(DecryptDES_CBC(Convert.FromBase64String(bb))) : bb = DecideMethod(bb, False, padding) : bb = DecideMethod(bb, False, "bobo")
            End If
        End If

        Return bb
    End Function
    Public Function DecryptDES_CBC(ByVal FileBytes As Byte()) As Byte()
        Dim engine As New Org.BouncyCastle.Crypto.Engines.DesEngine()
        Dim keyz As Byte() = System.Text.Encoding.UTF8.GetBytes(password)
        Dim cipher As Org.BouncyCastle.Crypto.BufferedBlockCipher = New Org.BouncyCastle.Crypto.Paddings.PaddedBufferedBlockCipher(New Org.BouncyCastle.Crypto.Modes.CbcBlockCipher(engine))
        cipher.Init(False, New Org.BouncyCastle.Crypto.Parameters.KeyParameter(keyz))
        Dim rv As Byte() = New Byte(cipher.GetOutputSize(FileBytes.Length) - 0) {}
        Dim Final As Byte() = FileBytes
        Dim ProcessedBytes As Integer = cipher.ProcessBytes(Final, 0, FileBytes.Length, rv, 0)
        Try
            cipher.DoFinal(rv, ProcessedBytes)
        Catch ce As Exception
            MsgBox(ce.Message)
        End Try
        Return rv
    End Function
    Public Function EncryptDES_CBC(ByVal FileBytes As String) As Byte()
        Dim engine As New Org.BouncyCastle.Crypto.Engines.DesEngine()
        Dim keyz As Byte() = System.Text.Encoding.UTF8.GetBytes(password)
        Dim ptBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(FileBytes)
        Dim cipher As Org.BouncyCastle.Crypto.BufferedBlockCipher = New Org.BouncyCastle.Crypto.Paddings.PaddedBufferedBlockCipher(New Org.BouncyCastle.Crypto.Modes.CbcBlockCipher(engine))
        cipher.Init(True, New Org.BouncyCastle.Crypto.Parameters.KeyParameter(keyz))
        Dim rv As Byte() = New Byte(cipher.GetOutputSize(ptBytes.Length) - 1) {}
        Dim ProcessedBytes As Integer = cipher.ProcessBytes(ptBytes, 0, ptBytes.Length, rv, 0)
        Try
            cipher.DoFinal(rv, ProcessedBytes)
        Catch ce As Exception
        End Try
        Return rv
    End Function
    Public Function DecideMethod(st As String, b As Boolean, method As String) As String
        Dim bb As String
        If method = "raw" Then 'RAW METHOD - Default
            bb = st
        ElseIf method.Contains("ascii") Then 'ASCII METHOD - Cipher Method 1 - {ASCII}
            bb = DoCipher(st, b, GetASCIIChars)
        ElseIf method = "utf" Then 'UNICODE METHOD - Cipher Method 2 - {UNICODE}
            bb = DoCipher(st, b, GetUTF8Chars)
        ElseIf method = "bobo" Then 'BOBO METHOD - Cipher Method 3 - {BOBO}
            bb = DoCipher(st, b, GetSimpleChars)
        ElseIf method = "ctx" Then 'CTX METHOD - Cipher Method 4 - {CANTAX}
            bb = DoCipher(st, b, GetCantaxChars)
        ElseIf method = "cust" Then 'CUSTOM METHOD - Cipher Method 5 - {CUSTOM}
            bb = DoCipher(st, b, GetCustomChars)
        ElseIf method = "yab" Then 'YABUJIN METHOD - Cipher Method 6 - {YABUJIN}
            bb = DoCipher(st, b, GetYabujin)
        ElseIf method = "dbl" Then 'DBL-Cipher METHOD - Cipher Method 7 - {DoubleOver}
            If b = True Then
                bb = DecideMethod(DecideMethod(st, True, "bobo"), True, "ctx")
            Else
                bb = DecideMethod(st, False, "ctx")
                bb = DecideMethod(bb, False, "bobo")
            End If
        ElseIf method.Contains("des") Then 'DES-CRYPTO METHOD - Encryption Method 1 - {CBC-DES}
            If method = "des" Then
                bb = DoCrypto(st, b, "des")
            ElseIf method = "des-padding" Then 'DES-CRYPTO METHOD WITH DOUBLE PADDING - ENCRYPTION METHOD 2 - {CBC-DES-DOUBLE-PADDED}
                bb = DoCrypto(st, b, "des-padding")
            End If
        End If
        Return bb
    End Function
    Public Function DoCipher(st As String, DoEncode As Boolean, chars As Object) As String
        Dim Output As String = ""
        If DoEncode = True Then
            For Each cc As String In st
                For Each c As String In chars
                    If cc.Contains(c.Split(":")(0)) Then
                        Output += c.Split(":")(1)
                    End If
                Next
            Next
        Else
            For Each cc As String In st
                For Each c As String In chars

                    If cc.Contains(c.Split(":")(1)) Then
                        Output += c.Split(":")(0)
                    End If
                Next
            Next
        End If
        Return Output
    End Function

    'Bobo Cipher, Created 08,01,2023
    Private Function GetSimpleChars()
        Dim Chars As String() = {
        "A:J", "B:K", "C:L", "D:M",
        "E:N", "F:O", "G:P", "H:Q",
        "I:R", "J:S", "K:T", "L:U",
        "M:V", "N:W", "O:X", "P:Y",
        "Q:Z", "R:A", "S:B", "T:C",
        "U:D", "V:E", "W:F", "X:G",
        "Y:H", "Z:I", " :-", ".:~",
        "-: ", "a:j", "b:k", "c:l",
        "d:m", "e:n", "f:o", "g:p",
        "h:q", "i:r", "j:s", "k:t",
        "l:u", "m:v", "n:w", "o:x",
        "p:y", "q:z", "r:a", "s:b",
        "t:c", "u:d", "v:e", "w:f",
        "x:g", "y:h", "z:i", "0:9",
        "1:8", "2:7", "3:6", "4:5",
        "5:4", "6:3", "7:2", "8:1",
        "9:0", "@:#", "\:\", "/:/",
        "':'", ">:+", "|:=", "?:‹"
        }
        Return Chars
    End Function
    'ASCII-NEW Cipher, Created 20,02,2023
    Private Function GetASCIIChars()
        Dim Chars As String() = {
        "A:À", "B:Á", "C:Â", "D:Ã",
        "E:Ý", "F:Þ", "G:ß", "H:à",
        "I:á", "J:â", "K:ã", "L:ä",
        "M:å", "N:æ", "O:ç", "P:è",
        "Q:é", "R:ê", "S:ë", "T:ì",
        "U:í", "V:î", "W:ï", "X:ð",
        "Y:ñ", "Z:ò", " :-", ".:~",
        "-: ", "a:!", "b:ô", "c:õ",
        "d:ö", "e:÷", "f:ø", "g:ù",
        "h:ú", "i:û", "j:ü", "k:ý",
        "l:þ", "m:ÿ", "n:È", "o:É",
        "p:Ê", "q:Ë", "r:Ì", "s:Î",
        "t:Ð", "u:Ñ", "v:Ò", "w:Ó",
        "x:Ô", "y:Õ", "z:Ö", "0:9",
        "1:8", "2:7", "3:6", "4:5",
        "5:4", "6:3", "7:2", "8:1",
        "9:0", "@:#", "\:\", "/:/",
        "':'", ">:+", "|:=", "?:ƒ",
        "!:ᄉ", "#:{"
        }
        Return Chars
    End Function
    'Cantax Cipher, Created 20,02,2023
    Private Function GetCantaxChars()
        Dim Chars As String() = {
        "A:תּ", "B:Ë", "C:Z", "D:Ã",
        "E:Ý", "F:Þ", "G:ß", "H:à",
        "I:á", "J:â", "K:რ", "L:ä",
        "M:å", "N:æ", "O:ç", "P:ओ",
        "Q:é", "R:জ", "S:ë", "T:Я",
        "U:í", "V:î", "W:ï", "X:ð",
        "Y:々", "Z:ò", " :-", ".:~",
        "-: ", "a:!", "b:ô", "c:õ",
        "d:ö", "e:÷", "f:ø", "g:ù",
        "h:ú", "i:û", "j:Ə", "k:ⴼ",
        "l:þ", "m:ÿ", "n:Ш", "o:É",
        "p:ⵣ", "q:პ", "r:Ì", "s:й",
        "t:Ð", "u:Ñ", "v:न", "w:Ó",
        "x:Ô", "y:ৠ", "z:এ", "0:9",
        "1:Ç", "2:ز", "3:ش", "4:앙",
        "5:က", "6:ဪ", "7:Ƙ", "8:ᄇ",
        "9:방", "@:#", "\:\", "/:/",
        "':'", ">:+", "|:=", "?:з",
        "!:ᄉ", "#:V", "=:|"
        }
        Return Chars
    End Function

    'Unicode Cipher, Created 22,02,2023
    Private Function GetUTF8Chars()
        Dim Chars As String() = {
        "A:Ć", "B:Ā", "C:Ă", "D:Ą",
        "E:Ĉ", "F:Ĥ", "G:Ħ", "H:Ĩ",
        "I:Ŋ", "J:Ō", "K:Ŏ", "L:Ő",
        "M:Œ", "N:Ŕ", "O:Ŗ", "P:Ř",
        "Q:Ś", "R:Ŝ", "S:Ş", "T:Š",
        "U:Ţ", "V:Ť", "W:Ŧ", "X:Ũ",
        "Y:Ū", "Z:Ŭ", " :-", ".:~",
        "-: ", "a:Ŵ", "b:Ŷ", "c:Ÿ",
        "d:Ź", "e:Ż", "f:Ž", "g:ſ",
        "h:Ƃ", "i:Ƅ", "j:Ɔ", "k:Ƈ",
        "l:Ɖ", "m:Ɗ", "n:Ƌ", "o:Ǝ",
        "p:Ɛ", "q:Ƒ", "r:ƒ", "s:Ɠ",
        "t:Ɣ", "u:ƕ", "v:Ɨ", "w:Ƙ",
        "x:ƚ", "y:ƛ", "z:Ɯ", "0:ɀ",
        "1:Ɲ", "2:Ɵ", "3:Ơ", "4:Ƣ",
        "5:Ƥ", "6:Ʀ", "7:Ʃ", "8:ƫ",
        "9:ƭ", "@:Ư", "\:ǽ", "/:Ǽ",
        "':Ƽ", ">:Ƕ", "|:ƶ", "?:Ƶ",
        "!:ǂ", "#:ǯ", "=:ƿ"
        }
        Return Chars
    End Function

    'Unicode Cipher, Created 22,02,2023
    Private Function GetYabujin()
        Dim Chars As String() = {
        "A:Ⰰ", "B:Ⰱ", "C:Ⰲ", "D:Ⰳ",
        "E:Ⰴ", "F:Ⰵ", "G:ⰰ", "H:ⰱ",
        "I:ⰲ", "J:ⰳ", "K:ⰴ", "L:ⰵ",
        "M:Ⰶ", "N:Ⰷ", "O:Ⰸ", "P:Ⰹ",
        "Q:Ⰺ", "R:ⰶ", "S:ⰷ", "T:ⰸ",
        "U:ⰹ", "V:ⰺ", "W:ⰻ", "X:Ⰻ",
        "Y:Ⰼ", "Z:Ⰽ", " :Ⰾ", ".:Ⰿ",
        "-:Ⱀ", "a:Ⱁ", "b:ⰼ", "c:ⰽ",
        "d:ⰾ", "e:ⰿ", "f:ⱀ", "g:ⱁ",
        "h:ⱂ", "i:ⱃ", "j:ⱄ", "k:ⱅ",
        "l:Ⱆ", "m:Ⱇ", "n:Ⱈ", "o:Ⱉ",
        "p:Ⱊ", "q:Ⱋ", "r:Ⱌ", "s:Ⱍ",
        "t:Ⱎ", "u:Ⱏ", "v:Ⱐ", "w:Ⱑ",
        "x:Ⱒ", "y:Ⱓ", "z:Ⱔ", "0:Ⱕ",
        "1:Ⱖ", "2:Ⱗ", "3:Ⱘ", "4:ⱆ",
        "5:ⱇ", "6:ⱈ", "7:ⱌ", "8:Ⱜ",
        "9:Ⱝ", "@:Ư", "\:ǽ", "/:Ǽ",
        "':Ƽ", ">:Ƕ", "|:ƶ", "?:Ƶ",
        "!:Ⱙ", "#:!", "=:ⱞ"
        }
        Return Chars
    End Function

    'Custom Ciphers, Created 20,02,2023
    Private Function GetCustomChars()
        Dim Chars As String() = IO.File.ReadAllLines(CustomSchema)
        Return Chars
    End Function
End Module
