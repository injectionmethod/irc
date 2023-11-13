Imports Org.BouncyCastle.Crypto.Engines
Imports Org.BouncyCastle.Crypto.Generators
Imports Org.BouncyCastle.Crypto.Parameters

Module Cryptography
    Public Function DoCrypto(st As String, DoEncrypt As Boolean, method As String) As String
        Dim bb As String = st
        If method = "des" Then
            If DoEncrypt = True Then
                bb = Convert.ToBase64String(EncryptDES_CBC(bb))
            Else
                bb = System.Text.Encoding.UTF8.GetString(DecryptDES_CBC(Convert.FromBase64String(bb)))
            End If
        End If

        If method = "des-padding" Then
            If DoEncrypt = True Then
                bb = Convert.ToBase64String(EncryptDES_CBC(DecideMethod(DecideMethod(bb, True, StrReverse("obob")), True, padding)))
            Else
                bb = System.Text.Encoding.UTF8.GetString(DecryptDES_CBC(Convert.FromBase64String(bb))) : bb = DecideMethod(bb, False, padding) : bb = DecideMethod(bb, False, StrReverse("obob"))
            End If
        End If

        If method = "aes-padding" Then
            If DoEncrypt = True Then
                bb = Convert.ToBase64String(EncryptAES_CBC(DecideMethod(DecideMethod(bb, True, StrReverse("obob")), True, padding), password))
            Else
                bb = System.Text.Encoding.UTF8.GetString(DecryptAES_CBC(Convert.FromBase64String(bb), password)) : bb = DecideMethod(bb, False, padding) : bb = DecideMethod(bb, False, StrReverse("obob"))
            End If
        End If

        'TESTED, FLOW WORKS BUT MESSAGE BLANK, WORK ON THIS
        If method = "zed-padding" Then
            If DoEncrypt = True Then
                ' Encrypt using DES-padding, then AES-padding
                bb = DoCrypto(bb, True, "des-padding")
                bb = DoCrypto(bb, True, "aes-padding")
            Else
                ' Decrypt using AES-padding, then DES-padding
                bb = DoCrypto(bb, False, "aes-padding")
                bb = DoCrypto(bb, False, "des-padding")
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
            Console.WriteLine("ERROR DECRYPTING MESSAGE - DES | " + ce.Message)
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
    Private hardcodedSalt As Byte() = New Byte() {67, 104, 32, 2, 1, 52, 26, 47}
    Public Function DeriveAESKey(password As String) As Byte()
        Dim iterationCount As Integer = 10000

        Dim generator As Pkcs5S2ParametersGenerator = New Pkcs5S2ParametersGenerator()
        generator.Init(System.Text.Encoding.UTF8.GetBytes(password), hardcodedSalt, iterationCount)
        Dim keyParameters As KeyParameter = DirectCast(generator.GenerateDerivedParameters(256), KeyParameter)
        Return keyParameters.GetKey()
    End Function

    Public Function EncryptAES_CBC(FileBytes As String, password As String) As Byte()
        Dim engine As New AesEngine()
        Dim key As Byte() = DeriveAESKey(password)

        Dim ptBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(FileBytes)
        Dim cipher As Org.BouncyCastle.Crypto.BufferedBlockCipher = New Org.BouncyCastle.Crypto.Paddings.PaddedBufferedBlockCipher(New Org.BouncyCastle.Crypto.Modes.CbcBlockCipher(engine))
        cipher.Init(True, New KeyParameter(key))

        Dim rv As Byte() = New Byte(cipher.GetOutputSize(ptBytes.Length) - 1) {}
        Dim processedBytes As Integer = cipher.ProcessBytes(ptBytes, 0, ptBytes.Length, rv, 0)

        Try
            cipher.DoFinal(rv, processedBytes)
        Catch ce As Exception
        End Try

        Return rv
    End Function
    Public Function DecryptAES_CBC(FileBytes As Byte(), password As String) As Byte()
        Dim engine As New Org.BouncyCastle.Crypto.Engines.AesEngine()
        Dim keyz As Byte() = DeriveAESKey(password)
        Dim cipher As Org.BouncyCastle.Crypto.BufferedBlockCipher = New Org.BouncyCastle.Crypto.Paddings.PaddedBufferedBlockCipher(New Org.BouncyCastle.Crypto.Modes.CbcBlockCipher(engine))
        cipher.Init(False, New Org.BouncyCastle.Crypto.Parameters.KeyParameter(keyz))
        Dim rv As Byte() = New Byte(cipher.GetOutputSize(FileBytes.Length) - 1) {}
        Dim ProcessedBytes As Integer = cipher.ProcessBytes(FileBytes, 0, FileBytes.Length, rv, 0)
        Try
            cipher.DoFinal(rv, ProcessedBytes)
        Catch ce As Exception
            Console.WriteLine("ERROR DECRYPTING MESSAGE - AES | " + ce.Message)
        End Try
        Return rv
    End Function
    Public Function DecideMethod(st As String, b As Boolean, method As String) As String

        'I HATE THIS CODE
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
            bb = DoCipher(st, b, GetYabujinChars)
        ElseIf method = "kyt" Then 'KYOTO METHOD - Cipher Method 7 - {KYOTO}
            bb = DoCipher(st, b, GetKyotoChars)
        ElseIf method = "occ" Then 'OCC METHOD - Cipher Method 8 - {OCEANIA}
            bb = DoCipher(st, b, GetOceaniaChars)
        ElseIf method = "jmk" Then 'JLMK METHOD - Cipher Method 9 - {JapaneseKoreanMongolianKyrgyz}
            bb = DoCipher(st, b, GetJLMKChars)
        ElseIf method = "dbl" Then 'DBL-Cipher METHOD - Cipher Method 10 - {DoubleOver}
            bb = GetDoubleCipher(b, st)
        ElseIf method = "qea" Then 'QEA-Cipher METHOD - Cipher Method 11 - {Quadripolar Element Arrangement}
            bb = DoCipher(st, b, GetQEAChars)
        ElseIf method = "pst" Then 'QEA-Cipher METHOD - Cipher Method 12 - {PST}
            bb = DoCipher(st, b, GetPSTChars)
        ElseIf method.Contains("des") Then 'DES-CRYPTO METHOD - Encryption Method 1 - {CBC-DES}
            If method = "des" Then
                bb = DoCrypto(st, b, "des")
            ElseIf method = "des-padding" Then 'DES-CRYPTO METHOD WITH DOUBLE PADDING - ENCRYPTION METHOD 2 - {CBC-DES-DOUBLE-PADDED}
                bb = DoCrypto(st, b, "des-padding")
            End If
        ElseIf method.Contains("aes") Then 'AES-CRYPTO METHOD WITH DOUBLE PADDING - Encryption Method 3 - {CBC-AES}
            If method = "aes-padding" Then
                bb = DoCrypto(st, b, "aes-padding")
            End If
        ElseIf method.Contains("zed") Then 'ZED-CRYPTO METHOD WITH DOUBLE PADDING - Encryption Method 4 - {CBC-AES-DES}
            If method = "zed-padding" Then
                bb = DoCrypto(st, b, "zed-padding")
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
        "':'", ">:+", "|:=", "?:‹",
        "=:|", """:.", ".:""",
        "#:@", "‹:?"
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
        "9:0", "@:#", "\:/", "/:\",
        "':'", ">:+", "|:=", "?:ƒ",
        "!:ᄉ", "#:{", "=:|", """:.",
        ".:""",
        "+:<"
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
        "5:က", "6:ဪ", "7:Ƙ",
        "8:ᄇ", "9:방", "@:#", "\:\",
        "/:/",
        "':'", ">:+", "|:=", "?:з",
        "!:ᄉ", "#:V", "=:|", """:.",
        ".:""", "з:?"
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
        "!:ǂ", "#:ǯ", "=:ƿ", """:.",
        ".:"""
        }
        Return Chars
    End Function

    'Yabujin Cipher, Created 22,02,2023
    Private Function GetYabujinChars()
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
        "!:Ⱙ", "#:!", "=:ⱞ", """:.",
        ".:"""
        }
        Return Chars
    End Function

    'Kyoto Cipher, Created 25,02,2023
    Private Function GetKyotoChars()
        Dim Chars As String() = {
        "a:昂", "b:卜", "c:嘘", "d:弐",
        "e:蛙", "f:鮎", "g:栞", "h:!",
        "i:井", "j:尉", "k:瓶", "l:碧",
        "m:眸", "n:杏", "o:桶", "p:袋",
        "q:亀", "r:檸", "s:茜", "t:凧",
        "u:植", "v:窺", "w:梧", "x:翔",
        "y:夭", "z:櫛", "A:衿", "B:暈",
        "C:瞑", "D:帖", "E:恢", "F:肇",
        "G:慕", "H:榊", "I:楠", "J:穎",
        "K:樺", "L:鱗", "M:弥", "N:廣",
        "O:翁", "P:躰", "Q:蛟", "R:礁",
        "S:柘", "T:秦", "U:梅", "V:蝶",
        "W:榧", "X:亨",
        "Y:鴻", "Z:蜥", "0:朋", "1:壱",
        "2:>", "3:参", "4:肆", "5:伍",
        "6:陸", "7:漆", "8:捌", "9:玖",
        "!:曳", "<:吃", ">:蔽", "@:欒",
        "\:謂", "/:稜", "|:=", "=:隈",
        "-:$", " :&", """:.", ".:"""
    }
        Return Chars
    End Function

    'OCEANIA CIPHER, Created 25,02,2023
    Private Function GetOceaniaChars()
        Dim chars() As String = {
    "a:昴", "b:瓢", "c:鷺", "d:弐",
    "e:蝦", "f:鰯", "g:橘", "h:&",
    "i:雲", "j:威", "k:鉢", "l:薔",
    "m:黛", "n:杏", "o:桶", "p:蓋",
    "q:麟", "r:檸", "s:錦", "t:凧",
    "u:植", "v:窺", "w:梧", "x:翔",
    "y:夭", "z:櫛",
    "A:霞", "B:瑠", "C:薫", "D:綴",
    "E:恵", "F:瑳", "G:>", "H:榊",
    "I:桐", "J:穎", "K:樺", "L:鱗",
    "M:綺", "N:廣", "O:翁", "P:躰",
    "Q:蛟", "R:礁", "S:柘", "T:秦",
    "U:梅", "V:蝶", "W:榧", "X:亨",
    "Y:鴻", "Z:蜥",
    "0:朋", "1:壱", "2:侶", "3:桜",
    "4:肆", "5:伍", "6:陸", "7:漆",
    "8:捌", "9:玖",
    "!:曳", "<:吃", ">:蔽", "@:欒",
    "\:謂", "/:稜", "|:=", "=:隈",
    "-:$", " :!", """:.", ".:"""
     }
        Return chars
    End Function

    'JLMK Cipher, Created 25,02,2023
    Private Function GetJLMKChars()
        Dim chars() As String = {
    "a:丁", "b:井", "c:冊", "d:医",
    "e:升", "f:Q", "g:夕", "h:妃",
    "i:姫", "j:屋", "k:巴", "l:幸",
    "m:弥", "n:彦", "o:斗", "p:旭",
    "q:曲", "r:栄", "s:桜", "t:樹",
    "u:歩", "v:毬", "w:石", "x:秀",
    "y:)", "z:#",
    "A:葵", "B:卯", "C:鳳", "D:鬼",
    "E:君", "F:>", "G:<", "H:K",
    "I:吉", "J:弓", "K:梓", "L:桃",
    "M:麻", "N:仁", "O:梨", "P:舞",
    "Q:綾", "R:栞", "S:千", "T:通",
    "U:羽", "V:波", "W:峰", "X:晶",
    "Y:(", "Z:@",
    "0:零", "1:壱", "2:弐", "3:参",
    "4:肆", "5:伍", "6:陸", "7:漆",
    "8:捌", "9:玖",
    "!:嗚", "<:吃", ">:蔽", "@:欒",
    "\:謂", "/:稜", "|:=", "=:隈",
    "-:&", " :$"
}
        Return chars
    End Function

    'PST Ciphers, Created 01,03,2023
    Public Function GetPSTChars()
        Dim chars() As String = {
"a:龱", "b:留", "c:鹊", "d:蜢",
"e:鳝", "f:飕", "g:螳", "h:甬",
"i:嶷", "j:巉", "k:椿", "l:蝴",
"m:蟋", "n:桤", "o:饕", "p:荏",
"q:醛", "r:棵", "s:茵", "t:刎",
"u:逅", "v:煦", "w:黼", "x:砺",
"y:栎", "z:藐",
"A:褫", "B:谧", "C:缪", "D:绔",
"E:聃", "F:奂", "G:醴", "H:礴",
"I:崤", "J:鲲", "K:篁", "L:泮",
"M:邴", "N:枳", "O:菘", "P:栉",
"Q:蚺", "R:跗", "S:仝", "T:垠",
"U:牝", "V:檎", "W:戆", "X:礓",
"Y:沓", "Z:缙",
"0:甙", "1:脲", "2:糠", "3:鸫",
"4:蚜", "5:铡", "6:箍", "7:膈",
"8:萁", "9:瘤",
"!:搏", "<:袍", ">:蔻", "@:鸬",
":砭", "/:旯", "|:瀑", "=:撰",
"-:牯", "$:谯", " :钾"
}
        Return chars
    End Function

    'QEA Ciphers, Created 26,02,2023
    Public Function GetQEAChars()
        Dim chars() As String = {
    "a:א", "b:ב", "c:ג", "d:ד",
    "e:ה", "f:ו", "g:ז", "h:ח",
    "i:ט", "j:י", "k:כ", "l:ל",
    "m:מ", "n:נ", "o:ע", "p:פ",
    "q:צ", "r:ק", "s:ר", "t:ש",
    "u:ת", "v:ץ", "w:ף", "x:ך",
    "y:ן", "z:ם",
     "A:昂", "B:兵", "C:晨", "D:德",
    "E:峨", "F:风", "G:革", "H:和",
    "I:嘉", "J:健", "K:康", "L:兰",
    "M:明", "N:能", "O:攀", "P:蒲",
    "Q:全", "R:荣", "S:山", "T:天",
    "U:文", "V:武", "W:西", "X:香",
    "Y:友", "Z:竹",
      "0:零", "1:壹", "2:贰", "3:叁",
    "4:肆", "5:伍", "6:陆", "7:柒",
    "8:捌", "9:玖",
    "!:鼓", "<:哥", ">:弓", "@:胡",
    "\:禾", "/:虎", "|:剑", "=:江",
    "-:雷", "$:龙", " :麻"
}
        Return chars
    End Function

    'Double-Cipher, Created 23,02,2023
    Public Function GetDoubleCipher(b As Boolean, st As String)
        Dim bb As String
        If b = True Then
            bb = DecideMethod(DecideMethod(st, True, "bobo"), True, "ctx")
        Else
            bb = DecideMethod(st, False, "ctx")
            bb = DecideMethod(bb, False, "bobo")
        End If
        Return bb
    End Function

    'Custom Ciphers, Created 20,02,2023
    Private Function GetCustomChars()
        Dim Chars As String() = IO.File.ReadAllLines(CustomSchema)
        Return Chars
    End Function
    Function AddCustomChar(chars As String(), st As String)
        chars.Append(st)
        Return chars
    End Function
End Module