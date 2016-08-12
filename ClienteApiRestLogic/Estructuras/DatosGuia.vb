Namespace Estructuras
    Public Class DatosGuia
        Public Property DireccionDestino() As Contribuyente
        Public Property DireccionOrigen() As Contribuyente
        Public Property RucTransportista() As String
        Public Property TipoDocTransportista() As String
        Public Property NombreTransportista() As String
        Public Property NroLicenciaConducir() As String
        Public Property PlacaVehiculo() As String
        Public Property CodigoAutorizacion() As String
        Public Property MarcaVehiculo() As String
        Public Property ModoTransporte() As String
        Public Property UnidadMedida() As String
        Public Property PesoBruto() As Decimal

        Public Sub New()
            DireccionDestino = New Contribuyente()
            DireccionOrigen = New Contribuyente()
        End Sub

    End Class
End Namespace