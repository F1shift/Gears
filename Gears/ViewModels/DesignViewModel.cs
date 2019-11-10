using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
//using Xamarin.Forms;
using Gears.ViewModels;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Gears.ViewModels
{
    class DesignViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public GearParameterViewModel GearParameterViewModel { get; set; }

        public RackParameterViewModel RackParameterViewModel { get; set; }

        public GearDetailViewModel GearDetailViewModel { get; set; }

        public ThreeDModelingViewModel ThreeDModelingViewModel { get; set; }

        public bool IsDetailViewPined { get; set; }
        public SimpleCommand UpdateCommand { get; set; }
        public SimpleCommand SaveProjectCommand { get; set; }
        public SimpleCommand ExportExcelCommand { get; set; }
        public SimpleCommand ExportglTFCommand { get; set; }

        public DesignViewModel()
        {


        }

        public async Task<bool> Initialize() {
            RackParameterViewModel = new RackParameterViewModel();
            await RackParameterViewModel.Initialize();
            GearParameterViewModel = new GearParameterViewModel();
            GearDetailViewModel = new GearDetailViewModel();
            ThreeDModelingViewModel = new ThreeDModelingViewModel(GearDetailViewModel);
            GearDetailViewModel.GearParameterViewModel = this.GearParameterViewModel;
            GearDetailViewModel.RackParameterViewModel = this.RackParameterViewModel;

            UpdateCommand = new SimpleCommand(async (para) => { await Update(); return true; });
            SaveProjectCommand = new SimpleCommand(async (para) => { SaveProject(); return true; });
            ExportExcelCommand = new SimpleCommand(async (para) => { ExportExcel(); return true; });
            ExportglTFCommand = new SimpleCommand(async (para) => {  return true; });
            return true;
        }

        public async Task<object> Update() {
            var updated = await Xamarin.Forms.Device.InvokeOnMainThreadAsync<bool>( ()=> GearDetailViewModel.CheckUpdate());
            if (updated)
                await ThreeDModelingViewModel.UpdateOrAddGear();
            return null;
        }

        System.Timers.Timer autoUpdateTimer = new System.Timers.Timer();
        public void StartAutoUpdate(int interval = 100) {
            autoUpdateTimer.Stop();
            autoUpdateTimer = new System.Timers.Timer();
            autoUpdateTimer.Interval = interval;
            autoUpdateTimer.Elapsed += (s, e) => UpdateCommand.Execute(null);
            autoUpdateTimer.Start();
        }

        public void StopAutoUpdate() {
            autoUpdateTimer.Stop();
        }

        public void InvokeCSHandler(string data) {
            switch (data)
            {
                case "Update":
                    UpdateCommand.Execute(null);
                    break;
                case "UpdateWebSide":
                    GearDetailViewModel.CheckUpdate();
                    ThreeDModelingViewModel.UpdateCommand.Execute(null);
                    break;
                case "StartAutoUpdate":
                    StartAutoUpdate();
                    break;
                default:
                    break;
            }
        }

        public void SaveProject() {
            App.AppViewModel.BrowseViewModel.SaveProject(GearDetailViewModel.Model);
        }

        public void ExportExcel() {
            var filename = "Gear_Parameters.xlsx";
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var filePath = Path.Combine(folderPath, filename);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            var targetStream = File.Create(filePath);
            var sourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Gears.Resources.{filename}");
            sourceStream.CopyTo(targetStream);
            targetStream.Close();
            sourceStream.Close();
            using (var document = SpreadsheetDocument.Open(filePath, true))
            {
                var modelType = this.GearDetailViewModel.Model.GetType();
                var wbPart = document.WorkbookPart;
                var wb = wbPart.Workbook;
                var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                foreach (var sheet in wb.Descendants<Sheet>())
                {
                    if (sheet.Name == "Sheet1")
                    {
                        var wsheetPart = wbPart.GetPartById(sheet.Id) as WorksheetPart;
                        if (wsheetPart == null)
                        {
                            throw new NullReferenceException("WorksheetPart Not Found !!");
                        }

                        var ws = wsheetPart.Worksheet;
                        foreach (var row in ws.Descendants<Row>())
                        {
                            var list = new List<string>();
                            foreach (Cell cell in row)
                            {
                                var name = (from item in wb.DefinedNames.ChildElements
                                           where item.InnerText.Replace(sheet.Name, "").Replace("$", "").Replace("!", "") == cell.CellReference
                                           select (item as DefinedName).Name.Value).FirstOrDefault();
                                Console.WriteLine($"definedName:{name}, cell reference:{cell.CellReference} ");
                                if (!String.IsNullOrEmpty(name))
                                {
                                    if (name.Contains("_"))
                                    {
                                        var strs = name.Split(',');
                                        name = strs[0];
                                        var index = Convert.ToInt32(strs[1]) - 1;
                                        var array = modelType.GetProperty(name).GetValue(this.GearDetailViewModel.Model) as Array;
                                        cell.CellValue = new CellValue(array.GetValue(index).ToString());
                                    }
                                    else
                                        cell.CellValue = new CellValue(modelType.GetProperty(name).GetValue(this.GearDetailViewModel.Model).ToString());
                                }
                            }
                        }
                    }
                }
                document.Save();
            }
        }
    }
}
