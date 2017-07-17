using System;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Xml;

using System.Windows.Forms;
using System.Drawing;
using System.Windows.Media;

namespace NotepadTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Window OwnerWindow;
        public MainWindow()
        {
            InitializeComponent();
        }

        public string _PersonName;

        private const string filename = "AccessRights.xml";
        private string openedFile;
        public string GetFileName() { return filename; }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Login login = new Login();
            login.Owner = this;
            login.ShowDialog();

        }

        //
        // method write to xml opened file by user
        private void writeToXMLopenedFile(string nameFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(GetFileName());
            XmlNodeList elList = doc.GetElementsByTagName("name");

            XmlNodeList el = doc.GetElementsByTagName("name");
            for (int i = 0; i < el.Count; i++)
            {
                if (el[i].InnerXml == _PersonName)
                {
                    XmlNode currNode = el[i].NextSibling;
                    currNode.InnerText = nameFile;
                    doc.Save(GetFileName());
                }

            }
        }

        //
        // true\false - some user use this file?
        private bool userUseThisFile(string dlgFileName)
        {
            XmlTextReader reader = null;
            reader = new XmlTextReader(GetFileName());
            reader.Read();
            while (reader.Read())
            {
                string tmpName = reader.ReadString();
                if (reader.Name == "fileOpen")
                {

                    if (tmpName == dlgFileName)
                    {
                        return true;
                    }
                }
            }
            reader.Close();
            return false;
        }



        private bool canOnNotRewriteFile(string dlgFileName)
        {
            using (XmlTextReader reader = new XmlTextReader(GetFileName()))
            {
                reader.Read();
                while (reader.Read())
                {
                    string userName = reader.ReadString();

                    if (reader.Name == "name")
                    {
                        if (userName == _PersonName)
                        {
                            //
                            // move to next 2 times
                            reader.Read();
                            reader.Read();

                            string tmpName = reader.ReadString();
                            if (reader.Name == "fileOpen")
                            {

                                if (tmpName == dlgFileName)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
        }


        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDlg.FilterIndex = 2;
            openFileDlg.RestoreDirectory = true;
            if (openFileDlg.ShowDialog() == true)
            {
                //
                // save opened filename(path + filename) into value
                openedFile = openFileDlg.FileName;


                if (userUseThisFile(openFileDlg.FileName) == false)
                {
                    writeToXMLopenedFile(openedFile);
                    try
                    {
                        using (FileStream fs = new FileStream(openFileDlg.FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            try
                            {
                                int size = (int)fs.Length;
                                byte[] buffer = new byte[size];
                                int count = 0;
                                int sum = 0;
                                while ((count = fs.Read(buffer, sum, size - sum)) > 0)
                                {
                                    sum += count;
                                }
                                textBox.Text = Encoding.UTF8.GetString(buffer);
                            }
                            finally
                            {
                                // if (fs != null)
                                // fs.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Could not read file. Error:" + ex.Message);
                    }
                }
                else
                {
                    using (FileStream fs = new FileStream(openFileDlg.FileName, FileMode.Open, FileAccess.Read, FileShare.None))
                    {

                        try
                        {
                            int size = (int)fs.Length;
                            byte[] buffer = new byte[size];
                            int count = 0;
                            int sum = 0;
                            while ((count = fs.Read(buffer, sum, size - sum)) > 0)
                            {
                                sum += count;
                            }
                            textBox.Text = Encoding.UTF8.GetString(buffer);
                        }
                        finally
                        {
                            // if (fs != null)
                            // fs.Close();
                        }
                    }
                }
            }
        }

        //if some user use this file - close saving, only read
        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            //
            // if it's new must be new file - just save as new file
            if (openedFile != null)
            {
                if (canOnNotRewriteFile(openedFile) == true)
                {
                    using (FileStream fs = new FileStream(openedFile, FileMode.Open, FileAccess.Write, FileShare.Read))
                    {
                        fs.Write(UnicodeEncoding.UTF8.GetBytes(textBox.Text), 0, UnicodeEncoding.UTF8.GetBytes(textBox.Text).Length);
                        fs.Close();
                        System.Windows.MessageBox.Show("ok");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("You can't save into same file, because another user use this file. Please, press \"Refresh\" and try again", "Stop", MessageBoxButton.OK, MessageBoxImage.Stop);
                }

            }
            else
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files(*.*)|*.*";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;
                //open dialog
                if (saveFileDialog.ShowDialog() == true)
                {
                    FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create);
                    fs.Write(UnicodeEncoding.UTF8.GetBytes(textBox.Text), 0,
                        UnicodeEncoding.UTF8.GetBytes(textBox.Text).Length);
                    fs.Close();
                    System.Windows.MessageBox.Show("Done!", "Saved!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void btnSaveUs_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files(*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            //open dialog
            if (saveFileDialog.ShowDialog() == true)
            {
                FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create);
                fs.Write(UnicodeEncoding.UTF8.GetBytes(textBox.Text), 0,
                    UnicodeEncoding.UTF8.GetBytes(textBox.Text).Length);
                fs.Close();
                System.Windows.MessageBox.Show("Done!", "Saved!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //if closing - clead data about user from xml
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_PersonName != null)
            {
                //
                //load xml document
                XmlDocument doc = new XmlDocument();
                doc.Load(GetFileName());

                //
                //choose node for deleting
                XmlNode node = doc.SelectSingleNode($"/users/user[name='{_PersonName}']");

                //
                //delete it
                node.ParentNode.RemoveChild(node);

                //
                //and save xml document
                doc.Save(GetFileName());
            }

        }

        private void updateStatusFile_Click(object sender, RoutedEventArgs e)
        {
            
            bool canChange = false;

            using (XmlTextReader reader = new XmlTextReader(GetFileName()))
            {
                reader.Read();
                while (reader.Read())
                {
                    reader.Read();
                    string userName = reader.ReadString();

                    if (reader.Name == "name")
                    {
                        if (userName == _PersonName)
                        {
                            canChange = true;
                            continue;
                        }
                        else
                        {
                            reader.Read();
                            reader.Read();
                            string tmpName = reader.ReadString();
                            if (reader.Name == "fileOpen")
                            {

                                if (tmpName == openedFile)
                                {
                                    System.Windows.MessageBox.Show("Some user use this file. Try again later", "Stop", MessageBoxButton.OK, MessageBoxImage.Stop);
                                    canChange = false;
                                    break;
                                }
                                else
                                    canChange = true;
                            }
                        }

                    }
                }
                
            }
            if (canChange == true)
            {
                if (openedFile != null || openedFile != " ")
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(GetFileName());
                    doc.SelectSingleNode($@"users/user[name=""{_PersonName}""]").LastChild.InnerText = openedFile;

                    doc.Save(GetFileName());
                    System.Windows.MessageBox.Show("Already you can save document!", "Sucessfull", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            //}
        }

        private void fontSize_Click(object sender, RoutedEventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if(fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Font font = fontDialog.Font;
                textBox.FontFamily = new System.Windows.Media.FontFamily(font.Name);
                textBox.FontSize = font.Size;
                textBox.FontWeight = font.Bold ? FontWeights.Bold : FontWeights.Regular;
                textBox.FontStyle = font.Italic ? FontStyles.Italic : FontStyles.Normal;
            }
        }

        private void fontColor_Click(object sender, RoutedEventArgs e)
        {
            var colorDlg = new ColorDialog();
            if(colorDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var WPFclr = System.Windows.Media.Color.FromArgb(colorDlg.Color.A, colorDlg.Color.R, colorDlg.Color.G, colorDlg.Color.B);
                var brush = new SolidColorBrush(WPFclr);
                textBox.Foreground = brush;
            }
        }

        private void BgrdColor_Click(object sender, RoutedEventArgs e)
        {
            var colorDlg = new ColorDialog();
            if (colorDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var WPFclr = System.Windows.Media.Color.FromArgb(colorDlg.Color.A, colorDlg.Color.R, colorDlg.Color.G, colorDlg.Color.B);
                var brush = new SolidColorBrush(WPFclr);
                textBox.Background = brush;
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetText(textBox.SelectedText);
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            textBox.Paste();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            textBox.SelectedText = "";
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetText(textBox.SelectedText);
            textBox.SelectedText = "";
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            textBox.SelectAll();
        }

        private void btnDateTime_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text +=DateTime.Now.ToString();
        }
    }
}
