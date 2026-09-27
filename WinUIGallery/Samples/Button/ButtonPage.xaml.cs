// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class ButtonPage : Page
{
    public ButtonPage()
    {
        this.InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button b)
        {
            string name = b.Name;

            switch (name)
            {
                case "Button1":
                    Control1Output.Text = LocalizationHelper.Translate("You clicked: {0}").Replace("{0}", name, StringComparison.Ordinal);
                    break;
                case "Button2":
                    Control2Output.Text = LocalizationHelper.Translate("You clicked: {0}").Replace("{0}", name, StringComparison.Ordinal);
                    break;

            }
        }
    }
}
