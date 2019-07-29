﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EightPuzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        BitmapImage bitmap;
        // Trang thai ket thuc
        bool isEnded = false;
        // Chi muc cac anh nho
        int[,] imgID = new int[3, 3];
        // cac anh nho
        Image[,] images = new Image[3, 3];
        // Tọa độ top-left của imgID[i]   Vi=1->8
        List<int> temp_pooli = new List<int> { 0, 1, 2, 0, 1, 2, 0, 1 };
        List<int> temp_poolj = new List<int> { 0, 0, 0, 1, 1, 1, 2, 2 };
        // kich thuoc anh nho
        int imageWidth;
        int imageHeight;
        int fullImageWidth = 420;
        int padding = 5;

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();

            if (screen.ShowDialog() == true)
            {
                bitmap = new BitmapImage(new Uri(screen.FileName));
                Debug.WriteLine(screen.FileName);
                var orgImage = bitmap;
                originalImage.Source = new BitmapImage(new Uri(screen.FileName));
                
                imageWidth = (int)fullImageWidth / 3;
                imageHeight = (int)(fullImageWidth * bitmap.Height / bitmap.Width) / 3;
                mainFrame.Height = imageHeight * 3 + 15;
                var rng = new Random();
                var pool = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
                var pooli = new List<int> { 0, 1, 2, 0, 1, 2, 0, 1 };
                var poolj = new List<int> { 0, 0, 0, 1, 1, 1, 2, 2 };

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (i != 2 || j != 2)
                        {
                            // Set vi tri

                            var k = rng.Next(pool.Count); // Chon ngau nhien mot chi muc trong pool

                            var cropped = new CroppedBitmap(bitmap, new Int32Rect(
                                (int)(pooli[k] * bitmap.PixelWidth / 3), (int)(poolj[k] * bitmap.PixelHeight / 3),
                                ((int)bitmap.PixelWidth / 3), ((int)bitmap.PixelHeight / 3)));

                            // Tao giao dien
                           
                            var imageView = new Image();
                            imageView.Source = cropped;
                            imageView.Width = imageWidth;
                            imageView.Height = imageHeight;
                            container.Children.Add(imageView);

                            Canvas.SetLeft(imageView, i * (imageWidth + padding));
                            Canvas.SetTop(imageView, j * (imageHeight + padding));

                            images[i, j] = imageView;
                            imgID[j, i] = pool[k];

                            pool.RemoveAt(k);
                            pooli.RemoveAt(k);
                            poolj.RemoveAt(k);
                        }
                    }
                }


                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Debug.Write(imgID[i, j]);
                    }
                    Debug.WriteLine("");
                }



            }
        }
        // Cac ham xu ly nghiep vu
        //Trạng thái chiến thắng
        int[,] winningState = new int[,]
        {
            {1,2,3 },
            {4,5,6 },
            {7,8,0 }
        };
        /// <summary>
        /// Kiểm tra chiến thắng
        /// </summary>
        /// <returns></returns>
        bool isWin()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (imgID[i, j] != winningState[i, j])
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Hàm thay đổi giá trị
        /// </summary>
        void SwapNum(ref int x, ref int y)
        {

            int tempswap = x;
            x = y;
            y = tempswap;
        }

        // Xu ly mouse Event
        bool isDragging = false;
        Image selectedImage = null;
        int newi = -1;
        int newj = -1;
        int oldi = 2;
        int oldj = 2;
        //int maxi = ;
        int maxj = -1;
        private void Container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            var position = e.GetPosition(this);
            selectedImage = null;

            oldi = (int)(position.X / (imageWidth + 5));
            oldj = (int)(position.Y / (imageHeight + 5));

            if (oldi < 3 && oldj < 3)
            {
                selectedImage = images[oldi, oldj];
            }

            isDragging = selectedImage != null;
        }

        private void Container_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var newPos = e.GetPosition(this);
                // Tao do moi
                var i = (int)(newPos.X / (imageWidth + 5));
                var j = (int)(newPos.Y / (imageHeight + 5));


                if (i < 3 && j < 3)
                {
                    newi = i;
                    newj = j;

                    Canvas.SetLeft(selectedImage, newPos.X);
                    Canvas.SetTop(selectedImage, newPos.Y);
                }
                else
                {
                    isDragging = false;

                    Canvas.SetLeft(selectedImage, oldi * (imageWidth + 5));
                    Canvas.SetTop(selectedImage, oldj * (imageHeight + 5));

                }



            }
        }

        private void Container_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            isDragging = false;
            if (selectedImage == null)
            {
                e.Handled = true;
                return;
            }

            if (selectedImage != null && (
                (newi == oldi + 1 && newj == oldj && oldi < 2 && images[newi, newj] == null) || (newi == oldi - 1 && newj == oldj && oldi >= 1 && images[newi, newj] == null)
                || (newi == oldi && newj == oldj + 1 && oldj < 2 && images[newi, newj] == null) || (newi == oldi && newj == oldj - 1 && oldj >= 1 && images[newi, newj] == null))
                && newi < 3 && newj < 3)
            {
                Canvas.SetLeft(selectedImage, newi * (imageWidth + 5));
                Canvas.SetTop(selectedImage, newj * (imageHeight + 5));
                images[newi, newj] = images[oldi, oldj];
                images[oldi, oldj] = null;
                SwapNum(ref imgID[newj, newi], ref imgID[oldj, oldi]);
            }
            else
            {
                Canvas.SetLeft(selectedImage, oldi * (imageWidth + 5));
                Canvas.SetTop(selectedImage, oldj * (imageHeight + 5));
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Debug.Write(imgID[i, j]);
                }
                Debug.WriteLine("");
            }


            if (isWin() == true)
            {
                Timer.Stop();
                MessageBox.Show("You Win", "Result", MessageBoxButton.OK, MessageBoxImage.Stop);
                isEnded = true;
            }
        }

        // TIMER

        int time = 180;
        private DispatcherTimer Timer;
        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (time > 0)
            {
                time--;
                CountDown.Text = string.Format("00:0" + time / 60 + ":" + time % 60);
            }
            else
            {
                Timer.Stop();
                MessageBox.Show("Time out. You lose!","Result",MessageBoxButton.OK, MessageBoxImage.Stop);
                isEnded = true;
            }
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
            var screen = new SaveFileDialog();

            if (screen.ShowDialog() == true)
            {

                var writer = new StreamWriter(screen.FileName);

                writer.WriteLine($"IsEnded={isEnded}");
                writer.WriteLine("Time: 00:0" + time / 60 + ":" + time % 60);
                writer.WriteLine($"Bitmap={bitmap}");
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        writer.Write($"{imgID[i, j]} ");
                    }
                    writer.WriteLine();
                }

                writer.Close();
            }

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            if (screen.ShowDialog() == true)
            {
                var reader = new StreamReader(screen.FileName);
                // 1st line
                var firstLine = reader.ReadLine();
                var tokens = firstLine.Split(new String[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                Debug.Write(tokens[1]);
                //isEnded = bool.Parse(tokens[1]);
                // 2nd Line
                var secondLine = reader.ReadLine();
                tokens = secondLine.Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                var numMin = int.Parse(tokens[2]);
                var numSecond = int.Parse(tokens[3]);
                time = numMin * 60 + numSecond;
                Timer_Tick(sender, e);
                // doc bitmap
                var thirdLine = reader.ReadLine();
                tokens = thirdLine.Split(new String[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                var fileName = tokens[1];
                Debug.Write(fileName);
                bitmap = new BitmapImage(new Uri(fileName));
                originalImage.Source = new BitmapImage(new Uri(tokens[1]));
                //Debug.WriteLine(bitmap);

                // Game state
                int[,] a = new int[3, 3];
                var line = reader.ReadLine(); Debug.WriteLine(line);
                tokens = line.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                a[0, 0] = int.Parse(tokens[0]);
                a[1, 0] = int.Parse(tokens[1]);
                a[2, 0] = int.Parse(tokens[2]);

                line = reader.ReadLine(); Debug.WriteLine(line);
                tokens = line.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                a[0, 1] = int.Parse(tokens[0]);
                a[1, 1] = int.Parse(tokens[1]);
                a[2, 1] = int.Parse(tokens[2]);

                line = reader.ReadLine(); Debug.WriteLine(line);
                tokens = line.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                a[0, 2] = int.Parse(tokens[0]);
                a[1, 2] = int.Parse(tokens[1]);
                a[2, 2] = int.Parse(tokens[2]);

                imageWidth = (int)fullImageWidth / 3;
                imageHeight = (int)(fullImageWidth * bitmap.Height / bitmap.Width) / 3;

                mainFrame.Height = imageHeight * 3 + 15;
                //// Giao dieenj
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // a(i,j) cho biết là ảnh của imgID nào

                        var k = a[j, i];
                        Debug.WriteLine("ImgIDD:" + k);
                        if (k == 0)
                        {
                            continue;
                        }
                        // Căt ảnh tại tọa độ top-left của imgID

                        var cropped = new CroppedBitmap(bitmap, new Int32Rect(
                            (int)(temp_poolj[k - 1] * bitmap.PixelWidth / 3), (int)(temp_pooli[k - 1] * bitmap.PixelHeight / 3),
                            ((int)bitmap.PixelWidth / 3), ((int)bitmap.PixelHeight / 3)));
                        Debug.WriteLine(temp_pooli[k - 1]);
                        Debug.WriteLine(temp_poolj[k - 1]);
                        // Tao giao dien
                        var imageView = new Image();
                        imageView.Source = cropped;
                        imageView.Width = imageWidth;
                        imageView.Height = imageHeight;
                        container.Children.Add(imageView);

                        Canvas.SetLeft(imageView, i * (imageWidth + padding));
                        Canvas.SetTop(imageView, j * (imageHeight + padding));

                        images[i, j] = imageView;
                        imgID[j, i] = a[i, j];

                        Debug.WriteLine("COUNT");

                    }
                }
                // Voi 
                Debug.Write(isEnded);
                Debug.Write(time);
                reader.Close();
            }
        }

        // KEY EVENT
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {

                Debug.Write("TOA DO cua o trong:");
                Debug.Write(oldi + " " + oldj);
                Debug.WriteLine("");
                if (oldi >= 0 && oldi <= 2 && oldj < 2 && oldj >= 0)
                {
                    Image isMoved = images[oldi, oldj + 1];

                    // Toa do moi : 2 0
                    // Buc anh bi di chuyen 2 1
                    Canvas.SetLeft(isMoved, oldi * (imageWidth + padding));
                    Canvas.SetTop(isMoved, oldj * (imageHeight + padding));

                    SwapNum(ref imgID[oldj, oldi], ref imgID[oldj + 1, oldi]);
                    images[oldi, oldj] = images[oldi, oldj + 1];
                    images[oldi , oldj + 1] = null;
                    oldj = oldj + 1;
                }
                else
                {
                    MessageBox.Show("Không thể thực hiện bước đi này!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (isWin())
                {
                    MessageBox.Show("You Win", "Result", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                Debug.Write("Toa do moi la :" + oldi + " " + oldj);
                Debug.WriteLine("");


            }
            else if (e.Key == Key.Up)
            {
                Debug.WriteLine("TOA DO cua o trong");
                Debug.Write(oldi + " " + oldj);
                Debug.WriteLine("");
                if (oldi >= 0 && oldi <= 2 && oldj <= 2 && oldj >= 1)
                {
                    // Phan anh duoc di chuyen
                    Image isMoved = images[oldi, oldj - 1];

                    // Toa do moi : 2 0
                    Canvas.SetLeft(isMoved, oldi * (imageWidth + padding));
                    Canvas.SetTop(isMoved, oldj * (imageHeight + padding));

                    SwapNum(ref imgID[oldj, oldi], ref imgID[oldj - 1, oldi]);
                    images[oldi, oldj] = images[oldi, oldj - 1];
                    images[oldi , oldj -1] = null;
                    oldj = oldj - 1;
                }
                else
                {
                    MessageBox.Show("Không thể thực hiện bước đi này!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (isWin())
                {
                    MessageBox.Show("You Win", "Result", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }

            else if (e.Key == Key.Left)
            {
                Debug.WriteLine("TOA DO cua o trong");
                Debug.Write(oldi + " " + oldj);
                Debug.WriteLine("");
                if (oldi >= 1 && oldi <= 2 && oldj <= 2 && oldj >= 0)
                {
                    // Phan anh duoc di chuyen
                    Image isMoved = images[oldi - 1, oldj];

                    // Toa do moi : 2 0
                    Canvas.SetLeft(isMoved, oldi * (imageWidth + padding));
                    Canvas.SetTop(isMoved, oldj * (imageHeight + padding));

                    SwapNum(ref imgID[oldj, oldi], ref imgID[oldj, oldi - 1]);
                    images[oldi, oldj] = images[oldi -1, oldj];
                    images[oldi - 1, oldj] = null;
                    oldi = oldi - 1;
                }
                else
                {
                    MessageBox.Show("Không thể thực hiện bước đi này!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (isWin())
                {
                    MessageBox.Show("You Win", "Result", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            else if (e.Key == Key.Right)
            {
                Debug.WriteLine("TOA DO cua o trong");
                Debug.Write(oldi + " " + oldj);
                Debug.WriteLine("");
                if (oldi >= 0 && oldi < 2 && oldj <= 2 && oldj >= 0)
                {
                    // Phan anh duoc di chuyen
                    Image isMoved = images[oldi + 1, oldj];

                    // Toa do moi : 2 0
                    Canvas.SetLeft(isMoved, oldi * (imageWidth + padding));
                    Canvas.SetTop(isMoved, oldj * (imageHeight + padding));

                    SwapNum(ref imgID[oldj, oldi], ref imgID[oldj, oldi + 1]);
                    images[oldi, oldj] = images[oldi + 1, oldj];
                    images[oldi + 1, oldj] = null;
                    oldi = oldi + 1;
                }
                else
                {
                    MessageBox.Show("Không thể thực hiện bước đi này!","ERROR",MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (isWin())
                {
                    MessageBox.Show("You Win", "Result",MessageBoxButton.OK,MessageBoxImage.Stop);
                }
            }

            Debug.Write("Toa do moi la :" + oldi + " " + oldj);
            Debug.WriteLine("");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Debug.Write(imgID[i, j]);
                }
                Debug.WriteLine("");
            }

        }
    }
}


