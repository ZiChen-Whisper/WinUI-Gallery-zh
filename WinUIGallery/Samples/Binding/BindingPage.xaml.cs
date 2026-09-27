// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class BindingPage : Page
{
    public string GreetingMessage { get; set; } = LocalizationHelper.Translate("Hello, WinUI 3!");

    public ExampleViewModel ViewModel { get; set; }

    public List<ListDetailItem> Items { get; set; }

    public BindingPage()
    {
        this.InitializeComponent();

        ViewModel = new ExampleViewModel
        {
            Title = LocalizationHelper.Translate("Welcome to WinUI 3"),
            Description = LocalizationHelper.Translate("This is an example of binding to a view model."),
            NullString = string.Empty,
        };
        DataContext = ViewModel;

        Items = new List<ListDetailItem>
        {
            new ListDetailItem
            {
                Id = 0,
                Title = LocalizationHelper.Translate("Item 1"),
                Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer id facilisis lectus. Cras nec convallis ante, quis pulvinar tellus.",
                DateCreated = new DateTime(2025, 6, 15, 9, 30, 0)
            },
            new ListDetailItem
            {
                Id = 1,
                Title = LocalizationHelper.Translate("Item 2"),
                Text = "Quisque accumsan pretium ligula in faucibus. Mauris sollicitudin augue vitae lorem cursus condimentum quis ac mauris.",
                DateCreated = new DateTime(2025, 7, 22, 14, 15, 0)
            },
            new ListDetailItem
            {
                Id = 2,
                Title = LocalizationHelper.Translate("Item 3"),
                Text = "Ut consequat magna luctus justo egestas vehicula. Integer pharetra risus libero, et posuere justo mattis et.",
                DateCreated = new DateTime(2025, 8, 3, 11, 0, 0)
            },
            new ListDetailItem
            {
                Id = 3,
                Title = LocalizationHelper.Translate("Item 4"),
                Text = "Duis facilisis, quam ut laoreet commodo, elit ex aliquet massa, non varius tellus lectus et nunc.",
                DateCreated = new DateTime(2025, 9, 10, 16, 45, 0)
            }
        };
    }

    public string FormatDate(DateTimeOffset? date)
    {
        if (date.HasValue)
        {
            var culture = LocalizationHelper.IsChinese ? CultureInfo.GetCultureInfo("zh-CN") : CultureInfo.CurrentCulture;
            string dateText = date.Value.ToString(
                LocalizationHelper.IsChinese ? "yyyy年M月d日 dddd" : "dddd, MMMM d, yyyy",
                culture);
            return LocalizationHelper.Translate("Selected date is: {0}").Replace("{0}", dateText, StringComparison.Ordinal);
        }
        else
        {
            return LocalizationHelper.Translate("No date selected");
        }
    }
}

public partial class ExampleViewModel : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private string _description = string.Empty;
    private string _nullString = string.Empty;

    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }

    public string NullString
    {
        get => _nullString;
        set
        {
            if (_nullString != value)
            {
                _nullString = value;
                OnPropertyChanged(nameof(_nullString));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ListDetailItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string DateCreatedFormatted => DateCreated.ToString(
        "MMM d, yyyy h:mm tt",
        LocalizationHelper.IsChinese ? CultureInfo.GetCultureInfo("zh-CN") : CultureInfo.CurrentCulture);
}
