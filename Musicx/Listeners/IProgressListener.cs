namespace Musicx.Listeners;

public interface IProgressListener
{
    public void UpdateProgress(int progress, int total);
}