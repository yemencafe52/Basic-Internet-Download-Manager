namespace BasicInternetDownloadManager
{
    using System;
    using System.Windows.Forms;
    public partial class frmMain : Form
    {
        private DownloadController dc;
        public frmMain()
        {
            InitializeComponent();
            this.dc = new DownloadController();

            DownloadController.UpdateView += new DownloadController.UpdateViewDlg(UpdateView);
        }

        private void UpdateView(FileDownloader fd)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new DownloadController.UpdateViewDlg(UpdateView), fd); return;
            }

            ListViewItem lvi = this.listView1.FindItemWithText(fd.Number.ToString());

            if((lvi is null))
            {
                lvi = new ListViewItem(fd.Number.ToString());
                lvi.SubItems.Add(fd.Link);
                lvi.SubItems.Add(fd.Precent.ToString("#0.#0"));
                lvi.SubItems.Add(fd.State.ToString());
                this.listView1.Items.Add(lvi);
            }
            else
            {
                lvi.SubItems[2].Text = fd.Precent.ToString("#0.#0");
                lvi.SubItems[3].Text =(fd.State.ToString());
            }

            toolStripStatusLabel2.Text = this.listView1.Items.Count.ToString();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmLinkEntry fle = new frmLinkEntry();
            fle.ShowDialog();

            if(!(fle.Link is null))
            {
                this.dc.Add((fle.Link));
            }

            //string link = "http://localhost/D%3A/BGL.zip";

            //for (int i = 1; i < 5; i++)
            //{
            //    this.dc.Add((link));
            //}
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                for(int i =0;i < listView1.SelectedItems.Count; i++)
                {
                    int num = int.Parse(listView1.SelectedItems[i].Text);
                    this.dc.Stop(num);
                }
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    int num = int.Parse(listView1.SelectedItems[i].Text);
                    this.dc.Resume(num);
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    int num = int.Parse(listView1.SelectedItems[i].Text);
                    this.dc.Cancel(num);

                    this.listView1.Items.RemoveAt(listView1.SelectedItems[i].Index);
                }
            }
        }

        private void loadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult r = ofd.ShowDialog();

            if(r == DialogResult.OK)
            {
                string[] ar = System.IO.File.ReadAllLines(ofd.FileName);
                foreach(string s in ar)
                {
                    try
                    {
                        Uri url = new Uri(s);
                        this.dc.Add(url.AbsoluteUri);
                    }
                    catch {  
                    }
                }
            }
        }
    }
}
