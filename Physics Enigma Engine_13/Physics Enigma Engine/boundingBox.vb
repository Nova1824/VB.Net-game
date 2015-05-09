Public Class boundingBox
    Public Property Location As Point
    Public Property Size As New Size
    Public ReadOnly Property Bounds As Rectangle
        Get
            Return New Rectangle(Location, Size)
        End Get
    End Property

End Class
