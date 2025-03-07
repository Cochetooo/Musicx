using log4net;
using Musicx.Core.Models;

namespace Musicx.Infrastructure.Services.Audio;

public class WriteAudioFile
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(WriteAudioFile));

    public static void Execute(in Song song)
    {
        
    }
}