﻿using Microsoft.VisualStudio.Shell;
using System;

namespace SolutionColor
{
    /// <summary>
    /// A few VS extension utils. Basically shortcuts to commonly functionality.
    /// </summary>
    internal static class VSUtils
    {
        public static EnvDTE80.DTE2 GetDTE()
        {
            if (!(Package.GetGlobalService(typeof(EnvDTE.DTE)) is EnvDTE80.DTE2 dte))
            {
                throw new Exception("Failed to retrieve DTE2!");
            }

            return dte;
        }

        public static string GetCurrentSolutionPath()
        {
            return GetDTE().Solution.FileName;
        }

        public static bool IsUsingDarkTheme()
        {
            // Probe the current theme.
            // Inspired by https://github.com/Irdis/VSTalk/blob/2d49471422a42513ac84179c27472ca6d9112047/trunk/VSTalk/VSTalk.Extension/Integration/ThemeManager.cs#L131
            uint colorSample = GetDTE().GetThemeColor(EnvDTE80.vsThemeColors.vsThemeColorToolboxBackground);
            int colorSum = (byte)(colorSample >> 16) + (byte)(colorSample >> 8) + (byte)(colorSample >> 0); // rgb
            return colorSum < 128 * 3;
        }
    }
}