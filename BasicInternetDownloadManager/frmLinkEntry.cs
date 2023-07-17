namespace BasicInternetDownloadManager
{
    using System;
    using System.Windows.Forms;
    public partial class frmLinkEntry : Form
    {
        private string link = null;
        public string Link
        {
            get
            {
                return link;
            }
        }
        public frmLinkEntry()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Uri url = new Uri(textBox1.Text);
                this.link = url.AbsoluteUri;
            }
            catch {
                MessageBox.Show("OOPS, SOMETHING WENT WRONG :("); return;
            }

            this.Close();
        }
    }
}
