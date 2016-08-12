Imports System.Configuration
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json

Public Class ApiRest


    Public Shared Function Execute(Of TResponse, TRequest)(url As String, data As TRequest) As TResponse

        Dim baseUrl = ConfigurationManager.AppSettings("UrlOpenInvoicePeru")

        Using restProxy As New WebClient()
            restProxy.BaseAddress = baseUrl
            restProxy.Headers("Content-Type") = "application/json"
            restProxy.Encoding = New UTF8Encoding()

            Dim requestString = JsonConvert.SerializeObject(data)

            Dim responseString = restProxy.UploadString(url, "POST", requestString)

            Dim response = JsonConvert.DeserializeObject(Of TResponse)(responseString)

            Return response

        End Using


    End Function

End Class
