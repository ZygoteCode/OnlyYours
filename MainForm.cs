using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

public partial class MainForm : MetroSuite.MetroForm
{
    [DllImport("psapi.dll")]
    static extern int EmptyWorkingSet(IntPtr hwProc);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

    public MainForm()
    {
        InitializeComponent();
        Globals.PasswordManagerInstance = new PasswordManager();
        timer1.Start();
        timer2.Start();
        new Thread(ClearRAM).Start();
    }

    private void guna2GradientButton1_Click(object sender, System.EventArgs e)
    {
        Globals.Purpose = 1;
        MultipurposeForm multipurposeForm = new MultipurposeForm();
        multipurposeForm.Show();
    }

    public void ClearRAM()
    {
        while (true)
        {
            Thread.Sleep(750);
            CleanRAM();
        }
    }

    public void CleanRAM()
    {
        EmptyWorkingSet(Process.GetCurrentProcess().Handle);
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect(GC.MaxGeneration);
        GC.WaitForPendingFinalizers();
        SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
    }

    private void guna2GradientButton2_Click(object sender, System.EventArgs e)
    {
        if (listBox1.SelectedIndex < 0 || listBox1.SelectedItem == null)
        {
            return;
        }

        if (MessageBox.Show("Are you sure you want to delete these credentials?", "OnlyYours", MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.No))
        {
            return;
        }

        Globals.PasswordManagerInstance.DeleteCredentials(listBox1.SelectedItem.ToString());
        listBox1.Items.Remove(listBox1.SelectedItem);
        listBox2.Items.Remove(listBox2.SelectedItem);
        guna2TextBox1.Text = "";
    }

    private void guna2GradientButton3_Click(object sender, System.EventArgs e)
    {
        if (Globals.CurrentPassword == "")
        {
            Globals.Purpose = 2;
            MultipurposeForm multipurposeForm = new MultipurposeForm();
            multipurposeForm.Show();
            return;
        }

        Globals.PasswordManagerInstance.LoadAllCredentials(Globals.CurrentPassword);
    }

    private void guna2GradientButton4_Click(object sender, System.EventArgs e)
    {
        if (Globals.CurrentPassword == "")
        {
            Globals.Purpose = 3;
            MultipurposeForm multipurposeForm = new MultipurposeForm();
            multipurposeForm.Show();
            return;
        }

        Globals.PasswordManagerInstance.SaveAllCredentials(Globals.CurrentPassword);
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (MessageBox.Show("Are you sure you want to exit from the program? Any unsaved change will be definitely lost.", "OnlyYours", MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.No))
        {
            e.Cancel = true;
        }

        CleanRAM();
        Process.GetCurrentProcess().Kill();
    }

    private void guna2TextBox1_TextChanged(object sender, System.EventArgs e)
    {
        if (listBox1.SelectedIndex < 0 || listBox1.SelectedItem == null || guna2TextBox1.ReadOnly)
        {
            return;
        }

        Globals.PasswordManagerInstance.UpdateCredentials(listBox1.SelectedItem.ToString(), guna2TextBox1.Text);
    }

    private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        if (listBox1.SelectedIndex < 0 || listBox1.SelectedItem == null)
        {
            return;
        }

        guna2TextBox1.Text = Globals.PasswordManagerInstance.GetCredentialsValue(listBox1.SelectedItem.ToString());
    }

    private void guna2TextBox2_TextChanged(object sender, System.EventArgs e)
    {
        listBox1.Items.Clear();

        foreach (KeyValuePair<string, string> credentialsEntry in Globals.PasswordManagerInstance.GetCredentials())
        {
            string originalKey = credentialsEntry.Key;

            string key = Utils.FilterString(originalKey),
                inputString = Utils.FilterString(guna2TextBox2.Text);

            if (key.Contains(inputString) || inputString.Contains(key))
            {
                listBox1.Items.Add(originalKey);
            }
        }
    }

    private void timer1_Tick_1(object sender, System.EventArgs e)
    {
        if (Globals.CredentialsUpdateReceived)
        {
            Globals.CredentialsUpdateReceived = false;
            int savedSelectedIndex = 0;

            if (listBox1.SelectedItem != null && listBox1.SelectedIndex >= 0)
            {
                savedSelectedIndex = listBox1.SelectedIndex;
            }

            listBox1.Items.Clear();
            listBox2.Items.Clear();

            foreach (KeyValuePair<string, string> credentialsEntry in Globals.PasswordManagerInstance.GetCredentials())
            {
                listBox1.Items.Add(credentialsEntry.Key);
                listBox2.Items.Add(credentialsEntry.Key);
            }

            if (listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = savedSelectedIndex;
            }
        }

        guna2TextBox1.ReadOnly = !(listBox1.Items.Count > 0) || !(listBox1.SelectedIndex >= 0);
    }

    private void timer2_Tick_1(object sender, System.EventArgs e)
    {
        timer2.Stop();

        if (!File.Exists("credentials.plf"))
        {
            guna2GradientButton4.PerformClick();
        }
        else
        {
            guna2GradientButton3.PerformClick();
        }
    }

    private void guna2GradientButton5_Click(object sender, System.EventArgs e)
    {
        ProtoRandom.ProtoRandom random = new ProtoRandom.ProtoRandom(200);
        char[] _characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!\"|\\£$%&/()=?'èé[{+*]+òç@à°#ù§,;.:-_<>".ToCharArray();
        string generatedPassword = random.GetRandomString(_characters, 64);
        Clipboard.SetText(generatedPassword);
    }
}