Namespace Estructuras
    Public Class DetalleDocumento
        Public Property Id() As Integer
        Public Property Cantidad() As Decimal
        Public Property UnidadMedida() As String
        Public Property Suma() As Decimal
        Public Property TotalVenta() As Decimal
        Public Property PrecioUnitario() As Decimal
        Public Property TipoPrecio() As String
        Public Property Impuesto() As Decimal
        Public Property TipoImpuesto() As String
        Public Property ImpuestoSelectivo() As Decimal
        Public Property OtroImpuesto() As Decimal
        Public Property Descripcion() As String
        Public Property CodigoItem() As String
        Public Property PrecioReferencial() As Decimal

        Public Sub New()
            Id = 1
            UnidadMedida = "NIU"
            TipoPrecio = "01"
            TipoImpuesto = "10"
        End Sub
    End Class
End Namespace