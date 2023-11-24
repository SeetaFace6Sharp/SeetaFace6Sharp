using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using CustomMessageBox.Avalonia;
using DynamicData;
using Epoxy;
using FlashCap;
using ReactiveUI;
using SeetaFace6Sharp.Example.Camera.Models;
using SkiaSharp;

namespace SeetaFace6Sharp.Example.Camera.ViewModels
{
    public class CaptureControlViewModel : ReactiveObject
    {
        public CaptureControlViewModel()
        {
            this.WhenAnyValue(o => o.IsEnabled)
                .Subscribe(o =>
                {
                    this.RaisePropertyChanged(nameof(IsEnabled));
                });

            this.WhenAnyValue(o => o.Device)
                .Subscribe(o =>
                {
                    this.RaisePropertyChanged(nameof(Device));
                    this.RaisePropertyChanged(nameof(DeviceList));

                    InitDeviceList(o);
                });

            this.WhenAnyValue(o => o.Characteristics)
                .Subscribe(o =>
                {
                    this.RaisePropertyChanged(nameof(Characteristics));
                    this.RaisePropertyChanged(nameof(CharacteristicsList));

                    if (_factory != null)
                    {
                        _factory.Dispose();
                        _factory = null;
                    }
                });

            this.WhenAnyValue(o => o.BtnName)
                .Subscribe(o =>
                {
                    this.RaisePropertyChanged(nameof(BtnName));
                });

            StartBtnCommand = ReactiveCommand.Create(BtnAction);

            //加载默认字体
            using var stream = Avalonia.Platform.AssetLoader.Open(new Uri($"avares://{GetType().Assembly.GetName().Name}/Assets/Fonts/MSYH.TTC"));
            this.SKTypeface = SKTypeface.FromStream(stream);
        }

        #region Commonds

        public ICommand StartBtnCommand { get; private set; }

        #endregion

        #region 属性

        private SKTypeface SKTypeface { get; }


        public FaceDetectOptions DetectOptions { get; private set; } = new FaceDetectOptions();

        public ObservableCollection<CaptureDeviceDescriptor?> DeviceList { get; } = new();


        public CaptureDeviceDescriptor? _device = null;
        public CaptureDeviceDescriptor? Device
        {
            get => this._device;
            set
            {
                this.RaiseAndSetIfChanged(ref _device, value);
            }
        }

        public ObservableCollection<VideoCharacteristics> CharacteristicsList { get; } = new();


        private VideoCharacteristics? _characteristics = null;
        public VideoCharacteristics? Characteristics
        {
            get => this._characteristics;
            set => this.RaiseAndSetIfChanged(ref _characteristics, value);
        }

        private bool _isEnabled = false;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }

        private bool _isEnabledDeviceSelector = false;

        public bool IsEnabledDeviceSelector
        {
            get => this.IsEnabled && _isEnabledDeviceSelector;
            set => this.RaiseAndSetIfChanged(ref _isEnabledDeviceSelector, value);
        }

        public string? _btnName = "Not Ready";
        public string? BtnName
        {
            get => _btnName;
            set => this.RaiseAndSetIfChanged(ref _btnName, value);
        }

        public string? _btnBackground = "Gainsboro";
        public string? BtnBackground
        {
            get => _btnBackground;
            set => this.RaiseAndSetIfChanged(ref _btnBackground, value);
        }

