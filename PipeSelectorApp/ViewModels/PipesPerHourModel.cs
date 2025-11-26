using Microsoft.EntityFrameworkCore;
using PipeSelectorApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PipeSelectorApp.ViewModels;

public class PipesPerHourViewModel : INotifyPropertyChanged
{
    private readonly PipesDbContext _context;
    private PipeNomenclature _selectedNomenclature;
    private PipeStrengthClass _selectedStrengthClass;
    private PipeThread _selectedPipeThread;
    private Section _selectedSection;
    private string _pipesPerHourResult;
    private ObservableCollection<PipeStrengthClass> _allStrengthClasses;
    private ObservableCollection<PipeThread> _allPipeThreads;

    public ObservableCollection<PipeNomenclature> Nomenclatures { get; }
    public ObservableCollection<PipeStrengthClass> StrengthClasses { get; }
    public ObservableCollection<PipeThread> PipeThreads { get; }
    public ObservableCollection<Section> Sections { get; }

    public PipeNomenclature SelectedNomenclature
    {
        get => _selectedNomenclature;
        set
        {
            _selectedNomenclature = value;
            UpdateFilteredLists();
            OnPropertyChanged(nameof(SelectedNomenclature));
        }
    }

    public PipeStrengthClass SelectedStrengthClass
    {
        get => _selectedStrengthClass;
        set
        {
            _selectedStrengthClass = value;
            OnPropertyChanged(nameof(SelectedStrengthClass));
        }
    }

    public PipeThread SelectedPipeThread
    {
        get => _selectedPipeThread;
        set
        {
            _selectedPipeThread = value;
            OnPropertyChanged(nameof(SelectedPipeThread));
        }
    }

    public Section SelectedSection
    {
        get => _selectedSection;
        set
        {
            _selectedSection = value;
            OnPropertyChanged(nameof(SelectedSection));
        }
    }

    public string PipesPerHourResult
    {
        get => _pipesPerHourResult;
        set
        {
            _pipesPerHourResult = value;
            OnPropertyChanged(nameof(PipesPerHourResult));
        }
    }

    public ICommand GetPipesPerHourCommand { get; }

    public PipesPerHourViewModel()
    {
        Nomenclatures = new ObservableCollection<PipeNomenclature>();
        StrengthClasses = new ObservableCollection<PipeStrengthClass>();
        PipeThreads = new ObservableCollection<PipeThread>();
        Sections = new ObservableCollection<Section>();
        _allStrengthClasses = new ObservableCollection<PipeStrengthClass>();
        _allPipeThreads = new ObservableCollection<PipeThread>();
        GetPipesPerHourCommand = new RelayCommand(async () => await GetPipesPerHourAsync(), CanGetPipesPerHour);
    }

