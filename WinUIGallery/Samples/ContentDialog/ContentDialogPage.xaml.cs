// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class ContentDialogPage : Page
{
    public ContentDialogPage()
    {
        this.InitializeComponent();
    }

    private void SetDialogResultText(TextBlock targetTextBlock, string text)
    {
        targetTextBlock.Text = LocalizationHelper.Translate(text);
        var peer = FrameworkElementAutomationPeer.FromElement(targetTextBlock) ?? FrameworkElementAutomationPeer.CreatePeerForElement(targetTextBlock);
        peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
    }

    private async void ShowDialog_Click(object sender, RoutedEventArgs e)
    {
        ContentDialogExample dialog = new ContentDialogExample();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = LocalizationHelper.Translate("Save your work?");
        dialog.PrimaryButtonText = LocalizationHelper.Translate("Save");
        dialog.SecondaryButtonText = LocalizationHelper.Translate("Don't Save");
        dialog.CloseButtonText = LocalizationHelper.Translate("Cancel");
        dialog.DefaultButton = ContentDialogButton.Primary;
        dialog.Content = new ContentDialogContent();
        LocalizationHelper.Apply(dialog.Content as UIElement);

        if (sender is Button button &&
            VisualTreeHelper.GetParent(button) is StackPanel stackPanel)
        {
            dialog.RequestedTheme = stackPanel.ActualTheme;
        }

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            SetDialogResultText(DialogResult, "User saved their work");
        }
        else if (result == ContentDialogResult.Secondary)
        {
            SetDialogResultText(DialogResult, "User did not save their work");
        }
        else
        {
            SetDialogResultText(DialogResult, "User cancelled the dialog");
        }
    }

    private async void ShowDialogNoDefault_Click(object sender, RoutedEventArgs e)
    {
        ContentDialogExample dialog = new ContentDialogExample();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = LocalizationHelper.Translate("Replace file?");
        dialog.PrimaryButtonText = LocalizationHelper.Translate("Replace");
        dialog.SecondaryButtonText = LocalizationHelper.Translate("Keep");
        dialog.CloseButtonText = LocalizationHelper.Translate("Cancel");
        dialog.DefaultButton = ContentDialogButton.None;
        dialog.Content = new ContentDialogContent();
        LocalizationHelper.Apply(dialog.Content as UIElement);

        if (sender is Button button &&
            VisualTreeHelper.GetParent(button) is StackPanel stackPanel)
        {
            dialog.RequestedTheme = stackPanel.ActualTheme;
        }

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            SetDialogResultText(DialogResultNoDefault, "User replaced the file");
        }
        else if (result == ContentDialogResult.Secondary)
        {
            SetDialogResultText(DialogResultNoDefault, "User kept the file");
        }
        else
        {
            SetDialogResultText(DialogResultNoDefault, "User cancelled the dialog");
        }
    }
}
