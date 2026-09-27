// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.StartScreen;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class JumpListPage : Page
{
    public JumpListPage()
    {
        this.InitializeComponent();
    }

    private async void AddTasksButton_Click(object sender, RoutedEventArgs e)
    {
        if (!NativeMethods.IsAppPackaged)
        {
            return;
        }

        JumpList jumpList = await JumpList.LoadCurrentAsync();

        JumpListItem composeTask = JumpListItem.CreateWithArguments("/compose", LocalizationHelper.Translate("New Message"));
        composeTask.Description = LocalizationHelper.Translate("Compose a new message");
        composeTask.Logo = new Uri("ms-appx:///Assets/Tiles/AppList.targetsize-48.png");

        JumpListItem searchTask = JumpListItem.CreateWithArguments("/search", LocalizationHelper.Translate("Search"));
        searchTask.Description = LocalizationHelper.Translate("Search for items");
        searchTask.Logo = new Uri("ms-appx:///Assets/Tiles/AppList.targetsize-48.png");

        jumpList.Items.Add(composeTask);
        jumpList.Items.Add(searchTask);

        await jumpList.SaveAsync();
    }

    private async void ClearTasksButton_Click(object sender, RoutedEventArgs e)
    {
        if (!NativeMethods.IsAppPackaged)
        {
            return;
        }

        JumpList jumpList = await JumpList.LoadCurrentAsync();
        jumpList.Items.Clear();
        await jumpList.SaveAsync();
    }

    private async void AddCustomGroupButton_Click(object sender, RoutedEventArgs e)
    {
        if (!NativeMethods.IsAppPackaged)
        {
            return;
        }

        JumpList jumpList = await JumpList.LoadCurrentAsync();

        JumpListItem item1 = JumpListItem.CreateWithArguments("/project-alpha", LocalizationHelper.Translate("Project Alpha"));
        item1.GroupName = LocalizationHelper.Translate("Projects");
        item1.Description = LocalizationHelper.Translate("Open Project Alpha");
        item1.Logo = new Uri("ms-appx:///Assets/Tiles/AppList.targetsize-48.png");

        JumpListItem item2 = JumpListItem.CreateWithArguments("/project-beta", LocalizationHelper.Translate("Project Beta"));
        item2.GroupName = LocalizationHelper.Translate("Projects");
        item2.Description = LocalizationHelper.Translate("Open Project Beta");
        item2.Logo = new Uri("ms-appx:///Assets/Tiles/AppList.targetsize-48.png");

        jumpList.Items.Add(item1);
        jumpList.Items.Add(item2);

        await jumpList.SaveAsync();
    }
}
