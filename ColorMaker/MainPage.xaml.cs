using System.Text.RegularExpressions;
using CommunityToolkit.Maui.Alerts;

namespace ColorMaker;

public partial class MainPage : ContentPage
{
    private Random _random = new Random();
    private Dictionary<string, Slider> _sliders = new Dictionary<string, Slider>() { };

    private Color State =>
        Color.FromRgb(red: Convert.ToByte(_sliders["red"].Value),
                        blue: Convert.ToByte(_sliders["blue"].Value),
                            green: Convert.ToByte(_sliders["green"].Value));

    private string HexColorString => State.ToArgbHex();

    private double Luma => State.GetLuminosity();

    public MainPage()
	{
		InitializeComponent();
        InitializeSliders();

        //Initialize event handlers
        SizeChanged += OnSizeChanged;
		
	}

    private void InitializeSliders()
    {

        var mainGridColor = mainGridLayout.BackgroundColor;
       
        _sliders.Add("red", redSlider);
        _sliders.Add("green", greenSlider);
        _sliders.Add("blue", blueSlider);

        mainGridColor.ToRgb(out byte red, out byte green, out byte blue);

        _sliders["red"].Value = red;
        _sliders["green"].Value = green;
        _sliders["blue"].Value = blue;

        _sliders["red"].ValueChanged += OnValueChanged;
        _sliders["green"].ValueChanged += OnValueChanged;
        _sliders["blue"].ValueChanged += OnValueChanged;

        UpdateColor(mainGridColor.ToArgbHex());

    }

    private void UpdateLabelHexColor(string text)
    {
        string pattern = @"\#\w+";

        var regex = new Regex(pattern);
        labelHexColor.Text = regex.Replace(labelHexColor.Text, text);
    }

    private void UpdateColor(string hexColorString)
    {
        UpdateLabelHexColor(hexColorString);
        mainGridLayout.BackgroundColor = Color.FromArgb(hexColorString);
        buttonGenerateRandomColor.BackgroundColor = Color.FromArgb(hexColorString);

        if(Luma > 0.5)
        {
            buttonGenerateRandomColor.TextColor = Colors.Black;
            buttonGenerateRandomColor.BorderColor = Colors.LightGrey;
            buttonGenerateRandomColor.BorderWidth = 1;
        }
        else
        {
            buttonGenerateRandomColor.TextColor = Colors.White;
            buttonGenerateRandomColor.BorderWidth = 0;
        }
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        mainFrame.WidthRequest = Width * 0.75;

        mainFrame.MinimumWidthRequest = Width * 0.75;
        mainFrame.MaximumWidthRequest = Width;
    }

    private void OnValueChanged(object sender, EventArgs e)
    {
        var senderSlider = (Slider)sender;
        var found = _sliders.First(x => x.Value.Id == senderSlider.Id);

        _sliders[found.Key].Value = senderSlider.Value;

        UpdateColor(hexColorString: HexColorString);
    }

    private async void CopyToClipboard(object sender, EventArgs e)
    {
        string pattern = @"\#\w+";

        var regex = new Regex(pattern);
        var match = regex.Match(labelHexColor.Text).Value;

        await Clipboard.Default.SetTextAsync(match);
        var toast = Toast.Make("Color copied", CommunityToolkit.Maui.Core.ToastDuration.Short, 12);
        await toast.Show(new CancellationTokenSource().Token);
        //await DisplayAlert("", "Hex color copied to clipboard", "OK");
    }

    private void CreateRandomColor(object sender, EventArgs e)
    {
        _sliders["red"].Value = _random.Next(0,255);
        _sliders["green"].Value = _random.Next(0,255);
        _sliders["blue"].Value = _random.Next(0,255);

        UpdateColor(HexColorString);
    }

}


