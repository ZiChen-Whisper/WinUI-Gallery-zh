// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class ClipboardPage : Page
{

    private string textToCopy = "";

    public ClipboardPage()
    {
        this.InitializeComponent();
        richEditBox.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, LocalizationHelper.Translate("This text will be copied to the clipboard."));
        UpdateHistoryRoamingStatus();
    }

    private void CopyText_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        richEditBox.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out textToCopy);
        var package = new DataPackage();
        package.SetText(textToCopy);
        Clipboard.SetContent(package);

        UIHelper.AnnounceActionForAccessibility(button, LocalizationHelper.Translate("Text copied to clipboard"), "TextCopiedSuccessNotificationId");

        VisualStateManager.GoToState(this, "ConfirmationClipboardVisible", false);
        Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        // Automatically hide the confirmation text after 2 seconds
        if (dispatcherQueue != null)
        {
            dispatcherQueue.TryEnqueue(async () =>
            {
                await Task.Delay(2000);
                VisualStateManager.GoToState(this, "ConfirmationClipboardCollapsed", false);
            });
        }

    }

    private async void PasteText_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        var package = Clipboard.GetContent();
        if (package.Contains(StandardDataFormats.Text))
        {
            var text = await package.GetTextAsync();
            PasteClipboard2.Text = text;

            UIHelper.AnnounceActionForAccessibility(button, LocalizationHelper.Translate("Text pasted from clipboard"), "TextPastedSuccessNotificationId");
        }

    }

    private void CopyImage_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        var package = new DataPackage();
        var imageUri = new Uri("ms-appx:///Assets/SampleMedia/rainier.jpg");
        package.SetBitmap(RandomAccessStreamReference.CreateFromUri(imageUri));

        if (Clipboard.SetContentWithOptions(package, null))
        {
            ImageStatusText.Text = LocalizationHelper.Translate("Image copied to clipboard.");
            ImageStatusText.Visibility = Visibility.Visible;
            UIHelper.AnnounceActionForAccessibility(button, LocalizationHelper.Translate("Image copied to clipboard"), "ImageCopiedSuccessNotificationId");
        }
        else
        {
            ImageStatusText.Text = LocalizationHelper.Translate("Error copying image to clipboard.");
            ImageStatusText.Visibility = Visibility.Visible;
        }
    }

    private async void PasteImage_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        var package = Clipboard.GetContent();
        if (package.Contains(StandardDataFormats.Bitmap))
        {
            try
            {
                IRandomAccessStreamReference imageReference = await package.GetBitmapAsync();
                using (IRandomAccessStreamWithContentType imageStream = await imageReference.OpenReadAsync())
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(imageStream);
                    PastedImage.Source = bitmapImage;
                    PastedImage.Visibility = Visibility.Visible;
                    ImageStatusText.Text = LocalizationHelper.Translate("Image pasted from clipboard.");
                    ImageStatusText.Visibility = Visibility.Visible;
                    UIHelper.AnnounceActionForAccessibility(button, LocalizationHelper.Translate("Image pasted from clipboard"), "ImagePastedSuccessNotificationId");
                }
            }
            catch (Exception ex)
            {
                ImageStatusText.Text = LocalizationHelper.Translate("Error pasting image: {0}").Replace("{0}", ex.Message, StringComparison.Ordinal);
                ImageStatusText.Visibility = Visibility.Visible;
            }
        }
        else
        {
            ImageStatusText.Text = LocalizationHelper.Translate("Bitmap format is not available in the clipboard.");
            ImageStatusText.Visibility = Visibility.Visible;
            PastedImage.Visibility = Visibility.Collapsed;
        }
    }

    private async void CopyFiles_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        var filePicker = new FileOpenPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);
        filePicker.FileTypeFilter.Add("*");

        var pickedFiles = await filePicker.PickMultipleFilesAsync();
        if (pickedFiles.Count > 0)
        {
            List<Windows.Storage.IStorageItem> storageItems = new();
            foreach (var file in pickedFiles)
            {
                storageItems.Add(await Windows.Storage.StorageFile.GetFileFromPathAsync(file.Path));
            }

            var package = new DataPackage();
            package.SetStorageItems(storageItems);
            package.RequestedOperation = DataPackageOperation.Copy;

            if (Clipboard.SetContentWithOptions(package, null))
            {
                FilesStatusText.Text = LocalizationHelper.Translate("{0} file(s) copied to clipboard.").Replace("{0}", pickedFiles.Count.ToString(), StringComparison.Ordinal);
                UIHelper.AnnounceActionForAccessibility(button, LocalizationHelper.Translate("{0} files copied to clipboard").Replace("{0}", pickedFiles.Count.ToString(), StringComparison.Ordinal), "FilesCopiedSuccessNotificationId");
            }
            else
            {
                FilesStatusText.Text = LocalizationHelper.Translate("Error copying files to clipboard.");
            }
        }
    }

    private async void PasteFiles_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        var package = Clipboard.GetContent();
        if (package.Contains(StandardDataFormats.StorageItems))
        {
            try
            {
                var storageItems = await package.GetStorageItemsAsync();
                DataPackageOperation operation = package.RequestedOperation;

                string operationName = operation switch
                {
                    DataPackageOperation.Copy => "Copy",
                    DataPackageOperation.Move => "Move",
                    DataPackageOperation.Link => "Link",
                    _ => "None"
                };

                var output = new StringBuilder();
                output.AppendLine(LocalizationHelper.Translate("Requested operation: {0}").Replace("{0}", LocalizationHelper.Translate(operationName), StringComparison.Ordinal));
                output.AppendLine(LocalizationHelper.Translate("File(s) on clipboard ({0}):").Replace("{0}", storageItems.Count.ToString(), StringComparison.Ordinal));
                foreach (var item in storageItems)
                {
                    output.AppendLine($"  • {item.Name}");
                }

                FilesStatusText.Text = output.ToString();
                UIHelper.AnnounceActionForAccessibility(button, LocalizationHelper.Translate("Files pasted from clipboard"), "FilesPastedSuccessNotificationId");
            }
            catch (Exception ex)
            {
                FilesStatusText.Text = LocalizationHelper.Translate("Error pasting files: {0}").Replace("{0}", ex.Message, StringComparison.Ordinal);
            }
        }
        else
        {
            FilesStatusText.Text = LocalizationHelper.Translate("StorageItems format is not available in the clipboard.");
        }
    }

    private void CopyWithOptions_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        string text = HistoryRoamingTextBox.Text;
        if (string.IsNullOrEmpty(text))
        {
            HistoryRoamingStatusText.Text = LocalizationHelper.Translate("Please enter text to copy.");
            return;
        }

        var package = new DataPackage();
        package.SetText(text);

        var options = new ClipboardContentOptions
        {
            IsAllowedInHistory = AllowHistoryToggle.IsOn,
            IsRoamable = AllowRoamingToggle.IsOn
        };

        if (Clipboard.SetContentWithOptions(package, options))
        {
            var status = new StringBuilder(LocalizationHelper.Translate("Text copied to clipboard."));
            status.Append(' ');
            status.Append(LocalizationHelper.Translate("History: {0}").Replace("{0}", LocalizationHelper.Translate(options.IsAllowedInHistory ? "allowed" : "excluded"), StringComparison.Ordinal));
            status.Append(". ");
            status.Append(LocalizationHelper.Translate("Roaming: {0}").Replace("{0}", LocalizationHelper.Translate(options.IsRoamable ? "allowed" : "excluded"), StringComparison.Ordinal));
            status.Append('.');
            HistoryRoamingStatusText.Text = status.ToString();
            UIHelper.AnnounceActionForAccessibility(button, LocalizationHelper.Translate("Text copied with options"), "OptionsCopiedSuccessNotificationId");
        }
        else
        {
            HistoryRoamingStatusText.Text = LocalizationHelper.Translate("Error copying content to clipboard.");
        }
    }

    private void UpdateHistoryRoamingStatus()
    {
        try
        {
            bool historyEnabled = Clipboard.IsHistoryEnabled();
            bool roamingEnabled = Clipboard.IsRoamingEnabled();
            HistoryEnabledText.Text = LocalizationHelper.Translate("Clipboard history: {0}").Replace("{0}", LocalizationHelper.Translate(historyEnabled ? "enabled" : "disabled"), StringComparison.Ordinal);
            RoamingEnabledText.Text = LocalizationHelper.Translate("Clipboard roaming: {0}").Replace("{0}", LocalizationHelper.Translate(roamingEnabled ? "enabled" : "disabled"), StringComparison.Ordinal);
        }
        catch
        {
            // IsHistoryEnabled/IsRoamingEnabled may not be available on all systems
        }
    }

    private void ShowFormats_Click(object sender, RoutedEventArgs args)
    {
        DataPackageView package = Clipboard.GetContent();
        var output = new StringBuilder();

        if (package != null && package.AvailableFormats.Count > 0)
        {
            output.AppendLine(LocalizationHelper.Translate("Available formats on the clipboard:"));
            foreach (string format in package.AvailableFormats)
            {
                output.AppendLine($"  • {format}");
            }
        }
        else
        {
            output.AppendLine(LocalizationHelper.Translate("The clipboard is empty."));
        }

        OtherOperationsStatusText.Text = output.ToString();
    }

    private void ClearClipboard_Click(object sender, RoutedEventArgs args)
    {
        try
        {
            Clipboard.Clear();
            OtherOperationsStatusText.Text = LocalizationHelper.Translate("Clipboard has been cleared.");
        }
        catch (Exception ex)
        {
            OtherOperationsStatusText.Text = LocalizationHelper.Translate("Error clearing clipboard: {0}").Replace("{0}", ex.Message, StringComparison.Ordinal);
        }
    }

    private void ContentChangedToggle_Toggled(object sender, RoutedEventArgs args)
    {
        if (ContentChangedToggle.IsOn)
        {
            Clipboard.ContentChanged += OnClipboardContentChanged;
            OtherOperationsStatusText.Text = LocalizationHelper.Translate("Monitoring clipboard changes...");
        }
        else
        {
            Clipboard.ContentChanged -= OnClipboardContentChanged;
            OtherOperationsStatusText.Text = LocalizationHelper.Translate("Stopped monitoring clipboard changes.");
        }
    }

    private void OnClipboardContentChanged(object? sender, object e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            DataPackageView package = Clipboard.GetContent();
            var output = new StringBuilder(LocalizationHelper.Translate("Clipboard content changed!\n"));

            if (package != null && package.AvailableFormats.Count > 0)
            {
                output.AppendLine(LocalizationHelper.Translate("New formats:"));
                foreach (string format in package.AvailableFormats)
                {
                    output.AppendLine($"  • {format}");
                }
            }
            else
            {
                output.AppendLine(LocalizationHelper.Translate("Clipboard is now empty."));
            }

            OtherOperationsStatusText.Text = output.ToString();
        });
    }
}
