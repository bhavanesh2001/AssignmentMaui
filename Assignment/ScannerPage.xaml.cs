using System.Threading.Tasks;
using Assignment.Models;
using ZXing.Net.Maui;

namespace Assignment;

public partial class ScannerPage : ContentPage
{
    private Func<BarCode, Task> callback;

    public ScannerPage(Func<BarCode, Task> callback)
    {
        InitializeComponent();
        this.callback = callback;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
		await Navigation.PopModalAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
		cameraBarcodeReaderView.Options = new BarcodeReaderOptions
		{
		Formats = BarcodeFormats.All,
		AutoRotate = true,
		Multiple = true
		};
    }

    bool isScanning = false;


    private void cameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
		if (isScanning) return;

		if (!isScanning)
		{
            isScanning = true;
            if (e.Results.Count() > 0)
			{
				foreach (var code in e.Results)
				{
					bool allDigits = code.Value.All(char.IsDigit);
					if (code.Value.Length == 16 && allDigits)
					{
						var barcode = new BarCode() { Code = code.Value };
                        callback?.Invoke(barcode);
					}
				}
				MainThread.BeginInvokeOnMainThread(() => Navigation.PopModalAsync());
			}
		}
    }
}