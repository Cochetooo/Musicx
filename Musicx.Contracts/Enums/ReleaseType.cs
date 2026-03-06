using System.ComponentModel;

namespace Musicx.Contracts.Enums;

/// <summary>
/// Enumerate all release types.
/// </summary>
/// <since>0.3.0</since>
public enum ReleaseType
{
    [Description("➕ Additional Release")]
    AdditionalRelease = 0,
    [Description("🗃️ Archival")]
    Archival = 15,
    [Description("🕵️ Bootleg")]
    Bootleg = 1,
    [Description("🧩 Compilation")]
    Compilation = 2,
    [Description("🎸 Covers")]
    Covers = 14,
    [Description("🧪 Demo")]
    Demo = 12,
    [Description("🎚️ DJ Mix")]
    DjMix = 3,
    [Description("🎵 EP")]
    Ep = 4,
    [Description("🎤 Live")]
    Live = 5,
    [Description("💿 LP")]
    Lp = 6,
    [Description("📼 Mixtape")]
    MixTape = 7,
    [Description("🔁 Remixes")]
    Remix = 13,
    [Description("🎧 Single")]
    Single = 8,
    [Description("🎬 Soundtrack")]
    Soundtrack = 9,
    [Description("❓ Unknown")]
    Unknown = 10,
    [Description("📺 Video Clip")]
    VideoClip = 11
}