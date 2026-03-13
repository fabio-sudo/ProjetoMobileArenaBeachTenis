namespace ArenaPOS.Helpers;

public static class ApiConfig
{
#if ANDROID
    public static string BaseUrl = "http://10.0.2.2:5151";
#else
    public static string BaseUrl = "http://localhost:5151";
#endif
}