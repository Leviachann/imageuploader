using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using Microsoft.Win32;

namespace ImageGalleryClient
{
    public partial class MainWindow : Window
    {
        private const string ServerIPAddress = "127.0.0.1";
        private const int ServerPort = 12345;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    TcpClient client = new TcpClient(ServerIPAddress, ServerPort);
                    using (NetworkStream stream = client.GetStream())
                    using (FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                        }
                    }
                    MessageBox.Show("Image uploaded successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error uploading image: " + ex.Message);
                }
            }
        }
    }
}
