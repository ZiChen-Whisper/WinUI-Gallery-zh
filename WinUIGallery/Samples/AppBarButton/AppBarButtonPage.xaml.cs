// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class AppBarButtonPage : Page
{
    public AppBarButtonPage()
    {
        this.InitializeComponent();
    }

    private void AppBarButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button b)
        {
            string name = b.Name;

            switch (name)
            {
                case "Button1":
                    Control1Output.Text = LocalizationHelper.Translate("You clicked: {0}").Replace("{0}", name, StringComparison.Ordinal);
                    UIHelper.AnnounceActionForAccessibility(Button1, Control1Output.Text, "AppBarButtonSuccessNotificationId");
                    break;
                case "Button2":
                    Control2Output.Text = LocalizationHelper.Translate("You clicked: {0}").Replace("{0}", name, StringComparison.Ordinal);
                    UIHelper.AnnounceActionForAccessibility(Button2, Control2Output.Text, "AppBarButtonSuccessNotificationId");
                    break;
                case "Button3":
                    Control3Output.Text = LocalizationHelper.Translate("You clicked: {0}").Replace("{0}", name, StringComparison.Ordinal);
                    UIHelper.AnnounceActionForAccessibility(Button3, Control3Output.Text, "AppBarButtonSuccessNotificationId");
                    break;
                case "Button4":
                    Control4Output.Text = LocalizationHelper.Translate("You clicked: {0}").Replace("{0}", name, StringComparison.Ordinal);
                    UIHelper.AnnounceActionForAccessibility(Button4, Control4Output.Text, "AppBarButtonSuccessNotificationId");
                    break;
                case "Button5":
                    Control5Output.Text = LocalizationHelper.Translate("You clicked: {0}").Replace("{0}", name, StringComparison.Ordinal);
                    UIHelper.AnnounceActionForAccessibility(Button5, Control5Output.Text, "AppBarButtonSuccessNotificationId");
                    break;
            }
        }
    }
}
