using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfTremAnimado
{
    public partial class MainWindow : Window
    {
        private double _previousTremLeft;
        private double _wheelAngle = 0;

        // NOVO: Adicione uma instância da classe MediaPlayer
        private MediaPlayer _mediaPlayer = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += CompositionTarget_Rendering;

            _previousTremLeft = Canvas.GetLeft(TremCompleto);

            // NOVO: Adiciona a lógica de áudio. A reprodução começa quando a janela é carregada.
            this.Loaded += MainWindow_Loaded;

            // NOVO: Assina o evento para criar o loop do áudio.
            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string audioPath = "Sounds/AUDIO DO TREM.wav";
            if (!System.IO.File.Exists(audioPath))
            {
                MessageBox.Show($"Arquivo de áudio não encontrado: {audioPath}");
                return;
            }
            _mediaPlayer.Open(new Uri(System.IO.Path.GetFullPath(audioPath)));
            _mediaPlayer.Play();
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            // NOVO: Reinicia o áudio do início para que ele toque em loop
            _mediaPlayer.Stop();
            _mediaPlayer.Play();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            // Lógica para a rotação das rodas
            double currentTremLeft = Canvas.GetLeft(TremCompleto);
            double deltaX = currentTremLeft - _previousTremLeft;

            double rotationMultiplier = EscalaTrem.ScaleX > 0 ? 5 : -5;
            double deltaAngle = deltaX * rotationMultiplier;
            _wheelAngle += deltaAngle;

            RotacaoRoda1.Angle = _wheelAngle;
            RotacaoRoda2.Angle = _wheelAngle;
            RotacaoRoda3.Angle = _wheelAngle;

            _previousTremLeft = currentTremLeft;

            // Lógica para a barra de acoplamento (conecta todas as rodas)
            double radius = 25;

            double angleRad = _wheelAngle * Math.PI / 180.0;

            double crankPin1X = 40 + radius * Math.Cos(angleRad);
            double crankPin1Y = 40 + radius * Math.Sin(angleRad);

            double crankPin3X = 190 + 40 + radius * Math.Cos(angleRad);
            double crankPin3Y = 40 + radius * Math.Sin(angleRad);

            double barraLength = Math.Sqrt(Math.Pow(crankPin3X - crankPin1X, 2) + Math.Pow(crankPin3Y - crankPin1Y, 2));
            double barraAngle = Math.Atan2(crankPin3Y - crankPin1Y, crankPin3X - crankPin1X) * 180 / Math.PI;

            BarraAcoplamento.Width = barraLength;
            Canvas.SetLeft(BarraAcoplamento, crankPin1X);
            Canvas.SetTop(BarraAcoplamento, crankPin1Y);
            RotacaoBarra.Angle = barraAngle;
        }
    }
}