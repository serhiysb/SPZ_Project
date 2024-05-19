using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSCP;

namespace FTPClient
{
    public partial class Form1 : Form
    {
        string uri;
        string username;
        string password;
        string currentDirectoryLV1;
        string currentDirectoryLV2 = "/";
        ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

        public Form1()
        {
            InitializeComponent();
            contextMenuStrip.Items.Add("Delete");
            contextMenuStrip.Items.Add("Make Directory");
            contextMenuStrip.Items.Add("Rename");
            contextMenuStrip.Items.Add("Refresh");

            contextMenuStrip.Items[0].Click += Delete_Click;
            contextMenuStrip.Items[1].Click += Make_Directory_Click;
            contextMenuStrip.Items[2].Click += Rename_Click;
            contextMenuStrip.Items[3].Click += Refresh_Click;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        private void Rename_Click(object sender, EventArgs e)
        {
            if(listView2.SelectedItems.Count!=0)
            {
                if (uri.Length != 0 && username.Length != 0 && password.Length != 0)
                {
                    MakeDirectoryForm makeDirectoryForm = new MakeDirectoryForm(listView2.SelectedItems[0].Text);
                    makeDirectoryForm.ShowDialog();
                    FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(uri + "/" + listView2.SelectedItems[0].Text);
                    ftpWebRequest.Method = WebRequestMethods.Ftp.Rename;
                    ftpWebRequest.Credentials = new NetworkCredential(username, password);
                    ftpWebRequest.RenameTo = makeDirectoryForm.Folder;
                    using (FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse())
                    {
                        textBox1.AppendText(ftpWebResponse.StatusDescription + "\n");
                    }
                    button1_Click(sender, e);
                }
            }
            
        }

        private void Make_Directory_Click(object sender, EventArgs e)
        {
            if (uri.Length != 0 && username.Length != 0 && password.Length != 0)
            {
                MakeDirectoryForm makeDirectoryForm = new MakeDirectoryForm(string.Empty);
                makeDirectoryForm.ShowDialog();
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create($"{uri}/{makeDirectoryForm.Folder}");
                ftpWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                ftpWebRequest.Credentials = new NetworkCredential(username, password);
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                ftpWebResponse.Close();
                textBox1.AppendText(ftpWebResponse.StatusDescription + "\n");
                button1_Click(null, null);
            }
        }
        
        private void Delete_Click(object sender, EventArgs e)
        {
            if(listView2.SelectedItems.Count != 0)
            {
                if (uri.Length != 0 && username.Length != 0 && password.Length != 0)
                {
                    SessionOptions sessionOptions = new SessionOptions
                    {
                        Protocol = Protocol.Ftp,
                        HostName = @"localhost",
                        UserName = textBox3.Text,
                        Password = textBox4.Text
                    };
                    using (Session session = new Session())
                    {
                        session.Open(sessionOptions);
                        session.RemoveFiles($"{currentDirectoryLV2}/{listView2.SelectedItems[0].Text}");
                        listView2.Items.Clear();
                        button1_Click(sender, e);
                    }
                }
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] disks = Environment.GetLogicalDrives();
            foreach (var disk in disks)
            {
                comboBox1.Items.Add(disk);
            }
            comboBox1.SelectedIndex = 1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentDirectoryLV1 = comboBox1.SelectedItem.ToString();
            textBox5.Text = currentDirectoryLV1;
            ImageList image = new ImageList();
            image.Images.Add("folder", Properties.Resources.icons8_live_folder_40);
            listView1.SmallImageList = image;

            string[] folders = Directory.GetDirectories(comboBox1.SelectedItem.ToString());
            string[] files = Directory.GetFiles(comboBox1.SelectedItem.ToString());
            listView1.Items.Clear();

            foreach (var folder in folders)
            {
                ListViewItem item = new ListViewItem(Path.GetFileName(folder));
                item.SubItems.Add(Directory.GetLastWriteTime(folder).ToString());
                item.SubItems.Add("File folder");
                item.ImageKey = "folder";
                listView1.Items.Add(item);
            }
            foreach (var file in files)
            {
                ListViewItem item = new ListViewItem(Path.GetFileName(file));
                item.SubItems.Add(Directory.GetLastWriteTime(file).ToString());
                item.SubItems.Add(Path.GetExtension(file));
                
                Icon icon = Icon.ExtractAssociatedIcon(file);
                listView1.SmallImageList.Images.Add(Path.GetExtension(file), icon);
                item.ImageKey = Path.GetExtension(file);

                listView1.Items.Add(item);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(listView1.SelectedItems != null)
            {
                if (listView1.SelectedItems[0].SubItems[2].Text == "File folder")
                {
                    ImageList image = new ImageList();
                    image.Images.Add("folder", Properties.Resources.icons8_live_folder_40);
                    listView1.SmallImageList = image;

                    currentDirectoryLV1 += listView1.SelectedItems[0].Text+"\\";
                    textBox5.Text = currentDirectoryLV1;

                    string[] folders = Directory.GetDirectories(currentDirectoryLV1);
                    string[] files = Directory.GetFiles(currentDirectoryLV1);
                    listView1.Items.Clear();

                    foreach (var folder in folders)
                    {
                        ListViewItem listViewItem = new ListViewItem(Path.GetFileName(folder));
                        listViewItem.SubItems.Add(Directory.GetLastWriteTime(folder).ToString());
                        listViewItem.SubItems.Add("File folder");
                        listViewItem.ImageKey = "folder";
                        listView1.Items.Add(listViewItem);
                    }
                    foreach (var file in files)
                    {
                        ListViewItem listViewItem = new ListViewItem(Path.GetFileName(file));
                        listViewItem.SubItems.Add(Directory.GetLastWriteTime(file).ToString());
                        listViewItem.SubItems.Add(Path.GetExtension(file));

                        Icon icon = Icon.ExtractAssociatedIcon(file);
                        listView1.SmallImageList.Images.Add(Path.GetExtension(file), icon);
                        listViewItem.ImageKey = Path.GetExtension(file);

                        listView1.Items.Add(listViewItem);
                    }
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            currentDirectoryLV2 = "/";
            uri = @"ftp://" + textBox2.Text;
            username = textBox3.Text;
            password = textBox4.Text;
            FtpWebRequest ftpWebRequestList = (FtpWebRequest)WebRequest.Create(uri);
            ftpWebRequestList.Credentials = new NetworkCredential(username, password);
            ftpWebRequestList.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            FtpWebResponse ftpWebResponseList = (FtpWebResponse)ftpWebRequestList.GetResponse();

            using (StreamReader streamReader = new StreamReader(ftpWebResponseList.GetResponseStream()))
            {
                string file;
                while ((file = streamReader.ReadLine()) != null)
                {
                    string[] strings = file.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime modifiedTime = DateTime.ParseExact(string.Join(" ", new string[] { strings[5], strings[6], strings[7] }), new string[] { "MMM dd HH:mm", "MMM dd yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None);

                    textBox6.Text = $"{uri}/";
                    ListViewItem listViewItem = new ListViewItem(strings[8]);   
                    listViewItem.SubItems.Add(modifiedTime.ToString());

                    string fileExtensoin = Path.GetExtension(strings[8]);
                    if (fileExtensoin.Length == 0)
                        listViewItem.SubItems.Add("File folder");
                    else
                        listViewItem.SubItems.Add(fileExtensoin);

                    listView2.Items.Add(listViewItem);
                }

            }
            ftpWebResponseList.Close();
            button1.Enabled = false;
        }

        private void listView2_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button== MouseButtons.Right) 
            {
                contextMenuStrip.Show(listView2, e.Location);
            }
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
            if (listView2.SelectedItems[0]!=null)
            {
                if (listView2.SelectedItems[0].SubItems[2].Text=="File folder")
                {
                    currentDirectoryLV2 += listView2.SelectedItems[0].Text+"/";
                    uri =  textBox6.Text + listView2.SelectedItems[0].Text;
                    username = textBox3.Text;
                    password = textBox4.Text;
                    listView2.Items.Clear();
                    textBox6.Text = uri;
                    FtpWebRequest ftpWebRequestList = (FtpWebRequest)WebRequest.Create(uri);
                    ftpWebRequestList.Credentials = new NetworkCredential(username, password);
                    ftpWebRequestList.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                    FtpWebResponse ftpWebResponseList = (FtpWebResponse)ftpWebRequestList.GetResponse();
                    using (StreamReader streamReader = new StreamReader(ftpWebResponseList.GetResponseStream()))
                    {
                        string file;
                        while ((file = streamReader.ReadLine()) != null)
                        {
                            string[] strings = file.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            DateTime modifiedTime = DateTime.ParseExact(string.Join(" ", new string[] { strings[5], strings[6], strings[7] }), new string[] { "MMM dd HH:mm", "MMM dd yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None);

                            textBox6.Text = $"{uri}/";
                            ListViewItem listViewItem = new ListViewItem(strings[8]);
                            listViewItem.SubItems.Add(modifiedTime.ToString());

                            string fileExtensoin = Path.GetExtension(strings[8]);
                            if (fileExtensoin.Length == 0)
                                listViewItem.SubItems.Add("File folder");
                            else
                                listViewItem.SubItems.Add(fileExtensoin);

                            listView2.Items.Add(listViewItem);
                        }

                    }
                    ftpWebResponseList.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create($"{uri}/{listView1.SelectedItems[0].Text}");
            ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
            ftpWebRequest.Credentials = new NetworkCredential(username, password);

            string filePath = currentDirectoryLV1 + listView1.SelectedItems[0].Text;
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();

            ftpWebRequest.ContentLength = data.Length;
            Stream requestStream = ftpWebRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
            textBox1.AppendText(ftpWebResponse.StatusDescription + "\n");
            ftpWebResponse.Close();
            button1_Click(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(listView2.SelectedItems.Count > 0)
            {
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create($"{uri}/{listView2.SelectedItems[0].Text}");
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpWebRequest.Credentials = new NetworkCredential(username, password);
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                Stream responseStream = ftpWebResponse.GetResponseStream();
                using (FileStream fs = new FileStream(currentDirectoryLV1 + listView2.SelectedItems[0].Text, FileMode.Create))
                {
                    byte[] buffer = new byte[fs.Length];
                    int size = 0;
                    while ((size = responseStream.Read(buffer, 0, buffer.Length)) > 0) ;
                    {
                        fs.Write(buffer, 0, size);
                    }
                }
                textBox1.AppendText(ftpWebResponse.StatusDescription + "\n");
                responseStream.Close();
                ftpWebResponse.Close();


                ImageList image = new ImageList();
                image.Images.Add("folder", Properties.Resources.icons8_live_folder_40);
                listView1.SmallImageList = image;

                string[] folders = Directory.GetDirectories(currentDirectoryLV1);
                string[] files = Directory.GetFiles(currentDirectoryLV1);
                listView1.Items.Clear();

                foreach (var folder in folders)
                {
                    ListViewItem item = new ListViewItem(Path.GetFileName(folder));
                    item.SubItems.Add(Directory.GetLastWriteTime(folder).ToString());
                    item.SubItems.Add("File folder");
                    item.ImageKey = "folder";
                    listView1.Items.Add(item);
                }
                foreach (var file in files)
                {
                    ListViewItem item = new ListViewItem(Path.GetFileName(file));
                    item.SubItems.Add(Directory.GetLastWriteTime(file).ToString());
                    item.SubItems.Add(Path.GetExtension(file));

                    Icon icon = Icon.ExtractAssociatedIcon(file);
                    listView1.SmallImageList.Images.Add(Path.GetExtension(file), icon);
                    item.ImageKey = Path.GetExtension(file);

                    listView1.Items.Add(item);
                }
            }
            
        }
    }
}
