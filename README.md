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

...         
            Container
                .Bind<MainRootLayout>()
                .FromInstance(_mainRoot)
                .AsSingle()
                .NonLazy();
```

- Далее биндится билдер

```
            Container
                .Bind<ILayoutBuilderService>()
                .To<LayoutBuilderService>()
                .AsSingle()
                .NonLazy();
```

```
            Container
                .Bind<LayoutFactory>()
                .AsSingle()
                .NonLazy();

            var windowsFactories = new List<CommonWindow>();

            windowsFactories.Add(Container.Instantiate<ProductionUpgradeFactory>());
            windowsFactories.Add(Container.Instantiate<FieldsUpgradeFactory>());
            windowsFactories.Add(Container.Instantiate<PhoneLayoutFactory>());
            windowsFactories.Add(Container.Instantiate<MachinesAndTransportFactory>());

            Container.Bind<List<CommonWindow>>()
                .FromInstance(windowsFactories)
                .AsSingle()
                .NonLazy();
```
```
        Container
                .BindInterfacesAndSelfTo<CommonUIService>()
                .AsSingle()
                .NonLazy();
```
