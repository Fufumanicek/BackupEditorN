using P3ABackupN.Services;
using BackupEditorN.Services;
using BackupEditorN.Presentation.Windows;
namespace BackupEditorN;

class Program
{
    static void Main(string[] args)
    {  
        var configService = new EditorConfigurationService();
        var app = new Application();
        var mainWindow = new MainWindow(configService, app);
        app.Run(mainWindow);    
    }
}