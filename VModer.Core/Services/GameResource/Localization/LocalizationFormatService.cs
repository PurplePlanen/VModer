﻿using System.Drawing;
using VModer.Core.Infrastructure.Parser;
using VModer.Core.Models;

namespace VModer.Core.Services.GameResource.Localization;

public sealed class LocalizationFormatService(
    LocalizationTextColorsService localizationTextColorsService,
    LocalizationService localizationService
)
{
    /// <summary>
    /// 获取格式化后的文本, 如果解析文本颜色失败, 则统一使用黑色
    /// </summary>
    /// <param name="text">文本</param>
    /// <returns>一个集合, 包含格式化后的文本</returns>
    /// <remarks>
    /// 现支持
    /// 1. 文本颜色格式
    /// 2. 对其他本地化键的引用
    /// </remarks>
    public IReadOnlyCollection<ColorTextInfo> GetFormatText(string text)
    {
        var result = new List<ColorTextInfo>(4);

        if (LocalizationFormatParser.TryParse(text, out var formats))
        {
            foreach (var format in formats)
            {
                if (format.Type == LocalizationFormatType.Placeholder)
                {
                    // 一般来说, 包含管道符的为格式说明字符串, 不需要处理
                    if (format.Text.Contains('|'))
                    {
                        continue;
                    }

                    result.Add(new ColorTextInfo(localizationService.GetValue(format.Text), Color.Black));
                }
                else if (format.Type != LocalizationFormatType.Icon)
                {
                    result.Add(GetColorText(format));
                }
            }
        }
        else
        {
            result.Add(new ColorTextInfo(text, Color.Black));
        }

        return result;
    }

    /// <summary>
    /// 尝试将文本解析为 <see cref="ColorTextInfo"/>, 并使用 <see cref="LocalizationFormatInfo"/> 中指定的颜色, 如果颜色不存在, 则使用默认颜色
    /// </summary>
    /// <param name="format">文本格式信息</param>
    /// <returns></returns>
    public ColorTextInfo GetColorText(LocalizationFormatInfo format)
    {
        if (format.Type == LocalizationFormatType.TextWithColor)
        {
            if (string.IsNullOrEmpty(format.Text))
            {
                return new ColorTextInfo(string.Empty, Color.Black);
            }

            if (localizationTextColorsService.TryGetColor(format.Text[0], out var colorInfo))
            {
                if (!_colorBrushes.TryGetValue(format.Text[0], out var brush))
                {
                    _colorBrushes.Add(format.Text[0], brush);
                }
                return new ColorTextInfo(format.Text[1..], brush);
            }
        }

        return new ColorTextInfo(format.Text, Color.Black);
    }

    private readonly Dictionary<char, Color> _colorBrushes = [];
}
