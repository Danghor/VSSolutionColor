﻿using System.Collections.Generic;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System.IO;
using System.Linq;

namespace SolutionColor
{
    public class SolutionColorSettingStore
    {
        private const string CollectionName = "SolutionColorSettings";
        private const string AutomaticColorPickIdentifier = "AutomaticColorPick";

        public static bool IsAutomaticColorPickEnabled()
        {
            var settingsStore = GetSettingsStore();
            if (settingsStore.PropertyExists(CollectionName, AutomaticColorPickIdentifier))
                return settingsStore.GetBoolean(CollectionName, AutomaticColorPickIdentifier);
            else
                return false; // off by default.
        }

        public static void SetAutomaticColorPickEnabled(bool enabled)
        {
            var settingsStore = GetSettingsStore();
            settingsStore.SetBoolean(CollectionName, AutomaticColorPickIdentifier, enabled);
        }

        public static void SaveOrOverwriteSolutionColor(string solutionPath, System.Drawing.Color color)
        {
            if (string.IsNullOrEmpty(solutionPath)) return;
            solutionPath = Path.GetFullPath(solutionPath);

            byte[] colorBytes = { color.A, color.R, color.G, color.B };

            var settingsStore = GetSettingsStore();
            settingsStore.SetMemoryStream(CollectionName, solutionPath, new MemoryStream(colorBytes));
        }

        public void RemoveSolutionColorSetting(string solutionPath)
        {
            if (string.IsNullOrEmpty(solutionPath)) return;
            solutionPath = Path.GetFullPath(solutionPath);

            var settingsStore = GetSettingsStore();
            if (settingsStore.PropertyExists(CollectionName, solutionPath))
                settingsStore.DeleteProperty(CollectionName, solutionPath);
        }

        /// <summary>
        /// Retrieves the color setting for a given solution.
        /// </summary>
        /// <param name="solutionPath">Path for the solution to check.</param>
        /// <param name="color">Color we saved for the solution</param>
        /// <returns>true if there was a color saved, false if not.</returns>
        public static bool GetSolutionColorSetting(string solutionPath, out System.Drawing.Color color)
        {
            color = System.Drawing.Color.Black;

            if (string.IsNullOrEmpty(solutionPath)) return false;
            solutionPath = Path.GetFullPath(solutionPath);

            var settingsStore = GetSettingsStore();
            if (settingsStore.PropertyExists(CollectionName, solutionPath))
            {
                MemoryStream colorBytes = settingsStore.GetMemoryStream(CollectionName, solutionPath);
                color = System.Drawing.Color.FromArgb(colorBytes.ReadByte(), colorBytes.ReadByte(), colorBytes.ReadByte(), colorBytes.ReadByte());
                return true;
            }
            else
                return false;
        }

        private const string CustomColorPaletteName = "CustomColorPalette";

        public static int[] GetCustomColorList()
        {
            var settingsStore = GetSettingsStore();
            if (settingsStore.PropertyExists(CollectionName, CustomColorPaletteName))
            {
                string customColorPaletteString = settingsStore.GetString(CollectionName, CustomColorPaletteName);
                return customColorPaletteString.Split(new[]{ ' ' }, System.StringSplitOptions.RemoveEmptyEntries).Select(x =>
                {
                    // Can't be cautious enough when reading user string.
                    if (!int.TryParse(x, out int color))
                        return -1;
                    else
                        return color;
                }).ToArray();
            }
            else
            {
                return new int[0];
            }
        }

        public static void SaveCustomColorList(IEnumerable<int> colorList)
        {
            // Save as string since it is easy and save.
            // Alternative would be memorystream like we do with the color per solution path.
            // Then however, we'd need to save the count separately [...]
            var settingsStore = GetSettingsStore();
            settingsStore.SetString(CollectionName, CustomColorPaletteName, colorList.Aggregate(string.Empty, (s, i) => s + " " + i.ToString()));
        }

        private static WritableSettingsStore GetSettingsStore()
        {
            var settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            WritableSettingsStore settingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            // Ensure our settings collection exists.
            if (!settingsStore.CollectionExists(CollectionName))
                settingsStore.CreateCollection(CollectionName);

            return settingsStore;
        }
    }
}