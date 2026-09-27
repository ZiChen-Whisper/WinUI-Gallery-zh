// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinUIGallery.Models;

namespace WinUIGallery.Helpers;

internal static class LocalizationHelper
{
    private static readonly Dictionary<string, string> UiTranslations = new(StringComparer.Ordinal);
    private static readonly string[] UiTranslationFiles =
    [
        "Localization/Ui.zh-CN.json",
        "Localization/UiSamples.A-H.zh-CN.json",
        "Localization/UiSamples.I-P.zh-CN.json",
        "Localization/UiSamples.P-Z.zh-CN.json"
    ];
    private static Dictionary<string, CatalogEntry> GroupTranslations = new(StringComparer.Ordinal);
    private static Dictionary<string, CatalogEntry> ItemTranslations = new(StringComparer.Ordinal);
    private static bool initialized;

    private static readonly string[] LocalizablePropertyNames =
    [
        "Text", "Content", "Header", "Description", "PlaceholderText", "Title",
        "Label", "Caption", "OffContent", "OnContent", "ToolTip",
        "PrimaryButtonText", "SecondaryButtonText", "CloseButtonText",
        "ColorName", "ColorExplanation", "CopiedMessage"
    ];

    public static bool IsChinese => SettingsHelper.Current.DisplayLanguage == "zh-CN";

    public static async Task InitializeAsync()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        if (!IsChinese)
        {
            return;
        }

        try
        {
            foreach (string translationFile in UiTranslationFiles)
            {
                try
                {
                    string uiText = await FileLoader.LoadText(translationFile);
                    var uiTranslations = JsonSerializer.Deserialize<Dictionary<string, string>>(uiText);
                    if (uiTranslations is null)
                    {
                        continue;
                    }

                    foreach (var translation in uiTranslations)
                    {
                        // 基础界面词条优先；示例分片只补充其未覆盖的原文。
                        UiTranslations.TryAdd(translation.Key, translation.Value);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"无法加载中文翻译文件 {translationFile}：{ex.Message}");
                }
            }

            string catalogText = await FileLoader.LoadText("Localization/Catalog.zh-CN.json");
            var catalog = JsonSerializer.Deserialize<CatalogTranslationFile>(catalogText,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (catalog is not null)
            {
                GroupTranslations = catalog.Groups;
                ItemTranslations = catalog.Items;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"无法加载中文翻译数据：{ex.Message}");
        }
    }

