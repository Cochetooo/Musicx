using log4net;
using Musicx.Core.Models;

namespace Musicx.Infrastructure.Services.Audio;

public class WriteAudioFileService : IService
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(WriteAudioFileService));

    public static void Execute(in Song song)
    {
        
    }
}