using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using Collections;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using MahApps.Metro.Controls.Dialogs;

namespace WpfClient.ViewModels
{
    public class TypesViewModel : ViewModelBase
    {
        private readonly TypesProvider _typesProvider;
        private TextDocument _codeDocument;
        private string _filesPath;
        private MethodInfo _selectedMethod;
        private LoadedType _selectedType;
        private ObservableCollection<LoadedType> _types;


        public TypesViewModel(TypesProvider typesProvider)
        {
            _typesProvider = typesProvider;
            _types = new ObservableCollection<LoadedType>();
            _codeDocument = new TextDocument();


            CmdLoadTypes = new RelayCommand(LoadTypes);

            CmdTypesSelectionChanged = new RelayCommand<SelectionChangedEventArgs>(e =>
            {
                if (e.AddedItems.Count == 0)
                {
                    CodeDocument.Text = String.Empty;
                    return;
                }

                CodeDocument.Text = ((LoadedType) e.AddedItems[0]).Source;
            });

            CmdMethodsSelectionChanged = new RelayCommand<SelectionChangedEventArgs>(e =>
            {
            });

            CmdCompile = new RelayCommand(() =>
            {
               
                List<string> errors;
                if (_typesProvider.TryCompileFromText(CodeDocument.Text, out errors) != null && !errors.Any())
                {

                    _typesProvider.SaveType(new LoadedType
                    {
                        FilePath = SelectedType.FilePath,
                        Source = CodeDocument.Text,
                        TypeInfo = SelectedType.TypeInfo
                    });

                    ViewModelLocator.Logger.InfoNow("Compilation succeeded");

                    LoadTypes();
                }
                else
                {
                    foreach (string error in errors)
                    {
                        ViewModelLocator.Logger.ErrorNow(error);
                    }
                }
            });
        }


        public RelayCommand CmdLoadTypes { get; private set; }

        public RelayCommand<SelectionChangedEventArgs> CmdTypesSelectionChanged { get; private set; }
        public RelayCommand<SelectionChangedEventArgs> CmdMethodsSelectionChanged { get; private set; }

        public RelayCommand CmdCompile { get; private set; }

        public ObservableCollection<LoadedType> Types
        {
            get
            {
                return _types;
            }
            set
            {
                _types = value;
                RaisePropertyChanged("Types");
            }
        }

        public TextDocument CodeDocument
        {
            get { return _codeDocument; }
            set
            {
                _codeDocument = value;
                RaisePropertyChanged("CodeDocument");
            }
        }

        public LoadedType SelectedType
        {
            get
            {
                return _selectedType;
            }
            set
            {
                _selectedType = value;
                RaisePropertyChanged("SelectedType");
            }
        }

        public MethodInfo SelectedMethod
        {
            get
            {
                return _selectedMethod;
            }
            set
            {
                _selectedMethod = value;
                RaisePropertyChanged("SelectedMethod");
            }
        }

        public string FilesPath
        {
            get { return _filesPath; }
            set
            {
                _filesPath = value;

                RaisePropertyChanged("FilesPath");
            }
        }


        private async void LoadTypes()
        {
            ProgressDialogController controller = await MainWindow.ShowProgress(
                "Please wait...",
                "loading types from " + FilesPath);

            var previousSelectedTypeName = SelectedType != null? SelectedType.TypeInfo.FullName:null;
            var previousSelectedMethodName = SelectedMethod != null? SelectedMethod.Name:null;

            _typesProvider.SetActiveCompilerService(Settings.Instance.Get(Settings.Keys.CompilerServiceType));
            var allTypes = new List<LoadedType>();


            if (Utils.IsValidAssembly(FilesPath))
            {
                List<LoadedType> types = await _typesProvider.FromAssemblyAsync(FilesPath);
                allTypes.AddRange(types);
            }
            else if (File.Exists(FilesPath) || Directory.Exists(FilesPath))
            {
                List<LoadedType> types = await _typesProvider.FromDiscAsync(FilesPath);
                allTypes.AddRange(types);
            }

            Types = new ObservableCollection<LoadedType>(allTypes);

            if (previousSelectedTypeName != null)
            {
                var foundType = Types.FirstOrDefault(t => t.TypeInfo.FullName == previousSelectedTypeName);
                SelectedType = foundType;
                if (foundType != null)
                {
                    SelectedMethod = foundType.MethodsInfos.FirstOrDefault(m => m.Name == previousSelectedMethodName);
                }
            }
            
            await controller.CloseAsync();
        }
    }
}