    public static string Translate(string source)
    {
        if (!IsChinese)
        {
            return source;
        }

        if (UiTranslations.TryGetValue(source, out string? translation))
        {
            return translation;
        }

        foreach (var format in UiTranslations)
        {
            MatchCollection placeholders = Regex.Matches(format.Key, @"\{(\d+)\}");
            if (placeholders.Count == 0)
            {
                continue;
            }

            var pattern = new System.Text.StringBuilder("^");
            var captureGroups = new List<(int PlaceholderIndex, string GroupName)>();
            int previousPosition = 0;
            foreach (Match placeholder in placeholders)
            {
                pattern.Append(Regex.Escape(format.Key[previousPosition..placeholder.Index]));
                string groupName = $"value{captureGroups.Count}";
                pattern.Append($"(?<{groupName}>.*?)");
                captureGroups.Add((int.Parse(placeholder.Groups[1].Value, CultureInfo.InvariantCulture), groupName));
                previousPosition = placeholder.Index + placeholder.Length;
            }

            pattern.Append(Regex.Escape(format.Key[previousPosition..]));
            pattern.Append('$');
            Match match = Regex.Match(source, pattern.ToString(), RegexOptions.CultureInvariant | RegexOptions.Singleline);
            if (!match.Success)
            {
                continue;
            }

            var values = new Dictionary<int, string>();
            foreach ((int placeholderIndex, string groupName) in captureGroups)
            {
                values.TryAdd(placeholderIndex, match.Groups[groupName].Value);
            }

            return Regex.Replace(format.Value, @"\{(\d+)\}", valueMatch =>
            {
                int placeholderIndex = int.Parse(valueMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                return values.TryGetValue(placeholderIndex, out string? value) ? value : valueMatch.Value;
            });
        }

        return source;
    }

    public static void ApplyCatalogTranslations(Root root)
    {
        if (!IsChinese)
        {
            return;
        }

        foreach (ControlInfoDataGroup group in root.Groups)
        {
            if (GroupTranslations.TryGetValue(group.UniqueId, out CatalogEntry? groupTranslation))
            {
                group.Title = groupTranslation.Title ?? group.Title;
            }

            foreach (ControlInfoDataItem item in group.Items)
            {
                if (!ItemTranslations.TryGetValue(item.UniqueId, out CatalogEntry? itemTranslation))
                {
                    continue;
                }

                item.Title = itemTranslation.Title ?? item.Title;
                item.Subtitle = itemTranslation.Subtitle ?? item.Subtitle;
                item.Description = itemTranslation.Description ?? item.Description;
            }
        }
    }

    public static void Apply(UIElement? root)
    {
        if (!IsChinese || root is null)
        {
            return;
        }

        var pending = new Stack<DependencyObject>();
        pending.Push(root);
        while (pending.Count > 0)
        {
            DependencyObject current = pending.Pop();
            ApplyToElement(current);

            int childCount = VisualTreeHelper.GetChildrenCount(current);
            for (int index = 0; index < childCount; index++)
            {
                pending.Push(VisualTreeHelper.GetChild(current, index));
            }

            if (current is NavigationView navigationView)
            {
                PushMenuItems(navigationView.MenuItems, pending);
                PushMenuItems(navigationView.FooterMenuItems, pending);
            }
            else if (current is NavigationViewItem navigationItem)
            {
                PushMenuItems(navigationItem.MenuItems, pending);
            }
        }
    }

    private static void PushMenuItems(IEnumerable<object> menuItems, Stack<DependencyObject> pending)
    {
        foreach (object menuItem in menuItems)
        {
            if (menuItem is DependencyObject dependencyObject)
            {
                pending.Push(dependencyObject);
            }
        }
    }

    private static void ApplyToElement(DependencyObject element)
    {
        foreach (string propertyName in LocalizablePropertyNames)
        {
            PropertyInfo? property = element.GetType().GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public);
            if (property is null || !property.CanRead || !property.CanWrite ||
                (property.PropertyType != typeof(string) && property.PropertyType != typeof(object)))
            {
                continue;
            }

            try
            {
                if (property.GetValue(element) is string source && UiTranslations.TryGetValue(source, out string? translation))
                {
                    property.SetValue(element, translation);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"无法翻译 {element.GetType().Name}.{propertyName}：{ex.Message}");
            }
        }

        if (element is FrameworkElement frameworkElement)
        {
            string automationName = AutomationProperties.GetName(frameworkElement);
            if (UiTranslations.TryGetValue(automationName, out string? translatedName))
            {
                AutomationProperties.SetName(frameworkElement, translatedName);
            }

            string automationHelp = AutomationProperties.GetHelpText(frameworkElement);
            if (UiTranslations.TryGetValue(automationHelp, out string? translatedHelp))
            {
                AutomationProperties.SetHelpText(frameworkElement, translatedHelp);
            }

            object? toolTip = ToolTipService.GetToolTip(frameworkElement);
            if (toolTip is string toolTipText && UiTranslations.TryGetValue(toolTipText, out string? translatedToolTip))
            {
                ToolTipService.SetToolTip(frameworkElement, translatedToolTip);
            }
        }
    }

    private sealed class CatalogTranslationFile
    {
        public Dictionary<string, CatalogEntry> Groups { get; set; } = new(StringComparer.Ordinal);
        public Dictionary<string, CatalogEntry> Items { get; set; } = new(StringComparer.Ordinal);
    }

    private sealed class CatalogEntry
    {
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
    }
}
