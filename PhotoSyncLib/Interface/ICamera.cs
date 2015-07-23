namespace PhotoSyncLib.Interface
{
    public interface ICamera
    {
        string Manufacturer { get; }
        string Model { get; }
        string SerialNumber { get; }
    }
}