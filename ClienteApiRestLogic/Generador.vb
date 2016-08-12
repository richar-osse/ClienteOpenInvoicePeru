Imports System.IO
Imports ClienteApiRestLogic.Estructuras

Public Class Generador

    Public Property RutaCertificado As String
    Public Property ClaveCertificado As String
    Public Property UsuarioSol As String
    Public Property ClaveSol As String
    Public Property EndPointUrl As String
    Public Property NroRuc As String

    ''' <summary>
    ''' Escribir una trama Base64 en un archivo fisico en disco
    ''' </summary>
    ''' <param name="nombreArchivo">Ruta de Destino (incluir extension)</param>
    ''' <param name="trama">Trama del Archivo</param>
    Public Sub EscribirArchivo(nombreArchivo As String, trama As String)
        File.WriteAllBytes(nombreArchivo, Convert.FromBase64String(trama))
    End Sub

    ''' <summary>
    ''' Genera un Documento Electrónico, lo firma, lo envia a SUNAT y lee la respuesta.
    ''' </summary>
    ''' <param name="documento">Documento Electronico</param>
    ''' <returns>Devuelve los Valores de Firma del XML</returns>
    Public Function GenerarDocumento(documento As DocumentoElectronico) As FirmadoResponse

        Dim metodo As String
        Select Case documento.TipoDocumento
            Case "01"
                metodo = "GenerarFactura"
            Case "03"
                metodo = "GenerarFactura"
            Case "07"
                metodo = "GenerarNotaCredito"
            Case "08"
                metodo = "GenerarNotaDebito"
            Case Else
                metodo = "GenerarFactura"
        End Select

        Dim documentoResponse = ApiRest.Execute(Of DocumentoResponse, DocumentoElectronico)(metodo, documento)

        If Not documentoResponse.Exito Then
            Throw New ApplicationException(documentoResponse.MensajeError)
        End If

        ' Firmado del Documento.
        Dim firmado As New FirmadoRequest() With {
            .TramaXmlSinFirma = documentoResponse.TramaXmlSinFirma,
            .CertificadoDigital = Convert.ToBase64String(File.ReadAllBytes(RutaCertificado)),
            .PasswordCertificado = ClaveCertificado,
            .UnSoloNodoExtension = False
        }

        Dim responseFirma = ApiRest.Execute(Of FirmadoResponse, FirmadoRequest)("Firmar", firmado)

        If Not responseFirma.Exito Then
            Throw New ApplicationException(responseFirma.MensajeError)
        End If

        'Guardado del XML generado y firmado en Disco
        Dim nombreArchivo As String = String.Format("{0}-{1}-{2}", documento.Emisor.NroDocumento, documento.TipoDocumento, documento.IdDocumento)

        EscribirArchivo(String.Format("{0}.xml", nombreArchivo), responseFirma.TramaXmlFirmado)

        Dim sendBill As New EnviarDocumentoRequest() With {
            .Ruc = NroRuc,
            .UsuarioSol = UsuarioSol,
            .ClaveSol = ClaveSol,
            .EndPointUrl = EndPointUrl,
            .IdDocumento = documento.IdDocumento,
            .TipoDocumento = documento.TipoDocumento,
            .TramaXmlFirmado = responseFirma.TramaXmlFirmado
        }

        Dim responseSendBill = ApiRest.Execute(Of EnviarDocumentoResponse, EnviarDocumentoRequest)("EnviarDocumento", sendBill)

        If Not responseSendBill.Exito Then
            Throw New ApplicationException(responseSendBill.MensajeError)
        End If

        'Guardar en una BD los datos de Respuesta de SUNAT
        'responseSendBill.CodigoRespuesta - Codigo de Respuesta SUNAT: 0 = EXITO, DIFERENTE DE 0 = ANEXO LISTA ERRORES
        'responseSendBill.MensajeRespuesta - Mensaje de Respuesta de SUNAT.
        'responseSendBill.TramaZipCdr - Contiene la cadena Base64 del CDR de SUNAT
        If responseSendBill.CodigoRespuesta = "0" Then
            EscribirArchivo(String.Format("R-{0}.zip", nombreArchivo), responseSendBill.TramaZipCdr)
        Else
            Throw New ApplicationException(responseSendBill.MensajeRespuesta)
        End If

        Return responseFirma

    End Function

    ''' <summary>
    ''' Genera un Resumen Diario de Boletas, lo firma, lo envia a SUNAT y devuelve el Nro de Ticket
    ''' </summary>
    ''' <param name="resumen">Resumen Diario</param>
    ''' <returns>Nro de Ticket</returns>
    Public Function GenerarResumenDiario(resumen As ResumenDiario) As EnviarResumenResponse

        Dim documentoResponse = ApiRest.Execute(Of DocumentoResponse, ResumenDiario)("GenerarResumenDiario", resumen)

        If Not documentoResponse.Exito Then
            Throw New ApplicationException(documentoResponse.MensajeError)
        End If

        ' Firmado del Documento.
        Dim firmado As New FirmadoRequest() With {
            .TramaXmlSinFirma = documentoResponse.TramaXmlSinFirma,
            .CertificadoDigital = Convert.ToBase64String(File.ReadAllBytes(RutaCertificado)),
            .PasswordCertificado = ClaveCertificado,
            .UnSoloNodoExtension = True
        }

        Dim responseFirma = ApiRest.Execute(Of FirmadoResponse, FirmadoRequest)("Firmar", firmado)

        If Not responseFirma.Exito Then
            Throw New ApplicationException(responseFirma.MensajeError)
        End If

        'Guardado del XML generado y firmado en Disco
        Dim nombreArchivo As String = String.Format("{0}-{1}", resumen.Emisor.NroDocumento, resumen.IdDocumento)

        EscribirArchivo(String.Format("{0}.xml", nombreArchivo), responseFirma.TramaXmlFirmado)

        Dim documentoRequest As New EnviarDocumentoRequest() With {
            .Ruc = NroRuc,
            .UsuarioSol = UsuarioSol,
            .ClaveSol = ClaveSol,
            .EndPointUrl = EndPointUrl,
            .IdDocumento = resumen.IdDocumento.Replace("RC-", String.Empty),
            .TipoDocumento = "RC",
            .TramaXmlFirmado = responseFirma.TramaXmlFirmado
        }

        Dim resumenResponse = ApiRest.Execute(Of EnviarResumenResponse, EnviarDocumentoRequest)("EnviarResumen", documentoRequest)

        If Not resumenResponse.Exito Then
            Throw New ApplicationException(resumenResponse.MensajeError)
        End If

        'Guardar en una BD este dato Respuesta de SUNAT
        'resumenResponse.NroTicket - 'Nro de Ticket de SUNAT para consultas.

        Return resumenResponse
    End Function

    ''' <summary>
    ''' Genera un Documento de Baja, lo firma, lo envia y lee el Nro de Ticket
    ''' </summary>
    ''' <param name="baja">Comunicacion de Baja</param>
    ''' <returns>Nro de Ticket</returns>
    Public Function GenerarDocumentoBaja(baja As ComunicacionBaja) As EnviarResumenResponse
        Dim documentoResponse = ApiRest.Execute(Of DocumentoResponse, ComunicacionBaja)("GenerarComunicacionBaja", baja)

        If Not documentoResponse.Exito Then
            Throw New ApplicationException(documentoResponse.MensajeError)
        End If

        ' Firmado del Documento.
        Dim firmado As New FirmadoRequest() With {
            .TramaXmlSinFirma = documentoResponse.TramaXmlSinFirma,
            .CertificadoDigital = Convert.ToBase64String(File.ReadAllBytes(RutaCertificado)),
            .PasswordCertificado = ClaveCertificado,
            .UnSoloNodoExtension = True
        }
        Dim responseFirma = ApiRest.Execute(Of FirmadoResponse, FirmadoRequest)("Firmar", firmado)

        If Not responseFirma.Exito Then
            Throw New ApplicationException(responseFirma.MensajeError)
        End If

        'Guardado del XML generado y firmado en Disco
        Dim nombreArchivo As String = String.Format("{0}-{1}", baja.Emisor.NroDocumento, baja.IdDocumento)

        EscribirArchivo(String.Format("{0}.xml", nombreArchivo), responseFirma.TramaXmlFirmado)

        Dim documentoRequest As New EnviarDocumentoRequest() With {
            .Ruc = NroRuc,
            .UsuarioSol = UsuarioSol,
            .ClaveSol = ClaveSol,
            .EndPointUrl = EndPointUrl,
            .IdDocumento = baja.IdDocumento.Replace("RA-", String.Empty),
            .TipoDocumento = "RA",
            .TramaXmlFirmado = responseFirma.TramaXmlFirmado
        }

        Dim resumenResponse = ApiRest.Execute(Of EnviarResumenResponse, EnviarDocumentoRequest)("EnviarResumen", documentoRequest)

        If Not resumenResponse.Exito Then
            Throw New ApplicationException(resumenResponse.MensajeError)
        End If

        'Guardar en una BD este dato Respuesta de SUNAT
        'resumenResponse.NroTicket - 'Nro de Ticket de SUNAT para consultas.

        Return resumenResponse

    End Function
    ''' <summary>
    ''' Devuelve el CDR de SUNAT en base al Nro de Ticket.
    ''' </summary>
    ''' <param name="nroTicket">Nro de ticket</param>
    ''' <returns>Devuelve un CDR</returns>
    Public Function ConsultarTicketSunat(nroTicket As String) As EnviarDocumentoResponse
        Dim consulta As New ConsultaTicketRequest With {
            .UsuarioSol = UsuarioSol,
            .ClaveSol = ClaveSol,
            .EndPointUrl = EndPointUrl,
            .NroTicket = nroTicket,
            .Ruc = NroRuc
        }

        Dim documentoResponse = ApiRest.Execute(Of EnviarDocumentoResponse, ConsultaTicketRequest)("ConsultarTicket", consulta)

        If Not documentoResponse.Exito Then
            Throw New ApplicationException(documentoResponse.MensajeError)
        End If

        'Guardar en una BD los datos de Respuesta de SUNAT
        'documentoResponse.CodigoRespuesta - Codigo de Respuesta SUNAT: 0 = EXITO, DIFERENTE DE 0 = ANEXO LISTA ERRORES
        'documentoResponse.MensajeRespuesta - Mensaje de Respuesta de SUNAT.
        'documentoResponse.TramaZipCdr - Contiene la cadena Base64 del CDR de SUNAT (Guardar en Cliente)

        Return documentoResponse

    End Function

End Class
