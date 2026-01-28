 Сменное задание (PipeSelectorApp)
Что это?
Локальное WPF-приложение для выбора параметров трубы и получения нормативной производительности (штук в час). Служит для быстрого определения плановой выработки на участке НОТ при заданной номенклатуре, классе прочности и типе резьбы. Использует базу данных PipeNomenclature для хранения нормативов и автоматического создания/обновления записей о производительности и сменных заданиях (ShiftTask).
Где находится?
На компе у Баймурадова, старшего мастера и мастера.
Подключается к центральной базе данных SQL Server:
сервер: 192.168.11.222, порт 1433
база: PipeNomenclature
пользователь: PipeDataUser
пароль: itpz2025zhargal
Сборка проекта PipeSelectorApp (net9.0-windows). Для распространения достаточно установить приложение из сетевой папки по пути \\192.168.50.20\public\Программы АСУ ТП\Сменное задание, запустив ярлык «setup.exe». Публикация идет через ClickOnce.

Кому надо?
Операторам станков, мастерам участков, технологам и инженерам, которые занимаются планированием и нормированием производства труб.
Основные потребители на текущий момент — участок НОТ, где требуется быстро определить норматив штук в час по выбранной номенклатуре и участку.
Как работает?
a. Приложение имеет один основной интерфейс — окно «Выбор параметров трубы».
После запуска сразу загружаются все доступные значения из базы:
номенклатура труб (диаметр × толщина стенки),
классы прочности,
типы резьбы,
участки производства.
b. Логика работы окна:
Пользователь последовательно выбирает в выпадающих списках:
Номенклатуру трубы
Класс прочности (список автоматически фильтруется — показываются только те классы, которые разрешены для выбранной номенклатуры согласно таблице PipesPerHourMap)
Тип резьбы (аналогично — фильтруется по карте)
Участок

После выбора всех четырёх параметров становится активной кнопка «Получить план в час».
При нажатии кнопки приложение:
Находит соответствующую запись в таблице PipesPerHourMap по номенклатуре и участку.
Если запись найдена — берёт значение PipesPerHour.
Автоматически создаёт (или находит существующие) записи в производных таблицах:
NomenclatureByStrengthClass
Product_Nomenclature
Product
Productivity (с нужным значением штук в час)

Обновляет или создаёт запись в ShiftTask для выбранного участка (привязывает к нему последнюю Productivity).
Выводит результат в текстовом поле в формате «Pipes per hour: 85» (или сообщение об отсутствии норматива).

c. Особенности:
Фильтрация списков классов прочности и резьбы происходит динамически — оператор видит только те варианты, для которых в системе заведён норматив.
Все операции с базой асинхронные — интерфейс не «зависает» при обращении к серверу.
При ошибках (нет соединения с БД, нет подходящей карты и т.п.) показывается MessageBox с описанием проблемы.
Приложение не предназначено для массового редактирования справочников — это инструмент именно для выбора и получения норматива штук в час.
 

 1. Введение

PipeSelectorApp — это WPF-приложение на базе .NET 9.0, предназначенное для выбора параметров труб (pipes nomenclature) и расчёта производительности (pipes per hour). Оно позволяет пользователям выбирать номенклатуру труб, классы прочности, типы резьбы и участки производства через интуитивный интерфейс, а затем генерировать или обновлять записи о производительности и сменных заданиях (shift tasks) на основе предопределённых карт (PipesPerHourMap). 

Приложение использует:
- Entity Framework Core для взаимодействия с базой данных SQL Server.
- MVVM-паттерн для управления данными и UI (ViewModel реализует INotifyPropertyChanged).
- Прямой доступ к DbContext без дополнительных репозиториев для простоты.
- Асинхронные операции для загрузки и сохранения данных, чтобы избежать блокировки UI.

 Основные функции:
- Загрузка и отображение базовых данных (номенклатура, классы прочности, типы резьбы, участки) из базы данных.
- Динамическая фильтрация списков классов прочности и типов резьбы на основе выбранной номенклатуры (используя данные из PipesPerHourMap).
- Расчёт и отображение производительности (pipes per hour) для выбранной комбинации параметров.
- Автоматическое создание или обновление производных записей (NomenclatureByStrengthClass, Product_Nomenclature, Product, Productivity) при запросе производительности.
- Генерация или обновление сменных заданий (ShiftTask) для выбранного участка.
- UI в виде единственного окна с ComboBox для выбора параметров и кнопкой для расчёта.

 Требования:
- .NET 9.0.
- SQL Server (конфигурация в App.xaml.cs: сервер 192.168.11.222, база PipeNomenclature, пользователь PipeDataUser).
- Библиотеки: Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.SqlServer, Microsoft.Extensions.DependencyInjection.

 Общая логика:
1. Базовые таблицы: хранят атомарные данные (например, PipeNomenclature — диаметр и толщина труб).
2. Производные таблицы: генерируются динамически при расчёте (например, комбинации номенклатуры с классами прочности).
3. Расчёт производительности: на основе выбранных параметров приложение ищет соответствующую карту в PipesPerHourMap, создаёт необходимые производные сущности и обновляет ShiftTask.
4. UI-логика: ViewModel управляет коллекциями данных, командами (расчёт) и обновлением UI через INotifyPropertyChanged. Нет полного CRUD для базовых данных — фокус на выборе и выводе результата.

