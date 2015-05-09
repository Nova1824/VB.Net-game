Imports System.Runtime.InteropServices
Public Class frmMain

    <DllImport("user32.dll")> _
    Public Shared Function GetAsyncKeyState(ByVal vKey As Int32) As UShort
    End Function

    'Game Engine
    'Designed and Programmed by: Noah Reardon
    'Date: 5/3/15
    '----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    'Questions:                                                                                                                                                            | 
    '                                                                                                                                                                      |
    'To do:                                                                                                                                                                |
    '    Make comments that say why the code is there not what it does                                                                                                     |
    '    make animations                                                                                                                                                   |
    '    stress-test the program/engine to see how many objects it can move before you see a noticable frame rate drop  (if there is alot of lag then I have to use GDI+)  |
    '                                                                                                                                                                      |
    'Maybe impliment later:                                                                                                                                                |
    '   bitmaps instead of picture boxes                                                                                                                                   |
    '                                                                                                                                                                      |
    'BUGS:                                                                                                                                                                 |
    '   boundingboxes of minions and bullets continue to collide with things after they were supposed to be destroyed                                                      |
    '                                                                                                                                                                      |
    'Changelog:                                                                                                                                                            |
    '                                                                                                                                                                      |
    '                                                                                                                                                                      |
    '                                                                                                                                                                      |
    '----------------------------------------------------------------------------------------------------------------------------------------------------------------------

#Region "Initialize Variables"

    'Player Variables
    Private decNetforce As Decimal
    Private decPlayerWeight As Decimal = 1
    Private decAcceleration As Decimal
    Private decYvelocity As Decimal
    Private decXvelocity As Decimal
    Private stwJump As New Stopwatch
    Private intDoublejumpAvailable As Integer
    Private blnFalling As Boolean
    Private blnStandingOnPlatform As Boolean
    Public intFacingDirection As Integer

    'Physics Engine Variables
    Private decGravity As Decimal = 15
    Private decTimestep As Decimal = 1 / 30

    'Player Collision variables and boundingboxes
    Private blnHorizontalCollision As Boolean
    Private blnVerticalCollision As Boolean
    Private VerticalBoundingBox As New boundingBox
    Private VBB_Xoffset As Decimal
    Private VBB_Yoffset As Decimal
    Private RHorizontalBoundingBox As New boundingBox
    Private RHorizontalDetectorBox As New boundingBox
    Private LHorizontalBoundingBox As New boundingBox
    Private LHorizontalDetectorBox As New boundingBox
    Private RHBB_Xoffset As Decimal
    Private RHBB_Yoffset As Decimal
    Private RHDet_Xoffset As Decimal
    Private RHDet_Yoffset As Decimal
    Private LHBB_Xoffset As Decimal
    Private LHBB_Yoffset As Decimal
    Private LHDet_Xoffset As Decimal
    Private LHDet_Yoffset As Decimal
    Private HeadBoundingBox As New boundingBox
    Private HeadBB_Xoffset As Decimal
    Private HeadBB_Yoffset As Decimal
    Private PlatformDetectorBoundingBox As New boundingBox
    Private PlatformBB_Xoffset As Decimal
    Private PlatformBB_Yoffset As Decimal
    Private intHorizontalCollisionDirection As Integer

    'list of objects that have collision properties
    Private lstCollision As New List(Of Rectangle)

    Public lstBulletCollision As New List(Of Rectangle)
    Public lstBullets As New List(Of Bullet)

    Public lstMinions As New List(Of minion)

    Public stwShoot As New Stopwatch
    Private decFirerate As Decimal
    Private blnShootavailability As Boolean

    'Debuger variables
    Private blnDebug As Boolean = False
    Private stwDebug As New Stopwatch

