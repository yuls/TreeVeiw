# TreeVeiw

Проект создан в Microsoft Visual Studio Community 2019 Версия 16.10.2.
Приложение: .NET Core 3.1, ReactJS 16, бд Microsoft SQL Server
Проверяла работу в Chrome и Firefox

Чтобы запустить приложение, я делала следующее:
- клонировала репозиторий
- открыла решение в Visual Studio (открыла файл TreeVeiw.sln)
- подключила базу данных, которая находится в файле TreeVeiw/database/TreeView.mdf
  в моей версии VS сделала следующее: 
    - открыла View->ServerExplorer,
    - кликнула правой кнопкой по Data Connections и выбрала Add Connection...
    - в появившемся списке выбрала Microsoft SQL Server DataBase File, нажала Continue
    - для поля Database Filename нажала Browse...
    - выбрала файл базы данных (TreeVeiw/database/TreeView.mdf)
    - нажала Ok
    - в файле Data/TreeViewDbData.cs поправила connectionString (значение взяла полностью из свойств подключенного файла базы данных)
- собрала решение
- запустила через IIS Express


С запуском IIS Express у меня возникли проблемы (как описано на этой странице https://developercommunity.visualstudio.com/t/Failed-to-load-resource:-net::ERR_HTTP2_/1446262),
чтобы не тратить много времени на разбирательство, удалила у себя обновление операционной системы KB5003637
(не лучший вариант, но для меня он был самый быстрый).

Сортировака элементов по имени или размеру в рамках папки реализована программно.
В файле TreeVeiw/ClientApp/src/components/TreeView.js параметр orderNodesBy принимает значение OrderBy.Name или OrderBy.Size;
