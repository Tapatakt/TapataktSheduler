using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;

namespace TapataktSheduler;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        AndroidEnvironment.UnhandledExceptionRaiser += (sender, e) =>
        {
            e.Handled = true;
            ShowFatalErrorDialog(e.Exception);
        };
    }

    protected override MauiApp CreateMauiApp()
    {
        try
        {
            return MauiProgram.CreateMauiApp();
        }
        catch (Exception ex)
        {
            ShowFatalErrorDialog(ex);
            throw;
        }
    }

    private static void ShowFatalErrorDialog(Exception exception)
    {
        try
        {
            string message = $"FATAL ERROR:\n{exception.GetType().Name}\n{exception.Message}\n\n{exception.StackTrace}";
            Android.Util.Log.Error("TapataktSheduler", message);

            Handler handler = new(Looper.MainLooper!);
            handler.Post(() =>
            {
                Android.App.Activity? activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                if (activity == null)
                    return;

                AlertDialog.Builder builder = new(activity);
                builder.SetTitle("FATAL ERROR");
                builder.SetMessage(message);
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", (s, e) =>
                {
                    Java.Lang.JavaSystem.Exit(1);
                });
                builder.Show();
            });
        }
        catch
        {
            // If even showing the dialog fails, at least log it.
            Android.Util.Log.Error("TapataktSheduler", $"Failed to show error dialog: {exception}");
        }
    }
}
