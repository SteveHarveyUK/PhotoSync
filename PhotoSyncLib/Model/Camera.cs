using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using PhotoSyncLib.Interface;

namespace PhotoSyncLib.Model
{
    internal class Camera : ICamera
    {
        public Camera(ShellObject shellObject)
        {
            //var sp = shellObject.Properties.GetProperty(SystemProperties.System.Photo.CameraManufacturer);

            //var manu = sp.GetValue<string>();

            var shellProperty = shellObject.Properties.GetProperty<string>(SystemProperties.System.Photo.CameraManufacturer);
            Manufacturer = shellProperty.Value;
            Model = shellObject.Properties.GetProperty<string>(SystemProperties.System.Photo.CameraModel).Value;
            SerialNumber = shellObject.Properties.GetProperty<string>(SystemProperties.System.Photo.CameraSerialNumber).Value;
        }

        public string Manufacturer { get; private set; }
        public string Model { get; private set; }
        public string SerialNumber { get; private set; }
    }

    internal static class ShellObjectExtentions
    {
        internal static TType GetValue<TType>(this IShellProperty shellProperty)
        {
            if (shellProperty == null || shellProperty.ValueAsObject == null)
            {
                return default(TType);
            }
            return (TType)shellProperty.ValueAsObject;
            
        }
    }
}