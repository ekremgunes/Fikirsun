
namespace Fikirsun.Tools
{
    public static class NotifyCSS
    {
        static string successNotify = "border: 3px solid #14c839eb;background-color: #0adb5c24 !important;";
        static string infoNotify = "border: 3px solid #1463c8eb;background-color: #0a8cdb24 !important;";
        static string errorNotify = "border: 3px solid #f91818eb;background-color: #f9060678 !important;";
        static string likeNotify = "border: 3px solid #d23030eb;background-color: #e3343438 !important;";
        static string warningNotify = "border: 3px solid #c88a14eb; background-color: #dbb10a38 !important;";


        public static string Invoke(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Info:
                    return infoNotify;
                case NotificationType.Warning:
                    return warningNotify;
                case NotificationType.Error:
                    return errorNotify;
                case NotificationType.Like:
                    return likeNotify;
                case NotificationType.Success:
                    return successNotify;
                default:
                    return "";

            }
        }
    }
}