        public SKBitmap? _image = null;
        public SKBitmap? Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }

        #endregion

        private long _countFrames;
        private enum States
        {
            Ready,
            Running,
            Stopped
        }

        private States _state = States.Stopped;
        private SeetaFace6SharpFactory? _factory = null;

        private void UpdateCurrentState(States state)
        {
            this._state = state;

            switch (this._state)
            {
                case States.Ready:
                    {
                        this.IsEnabled = true;
                        this.IsEnabledDeviceSelector = true;
                        this.BtnName = "Start";
                        this.BtnBackground = "GreenYellow";
                    }
                    break;
                case States.Running:
                    {
                        this.BtnName = "Stop";
                        this.BtnBackground = "Red";
                        this.IsEnabledDeviceSelector = false;
                    }
                    break;
                case States.Stopped:
                    {
                        this.BtnName = "Start";
                        this.BtnBackground = "GreenYellow";
                        this.IsEnabledDeviceSelector = true;
                        if (this.Image != null)
                        {
                            try { this.Image.Dispose(); } catch { }
                            this.Image = new SKBitmap(100, 100);
                            this.Image.Dispose();
                        }
                    }
                    break;
                default:

                    break;
            }
        }

        private CaptureDevice? _captureDevice = null;

        private async Task BtnAction()
        {
            if (_state == States.Stopped || _state == States.Ready)
            {
                await StartAsync();
            }
            else
            {
                await StopAsync();
            }
        }

        private async Task StartAsync()
        {
            if (this.Device == null)
            {
                await MessageBox.Show("请指定设备", "提示", MessageBoxButtons.OK);
                return;
            }
            if (this.Characteristics == null)
            {
                await MessageBox.Show("请指定分辨率", "提示", MessageBoxButtons.OK);
                return;
            }
            try
            {
                if (_factory == null)
                {
                    _factory = new SeetaFace6SharpFactory(this.Characteristics.Width, this.Characteristics.Height);
                    var faceTracker = _factory.Get<FaceTracker>();
                    faceTracker?.SetVideoStable(true);
                }
                _captureDevice = await this.Device.OpenAsync(this.Characteristics, this.OnPixelBufferArrivedAsync);
                if (_captureDevice == null)
                {
                    await MessageBox.Show($"无法打开指定的设备：{this.Device.Name}", "提示", MessageBoxButtons.OK);
                    return;
                }
                await _captureDevice.StartAsync();
                UpdateCurrentState(States.Running);
            }
            catch (Exception ex)
            {
                _captureDevice = null;
                await MessageBox.Show($"无法打开指定的设备：{this.Device.Name}，{ex.Message}", "提示", MessageBoxButtons.OK);
                return;
            }
        }

        private async Task StopAsync()
        {
            try
            {
                if (_captureDevice == null) return;
                await _captureDevice.StopAsync();
                UpdateCurrentState(States.Stopped);
            }
            finally
            {
                _captureDevice?.Dispose();
                _captureDevice = null;
            }
        }

        public void InitDeviceList(CaptureDeviceDescriptor? captureDevice = null)
        {
            if (captureDevice == null)
            {
                var devices = new CaptureDevices();
                this.DeviceList.Clear();
                var canListDevice = devices.EnumerateDescriptors().Where(d => d.Characteristics.Length >= 1);
                if (canListDevice?.Any() != true)
                {
                    return;
                }
                this.DeviceList.AddRange(canListDevice);
                this.Device = this.DeviceList.FirstOrDefault();

                VideoCharacteristics[]? characteristics = this.Device?.Characteristics;
                if (characteristics?.Any() != true)
                {
                    return;
                }
                CharacteristicsList.AddRange(characteristics);
                UpdateCurrentState(States.Ready);
                var targetCharacteristics = CharacteristicsList.FirstOrDefault(p => p.Width == 1280 && p.Height == 720 && p.PixelFormat == PixelFormats.JPEG);
                if (targetCharacteristics != null)
                {
                    this.Characteristics = targetCharacteristics;
                }
            }
            else
            {
                VideoCharacteristics[]? characteristics = this.Device?.Characteristics;
                if (characteristics?.Any() != true)
                {
                    this.CharacteristicsList.Clear();
                    return;
                }
                VideoCharacteristics? targetCharacteristics = null;
                if (this.Characteristics != null)
                {
                    targetCharacteristics = characteristics.FirstOrDefault(p => p.Width == this.Characteristics.Width && p.Height == this.Characteristics.Height && p.PixelFormat == this.Characteristics.PixelFormat);
                    if (targetCharacteristics == null)
                    {
                        targetCharacteristics = characteristics.FirstOrDefault(p => p.PixelFormat != PixelFormats.Unknown);
                    }
                }
                else
                {
                    targetCharacteristics = characteristics.FirstOrDefault(p => p.Width == 1280 && p.Height == 720 && p.PixelFormat == PixelFormats.JPEG);
                }
                this.CharacteristicsList.Clear();
                this.CharacteristicsList.AddRange(characteristics);
                if (targetCharacteristics != null)
                {
                    this.Characteristics = targetCharacteristics;
                }
            }
        }

        private async Task OnPixelBufferArrivedAsync(PixelBufferScope bufferScope)
        {
            ArraySegment<byte> image = bufferScope.Buffer.ReferImage();
            // Decode image data
            var bitmap = SKBitmap.Decode(image);
            // Capture statistics variables.
            var countFrames = Interlocked.Increment(ref this._countFrames);
            var frameIndex = bufferScope.Buffer.FrameIndex;
            var timestamp = bufferScope.Buffer.Timestamp;

            bufferScope.ReleaseNow();

            if (_state == States.Running && await UIThread.TryBind())
            {
                this.Image?.Dispose();

                var realFps = countFrames / timestamp.TotalSeconds;
                FaceDetect(bitmap, frameIndex, realFps);

                this.Image = bitmap;

            }
        }

        private void FaceDetect(SKBitmap bitmap, long frameIndex, double fps)
        {
            if (bitmap == null) return;
            using FaceImage faceImage = bitmap.ToFaceImage();
            FaceTrackInfo[]? infos = DetectOptions.IsTrack ? _factory?.Get<FaceTracker>()?.TrackVideo(faceImage, (int)(frameIndex % int.MaxValue)) : null;
            if (infos == null || infos?.Length == 0)
            {
                DrawFaceRect(bitmap, null, fps);
                return;
            }
            foreach (FaceTrackInfo faceInfo in infos)
            {
                VideoFaceInfo videoFaceInfo = new VideoFaceInfo
                {
                    Pid = faceInfo.Pid,
                    Location = faceInfo.Location
                };

                if (videoFaceInfo.IsDetectMask = DetectOptions.MaskDetect)
                {
                    var maskStatus = _factory.Get<MaskDetector>().Detect(faceImage, faceInfo);
                    videoFaceInfo.IsMasked = maskStatus.Masked;
                }
                if (DetectOptions.PropertyDetect)
                {
                    //检测人脸属性
                    FaceRecognizer? faceRecognizer = null;
                    if (videoFaceInfo.IsMasked)
                    {
                        faceRecognizer = _factory.GetFaceRecognizerWithMask();
                    }
                    else
                    {
                        faceRecognizer = _factory.Get<FaceRecognizer>();
                    }
                    var points = _factory.Get<FaceLandmarker>().Mark(faceImage, faceInfo);
                    //特征值
                    float[] extractData = faceRecognizer.Extract(faceImage, points);

                    //年龄和性别
                    videoFaceInfo.Age = _factory.Get<AgePredictor>().PredictAgeWithCrop(faceImage, points);
                    videoFaceInfo.Gender = _factory.Get<GenderPredictor>().PredictGenderWithCrop(faceImage, points);
                    videoFaceInfo.HasProperty = true;
                }
                DrawFaceRect(bitmap, videoFaceInfo, fps);
            }
        }

        private void DrawFaceRect(SKBitmap bitmap, VideoFaceInfo? faceInfo, double fps)
        {
            using (SKCanvas canvas = new SKCanvas(bitmap))
            {
                //画fps
                using (SKPaint paint = new SKPaint())
                {
                    paint.Style = SKPaintStyle.Fill;
                    paint.Color = SKColors.GreenYellow;
                    paint.TextSize = ObtainDynamic(45);

                    canvas.DrawText(fps.ToString("f2"), 20, ObtainDynamic(50), paint);
                }

                //画人脸框
                if (faceInfo != null)
                {
                    using (SKPaint paint = new SKPaint())
                    {
                        paint.Style = SKPaintStyle.Stroke;
                        paint.Color = SKColors.Red;
                        paint.StrokeWidth = ObtainDynamic(3);
                        paint.StrokeCap = SKStrokeCap.Round;
                        canvas.DrawRect(faceInfo.Location.X, faceInfo.Location.Y, faceInfo.Location.Width, faceInfo.Location.Height, paint);
                    }

                    if (faceInfo.HasProperty)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append($"PID:{faceInfo.Pid}|");
                        if (faceInfo.IsDetectMask)
                        {
                            builder.Append($"口罩:{(faceInfo.IsMasked ? "是" : "否")}|");
                        }
                        builder.Append($"年龄:{faceInfo.Age}|");
                        builder.Append($"性别:{faceInfo.GenderDescribe}");


                        using (SKPaint paint = new SKPaint())
                        {
                            paint.Style = SKPaintStyle.Fill;
                            paint.Color = SKColors.Red;
                            paint.TextSize = ObtainDynamic(40);
                            paint.Typeface = SKTypeface;

                            canvas.DrawText(builder.ToString(), faceInfo.Location.X - 3, faceInfo.Location.Y + faceInfo.Location.Height + paint.TextSize, paint);
                        }
                    }
                }
            }
        }

        private int ObtainDynamic(int baseNum)
        {
            int result = (int)(this.Characteristics.Height / 720.0 * baseNum);
            if (result <= 0)
            {
                result = 1;
            }
            return result;
        }
    }
}
