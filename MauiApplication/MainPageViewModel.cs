using System;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiApplication.Models;

namespace MauiApplication;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    List<Item> items;
}
