using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls; 
using System.Windows.Media.Imaging;


namespace ImageGalleryServer
{
    public partial class MainWindow : Window
    {
        private List<byte[]> imageBytesList = new List<byte[]>();
        private Thread serverThread;

        public MainWindow()
        {
            InitializeComponent();
            serverThread = new Thread(StartServer);
            serverThread.Start();
        }

        private void StartServer()
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, 12345);
                listener.Start();

                while (true)
                {
                    using (TcpClient client = listener.AcceptTcpClient())
                    {
                        byte[] buffer = new byte[1024];
                        using (NetworkStream stream = client.GetStream())
                        {
                            int bytesRead;
                            using (var ms = new System.IO.MemoryStream())
                            {
                                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    ms.Write(buffer, 0, bytesRead);
                                }
                                imageBytesList.Add(ms.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server Error: " + ex.Message);
            }
        }

        private void UpdateImageGallery()
        {
            GalleryStackPanel.Children.Clear();
            foreach (byte[] imageBytes in imageBytesList)
            {
                BitmapImage bitmap = new BitmapImage();
                using (var stream = new System.IO.MemoryStream(imageBytes))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                System.Windows.Controls.Image imageControl = new System.Windows.Controls.Image();
                imageControl.Source = bitmap;
                imageControl.Width = 150;
                imageControl.Height = 150;
                GalleryStackPanel.Children.Add(imageControl);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateImageGallery();
        }
    }
}
