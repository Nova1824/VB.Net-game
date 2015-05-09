Public Class minion
    Inherits PictureBox
    Private decXvelocity As Decimal
    Private decYvelocity As Decimal
    Private lstWalls As New List(Of Rectangle)
    Private lstPlatforms As New List(Of Rectangle)

    Public Sub New()
        With Me
            .BringToFront()
            .Visible = True
            .Size = New Size(50, 50)
            .SizeMode = PictureBoxSizeMode.StretchImage
            .BackColor = Color.Green
            .Location = New Point(450, 10)
        End With
        decXvelocity = 2
        decYvelocity = 0
        lstWalls.Add(frmMain.picWall1.Bounds)
        lstWalls.Add(frmMain.PicWall2.Bounds)

        lstPlatforms.Add(frmMain.picPlatform1.Bounds)
        lstPlatforms.Add(frmMain.picPlatform2.Bounds)
        lstPlatforms.Add(frmMain.picPlatform3.Bounds)
        lstPlatforms.Add(frmMain.picPlatform4.Bounds)
        lstPlatforms.Add(frmMain.picPlatform5.Bounds)
        lstPlatforms.Add(frmMain.picPlatform6.Bounds)

    End Sub

    Public Sub life()

        Me.Location = New Point(Me.Location.X + decXvelocity, Me.Location.Y + decYvelocity)
        gravity()
        collision()

    End Sub

    Private Sub gravity()
        decYvelocity += 15 * 1 / 30
    End Sub
    Private Sub collision()
        For Each wall In lstWalls
            If Me.Bounds.IntersectsWith(wall) Then
                decXvelocity *= -1
            End If
        Next
        For Each platform In lstPlatforms
            If Me.Bounds.IntersectsWith(platform) Then
                Me.Location = New Point(Me.Location.X, Me.Location.Y - decYvelocity)
                decYvelocity *= 0.3
            End If
        Next
        For Each Bullet In frmMain.lstBullets
            If Me.Bounds.IntersectsWith(Bullet.Bounds) Then
                frmMain.Controls.Remove(Bullet)
                Bullet.Dispose()
                frmMain.Controls.Remove(Me)
                Me.Dispose()
            End If
        Next
        If Me.Location.Y > 1000 Then
            frmMain.Controls.Remove(Me)
            Me.Dispose()
        End If

    End Sub


End Class
