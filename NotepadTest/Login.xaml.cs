using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace NotepadTest
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }
        MainWindow myOwner;
        private void btLogOK_Click(object sender, RoutedEventArgs e)
        {
            if (this.tbName.Text != "")
            {
                string tmpName = tbName.Text;
                bool canWrite = true;
                
                if (File.Exists(myOwner.GetFileName()))
                {
                    XmlTextReader reader = null;
                    try
                    {
                        reader = new XmlTextReader(myOwner.GetFileName());
                        reader.Read();
                        while (reader.Read())
                        {
                            if (reader.Name == "name")
                            {
                                if (reader.ReadString() == tbName.Text)
                                {
                                    MessageBox.Show("Write another name", "It name is exist", MessageBoxButton.OK, MessageBoxImage.Information);
                                    tbName.Text = "";
                                    canWrite = false;
                                    break;
                                }
                            }
                        }

                        reader.Close();

                        if (canWrite)
                        {
                            //
                            //if can't see user with too name - write it
                            XmlDocument DocXml = new XmlDocument();
                            XmlNode body;
                            DocXml.Load(myOwner.GetFileName());
                            body = DocXml.SelectSingleNode("users");

                            XmlNode newuser = DocXml.CreateElement("user");
                            body.AppendChild(newuser);

                            XmlNode nameUser = DocXml.CreateElement("name");
                            nameUser.InnerText = tbName.Text;

                            XmlNode userOpnFilename = DocXml.CreateElement("fileOpen");
                            userOpnFilename.InnerText = " ";

                            newuser.AppendChild(nameUser);
                            newuser.AppendChild(userOpnFilename);

                            DocXml.Save(myOwner.GetFileName());

                            myOwner._PersonName = tbName.Text;
                            this.Close();
                        }
                    }
                    finally
                    {
                        if (reader != null)
                            reader.Close();
                    }
                }
                else
                {
                    //if xml not exist - create it
                    XmlTextWriter creator = null;
                    try
                    {
                        creator = new XmlTextWriter(myOwner.GetFileName(), System.Text.Encoding.UTF8);
                        creator.WriteStartDocument();
                        creator.WriteStartElement("users");
                        creator.WriteEndElement();
                        creator.WriteEndDocument();
                        creator.Close();
                        XmlDocument DocXml = new XmlDocument();
                        XmlNode body;
                        DocXml.Load(myOwner.GetFileName());
                        body = DocXml.SelectSingleNode("users");

                        XmlNode newuser = DocXml.CreateElement("user");
                        body.AppendChild(newuser);

                        XmlNode nameUser = DocXml.CreateElement("name");
                        nameUser.InnerText = tbName.Text;

                        XmlNode userOpnFilename = DocXml.CreateElement("fileOpen");
                        userOpnFilename.InnerText = " ";

                        newuser.AppendChild(nameUser);
                        newuser.AppendChild(userOpnFilename);

                        DocXml.Save(myOwner.GetFileName());

                        myOwner._PersonName = tbName.Text;
                        
                    }
                    finally
                    {
                        if (creator != null)
                            creator.Close();
                    }
                    this.Close();
                }
            }
            else
                MessageBox.Show("Enter the name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btLogCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            myOwner.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myOwner = Owner as MainWindow;
        }
    }
}
