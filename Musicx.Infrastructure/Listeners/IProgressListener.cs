namespace Musicx.Infrastructure.Listeners;

public interface IProgressListener
{
    public void UpdateProgress(int progress, int total);
}