using System.Windows.Forms;

public partial class MultipurposeForm : MetroSuite.MetroForm
{
    public MultipurposeForm()
    {
        InitializeComponent();

        switch (Globals.Purpose)
        {
            case 1:
                Text = "OnlyYours - Add new credentials";
                guna2GradientButton1.Text = "Add new credentials";
                guna2TextBox1.UseSystemPasswordChar = false;
                break;
            case 2:
                Text = "OnlyYours - Load all credentials";
                guna2GradientButton1.Text = "Use password to load all credentials";
                guna2TextBox1.UseSystemPasswordChar = true;
                break;
            case 3:
                Text = "OnlyYours - Save all credentials";
                guna2GradientButton1.Text = "Use password to save all credentials";
                guna2TextBox1.UseSystemPasswordChar = true;
                break;
        }

        guna2TextBox1.Focus();
        timer1.Start();
    }

    private void guna2GradientButton1_Click(object sender, System.EventArgs e)
    {
        switch (Globals.Purpose)
        {
            case 1:
                Globals.PasswordManagerInstance.AddCredentials(guna2TextBox1.Text);
                break;
            case 2:
                Globals.CurrentPassword = guna2TextBox1.Text;
                Globals.PasswordManagerInstance.LoadAllCredentials(Globals.CurrentPassword);
                break;
            case 3:
                Globals.CurrentPassword = guna2TextBox1.Text;
                Globals.PasswordManagerInstance.SaveAllCredentials(Globals.CurrentPassword);
                break;
        }

        Globals.CredentialsUpdateReceived = true;
        Close();
    }

    private void guna2TextBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
    {
        if (e.KeyCode.Equals(Keys.Enter))
        {
            guna2GradientButton1.PerformClick();
        }
    }

    private void timer1_Tick(object sender, System.EventArgs e)
    {
        timer1.Stop();
        guna2TextBox1.Focus();
    }
}