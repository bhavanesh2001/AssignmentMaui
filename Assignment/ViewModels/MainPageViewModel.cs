using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Assignment.DataBase;
using Assignment.Models;

namespace Assignment.ViewModels;

public class MainPageViewModel: INotifyPropertyChanged
{
    #region  Commands
     public ICommand ScanCommand => new Command(StartScan);

     public ICommand ClearCommand => new Command(ClearItems);

     public ICommand DeleteCommand => new Command(DeleteSelectedItem);

    public ICommand ReloadCommand => new Command(ReloadItems);

    public ICommand SubmitCommand => new Command(SubmitItems);

    public ICommand DetailsCommand => new Command(GoToDetailsView);

    public ICommand ScanSelectedCommand => new Command(SelectScanMode);

    #endregion

    #region Properties
    private ObservableCollection<BarCode>? currentBarCodes;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<BarCode>? CurrentBarCodes
    {
        get { return currentBarCodes; }
        set 
        { 
            currentBarCodes = value; 
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(CurrentBarCodes)));
        }
    }

    private BarCode? currentSelectedItem;
    public BarCode? CurrentSelectedItem
    {
        get { return currentSelectedItem; }
        set 
        { 
            currentSelectedItem = value; 
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(CurrentSelectedItem)));
        }
    }

    private string entryText;

    public string EntryText
    {
        get { return entryText; }
        set 
        {
            bool allDigits = value.All(char.IsDigit);
            if (value.Length == 16 && allDigits)
            {
               if(CurrentBarCodes is null)
                    CurrentBarCodes = new ObservableCollection<BarCode>();

                if (!CurrentBarCodes.Any(x => x.Code == value))
                {
                    var barcode = new BarCode() { Code = value };
                    CurrentBarCodes.Add(barcode);
                }
                else
                {
                    App.Current.MainPage?.DisplayAlert("Alert", "Barcode alredy Exists", "Ok");

                }
                EntryText = string.Empty;
            }
            else
            {
                entryText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntryText)));
            }

        }
    }

    private bool isScanning = true;

    public bool IsScanning
    {
        get { return isScanning; }
        set
        { 
            isScanning = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsScanning)));
        }
    }



    BarcodeDatabase _db;
    #endregion

    #region  Constructor
    public MainPageViewModel(BarcodeDatabase db)
    {
        _db = db;
    }
    #endregion

    #region  Private
    private async void StartScan()
    {
        if(CurrentBarCodes is null)
            CurrentBarCodes = new ObservableCollection<BarCode>();

        var callback = async (BarCode barcodeValue) => 
        {
            if (barcodeValue is not null && barcodeValue.Code is not null)
            {
                var isInDb = await _db.IsCodeExistsAsync(barcodeValue.Code);
                if (CurrentBarCodes.Any(x => x.Code == barcodeValue.Code) || isInDb)
                {
                    MainThread.BeginInvokeOnMainThread(() => App.Current.MainPage?.DisplayAlert("Alert", "Barcode alredy Exists", "Ok"));
                    return;
                }

                CurrentBarCodes.Add(barcodeValue);
            }
        };
        await Shell.Current.Navigation.PushModalAsync(new ScannerPage(callback));
    }

    private void ClearItems()
    {
       CurrentBarCodes?.Clear();
    }

    private async void DeleteSelectedItem()
    {
        if (CurrentSelectedItem is not null && CurrentBarCodes is not null)
        {
            CurrentBarCodes.Remove(CurrentSelectedItem);
            if (CurrentSelectedItem.Code is not null)
            {
                var isInDb = await _db.IsCodeExistsAsync(CurrentSelectedItem.Code);
                if (isInDb)
                {
                    var item = await _db.GetBarCodeAsync(CurrentSelectedItem.Id);
                    if (item is not null)
                        await _db.DeleteBarCodeAsync(item);

                }
            }
        }
    }

    private async void ReloadItems()
    {
        var items = await _db.GetBarCodesAsync();
      
        if(CurrentBarCodes is null)
            CurrentBarCodes = new ObservableCollection<BarCode>();

        foreach (var item in items)
        {
            if (!CurrentBarCodes.Any(x => x.Code == item.Code))
            {
                CurrentBarCodes.Add(item);
            }
        }
    }

    private async void SubmitItems(object obj)
    {
        if(CurrentBarCodes is null || CurrentBarCodes.Count == 0)
            return;

       await _db.SaveBarCodesAsync(CurrentBarCodes.ToList());

        CurrentBarCodes.Clear();
    }

    private async void GoToDetailsView(object obj)
    {
        IsScanning = false;
    }

    private void SelectScanMode(object obj)
    {
        IsScanning = true;
    }
    #endregion
}