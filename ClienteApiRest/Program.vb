
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
                .TipoDocumento = "6"
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
                .FechaEmision = Date.Today.AddDays(-5).ToString("yyyy-MM-dd")
                .Moneda = "PEN"
                .MontoEnLetras = "SON CIENTO DIECIOCHO SOLES CON 0/100"
                .CalculoIgv = 0.18D
                .CalculoIsc = 0.1D
                .CalculoDetraccion = 0.04D
                .TipoDocumento = "01" 'Factura
                .TotalIgv = 18
                .TotalVenta = 118
                .Gravadas = 100
            End With

            Dim detalle1 As New DetalleDocumento()
            With detalle1
                .Id = 1
                .Cantidad = 5
                .PrecioReferencial = 20
                .PrecioUnitario = 20
                .TipoPrecio = "01" 'Catálogo No. 16
                .CodigoItem = "1234234"
                .Descripcion = "Arroz Costeño"
                .UnidadMedida = "KG" 'CATALOGO No. 03
                .Impuesto = 18
                .TipoImpuesto = "10" 'Catálogo No. 07
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

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        Finally
            Console.ReadLine()
        End Try
    End Sub

End Module
