﻿using MiniExcelLibs;
using System.Collections.ObjectModel;
using WASender.Models;

namespace WASender;

public partial class MainPage : ContentPage
{
    public bool IsRefreshing { get; set; }
    public ObservableCollection<WAMessage> WAmsgs { get; set; } = new();
    public Command RefreshCommand { get; set; }
    public WAMessage SelectedMsg { get; set; }
    public string SelectedExcel { get; set; }
    public string SelectedExcelFullPath { get; set; }


    public MainPage()
	{
        SelectedExcel = "Silahkan Pilih Excel";

        RefreshCommand = new Command(async () =>
        {
            await LoadExcel(SelectedExcelFullPath);
            IsRefreshing = false;
            OnPropertyChanged(nameof(IsRefreshing));
        });

        BindingContext = this;
        InitializeComponent();
	}

    private async void SelectFile(object sender, EventArgs e)
    {
        var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".xls", ".xlsx" } },
                    });

        PickOptions options = new()
        {
            PickerTitle = "Silahkan Pilih File Excel",
            FileTypes = customFileType,
        };

        await PickAndShow(options);
    }

    public async Task<FileResult> PickAndShow(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith("xls", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    SelectedExcelFullPath = result.FullPath;
                    SelectedExcel = result.FileName;
                    OnPropertyChanged(nameof(SelectedExcel));

                    var msgs = await LoadExcel(SelectedExcelFullPath);
                    WAmsgs.Clear();
                    foreach (var msg in msgs)
                    {
                        WAmsgs.Add(msg);
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }

        return null;
    }

    public async Task<List<WAMessage>> LoadExcel(string path)
    {
        List<WAMessage> result = new List<WAMessage>();
        try
        {
            var resmsg = MiniExcel.Query<WAMessage>(path);
            foreach(var msg in resmsg)
            {
                msg.Status = (string.IsNullOrEmpty(msg.Status)) ? "Waiting" : msg.Status;
                result.Add(msg);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }

        return result;
    }
}

