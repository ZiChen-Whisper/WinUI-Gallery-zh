// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.Storage.FileProperties;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;
public sealed partial class StoragePickersPage : Page
{
    private IReadOnlyList<PickerLocationId> pickerLocationIds { get; set; } = new List<PickerLocationId>(Enum.GetValues<PickerLocationId>());
    private IReadOnlyList<PickerViewMode> pickerViewModes { get; set; } = new List<PickerViewMode>(Enum.GetValues<PickerViewMode>());
    private IReadOnlyList<ThumbnailMode> thumbnailModes { get; set; } = new List<ThumbnailMode>(Enum.GetValues<ThumbnailMode>());

    public StoragePickersPage()
    {
        InitializeComponent();
    }

    private async void PickSingleFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            //disable the button to avoid double-clicking
            button.IsEnabled = false;

            var picker = new FileOpenPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);

            // Define allowed file types
            if (FileTypeComboBox1.SelectedItem is ComboBoxItem selectedItem)
            {
                string? tag = selectedItem.Tag?.ToString();

                switch (tag)
                {
                    case ".txt":
                        picker.FileTypeFilter.Add(".txt");
                        break;

                    case "images":
                        picker.FileTypeFilter.Add(".jpg");
                        picker.FileTypeFilter.Add(".png");
                        break;
                }
            }

            picker.CommitButtonText = CommitButtonTextTextBox.Text;

            picker.SuggestedStartLocation = (PickerLocationId)PickerLocationComboBox1.SelectedItem;

            picker.ViewMode = (PickerViewMode)PickerViewModeComboBox1.SelectedItem;

            // Show the picker dialog window
            var file = await picker.PickSingleFileAsync();
            PickedSingleFileTextBlock.Text = file != null
                ? string.Format(LocalizationHelper.Translate("Picked: {0}"), file.Path)
                : LocalizationHelper.Translate("No file selected.");

            //re-enable the button
            button.IsEnabled = true;

            UIHelper.AnnounceActionForAccessibility(button, PickedSingleFileTextBlock.Text, "FilePickedNotificationId");
        }
    }

    private async void PickMultipleFilesButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            // Disable the button to avoid double-clicking
            button.IsEnabled = false;

            var picker = new FileOpenPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);

            // Define allowed file types
            if (FileTypeComboBox2.SelectedItem is ComboBoxItem selectedItem)
            {
                string? tag = selectedItem.Tag?.ToString();
                switch (tag)
                {
                    case ".txt":
                        picker.FileTypeFilter.Add(".txt");
                        break;

                    case "images":
                        picker.FileTypeFilter.Add(".jpg");
                        picker.FileTypeFilter.Add(".png");
                        break;

                    case "*":
                        picker.FileTypeFilter.Add("*");
                        break;
                }
            }

            picker.CommitButtonText = CommitButtonTextTextBox2.Text;
            picker.SuggestedStartLocation = (PickerLocationId)PickerLocationComboBox2.SelectedItem;
            picker.ViewMode = (PickerViewMode)PickerViewModeComboBox2.SelectedItem;

            // Show the picker dialog
            var files = await picker.PickMultipleFilesAsync();

            if (files.Count > 0)
            {
                PickedMultipleFilesTextBlock.Text = "";
                foreach (var file in files)
                {
                    PickedMultipleFilesTextBlock.Text += "- " + string.Format(LocalizationHelper.Translate("Picked: {0}"), file.Path) + Environment.NewLine;
                }
            }
            else
            {
                PickedMultipleFilesTextBlock.Text = LocalizationHelper.Translate("No files selected.");
            }

            // Re-enable the button
            button.IsEnabled = true;

            // Announce result for accessibility
            UIHelper.AnnounceActionForAccessibility(button, PickedMultipleFilesTextBlock.Text, "FilesPickedNotificationId");
        }
    }

    private string ComboBoxItemToFileFilter(Object ob)
    {
        if (ob is ComboBoxItem item)
        {
            if ((string)item.Tag == ".txt") return "\r\n\r\n        picker.FileTypeFilter.Add(\".txt\");";
            if ((string)item.Tag == "images") return "\r\n\r\n        picker.FileTypeFilter.Add(\".jpg\");\r\n        picker.FileTypeFilter.Add(\".png\");";
        }
        return "";
    }

    private async void SaveFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            button.IsEnabled = false;

            var picker = new FileSavePicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);

            if (TxtCheckBox.IsChecked == true)
                picker.FileTypeChoices.Add(LocalizationHelper.Translate("Text Files"), new List<string>() { ".txt" });

            if (JsonCheckBox.IsChecked == true)
                picker.FileTypeChoices.Add(LocalizationHelper.Translate("JSON Files"), new List<string>() { ".json" });

            if (XmlCheckBox.IsChecked == true)
                picker.FileTypeChoices.Add(LocalizationHelper.Translate("XML Files"), new List<string>() { ".xml" });

            picker.DefaultFileExtension = DefaultExtensionComboBox.SelectedItem.ToString();

            picker.SuggestedFileName = SuggestedFileNameTextBox.Text;

            picker.CommitButtonText = CommitButtonTextTextBox3.Text;

            picker.SuggestedStartLocation = (PickerLocationId)PickerLocationComboBox3.SelectedItem;

            picker.SuggestedFolder = SuggestedFolderTextBox.Text;

            var result = await picker.PickSaveFileAsync();

            if (result != null)
            {
                string savePath = result.Path;
                await File.WriteAllTextAsync(savePath, FileContentTextBox.Text);
                SavedFileTextBlock.Text = string.Format(LocalizationHelper.Translate("File saved to: {0}"), savePath);
            }
            else
            {
                SavedFileTextBlock.Text = LocalizationHelper.Translate("File save canceled.");
            }

            button.IsEnabled = true;

            UIHelper.AnnounceActionForAccessibility(button, SavedFileTextBlock.Text, "FileSavedNotificationId");
        }
    }

    string TxtCheckBoxIsCheckedToCode(bool? isChecked)
    {
        if (isChecked == true) return "\r\n        picker.FileTypeChoices.Add(\"Text Files\", new List<string>() { \".txt\" });\r\n";
        return "";
    }

    string JsonCheckBoxIsCheckedToCode(bool? isChecked)
    {
        if (isChecked == true) return "\r\n        picker.FileTypeChoices.Add(\"JSON Files\", new List<string>() { \".json\" });\r\n";
        return "";
    }

    string XmlCheckBoxIsCheckedToCode(bool? isChecked)
    {
        if (isChecked == true) return "\r\n        picker.FileTypeChoices.Add(\"XML Files\", new List<string>() { \".xml\" });\r\n";
        return "";
    }

    private async void PickFolderButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            // disable the button to avoid double-clicking
            button.IsEnabled = false;

            var picker = new FolderPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);

            picker.CommitButtonText = CommitButtonTextTextBox4.Text;
            picker.SuggestedStartLocation = (PickerLocationId)PickerLocationComboBox4.SelectedItem;
            picker.ViewMode = (PickerViewMode)PickerViewModeComboBox3.SelectedItem;

            // Show the picker dialog
            var folder = await picker.PickSingleFolderAsync();
            PickedFolderTextBlock.Text = folder != null
                ? string.Format(LocalizationHelper.Translate("Picked: {0}"), folder.Path)
                : LocalizationHelper.Translate("No folder selected.");

            // re-enable the button
            button.IsEnabled = true;

            UIHelper.AnnounceActionForAccessibility(button, PickedFolderTextBlock.Text, "FolderPickedNotificationId");
        }
    }

    private async void PickFileForThumbnailButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            button.IsEnabled = false;

            var picker = new FileOpenPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);
            picker.FileTypeFilter.Add("*");

            var pickResult = await picker.PickSingleFileAsync();
            if (pickResult == null)
            {
                ThumbnailDetailsTextBlock.Text = LocalizationHelper.Translate("No file selected.");
                ThumbnailImage.Source = null;
                button.IsEnabled = true;

                return;
            }

            // The new WinAppSDK picker returns a path string; bridge to StorageFile so we can
            // use the Windows.Storage.FileProperties thumbnail APIs (no WinAppSDK equivalent exists).
            var file = await StorageFile.GetFileFromPathAsync(pickResult.Path);

            var thumbnailMode = (ThumbnailMode)ThumbnailModeComboBox.SelectedItem;

            // NumberBox.Value is NaN when the user clears the field; fall back to a sane default.
            double rawSize = ThumbnailSizeNumberBox.Value;
            uint size = double.IsNaN(rawSize) || rawSize <= 0 ? 200u : (uint)rawSize;

            try
            {
                using var thumbnail = await file.GetThumbnailAsync(thumbnailMode, size, ThumbnailOptions.UseCurrentScale);
                if (thumbnail != null)
                {
                    var bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(thumbnail);
                    ThumbnailImage.Source = bitmap;

                    ThumbnailDetailsTextBlock.Text = string.Format(LocalizationHelper.Translate("File: {0}\nMode: ThumbnailMode.{1}\nRequested size: {2}\nReturned size: {3} x {4}"),
                        file.Name, thumbnailMode, size, thumbnail.OriginalWidth, thumbnail.OriginalHeight);
                }
                else
                {
                    ThumbnailImage.Source = null;
                    ThumbnailDetailsTextBlock.Text = LocalizationHelper.Translate("No thumbnail available for the selected file.");
                }
            }
            catch (Exception ex)
            {
                ThumbnailImage.Source = null;
                ThumbnailDetailsTextBlock.Text = string.Format(LocalizationHelper.Translate("Could not retrieve a thumbnail for \"{0}\" using ThumbnailMode.{1}.\nTry a different mode (for example, SingleItem) or a different file.\n({2})"),
                    file.Name, thumbnailMode, ex.HResult.ToString("X8"));
            }

            button.IsEnabled = true;

            UIHelper.AnnounceActionForAccessibility(button, ThumbnailDetailsTextBlock.Text, "ThumbnailPickedNotificationId");
        }
    }

    private async void SelectSuggestedFolderButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            button.IsEnabled = false;

            var picker = new FolderPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);

            picker.CommitButtonText = LocalizationHelper.Translate("Select folder");

            var folder = await picker.PickSingleFolderAsync();

            if (folder != null)
            {
                SuggestedFolderTextBox.Text = folder.Path;
            }

            button.IsEnabled = true;

            UIHelper.AnnounceActionForAccessibility(
                button,
                folder != null && !string.IsNullOrEmpty(folder.Path)
                    ? "Folder selected: " + SuggestedFolderTextBox.Text
                    : LocalizationHelper.Translate("No folder selected."),
                "SuggestedFolderNotificationId");
        }
    }
}