#End Region
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'debuger objects
        picRHBB.Visible = False
        picLHBB.Visible = False
        picLHdetector.Visible = False
        picRHdetector.Visible = False
        picVBB.Visible = False
        picHeadBB.Visible = False
        picFeet.Visible = False
        picMan.SendToBack()
        picFeet.BringToFront()
        picVBB.BringToFront()
        picLHdetector.BringToFront()
        picRHdetector.BringToFront()
        picRHBB.BringToFront()
        picLHBB.BringToFront()
        picHeadBB.BringToFront()


        'bounding boxes
        VerticalBoundingBox.Size = picVBB.Size
        VBB_Xoffset = picVBB.Location.X - picMan.Location.X
        VBB_Yoffset = picVBB.Location.Y - picMan.Location.Y

        RHorizontalBoundingBox.Size = picRHBB.Size
        RHBB_Xoffset = picRHBB.Location.X - picMan.Location.X
        RHBB_Yoffset = picRHBB.Location.Y - picMan.Location.Y

        RHorizontalDetectorBox.Size = picRHdetector.Size
        RHDet_Xoffset = picRHdetector.Location.X - picMan.Location.X
        RHDet_Yoffset = picRHdetector.Location.Y - picMan.Location.Y

        LHorizontalBoundingBox.Size = picLHBB.Size
        LHBB_Xoffset = picLHBB.Location.X - picMan.Location.X
        LHBB_Yoffset = picLHBB.Location.Y - picMan.Location.Y

        LHorizontalDetectorBox.Size = picLHdetector.Size
        LHDet_Xoffset = picLHdetector.Location.X - picMan.Location.X
        LHDet_Yoffset = picLHdetector.Location.Y - picMan.Location.Y

        HeadBoundingBox.Size = picHeadBB.Size
        HeadBB_Xoffset = picHeadBB.Location.X - picMan.Location.X
        HeadBB_Yoffset = picHeadBB.Location.Y - picMan.Location.Y

        PlatformDetectorBoundingBox.Size = picFeet.Size
        PlatformBB_Xoffset = picFeet.Location.X - picMan.Location.X
        PlatformBB_Yoffset = picFeet.Location.Y - picMan.Location.Y

        'Player Object
        decNetforce = decPlayerWeight * decGravity

        'Stopwatches
        stwJump.Start()
        stwDebug.Start()
        stwShoot.Start()

        'weapons
        decFirerate = 100

        With picSplashscreen
            .Location = New Point(-51, -11)
            .Visible = True
            .BringToFront()
        End With

    End Sub
    Private Sub tmrSplashscreen_Tick(sender As Object, e As EventArgs) Handles tmrSplashscreen.Tick
        picSplashscreen.Dispose()
        Controls.Remove(picSplashscreen)
        tmrGameLoop.Enabled = True
        tmrSplashscreen.Enabled = False
    End Sub
    Private Sub GameEngine(sender As Object, e As EventArgs) Handles tmrGameLoop.Tick

        Debuger()               'Turns on or off the debuger
        AI()
        Projectiles()
        Input()                 'Takes the input from the keyboard and changes variables
        gravity()               'Moves the character based on gravity and changes in velocity
        collision()             'If the character is colliding with anything then move the character and update velocity
        animate()               'animates the objects
        Invalidate()            'Redraw the screen

    End Sub
    Private Sub Debuger()

        'update the debuger labels
        lblDebug1.BringToFront()
        lblDebug2.BringToFront()
        lblDebug3.BringToFront()
        lblDebug4.BringToFront()
        lblDebug5.BringToFront()
        lblDebug6.BringToFront()
        lblDebug7.BringToFront()
        lblDebug8.BringToFront()
        lblDebug1.Text = "X-velocity: " & decXvelocity
        lblDebug2.Text = "Y-velocity: " & decYvelocity
        lblDebug3.Text = "DoubleJump: " & intDoublejumpAvailable
        lblDebug4.Text = "Falling: " & blnFalling
        lblDebug5.Text = "On Platform? " & blnStandingOnPlatform
        lblDebug6.Text = "Horizontal Collision: " & blnHorizontalCollision & "  " & intHorizontalCollisionDirection
        lblDebug7.Text = "Vertical Collision: " & blnVerticalCollision
        lblDebug8.Text = "Facing Direction: " & intFacingDirection

        If GetAsyncKeyState(Keys.O) Then
            If stwDebug.ElapsedMilliseconds > 80 Then
                If blnDebug Then

                    Me.Size = New Size(Me.Size.Width - 400, Me.Size.Height)

                    lblDebug1.Visible = False
                    lblDebug2.Visible = False
                    lblDebug3.Visible = False
                    lblDebug4.Visible = False
                    lblDebug5.Visible = False
                    lblDebug6.Visible = False
                    lblDebug7.Visible = False
                    lblDebug8.Visible = False
                    picRHBB.Visible = False
                    picRHdetector.Visible = False
                    picLHBB.Visible = False
                    picLHdetector.Visible = False
                    picVBB.Visible = False
                    picHeadBB.Visible = False
                    picFeet.Visible = False

                    blnDebug = False

                ElseIf Not (blnDebug) Then

                    Me.Size = New Size(Me.Size.Width + 400, Me.Size.Height)

                    lblDebug1.Visible = True
                    lblDebug2.Visible = True
                    lblDebug3.Visible = True
                    lblDebug4.Visible = True
                    lblDebug5.Visible = True
                    lblDebug6.Visible = True
                    lblDebug7.Visible = True
                    lblDebug8.Visible = True
                    picRHBB.Visible = True
                    picRHdetector.Visible = True
                    picLHBB.Visible = True
                    picLHdetector.Visible = True
                    picVBB.Visible = True
                    picHeadBB.Visible = True
                    picFeet.Visible = True

                    blnDebug = True

                End If
                stwDebug.Reset()
                stwDebug.Start()
            End If
        End If

    End Sub
    Private Sub AI()

        For Each minion In lstMinions
            minion.life()
        Next

    End Sub
    Private Sub Projectiles()
        For Each Bullet In lstBullets
            Bullet.shoot()
        Next
    End Sub
    Private Sub Input()

            'shooting
            If GetAsyncKeyState(Keys.X) Then
                If stwShoot.ElapsedMilliseconds > decFirerate And blnShootavailability Then
                    Dim bullet As New Bullet
                    lstBullets.Add(bullet)
                    Controls.Add(bullet)
                    stwShoot.Reset()
                    stwShoot.Start()
                    blnShootavailability = False
                End If
            Else
                blnShootavailability = True
            End If

            'Horizontal Movement
            If GetAsyncKeyState(Keys.Left) Then
                If intHorizontalCollisionDirection >= 0 Then
                    decXvelocity = -5
                    intFacingDirection = -1
                End If
            ElseIf GetAsyncKeyState(Keys.Right) Then
                If intHorizontalCollisionDirection <= 0 Then
                    decXvelocity = 5
                    intFacingDirection = 1
                End If
            Else
                decXvelocity *= 0.8
            End If

            'Jumping
            If GetAsyncKeyState(Keys.Up) Then

                picMan.Location = New Point(picMan.Location.X, picMan.Location.Y + 2)

                blnStandingOnPlatform = False
                For Each item In lstCollision
                    If PlatformDetectorBoundingBox.Bounds.IntersectsWith(item) Then
                        stwJump.Reset()
                        stwJump.Start()
                        picMan.Location = New Point(picMan.Location.X, picMan.Location.Y - 2)
                        decYvelocity = -10
                        intDoublejumpAvailable = 0
                        blnStandingOnPlatform = True
                        Exit For
                    End If
                Next

                If blnStandingOnPlatform = False Then
                    If intDoublejumpAvailable = 3 Then
                        decYvelocity = -10
                        intDoublejumpAvailable = 2
                    End If
                End If

                If stwJump.ElapsedMilliseconds < 400 And decYvelocity > -6 Then
                    If blnFalling = False Then
                        decYvelocity -= 1
                    End If
                End If

                picMan.Location = New Point(picMan.Location.X, picMan.Location.Y - 1)

                'double jump
                If intDoublejumpAvailable = 1 And stwJump.ElapsedMilliseconds > 50 Then
                    decYvelocity = -10
                    intDoublejumpAvailable = 2
                End If

            Else 'W not pressed

                If intDoublejumpAvailable = 0 Then
                    intDoublejumpAvailable = 1
                End If

            End If

    End Sub
    Private Sub gravity()

        decAcceleration = decNetforce / decPlayerWeight                                                             'acceleration is force over mass (acceleration of gravity)
        decYvelocity += decAcceleration * decTimestep                                                               'the velocity is the previous velocity plus the acceleration multiplied by the time step
        picMan.Location = New Point(picMan.Location.X + decXvelocity, picMan.Location.Y + decYvelocity)             'move the player based on velocity

    End Sub
    Private Sub collision()

        blnHorizontalCollision = False
        blnVerticalCollision = False
        intHorizontalCollisionDirection = 0

        UpdateBoundingboxPos()

        For Each minion In lstMinions
            If picMan.Bounds.IntersectsWith(minion.Bounds) Then
                picMan.Location = New Point(524, 367)
            End If
        Next

        For Each item As Rectangle In lstCollision
            If VerticalBoundingBox.Bounds.IntersectsWith(item) Then
                blnVerticalCollision = True
                picMan.Location = New Point(picMan.Location.X, picMan.Location.Y - decYvelocity)
                blnFalling = False
                decYvelocity *= -0.05
            End If
            If HeadBoundingBox.Bounds.IntersectsWith(item) Then
                blnVerticalCollision = True
                picMan.Location = New Point(picMan.Location.X, picMan.Location.Y - decYvelocity + 1)
                blnFalling = True
                decYvelocity *= -0.05
            End If
            If PlatformDetectorBoundingBox.Bounds.IntersectsWith(item) Then
                blnVerticalCollision = True
                blnFalling = False
                intDoublejumpAvailable = 3
            End If
        Next

        'Horizontal Collision
        For Each item As Rectangle In lstCollision
            If LHorizontalDetectorBox.Bounds.IntersectsWith(item) Then
                blnHorizontalCollision = True
                intHorizontalCollisionDirection = -1
            ElseIf RHorizontalDetectorBox.Bounds.IntersectsWith(item) Then
                blnHorizontalCollision = True
                intHorizontalCollisionDirection = 1
            End If

            If RHorizontalBoundingBox.Bounds.IntersectsWith(item) Then
                blnHorizontalCollision = True
                picMan.Location = New Point(picMan.Location.X - 5, picMan.Location.Y)
                decXvelocity = 0
            End If
            If LHorizontalBoundingBox.Bounds.IntersectsWith(item) Then
                blnHorizontalCollision = True
                picMan.Location = New Point(picMan.Location.X + 5, picMan.Location.Y)
                decXvelocity = 0
            End If
        Next

        UpdateBoundingboxPos()

    End Sub
    Private Sub UpdateBoundingboxPos()
        'Update the location of the bounding boxes
        VerticalBoundingBox.Location = New Point(picMan.Location.X + VBB_Xoffset, picMan.Location.Y + VBB_Yoffset)
        picVBB.Location = New Point(VerticalBoundingBox.Location.X, VerticalBoundingBox.Location.Y)

        RHorizontalBoundingBox.Location = New Point(picMan.Location.X + RHBB_Xoffset, picMan.Location.Y + RHBB_Yoffset)
        picRHBB.Location = New Point(RHorizontalBoundingBox.Location.X, RHorizontalBoundingBox.Location.Y)

        RHorizontalDetectorBox.Location = New Point(picMan.Location.X + RHDet_Xoffset, picMan.Location.Y + RHDet_Yoffset)
        picRHdetector.Location = New Point(RHorizontalDetectorBox.Location.X, RHorizontalDetectorBox.Location.Y)

        LHorizontalBoundingBox.Location = New Point(picMan.Location.X + LHBB_Xoffset, picMan.Location.Y + LHBB_Yoffset)
        picLHBB.Location = New Point(LHorizontalBoundingBox.Location.X, LHorizontalBoundingBox.Location.Y)

        LHorizontalDetectorBox.Location = New Point(picMan.Location.X + LHDet_Xoffset, picMan.Location.Y + LHDet_Yoffset)
        picLHdetector.Location = New Point(LHorizontalDetectorBox.Location.X, LHorizontalDetectorBox.Location.Y)

        HeadBoundingBox.Location = New Point(picMan.Location.X + HeadBB_Xoffset, picMan.Location.Y + HeadBB_Yoffset)
        picHeadBB.Location = New Point(HeadBoundingBox.Location.X, HeadBoundingBox.Location.Y)

        PlatformDetectorBoundingBox.Location = New Point(picMan.Location.X + PlatformBB_Xoffset, picMan.Location.Y + PlatformBB_Yoffset)
        picFeet.Location = New Point(PlatformDetectorBoundingBox.Location.X, PlatformDetectorBoundingBox.Location.Y)

        'update picBox bounding box locations
        lstCollision.Clear()
        lstCollision.Add(picPlatform1.Bounds)
        lstCollision.Add(picPlatform2.Bounds)
        lstCollision.Add(picPlatform3.Bounds)
        lstCollision.Add(picPlatform4.Bounds)
        lstCollision.Add(picPlatform5.Bounds)
        lstCollision.Add(picPlatform6.Bounds)
        lstCollision.Add(picWall1.Bounds)
        lstCollision.Add(PicWall2.Bounds)

        lstBulletCollision.Clear()
        lstBulletCollision.Add(picPlatform1.Bounds)
        lstBulletCollision.Add(picPlatform2.Bounds)
        lstBulletCollision.Add(picPlatform3.Bounds)
        lstBulletCollision.Add(picPlatform4.Bounds)
        lstBulletCollision.Add(picWall1.Bounds)
        lstBulletCollision.Add(PicWall2.Bounds)

        'For Each minion In lstMinions
        '    lstBulletCollision.Add(minion.Bounds)
        'Next

    End Sub
    Private Sub animate()
        If intFacingDirection = 1 Then
            picMan.Image = My.Resources.megaman_lookingright
        ElseIf intFacingDirection = -1 Then
            picMan.Image = My.Resources.megaman_lookingleft
        End If
    End Sub
    Private Sub tmrMinions_Tick(sender As Object, e As EventArgs) Handles tmrMinions.Tick
        Dim minion1 As New minion
        lstMinions.Add(minion1)
        Controls.Add(minion1)
    End Sub
End Class
