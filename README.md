# NginxLogAnalyzer

is a small tool for analyzing your nginx access logs. This application was teset on macOS and Linux

# Usage

## Requierments

This application is writen in C#. This means you have to install .NET 5. You can download .NET from it's official site: [https://dotnet.microsoft.com/download/dotnet/5.0](https://dotnet.microsoft.com/download/dotnet/5.0).

## Build and run

### Build

```shell
dotnet build ./NginxLogAnalyzer/NginxLogAnalyzer.csproj
```

### Run

After build you can execute the output. You can find it inside `/NginxLogAnalyzer/bin/Debug/net5.0/` as `NginxLogAnalyzer.dll`

```shell
cd ./NginxLogAnalyzer/bin/Debug/net5.0/
dotnet NginxLogAnalyzer.dll
```

## Run from .NET project

this can be used if you want to run this tool directly from its project files


```
dotnet run --project ./NginxLogAnalyzer/NginxLogAnalyzer.csproj
```

With arguments (everyting after `--`  will be send to the application):

```
dotnet run --project ./NginxLogAnalyzer/NginxLogAnalyzer.csproj -- /var/log/nginx/ -A
```


## Analyzing directory

```shell
dotnet NginxLogAnalyzer.dll /var/log/nginx/
```

if you do not specify this argument the tool will use default pats. These are
OS | Path | Note
--- | --- | ---
macOS | /usr/local/var/log/nginx/ | default homebrew path
Linux | /var/log/nginx/ | default

## Analyzing file

```shell
dotnet NginxLogAnalyzer.dll /var/log/nginx/access.log
```

you can also use a `.gz` file

```shell
dotnet NginxLogAnalyzer.dll /var/log/nginx/access.log.2.gz
```

## Switches

By default all switches are set (`-A`). Switches can be set individually (`-a -p`) or as a single block (`-ap`).

Switch|Name|Description
--- | --- | ---
 `-A` | All | this will set all Switches (default)
 `-a` | Analyze by addresses | This will display a list where requests are grouped by IP
 `-p` | Analyze page accesses | This will display a list with the most requested pages

 # Example

 # Default

This will analyze the default log dir. (see `Usage -> Analyzing directory`)

```shell
dotnet NginxLogAnalyzer.dll
```
# Path and Switches

```shell
dotnet NginxLogAnalyzer.dll /var/log/nginx/ -A
```

This will analyze all logs in `/var/log/nginx/` with all switches set (`-A`).

``` shell

=====================================================
Using Switches

Addresses
Pages

=====================================================
Most Requests by Addresse

10.0.0.112       => 1229
        1229 => GET /hlnugToLaMetric/ HTTP/1.1
10.0.1.1         => 484
        88  => GET /vendor/phpunit/phpunit/src/Util/PHP/eval-stdin.php HTTP/1.1
        44  => GET /?XDEBUG_SESSION_START=phpstorm HTTP/1.1
        44  => POST /api/jsonws/invoke HTTP/1.1
        44  => GET /_ignition/execute-solution HTTP/1.1
        44  => GET /wp-content/plugins/wp-file-manager/readme.txt HTTP/1.1
10.0.1.2         => 466
        10  => HEAD http://80.147.197.201:80/phpMyAdmin/ HTTP/1.1
        6   => HEAD http://80.147.197.201:80/phpmyadmin/ HTTP/1.1
        6   => HEAD http://80.147.197.201:80/phpmyAdmin/ HTTP/1.1
        6   => HEAD http://80.147.197.201:80/phpmanager/ HTTP/1.1
        4   => HEAD http://80.147.197.201:80/1phpmyadmin/ HTTP/1.1
10.0.1.3         => 255
        1   => GET /robots.txt HTTP/1.1
        1   => GET / HTTP/1.1
        1   => GET /test_404_page/ HTTP/1.1
        1   => GET /issmall/ HTTP/1.1
        1   => GET /administrator/manifests/files/joomla.xml HTTP/1.1
10.0.1.4          => 254
        1   => GET /robots.txt HTTP/1.1
        1   => GET / HTTP/1.1
        1   => GET /test_404_page/ HTTP/1.1
        1   => GET /issmall/ HTTP/1.1
        1   => GET /administrator/manifests/files/joomla.xml HTTP/1.1
10.0.1.5         => 139
        6   => GET / HTTP/1.1
        1   => GET /h5/ HTTP/1.1
        1   => GET /admin/index HTTP/1.1
        1   => GET /wap/trading/lastKlineParameter HTTP/1.1
        1   => GET /js/config20181225.js HTTP/1.1
10.0.1.6         => 135
        6   => GET / HTTP/1.1
        2   => GET /assets/js/main.js HTTP/1.1
        1   => GET /admin/index HTTP/1.1
        1   => GET /market/market-ws/iframe.html HTTP/1.1
        1   => GET /user/Login HTTP/1.1
10.0.1.7         => 72
        69  => GET / HTTP/1.1
        1   => GET /assets/js/jquery.min.js HTTP/1.1
        1   => GET /assets/js/main.js HTTP/1.1
        1   => GET /index.html/assets/js/main.js HTTP/1.1
10.0.1.8         => 71
        3   => GET / HTTP/1.1
        3   => GET /assets/css/main.css HTTP/1.1
        3   => GET /assets/js/jquery.min.js HTTP/1.1
        3   => GET /assets/js/jquery.poptrox.min.js HTTP/1.1
        3   => GET /assets/js/browser.min.js HTTP/1.1
10.0.1.9         => 70
        68  => GET / HTTP/1.1
        1   => GET /assets/js/breakpoints.min.js HTTP/1.1
        1   => GET /index.html/assets/js/jquery.poptrox.min.js HTTP/1.1

=====================================================
Most requested Pages

1503 (24%) => /
1229 (20%) => /hlnugToLaMetric/
189  (03%) => 
169  (03%) => /robots.txt
96   (02%) => /vendor/phpunit/phpunit/src/Util/PHP/eval-stdin.php
80   (01%) => /favicon.ico
78   (01%) => /boaform/admin/formLogin
66   (01%) => /wp-login.php
57   (01%) => /.env
46   (01%) => /_ignition/execute-solution
44   (01%) => /?XDEBUG_SESSION_START=phpstorm
44   (01%) => /api/jsonws/invoke
44   (01%) => /wp-content/plugins/wp-file-manager/readme.txt
44   (01%) => /index.php?s=/Index/\x5Cthink\x5Capp/invokefunction&function=...
44   (01%) => /Autodiscover/Autodiscover.xml
44   (01%) => /console/
39   (01%) => mstshash=Administr
34   (01%) => /handler.php?get
27   (00%) => /config/getuser?index=0
27   (00%) => /assets/js/main.js
26   (00%) => /assets/js/jquery.min.js
26   (00%) => /assets/js/browser.min.js
25   (00%) => /index.html
25   (00%) => /assets/js/util.js
24   (00%) => /assets/js/breakpoints.min.js
24   (00%) => /assets/js/jquery.poptrox.min.js
```