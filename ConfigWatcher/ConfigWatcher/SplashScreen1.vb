Public NotInheritable Class SplashScreen1

    Private Sub SplashScreen1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Set up the dialog text at runtime according to the application's assembly information.  
        ApplicationTitle.Text = System.String.Format(ApplicationTitle.Text, My.Application.Info.Version.Major, My.Application.Info.Version.Minor)

    End Sub

End Class
