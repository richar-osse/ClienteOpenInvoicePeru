
Imports ClienteApiRestLogic
Imports ClienteApiRestLogic.Estructuras

Module Program

    Sub Main()
        Try
            Console.WriteLine("Generación de una Factura")

            Dim documento As New DocumentoElectronico()

            Dim emisor As New Contribuyente()
            With emisor
                .NroDocumento = "20493654463"
                .TipoDocumento = "6"
                .Direccion = "CAL.MORONA NRO. 171"
                .Urbanizacion = "-"
                .Ubigeo = "120101"
                .Departamento = "LORETO"
                .Provincia = "MAYNAS"
                .Distrito = "IQUITOS"
                .NombreComercial = "CSM CORPORACION ORIENTE S.A.C."
                .NombreLegal = "CSM CORPORACION ORIENTE S.A.C."
            End With

            Dim receptor As New Contribuyente()
            With receptor
                .NroDocumento = "20100039207"
                .TipoDocumento = "6" 'RUC Catálogo No. 06
                .NombreLegal = "RANSA COMERCIAL S.A."
                .NombreComercial = String.Empty
                .Direccion = String.Empty
                .Urbanizacion = String.Empty
                .Ubigeo = String.Empty
                .Departamento = String.Empty
                .Provincia = String.Empty
                .Distrito = String.Empty
            End With

            'CABECERA DEL DOCUMENTO
            With documento
                .Emisor = emisor
                .Receptor = receptor
                .IdDocumento = "FF11-002"
                .FechaEmision = Date.Today.ToString("yyyy-MM-dd")
                .Moneda = "PEN"
                .MontoEnLetras = "SON CIENTO DIECIOCHO SOLES CON 0/100"
                .CalculoIgv = 0.18D
                .CalculoIsc = 0.1D
                .CalculoDetraccion = 0.04D
                .TipoDocumento = "01" 'Factura Catálogo No. 01
                .TotalIgv = 0
                .TotalVenta = 100
                .Gravadas = 0
                .Inafectas = 0
                .Exoneradas = 100
                .Gratuitas = 0
            End With

            Dim detalle1 As New DetalleDocumento()
            With detalle1
                .Id = 1
                .Cantidad = 5
                .PrecioReferencial = 20
                .PrecioUnitario = 20
                .TipoPrecio = "01" 'Catálogo No. 16
                .CodigoItem = "1234234" 'Codigo de Producto Interno.
                .Descripcion = "Arroz Costeño"
                .UnidadMedida = "KG" 'CATALOGO No. 03
                .Impuesto = 0
                .TipoImpuesto = "20" 'Catálogo No. 07
                .TotalVenta = 100
                .Suma = 100
            End With

            documento.Items.Add(detalle1)

            'ACA VIENE LA MAGIA DEL DLL PARA .NET FRAMEWORK 2.0
            Dim generador As New Generador()

            With generador
                .UsuarioSol = "MODDATOS"
                .ClaveSol = "MODDATOS"
                .ClaveCertificado = String.Empty
                .RutaCertificado = "Certificado.pfx"
                .EndPointUrl = "https://e-beta.sunat.gob.pe/ol-ti-itcpfegem-beta/billService"
                .NroRuc = "20493654463"
            End With

            Dim respuesta As FirmadoResponse = generador.GenerarDocumento(documento)
            If Not respuesta.Exito Then
                Console.WriteLine(respuesta.MensajeError)
            End If

            Console.WriteLine("Respuesta de SUNAT:" & respuesta.TramaXmlFirmado)
            Console.WriteLine("Codigo Hash:" & respuesta.ValorFirma)
            Console.WriteLine("Resumen de Firma:" & respuesta.ResumenFirma)

            Console.ReadLine()
            Console.WriteLine("Generación de un Resumen Diario de Boletas")
            Dim resumenDiario As New ResumenDiario()
            With resumenDiario
                .IdDocumento = String.Format("RC-{0}-1", Date.Today.ToString("yyyyMMdd"))
                .Emisor = emisor
                .FechaEmision = Date.Today.ToString("yyyy-MM-dd")
                .FechaReferencia = .FechaEmision
            End With
            Dim grupoResumen As New GrupoResumen
            With grupoResumen
                .Id = 1
                .TipoDocumento = "03"
                .Serie = "BB14"
                .Moneda = "PEN"
                .CorrelativoInicio = 10
                .CorrelativoFin = 500
                .TotalVenta = 5000
                .Gravadas = 0
                .Inafectas = 5000
                .TotalIgv = 0
                .TotalIsc = 0
                .TotalDescuentos = 0
                .Exoneradas = 0
                .Exportacion = 0
                .TotalOtrosImpuestos = 0
            End With
            resumenDiario.Resumenes = New List(Of GrupoResumen)
            resumenDiario.Resumenes.Add(grupoResumen)

            Dim respuestaResumen As EnviarResumenResponse = generador.GenerarResumenDiario(resumenDiario)

            If Not respuestaResumen.Exito Then
                Console.WriteLine(respuestaResumen.MensajeError)
            End If

            Console.WriteLine("Nro. Ticket para el Resumen: " & respuestaResumen.NroTicket)

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        Finally
            Console.ReadLine()
        End Try
    End Sub

End Module
