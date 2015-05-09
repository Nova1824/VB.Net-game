Public Class Bullet
    Inherits PictureBox
    Private velocity As Integer
    Public Sub New()
        With Me
            .BringToFront()
            .Visible = True
            .Size = New Size(25, 20)
            .SizeMode = PictureBoxSizeMode.StretchImage
            .Image = My.Resources.bullet
            .Location = New Point(frmMain.picMan.Location.X, frmMain.picMan.Location.Y + 10)
            If frmMain.intFacingDirection = 1 Then
                velocity = 20
            Else
                velocity = -20
            End If
        End With
    End Sub

    Public Sub shoot()
        Me.Location = New Point(Me.Location.X + velocity, Me.Location.Y)

        For Each item In frmMain.lstBulletCollision
            If Me.Bounds.IntersectsWith(item) Then
                frmMain.lstBullets.Remove(Me)
                frmMain.Controls.Remove(Me)
                Me.Dispose()
            End If
        Next
    End Sub


End Class
