# Введение

Модуль ui. Предоставляет инструменты для более удобной работы с ui.

## Зависимости и требования:
* Unity version: 2021.1.6f1 и выше
* Api compatibility level: .Net 4.x
* [Evolution-engine-core](https://bitbucket.org/little-bit-games/evolution-engine-core/src/master/)
* [Evolution-engine-pool](https://github.com/LittleBitOrganization/evolution-engine-pool-service)

## Импорт
```JSON
"dependencies": {
    "com.littlebitgames.uimodule": "https://github.com/LittleBitOrganization/evolution-engine-ui.git",
}
```
## Использование в проекте

- Сначала нужно создать заранее 1 или несколько канвасов, куда будут спавниться окна
![image](https://user-images.githubusercontent.com/66946236/203394415-fe1ba191-7fe4-4405-ba37-fdfa3255dd2a.png)

Далее они биндятся через накинутый на них MonoBeh, унаследованный от RootLayout

```ruby

    public class MainRootLayout:RootLayout { }
        
            Container
                .Bind<MainRootLayout>()
                .FromInstance(_mainRoot)
                .AsSingle()
                .NonLazy();
```

- Далее биндится билдер

```ruby
            Container
                .Bind<ILayoutBuilderService>()
                .To<LayoutBuilderService>()
                .AsSingle()
                .NonLazy();
```

Далее для каждого окна следует создать конфиг со стилями. 

![image](https://user-images.githubusercontent.com/66946236/203487570-2704a1d2-6f82-4604-a01f-2df8e03ebbb9.png)

После чего для каждого окна делается фабрика.
Фабрика представляет собой класс, унаследованный от HidingWindowFactory
    
```ruby
    
    public class ProductionUpgradeFactory : AnimationWindowFactory<ProductionUiViewLayout>
    {
        public ProductionUpgradeFactory(ILayoutBuilderService layoutBuilderService, CommonRootLayout rootLayout)
            : base("Default", layoutBuilderService, rootLayout) { }
    }
    
```

- После чего биндится фабрика и создается список фабрик для всех окон в проекте.

```ruby
    
            Container
                .Bind<LayoutFactory>()
                .AsSingle()
                .NonLazy();
                
                // Далее следует создать список всех окон в проекте.
                // По мерее добавления окон в проект список следует пополнять.

            var windowsFactories = new List<CommonWindow>();

            windowsFactories.Add(Container.Instantiate<ShopLayoutFactory>());
            windowsFactories.Add(Container.Instantiate<MenuLayoutFactory>());
            windowsFactories.Add(Container.Instantiate<QuestsLayoutFactory>());

            Container.Bind<List<CommonWindow>>()
                .FromInstance(windowsFactories)
                .AsSingle()
                .NonLazy();
```
    
- В конце биндится класс, унаследованный от BaseCommonUIService.
    
    
```ruby

        Container.BindInterfacesAndSelfTo<CommonUIService>()
                .AsSingle()
                .NonLazy();
```
    
Основное назначение сервиса - помощь с открытием окон. 
Для открытия окна Следует вызвать команду и метод открытия.
    
```ruby
    CommonUIService _commonUIService;
    
    new OpenUiWindowCommand<PhoneLayoutFactory>(_commonUIService).OpenWithEmptyContext();    
    // or
    new OpenUiWindowCommand<PhoneLayoutFactory>(_commonUIService).OpenWindow(new WindowContext(_shopTrigger));
```
    
Так же в сервисе происходит регистрация окон, которые открываются при нажатии на триггер на локации:
    
```ruby
        public class CommonUIService : BaseCommonUIService
    {
        private readonly BlockClickLayout _blockClickLayout;
        private readonly MainRootLayout _mainRootLayout;
    
        public CommonUIService(BlockClickLayout blockClickLayout,
                                MainRootLayout mainRootLayout,
                                List<CommonWindow> windowsFactorys,
                                IRaycastService raycastService)
        {
            _blockClickLayout = blockClickLayout;
            _mainRootLayout = mainRootLayout;
            _windowsFactorys = windowsFactorys;
            _raycastService = raycastService;
        }
    
        public void Initialize()
        {
           // Инициализация магазинов
           foreach (var windowsFactory in _windowsFactorys)
            {
                InitShop(windowsFactory,0, () => { }, () => { });
            }
            // Подписка на рейкаст
            _raycastService.AddOnRaycastHitListener(OnRaycastHit);
        }

        //  Коллбек при закрытии какого-либо окна
        protected override void OnCloseWindow(CommonWindow commonWindow,int layer)
        {
            base.OnCloseWindow(commonWindow,layer);
            _blockClickLayout.Disable();
            OnCloseWindowEvent?.Invoke();
        }
    
        //  Коллбек при открытии какого-либо окна    
        protected override void OnOpenWindow(CommonWindow window, WindowContext windowContext, int layer = 0)
        {
            _blockClickLayout.Enable();
            OnOpenWindowEvent?.Invoke();
        }
    
        // Регистрация коллайдеров для открытия окон
        protected override void OnRaycastHit(GameObject go)
        {
            if (go.TryGetComponent(out ProductionTrigger buildingTrigger))
            {
                OpenWindow<ProductionUpgradeFactory>(new WindowContext(buildingTrigger));
            }      
            
            if (go.TryGetComponent(out FieldsTrigger fieldsTrigger))
            {
                OpenWindow<FieldsUpgradeFactory>(new WindowContext(fieldsTrigger));
            }
        }
    }
    
```

Так же система реализует стэк окон, поэтому при открытии нескольких окон друг поверх другого, они будут закрываться в обратном порядке.
    
    
