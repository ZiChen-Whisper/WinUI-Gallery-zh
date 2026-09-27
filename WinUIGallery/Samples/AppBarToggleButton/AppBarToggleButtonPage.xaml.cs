// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class AppBarToggleButtonPage : Page
{
    public AppBarToggleButtonPage()
    {
        this.InitializeComponent();
    }

    private void AppBarButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is AppBarToggleButton b)
        {
            string name = b.Name;
            string state = LocalizationHelper.Translate(b.IsChecked?.ToString() ?? "Indeterminate");
            string outputTemplate = LocalizationHelper.Translate("IsChecked = {0}");

            switch (name)
            {
                case "Button1":
                    Control1Output.Text = outputTemplate.Replace("{0}", state, StringComparison.Ordinal);
                    break;
                case "Button2":
                    Control2Output.Text = outputTemplate.Replace("{0}", state, StringComparison.Ordinal);
                    break;
                case "Button3":
                    Control3Output.Text = outputTemplate.Replace("{0}", state, StringComparison.Ordinal);
                    break;
                case "Button4":
                    Control4Output.Text = outputTemplate.Replace("{0}", state, StringComparison.Ordinal);
                    break;
            }
        }
    }
}
