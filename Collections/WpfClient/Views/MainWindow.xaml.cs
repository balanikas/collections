using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Collections.Messages;
using Collections.Runtime;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Native;
using WpfClient.Tutorial;

namespace WpfClient
{
    public partial class MainWindow : MetroWindow
    {
        private static MainWindow _self;

        public MainWindow()
        {
            InitializeComponent();
            
            DataContext = ViewModelLocator.MainWindow;
            _self = this;

            ContentRendered += (sender, args) =>
            {
                if (!TutorialStatus.HasRun)
                {
                    TutorialStatus.HasRun = true;
                    ShowTutorial();
                    
                }
                
            };

        }

        public static Task<ProgressDialogController> ShowProgress(string title, string message)
        {
            return _self.ShowProgressAsync(title, message);
            
        }

        public static void ShowTutorial()
        {
            
            var tutorial = new Tutorial.Tutorial(_self);
            tutorial.StartText = "This is just a quick guide to this app";
            tutorial.EndText = "Thank you! You can restart this tutorial any time from the settings view.";
            tutorial.CloseButtonText = "Ok, got it";

            var tc = UIHelper.FindChild<TabControl>(_self, "TabControl");
            
           
            tutorial.AddStep<TextBox>("TxtFilePath", 0, new StepOptions("Here you can enter a path to a source file, a path to a folder containing source files or a path to an assembly file", "", false),
                () =>
                {
                    tc.SelectedIndex = 0;
                });
            tutorial.AddStep<Button>("BtnLoad", 1, new StepOptions("Click here to compile and/or load the code", "", true));
            tutorial.AddStep<Button>("LstTypes", 2, new StepOptions("Here are all the loaded types you can play with", "", false));
            tutorial.AddStep<Button>("LstMethods", 3, new StepOptions("Selecting a type will list all its methods here. Inherited methods are usually no listed.", "", false));
            tutorial.AddStep<Button>("Canvas", 4, new StepOptions("This is where the visualization happens. Lets click somewhere in the canvas", "Do it!", false));
            tutorial.AddStep<Button>("Canvas", 5, new StepOptions("We just executed a method!", "", false), () =>
            {
                var canvas = UIHelper.FindChild<Canvas>(_self, "Canvas");
                Point relativePoint = canvas.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
                relativePoint.Offset(-200, 200);
                var shape = UIHelper.CreateDrawingShape(canvas, relativePoint);
                ViewModelLocator.ExploreMode.CreateRunner(canvas, shape);

            });
            tutorial.AddStep<Expander>("ExpInfoView", 6,
                new StepOptions("Here you can view how the selected method is doing", "", false), () =>
                {
                    ViewModelLocator.ExploreMode.InfoView.IsExpanded = true;
                });

            tutorial.AddStep<Control>("AvalonTextEditor", 7, new StepOptions("This is the code editor. You can view and edit code, and then re-compile it", "Re-compile", false), null,
                () =>
                {
                    ViewModelLocator.ExploreMode.CmdClearCanvas.Execute(null);
                    ViewModelLocator.ExploreMode.InfoView.IsExpanded = false;
                });
            tutorial.AddStep<Button>("BtnCompile", 8, new StepOptions("We just re-compiled some code! You could try and execute the same code again and see a difference", "Continue", true));

            tutorial.AddStep<Button>("TabControl", 9, new StepOptions("This is the Play mode. It is about visualizing the code you write in realtime", "", false),()=>
            {
                tc.SelectedIndex = 1;
            });
            tutorial.AddStep<Button>("BtnActivate", 10, new StepOptions("Click here to activate play mode", "", false),() =>
            {
                ViewModelLocator.PlayModeMode.IsActivated = true;
            });
            tutorial.AddStep<Button>("CmbCompiledMethods", 11, new StepOptions("Select a method you want to keep executing", "", false), () =>
            {
                var method = ViewModelLocator.PlayModeMode.CompiledMethods[0];
                ViewModelLocator.PlayModeMode.SelectedCompiledMethod = method;
            });
            tutorial.AddStep<Control>("AvalonEditPlayMode",12,new StepOptions("Now you can edit code and see in realtime on the right side. Happy coding!","Next",false));
            tutorial.AddStep<Control>("SettingsFlyout", 13, new StepOptions("Finally, here are some things you can adjust", "Ok", false),()=> { MainWindow.ToggleFlyout(0);});
            tutorial.AddStep<Control>("AboutFlyout", 14, new StepOptions("If you have comments or questions, please contact me here", "Done", false), () => { MainWindow.ToggleFlyout(1); });

            var tutorialWindow = new TutorialWindow(tutorial);
            tutorialWindow.ShowDialog();
        }

        public static void ToggleFlyout(int index,bool keepOpenIfOpened = false)
        {
            var flyout = _self.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }
            if (keepOpenIfOpened)
            {
                flyout.IsOpen = true;
            }
            else
            {
                flyout.IsOpen = !flyout.IsOpen;
            }
        }

    }
}