    public PipesPerHourViewModel(PipesDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        try
        {
            Nomenclatures = new ObservableCollection<PipeNomenclature>(_context.PipeNomenclature.ToList());
            _allStrengthClasses = new ObservableCollection<PipeStrengthClass>(_context.PipeStrengthClass.ToList());
            _allPipeThreads = new ObservableCollection<PipeThread>(_context.PipeThread.ToList());
            Sections = new ObservableCollection<Section>(_context.Sections.ToList());
            StrengthClasses = new ObservableCollection<PipeStrengthClass>();
            PipeThreads = new ObservableCollection<PipeThread>();
            PipesPerHourResult = $"Загружено: {Nomenclatures.Count} номенклатур, {_allStrengthClasses.Count} групп прочности, {_allPipeThreads.Count} типов резьбы, {Sections.Count} участков";
            Console.WriteLine(PipesPerHourResult);
        }
        catch (Exception ex)
        {
            PipesPerHourResult = $"Ошибка загрузки данных: {ex.Message}";
            Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
            MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        GetPipesPerHourCommand = new RelayCommand(async () => await GetPipesPerHourAsync(), CanGetPipesPerHour);
    }

    //public PipesPerHourViewModel(PipesDbContext context)
    //{
    //    _context = context ?? throw new ArgumentNullException(nameof(context));

    //    try
    //    {
    //        Nomenclatures = new ObservableCollection<PipeNomenclature>(_context.PipeNomenclature.ToList());
    //        _allStrengthClasses = new ObservableCollection<PipeStrengthClass>(_context.PipeStrengthClass.ToList());
    //        _allPipeThreads = new ObservableCollection<PipeThread>(_context.PipeThread.ToList());
    //        Sections = new ObservableCollection<Section>(_context.Sections.ToList());
    //        StrengthClasses = new ObservableCollection<PipeStrengthClass>();
    //        PipeThreads = new ObservableCollection<PipeThread>();
    //        PipesPerHourResult = $"Загружено: {Nomenclatures.Count} номенклатур, {_allStrengthClasses.Count} групп прочности, {_allPipeThreads.Count} типов резьбы, {Sections.Count} участков";
    //        Console.WriteLine(PipesPerHourResult);
    //    }
    //    catch (Exception ex)
    //    {
    //        PipesPerHourResult = $"Ошибка загрузки данных: {ex.Message}";
    //        Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
    //        MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    //    }
    //    GetPipesPerHourCommand = new RelayCommand(async () => await GetPipesPerHourAsync(), CanGetPipesPerHour);
    //}

    public async Task RefreshDataAsync()
    {
        try
        {
            Nomenclatures.Clear();
            foreach (var nom in await _context.PipeNomenclature.ToListAsync())
            {
                Nomenclatures.Add(nom);
            }

            _allStrengthClasses.Clear();
            foreach (var sc in await _context.PipeStrengthClass.ToListAsync())
            {
                _allStrengthClasses.Add(sc);
            }

            _allPipeThreads.Clear();
            foreach (var pt in await _context.PipeThread.ToListAsync())
            {
                _allPipeThreads.Add(pt);
            }

            Sections.Clear();
            foreach (var section in await _context.Sections.ToListAsync())
            {
                Sections.Add(section);
            }

            UpdateFilteredLists();
            PipesPerHourResult = $"Обновлено: {Nomenclatures.Count} номенклатур, {_allStrengthClasses.Count} групп прочности, {_allPipeThreads.Count} типов резьбы, {Sections.Count} участков";
            Console.WriteLine(PipesPerHourResult);
        }
        catch (Exception ex)
        {
            PipesPerHourResult = $"Ошибка обновления данных: {ex.Message}";
            Console.WriteLine($"Ошибка обновления данных: {ex.Message}");
            MessageBox.Show($"Ошибка обновления данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateFilteredLists()
    {
        StrengthClasses.Clear();
        PipeThreads.Clear();
        SelectedStrengthClass = null;
        SelectedPipeThread = null;

        if (SelectedNomenclature == null)
        {
            return;
        }

        try
        {
            // Фильтрация групп прочности на основе NomenclatureByStrengthClass
            var strengthClassIds = _context.NomenclatureByStrengthClass
                .Where(ns => ns.PipeNom_id == SelectedNomenclature.Id)
                .Select(ns => ns.StrengthClass_id)
                .ToList();
            foreach (var strengthClass in _allStrengthClasses
                .Where(sc => strengthClassIds.Contains(sc.id)))
            {
                StrengthClasses.Add(strengthClass);
            }

            // Фильтрация типов резьбы на основе Product_Nomenclature и Product
            var threadIds = _context.Product_Nomenclature
                .Where(pn => pn.PipeNom_id == SelectedNomenclature.Id)
                .Join(_context.Product,
                    pn => pn.id,
                    p => p.ProductNomencl_id,
                    (pn, p) => p.Thread_id)
                .Distinct()
                .ToList();
            foreach (var pipeThread in _allPipeThreads
                .Where(pt => threadIds.Contains(pt.id)))
            {
                PipeThreads.Add(pipeThread);
            }

            Console.WriteLine($"Фильтрация для номенклатуры {SelectedNomenclature?.DisplayName}: " +
                $"{StrengthClasses.Count} групп прочности, {PipeThreads.Count} типов резьбы");
        }
        catch (Exception ex)
        {
            PipesPerHourResult = $"Ошибка фильтрации данных: {ex.Message}";
            Console.WriteLine($"Ошибка фильтрации данных: {ex.Message}");
            MessageBox.Show($"Ошибка фильтрации данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool CanGetPipesPerHour()
    {
        return _context != null &&
               SelectedNomenclature != null &&
               SelectedStrengthClass != null &&
               SelectedPipeThread != null &&
               SelectedSection != null;
    }

    private async Task GetPipesPerHourAsync()
    {
        try
        {
            if (!CanGetPipesPerHour())
            {
                PipesPerHourResult = "Выберите все параметры или проверьте подключение к базе данных.";
                return;
            }

            // Получение значения PipesPerHour и Productivity_id
            var queryResult = await (from m in _context.MainInfo
                                     join ns in _context.NomenclatureByStrengthClass
                                     on m.NomByStrClass_id equals ns.id
                                     join prod in _context.Productivity
                                     on m.Prod_id equals prod.Id
                                     join p in _context.Product
                                     on prod.Product_id equals p.id
                                     where m.PipeNom_id == SelectedNomenclature.Id
                                     && ns.PipeNom_id == SelectedNomenclature.Id
                                     && ns.StrengthClass_id == SelectedStrengthClass.id
                                     && p.Thread_id == SelectedPipeThread.id
                                     && prod.Section_id == SelectedSection.id
                                     select new { prod.PipesPerHour, prod.Id })
                                    .FirstOrDefaultAsync();

            if (queryResult == null)
            {
                PipesPerHourResult = "Значение PipesPerHour не задано для данной комбинации.";
                Console.WriteLine(PipesPerHourResult);
                return;
            }

            PipesPerHourResult = $"PipesPerHour: {queryResult.PipesPerHour} труб/час";
            Console.WriteLine($"Выбрано: {SelectedNomenclature?.DisplayName}, {SelectedStrengthClass?.StrengthClass}, {SelectedPipeThread?.pipe_thread}, {SelectedSection?.section}");

            // Добавление или обновление записи в ShiftTask с учётом участка
            await UpdateOrAddShiftTaskAsync(queryResult.Id, SelectedSection.id);
        }
        catch (Exception ex)
        {
            PipesPerHourResult = $"Ошибка: {ex.Message}";
            Console.WriteLine($"Ошибка в GetPipesPerHourAsync: {ex.Message}");
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task UpdateOrAddShiftTaskAsync(int productivityId, int sectionId)
    {
        try
        {
            // Проверка, существует ли запись в ShiftTask для данного участка
            var existingShiftTask = await _context.ShiftTasks
                .FirstOrDefaultAsync(st => st.Section_id == sectionId);

            if (existingShiftTask != null)
            {
                // Обновление существующей записи для этого участка
                existingShiftTask.productivity_id = productivityId;

                _context.ShiftTasks.Update(existingShiftTask);
                Console.WriteLine($"Обновлена запись в ShiftTask с Id={existingShiftTask.id} для Section_id={sectionId}, productivity_id={productivityId}");
            }
            else
            {
                // Создание новой записи для нового участка
                var newShiftTask = new ShiftTask
                {
                    productivity_id = productivityId,
                    Section_id = sectionId,
                };
                await _context.ShiftTasks.AddAsync(newShiftTask);
                Console.WriteLine($"Добавлена новая запись в ShiftTask для Section_id={sectionId}, productivity_id={productivityId}");
            }

            await _context.SaveChangesAsync();
            PipesPerHourResult += $"\nСменное задание {(existingShiftTask != null ? "обновлено" : "добавлено")} для Section_id={sectionId}, productivity_id={productivityId}";
        }
        catch (DbUpdateException dbEx)
        {
            var innerEx = dbEx.InnerException?.ToString() ?? dbEx.ToString();
            PipesPerHourResult = $"Ошибка при обновлении/добавлении сменного задания: {innerEx}";
            Console.WriteLine($"Ошибка в UpdateOrAddShiftTaskAsync: {innerEx}");
            MessageBox.Show($"Ошибка: {innerEx}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            PipesPerHourResult = $"Ошибка при обновлении/добавлении сменного задания: {ex.Message}";
            Console.WriteLine($"Ошибка в UpdateOrAddShiftTaskAsync: {ex.Message}");
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<Task> _executeAsync;
    private readonly Func<bool> _canExecute;

    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public RelayCommand(Func<Task> executeAsync, Func<bool> canExecute = null)
    {
        _executeAsync = executeAsync;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

    public async void Execute(object parameter)
    {
        if (_executeAsync != null)
        {
            await _executeAsync();
        }
        else if (_execute != null)
        {
            _execute();
        }
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public void NotifyCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}