Приложение запускается из MainWindow, где пользователь выбирает параметры и получает результат.

 2. Архитектура

 2.1. Структура проекта
- Models: Модели данных (например, PipeNomenclature.cs, PipesPerHourMap.cs).
- ViewModels: ViewModels (PipesPerHourViewModel.cs).
- UI: WPF-окна (MainWindow.xaml.cs, App.xaml.cs).
- Контекст БД: PipesDbContext.cs.
- Сборка: AssemblyInfo.cs для тем WPF.
- Проект: PipeSelectorApp.csproj.

 2.2. Паттерны
- MVVM: Модели — данные; ViewModel — логика (команды, свойства); View — XAML-UI.
- Dependency Injection: В App.xaml.cs через ServiceCollection для регистрации DbContext.
- Асинхронность: Все операции с БД асинхронные (Task-based).
- Нет репозиториев или сервисов автозаполнения — прямой доступ к DbContext для упрощённой логики.

 2.3. База данных
- Контекст: PipesDbContext наследует от DbContext.
- Таблицы (явно сопоставлены в OnModelCreating):
  - Базовые:
    - PipeNomenclature: id, pipe_diameter, pipe_wall.
    - PipeStrengthClass: id, StrengthClass.
    - SteelMark: id, steelMark.
    - Product_type: id, product_type.
    - PipeThread: id, pipe_thread.
    - Sections: id, section.
    - PipesPerHourMap: Id, PipeNom_id, StrengthClasses (строка с классами, разделёнными запятыми), ThreadTypes (строка с типами резьбы), PipesPerHour, Section_id.
  - Производные:
    - NomenclatureByStrengthClass: id, PipeNom_id, StrengthClass_id.
    - Product_Nomenclature: id, PipeNom_id, StrClass_id.
    - Product: id, ProductNomencl_id, Thread_id.
    - Productivity: Id, Product_id, Section_id, PipesPerHour (nullable).
    - MainInfo: id, PipeNom_id, NomByStrClass_id, Prod_id, Type_id, Steel_id.
    - ShiftTask: id, productivity_id (nullable), Section_id.
- Отношения (настроены в OnModelCreating):
  - Многие-ко-многим через промежуточные таблицы (например, NomenclatureByStrengthClass связывает PipeNomenclature и PipeStrengthClass).
  - Каскадное удаление в некоторых случаях (OnDelete(DeleteBehavior.Cascade)).
  - Уникальный индекс на ShiftTask.Section_id.
- Миграции: Не описаны в коде; предполагается ручное управление схемой.

 3. Модели данных

 3.1. Базовые модели
- PipeNomenclature.cs: представляет номенклатуру труб.
  - Свойства: Id (int), pipe_diameter (double), pipe_wall (double).
  - DisplayName: Форматированная строка "{diameter:F2} x {wall:F2}".
  - Логика: хранит базовые размеры труб.

- PipeStrengthClass.cs: Классы прочности.
  - Свойства: id (int), StrengthClass (string).

- SteelMark.cs: Марки стали.
  - Свойства: id (int), steelMark (string).

- Product_type.cs: Типы продуктов.
  - Свойства: id (int), product_type (string).

- PipeThread.cs: Типы резьбы.
  - Свойства: id (int), pipe_thread (string).

- Section.cs: Участки производства.
  - Свойства: id (int), section (string).

- PipesPerHourMap.cs: Карты производительности.
  - Свойства: Id (int), PipeNom_id (int), StrengthClasses (string, разделённые запятыми), ThreadTypes (string, разделённые запятыми), PipesPerHour (int), Section_id (int).
  - Навигация: PipeNomenclature, Section.
  - Логика: хранит шаблоны производительности для комбинаций номенклатуры, классов, резьбы и участка.

 3.2. Производные модели
- NomenclatureByStrengthClass.cs: Комбинации номенклатуры и классов прочности.
  - Свойства: id (int), PipeNom_id (int), StrengthClass_id (int).
  - Навигация: PipeNomenclature, StrengthClass.

- Product_Nomenclature.cs: Продуктовая номенклатура (комбинация PipeNom и StrengthClass).
  - Свойства: id (int), PipeNom_id (int), StrClass_id (int).
  - Навигация: PipeNomenclature, StrengthClass.

- Product.cs: Продукты (комбинация Product_Nomenclature и PipeThread).
  - Свойства: id (int), ProductNomencl_id (int), Thread_id (int).
  - Навигация: Product_Nomenclature, PipeThread.

- Productivity.cs: Производительность.
  - Свойства: Id (int), Product_id (int), Section_id (int), PipesPerHour (int?, nullable для поддержки NULL).
  - Навигация: Product, Section.
  - Логика: Связывает продукт с участком и производительностью.

- MainInfo.cs: Основная информация (агрегат всех данных).
  - Свойства: id (int), PipeNom_id (int), NomByStrClass_id (int), Prod_id (int), Type_id (int), Steel_id (int).
  - Навигация: PipeNomenclature, NomenclatureByStrengthClass, Productivity, ProductType, SteelMark.
  - Логика: Полная комбинация для отчётов/запросов.

