using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfApp3._1._1
{
    public partial class MainWindow : Window
    {
        private List<string> _fullPaths = new List<string>();
        private bool _isRepeatEnabled = false;
        private bool _isPlaying = false;
        private DispatcherTimer timer;
        private bool isDraggingSlider;
        private int currentTrackIndex = 0;


        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeMediaElements(); // Вызываем метод инициализации после загрузки окна
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100); // Установите интервал на 100 миллисекунд
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isDraggingSlider && mediaElement.NaturalDuration.HasTimeSpan)
            {
                positionSlider.Value = mediaElement.Position.TotalSeconds;
                lblCurrentTime.Content = mediaElement.Position.ToString(@"mm\:ss");
            }
        }

        private void AddMusic_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Музыкальные файлы (*.mp3, *.wav)|*.mp3;*.wav";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    _fullPaths.Add(filename);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
                    PlayList.Items.Add(fileNameWithoutExtension);
                }
            }

            StartPauseMusic_Click(sender, e);
        }

        private void StartPauseMusic_Click(object sender, RoutedEventArgs e)
        {
            if (mediaElement.Source != null)
            {
                if (!_isPlaying)
                {
                    mediaElement.Play();
                    _isPlaying = true;
                    StartPauseMusic.Content = "Пауза";
                }
                else
                {
                    mediaElement.Pause();
                    _isPlaying = false;
                    StartPauseMusic.Content = "Воспроизвести";
                }
            }
            else
            {
                if (PlayList.Items.Count > 0)
                {
                    PlayList.SelectedIndex = 0;
                    string path = _fullPaths[PlayList.SelectedIndex];
                    mediaElement.Source = new Uri(path, UriKind.Absolute);
                    mediaElement.Play();
                    _isPlaying = true;
                    StartPauseMusic.Content = "Пауза";
                }
            }
        }

        private void RepeatMusic_Click(object sender, RoutedEventArgs e)
        {
            _isRepeatEnabled = !_isRepeatEnabled;
            RepeatMusic.Content = _isRepeatEnabled ? "Повтор: ВКЛ" : "Повтор: ВЫКЛ";
        }

        private void ShuffleMusic_Click(object sender, RoutedEventArgs e)
        {
            Random rng = new Random();
            List<string> shuffledList = new List<string>();

            foreach (string item in PlayList.Items)
            {
                shuffledList.Add(item);
            }

            int n = shuffledList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = shuffledList[k];
                shuffledList[k] = shuffledList[n];
                shuffledList[n] = value;
            }

            PlayList.Items.Clear();
            foreach (string item in shuffledList)
            {
                PlayList.Items.Add(item);
            }
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isDraggingSlider)
            {
                mediaElement.Position = TimeSpan.FromSeconds(positionSlider.Value);
            }
        }

        private void PositionSlider_DragStarted(object sender, RoutedEventArgs e)
        {
            isDraggingSlider = true;
        }

        private void PositionSlider_DragCompleted(object sender, RoutedEventArgs e)
        {
            isDraggingSlider = false;
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            sliderPosition.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            timer.Start();
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_isRepeatEnabled)
            {
                mediaElement.Position = TimeSpan.Zero;
                mediaElement.Play();
            }
            else
            {
                // Implement logic for playing next track
            }
        }

        private void sliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = sliderVolume.Value / 100;

        }

        private void InitializeMediaElements()
        {
            mediaElement.LoadedBehavior = MediaState.Manual; // Ручное управление воспроизведением
            mediaElement.MediaOpened += mediaElement_MediaOpened;
            mediaElement.MediaEnded += mediaElement_MediaEnded;
            InitializeVolumeSlider();

            sliderVolume.Value = 50;

        }

        private void MediaElement_PositionChanged(object sender, EventArgs e)
        {
            if (!isDraggingSlider && mediaElement.NaturalDuration.HasTimeSpan)
            {
                positionSlider.Value = mediaElement.Position.TotalSeconds;
                lblCurrentTime.Content = mediaElement.Position.ToString(@"mm\:ss");
            }
        }

        private void InitializeVolumeSlider()
        {
            sliderVolume.Value = 50; // Установка начального значения громкости
            mediaElement.Volume = 50;
        }

        private void PlayNextTrack()
        {
            if (currentTrackIndex < _fullPaths.Count - 1)
            {
                currentTrackIndex++;
                string nextTrackPath = _fullPaths[currentTrackIndex];
                mediaElement.Source = new Uri(nextTrackPath, UriKind.Absolute);
                mediaElement.Play();
                _isPlaying = true;
                StartPauseMusic.Content = "Пауза";
            }

        }

        private void NextMusic_Click(object sender, RoutedEventArgs e)
        {
            PlayNextTrack();
        }

        private void PlayPreviousTrack()
        {
            if (currentTrackIndex > 0)
            {
                currentTrackIndex--;
                string previousTrackPath = _fullPaths[currentTrackIndex];
                mediaElement.Source = new Uri(previousTrackPath, UriKind.Absolute);
                mediaElement.Play();
                _isPlaying = true;
                StartPauseMusic.Content = "Пауза";
            }
        }

        private void PreviousMusic_Click(object sender, RoutedEventArgs e)
        {
            PlayPreviousTrack();
        }

    }
}
