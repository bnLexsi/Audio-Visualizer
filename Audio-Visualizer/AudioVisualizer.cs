using Guna.UI2.WinForms;
using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AudioVisualizer
{
    public partial class AudioVisualizer : Form
    {
        private WasapiLoopbackCapture capture;
        private const int fftLength = 1024;
        private Complex[] fftBuffer = new Complex[fftLength];
        private int fftPos = 0;
        private float[] spectrumData = new float[fftLength / 2];
        private const int BAR_COUNT = 12;
        private Guna2Panel[] audioPanels = new Guna2Panel[BAR_COUNT];
        private float[] currentHeights = new float[BAR_COUNT];
        private int[] bandIndices = new int[BAR_COUNT + 1];
        private int[] panelBottomPositions = new int[BAR_COUNT];
        private const int MAX_PANEL_HEIGHT = 400;
        private readonly float[] frequencyScales = new float[12]
        {
            3.5f,  
            3.8f,  
            2.8f,  
            3.5f,  
            4.5f,  
            5.5f,  
            7.0f,  
            8.5f,  
            10.0f, 
            11.0f, 
            13.0f, 
            3.5f   
        };
        public AudioVisualizer()
        {
            InitializeComponent();     
            FindPanels();
            CalculateLogarithmicBands();
            InitAudioCapture();

            this.Location = new Point(120, 120);
            Timer animTimer = new Timer();
            animTimer.Interval = 16;
            animTimer.Tick += AnimTimer_Tick;
            animTimer.Start();
        }

        

        private void FindPanels()
        {
            for (int i = 0; i < BAR_COUNT; i++)
            {
                string panelName = $"ses{i + 1}pnl";
                Control[] found = this.Controls.Find(panelName, true);

                if (found.Length > 0 && found[0] is Guna2Panel panel)
                {
                    audioPanels[i] = panel;
                    panelBottomPositions[i] = panel.Top + panel.Height;
                    currentHeights[i] = panel.Height;
                }
                else
                {
                    MessageBox.Show($"{panelName} isimli Guna2Panel bulunamadı!");
                }
            }
        }

        private void CalculateLogarithmicBands()
        {
            int maxIndex = fftLength / 2;
            double minFreq = 1;
            double maxFreq = maxIndex;

            for (int i = 0; i <= BAR_COUNT; i++)
            {
                double logIndex = minFreq * Math.Pow(maxFreq / minFreq, (double)i / BAR_COUNT);
                bandIndices[i] = Math.Min((int)logIndex, maxIndex);
            }
            bandIndices[10] = 220; 
            bandIndices[11] = 380; 
            bandIndices[12] = 512;
        }

        private void InitAudioCapture()
        {
            capture = new WasapiLoopbackCapture();

            capture.DataAvailable += (s, e) =>
            {
                for (int i = 0; i < e.BytesRecorded; i += 4)
                {
                    float sample = BitConverter.ToSingle(e.Buffer, i);

                    fftBuffer[fftPos].X = sample;
                    fftBuffer[fftPos].Y = 0;
                    fftPos++;

                    if (fftPos >= fftLength)
                    {
                        fftPos = 0;
                        FastFourierTransform.FFT(true, (int)Math.Log(fftLength, 2), fftBuffer);

                        for (int j = 0; j < fftLength / 2; j++)
                        {
                            spectrumData[j] = (float)Math.Sqrt(fftBuffer[j].X * fftBuffer[j].X + fftBuffer[j].Y * fftBuffer[j].Y);
                        }
                    }
                }
            };

            capture.StartRecording();
        }

        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            float attackSpeed = 0.50f; 
            float decaySpeed = 7.0f; 

            for (int i = 0; i < BAR_COUNT; i++)
            {
                if (audioPanels[i] == null) continue;

                int startBin = bandIndices[i];
                int endBin = bandIndices[i + 1];
                if (endBin <= startBin) endBin = startBin + 1;

                float maxIntensity = 0;
                for (int j = startBin; j < endBin; j++)
                {
                    if (spectrumData[j] > maxIntensity)
                        maxIntensity = spectrumData[j];
                }

                if (maxIntensity < 0.005f)
                    maxIntensity = 0;

                float targetHeight = maxIntensity * 900f * frequencyScales[i];

                if (targetHeight > MAX_PANEL_HEIGHT) targetHeight = MAX_PANEL_HEIGHT;

                if (targetHeight > currentHeights[i])
                {
                    currentHeights[i] += (targetHeight - currentHeights[i]) * attackSpeed;
                }
                else
                {
                    currentHeights[i] -= decaySpeed;
                    if (currentHeights[i] < 15) currentHeights[i] = 15;
                }
                UpdatePanelBounds(i, (int)currentHeights[i]);
            }
        }

        private void UpdatePanelBounds(int index, int newHeight)
        {
            Guna2Panel panel = audioPanels[index];           
            panel.Height = newHeight;
            panel.Top = panelBottomPositions[index] - newHeight;
        }

        private void AudioVisualizer_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Control)
            {
                
                Rectangle workingArea = Screen.FromControl(this).WorkingArea;
                int padding = 12;
                int padding2 = 120;

                switch (e.KeyCode)
                {
                    case Keys.Left: // Ekranın en soluna yasla (Yüksekliği koru)
                        this.Left = workingArea.Left + padding;
                        e.Handled = true; // Tuş vuruşunu işlendi olarak işaretle
                        break;

                    case Keys.Right: // Ekranın en sağına yasla (Yüksekliği koru)
                        this.Left = workingArea.Right - this.Width - padding;
                        e.Handled = true;
                        break;

                    case Keys.Up: // Ekranın en üstüne yasla (Yatay konumu koru)
                        this.Top = workingArea.Top + padding;
                        e.Handled = true;
                        break;

                    case Keys.Down: // Ekranın en altına yasla (Yatay konumu koru)
                        this.Top = workingArea.Bottom - this.Height - padding;
                        e.Handled = true;
                        break;

                    case Keys.Space:
                        this.Top = workingArea.Top + padding2;
                        this.Left = workingArea.Left + padding2;
                        e.Handled = true;
                        break;

                }
            }
        }

        private void kapabtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CoolBoy13_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void CoolBoy13_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void AudioVisualizer_Load(object sender, EventArgs e)
        {
           
            this.KeyDown += AudioVisualizer_KeyDown;
        }
    }
}