- ShiftTask.cs: Сменные задания.
  - Свойства: id (int), productivity_id (int?, nullable), Section_id (int).
  - Навигация: Productivity, Section.
  - Логика: назначает производительность участку; уникальный по Section_id.

 4. ViewModels

 4.1. PipesPerHourViewModel.cs
- Реализует INotifyPropertyChanged.
- Конструктор: Принимает PipesDbContext; загружает все базовые коллекции (Nomenclatures, StrengthClasses и т.д.) асинхронно через LoadDataAsync().
- Свойства:
  - ObservableCollection для Nomenclatures, StrengthClasses, PipeThreads, Sections.
  - Выбранные: SelectedNomenclature (обновляет фильтрованные списки StrengthClasses и PipeThreads), SelectedStrengthClass, SelectedPipeThread, SelectedSection.
  - PipesPerHourResult (string): Результат расчёта (например, "Pipes per hour: X").
- Команды:
  - GetPipesPerHourCommand (RelayCommand async): Вызывает GetPipesPerHourAsync().
- Методы:
  - LoadDataAsync(): Асинхронно загружает данные из DbSet в ObservableCollection; вызывает UpdateFilteredLists().
  - UpdateFilteredLists(): Фильтрует StrengthClasses и PipeThreads на основе SelectedNomenclature.
    - Логика: Находит карту по PipeNom_id в PipesPerHourMap; парсит StrengthClasses/ThreadTypes (разделённые запятыми); фильтрует все списки по совпадениям (LINQ Where с Contains).
    - Если нет карты, очищает списки.
  - GetPipesPerHourAsync(): 
    - Валидация: проверяет, выбраны ли все параметры (Nomenclature, StrengthClass, PipeThread, Section).
    - Находит карту по номенклатуре и участку (FirstOrDefaultAsync).
    - Если карта найдена:
      - Создаёт/находит NomenclatureByStrengthClass (по PipeNom_id и StrengthClass_id).
      - Создаёт/находит Product_Nomenclature (по PipeNom_id и StrClass_id).
      - Создаёт/находит Product (по ProductNomencl_id и Thread_id).
      - Создаёт/находит Productivity (по Product_id и Section_id, устанавливает PipesPerHour из карты).
      - Вызывает UpdateOrAddShiftTaskAsync для обновления ShiftTask.
      - Устанавливает PipesPerHourResult в "Pipes per hour: X".
    - Если не найдена: устанавливает результат "Pipes per hour: Not available".
    - SaveChangesAsync() после изменений.
  - UpdateOrAddShiftTaskAsync(int productivityId, int sectionId):
    - Находит существующий ShiftTask по Section_id (FirstOrDefaultAsync).
    - Если существует: Обновляет productivity_id.
    - Если нет: Создаёт новый ShiftTask.
    - SaveChangesAsync; обновляет PipesPerHourResult (например, "Shift task updated successfully").
- RelayCommand.cs: Вспомогательный класс для команд (поддержка sync/async, CanExecute). Использует CommandManager для RequerySuggested.
- Логика: Фокус на динамической фильтрации и расчёте; обработка ошибок (DbUpdateException, Exception) с MessageBox и консолью; асинхронность для UI-responsiveness.

 5. UI-Компоненты

 5.1. App.xaml.cs
- Наследует Application.
- OnStartup: Настраивает DI (ServiceCollection); регистрирует DbContext.
- Логика: Создаёт MainWindow, инициализирует DataContext через InitializeDataContext с DbContext, показывает окно.

 5.2. MainWindow.xaml.cs
- Конструктор: InitializeComponent().
- Методы:
  - InitializeDataContext(PipesDbContext context): Устанавливает DataContext как новый PipesPerHourViewModel(context).
- Логика: Основное окно; загружает ViewModel с контекстом из DI.

 5.3. MainWindow.xaml (XAML)
- Окно с Grid: ComboBox для Nomenclature, StrengthClass, PipeThread, Section (DisplayMemberPath для отображения).
- Кнопка "Получить план в час" с Command GetPipesPerHourCommand.
- TextBlock для PipesPerHourResult с StringFormat и FallbackValue.
- Логика: Центрированное окно без ресайза; вертикальное выравнивание.

 5.4. App.xaml (XAML)
- Базовый Application без ресурсов.

 5.5. AssemblyInfo.cs
- Атрибут ThemeInfo: настраивает расположение словарей ресурсов для тем WPF.

 6. Общая логика и потоки

- Запуск: App -> MainWindow -> ViewModel инициализирует коллекции и команды асинхронно.
- Выбор параметров: Изменение SelectedNomenclature вызывает фильтрацию списков.
- Расчёт: Кнопка проверяет валидность, генерирует производные сущности, обновляет ShiftTask и выводит результат.
- Ошибки: Логи в консоль; MessageBox для пользователя.
- Производительность: Асинхронные запросы; нет batch-сохранения, так как операции единичные.
- Безопасность: нет аутентификации; прямой доступ к БД.
