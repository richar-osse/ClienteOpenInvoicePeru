Namespace Estructuras
    Public Class GrupoResumen
        Inherits DocumentoResumenDetalle
        Public Property CorrelativoInicio() As Integer
        Public Property CorrelativoFin() As Integer
        Public Property Moneda() As String
        Public Property TotalVenta() As Decimal
        Public Property TotalDescuentos() As Decimal
        Public Property TotalIgv() As Decimal
        Public Property TotalIsc() As Decimal
        Public Property TotalOtrosImpuestos() As Decimal
        Public Property Gravadas() As Decimal
        Public Property Exoneradas() As Decimal
        Public Property Inafectas() As Decimal
        Public Property Exportacion() As Decimal
        Public Property Gratuitas() As Decimal
    End Class
End Namespace