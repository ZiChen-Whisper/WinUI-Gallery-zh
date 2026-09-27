// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class RatingControlPage : Page
{
    public RatingControlPage()
    {
        this.InitializeComponent();
    }

    private void RatingControl1_ValueChanged(RatingControl sender, object args)
    {
        RatingControl1.Caption = LocalizationHelper.Translate("Your rating");
    }
}
