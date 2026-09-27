// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class BreadcrumbBarPage : Page
{
    // We use a separate _defaultFolders list to preserve the original folder structure,
    // because BreadcrumbBar2.ItemsSource is bound directly to the Folders collection.
    // When a breadcrumb item is clicked, items are removed directly from Folders,
    // which means we lose access to the full original list.
    private readonly List<Folder> _defaultFolders = new()
    {
        new Folder { Name = LocalizationHelper.Translate("Home") },
        new Folder { Name = LocalizationHelper.Translate("Folder1") },
        new Folder { Name = LocalizationHelper.Translate("Folder2") },
        new Folder { Name = LocalizationHelper.Translate("Folder3") },
    };

    public ObservableCollection<Folder> Folders { get; } = new();

    public readonly string[] FoldersString = new string[]
    {
        LocalizationHelper.Translate("Home"), LocalizationHelper.Translate("Documents"),
        LocalizationHelper.Translate("Design"), LocalizationHelper.Translate("Northwind"),
        LocalizationHelper.Translate("Images"), LocalizationHelper.Translate("Folder1"),
        LocalizationHelper.Translate("Folder2"), LocalizationHelper.Translate("Folder3")
    };
    public BreadcrumbBarPage()
    {
        this.InitializeComponent();

        BreadcrumbBar2.ItemClicked += BreadcrumbBar2_ItemClicked;
        Folders.Clear();
        foreach (var folder in _defaultFolders)
            Folders.Add(folder);
    }

    private void BreadcrumbBar2_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        if (BreadcrumbBar2.ItemsSource is not ObservableCollection<Folder> items)
        {
            return;
        }

        for (int i = items.Count - 1; i >= args.Index + 1; i--)
        {
            items.RemoveAt(i);
        }
    }

    private void ResetSampleButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // To restore the BreadcrumbBar to its initial state, we compare Folders (the live collection)
        // with _defaultFolders (the original state), and add back any missing items.
        // This ensures reset works even after user navigation modifies the ItemsSource.
        if (BreadcrumbBar2.ItemsSource is not ObservableCollection<Folder> items)
        {
            return;
        }

        foreach (var folder in _defaultFolders)
        {
            if (!items.Contains(folder))
            {
                items.Add(folder);
            }
        }


        // Announce reset success notifiication.
        UIHelper.AnnounceActionForAccessibility(ResetSampleBtn, LocalizationHelper.Translate("BreadcrumbBar sample reset successful."), "BreadCrumbBarSampleResetNotificationId");
    }
}

public class Folder
{
    public string Name { get; set; } = string.Empty;
}
