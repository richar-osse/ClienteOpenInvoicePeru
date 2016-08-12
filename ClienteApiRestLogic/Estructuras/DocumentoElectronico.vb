
Imports System.Collections.Generic

Namespace Estructuras
    Public Class DocumentoElectronico

        Public Property TipoDocumento() As String

        Public Property Emisor() As Contribuyente
        Public Property Receptor() As Contribuyente

        Public Property IdDocumento() As String
        Public Property FechaEmision() As String
        Public Property Moneda() As String

        Public Property Gravadas() As Decimal
        Public Property Gratuitas() As Decimal
        Public Property Inafectas() As Decimal
        Public Property Exoneradas() As Decimal

        Public Property DescuentoGlobal() As Decimal

        Public Property TotalVenta() As Decimal
        Public Property TotalIgv() As Decimal
        Public Property TotalIsc() As Decimal
        Public Property TotalOtrosTributos() As Decimal

        Public Property MontoEnLetras() As String
        Public Property TipoOperacion() As String

        Public Property CalculoIgv() As Decimal
        Public Property CalculoIsc() As Decimal
        Public Property CalculoDetraccion() As Decimal

        Public Property MontoPercepcion() As Decimal
        Public Property MontoDetraccion() As Decimal

        Public Property TipoDocAnticipo() As String
        Public Property DocAnticipo() As String
        Public Property MonedaAnticipo() As String
        Public Property MontoAnticipo() As Decimal

        Public Property DatoAdicionales() As List(Of DatoAdicional)
        Public Property Relacionados() As List(Of DocumentoRelacionado)
        Public Property Items() As List(Of DetalleDocumento)
        Public Property DatosGuiaTransportista() As DatosGuia
        Public Property Discrepancias() As List(Of Discrepancia)

        Public Sub New()
            ' RUC.
            Emisor = New Contribuyente() With { _
             .TipoDocumento = "6" _
            }
            ' RUC.
            Receptor = New Contribuyente() With { _
             .TipoDocumento = "6" _
            }
            CalculoIgv = 0.18D
            CalculoIsc = 0.1D
            CalculoDetraccion = 0.04D
            Items = New List(Of DetalleDocumento)()
            DatoAdicionales = New List(Of DatoAdicional)()
            Relacionados = New List(Of DocumentoRelacionado)()
            Discrepancias = New List(Of Discrepancia)()
            TipoDocumento = "01"
            ' Factura.
            TipoOperacion = "01"
            ' Venta Interna.
            ' Soles.
            Moneda = "PEN"
        End Sub

    End Class

End Namespace