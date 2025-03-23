using log4net;
using Musicx.Core.Logging;
using NAudio.CoreAudioApi;
using TagLib.Id3v2;

namespace Musicx.Infrastructure.Helpers;

public static class AudioTagHelper
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(AudioTagHelper));

    public static void WriteCustomTag(string filePath, string tagKey, string tagValue)
    {
        using var file = TagLib.File.Create(filePath);

        if (file.Tag is TagLib.Ogg.XiphComment oggTag)
        {
            oggTag.SetField(tagKey, [tagValue]);
        }
        else if (file.Tag is TagLib.Id3v2.Tag id3v2Tag)
        {
            var frame = TagLib.Id3v2.TextInformationFrame.Get(id3v2Tag, tagKey, true);
            frame.Text = [tagValue];
        }
        else
        {
            Logger.Warn($"⚠️ Format from file {filePath} does not support custom tags.");
            return;
        }

        file.Save();
        Logger.Debug($"✅ Custom Tag {tagKey} added successfully to {filePath}");
    }

    public static string? ReadCustomTag(string filePath, string key)
    {
        using var file = TagLib.File.Create(filePath);
        
        TagLib.Tag? tag = null;

// Essayer d'abord ID3v2, puis Ogg XiphComment
        foreach (var tagType in new[] { TagLib.TagTypes.Id3v2, TagLib.TagTypes.Xiph })
        {
            tag = file.GetTag(tagType);
            if (tag != null) break; // Sortir dès qu'on trouve un tag valide
        }

        if (tag is TagLib.Ogg.XiphComment oggTag)
        {
            return oggTag.GetField(key)?.FirstOrDefault();
        }
        if (tag is TagLib.Id3v2.Tag id3v2Tag)
        {
            var frame = id3v2Tag
                .GetFrames<TextInformationFrame>()
                .FirstOrDefault(f => f.Text.Length > 1 && f.Text[0] == key);
            return frame?.Text?.FirstOrDefault();
        }
        
        Logger.Warn($"⚠️ No tag {key} found in file {filePath}, available types: {file.TagTypes}");
        return null;
    }
}