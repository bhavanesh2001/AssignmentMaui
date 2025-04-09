using Assignment.DataBase;
using Assignment.ViewModels;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;

namespace Assignment;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseBarcodeReader()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		//View Models
		builder.Services.AddSingleton<MainPageViewModel>();
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "barcodes.db3");
        builder.Services.AddSingleton(new BarcodeDatabase(dbPath));
        //Views
        builder.Services.AddSingleton<MainPage>();

        //mappings
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.BackgroundTintList = global::Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);

#endif
        });
#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
