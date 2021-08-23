# NginxLogAnalyzer

is a small tool for analyzing your nginx access logs. This application was teset on macOS and Linux

## Don't care just make it work (Debian 10)

Replace `{ARGUMENTS}` at the end with your arguments.

```shell
wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
sudo dpkg -i packages-microsoft-prod.deb && \
rm packages-microsoft-prod.deb && \
sudo apt-get update  && \
audo apt-get install dotnet-sdk-5.0 git -y && \
git clone https://github.com/janwiesemann/NginxLogAnalyzer.git && \
dotnet run --project ./NginxLogAnalyzer/NginxLogAnalyzer/NginxLogAnalyzer.csproj -- {ARGUMENTS}
```

## Quick guide on arguments

Argument | Info
--- | ---
‍ | Analyse `/var/log/nginx`
`/var/log/nginx` | Analyse `/var/log/nginx`
`sftp://root:1234@10.0.0.1/var/log/nginx` | Analyse `/var/log/nginx` on server `10.0.0.1` using `root` as username and `123` as password
`/var/log/nginx sftp://root:1234@10.0.0.1/var/log/nginx` | Analyse `/var/log/nginx` and `/var/log/nginx` on server `10.0.0.1` using `root` as username and `123` as password
`/var/log/nginx --address=10.0.0.2` | Analyse `/var/log/nginx` showing only requests form 10.0.0.2

## Build

### Requierments

This application is written in C#. This means you have to install .NET 5. You can download .NET from it's official site: <https://dotnet.microsoft.com/download/dotnet/5.0>.

You can build this as a standalone executable. More infomations can be found in the section `Build -> Build as Standalone executable`

Direct link to Debian 10 based systems: <https://docs.microsoft.com/en-gb/dotnet/core/install/linux-debian>

### Build code

```shell
dotnet build ./NginxLogAnalyzer/NginxLogAnalyzer.csproj
```

### Run output

After build you can execute the output. You can find it inside `/NginxLogAnalyzer/bin/Debug/net5.0/` as `NginxLogAnalyzer.dll`

```shell
cd ./NginxLogAnalyzer/bin/Debug/net5.0/
dotnet NginxLogAnalyzer.dll
```

### Run from .NET project

this can be used if you want to run this tool directly from its project files

```shell
dotnet run --project ./NginxLogAnalyzer/NginxLogAnalyzer.csproj
```

With arguments (everyting after `--`  will be send to the application):

```shell
dotnet run --project ./NginxLogAnalyzer/NginxLogAnalyzer.csproj -- /var/log/nginx/ -A
```

### Build as Standalone executable

If you want to run this application without installing .NET you can follow this article: <https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish>

## Sources / Log files

You can use following log sources:

Name | Example | Info
--- | --- | ---
Directory | `/var/log/nginx/` | This will load all files starting with the name `access.`(`access.log`, `access.log.1`, `access.log.2.gz`)
File | `/var/log/nginx/access.log` | Text file or `.gz` compressed (compressed Files must end with `.gz`)
SFTP | `sftp://user:password@10.0.0.1/var/log/nginx/` | This can be used to specify a remote Server as Source. This will connect to `10.0.0.1` using `user` as username, `password` as password and analyse all files in `/var/log/nginx` Currently you can not use a Key file as login.

You can use a single Source

```shell
dotnet NginxLogAnalyzer.dll /var/log/nginx/
```

Or multiple sources

```shell
dotnet NginxLogAnalyzer.dll /var/log/nginx/ sftp://user:root@10.0.0.1/var/log/nginx/
```

### Default

if you do not specify a source argument the tool will use default directorys as source These are
OS | Path | Note
--- | --- | ---
macOS | `/usr/local/var/log/nginx/` | default homebrew path
Linux | `/var/log/nginx/` | default

## Switches

By default all switches are set (`-A`). Switches can be set individually (`-a -p`) or as a single block (`-ap`).

Switch|Name|Description
--- | --- | ---
 `-A` | All | this will set all Switches (default)
 `-a` | Analyse by addresses | This will display a list where requests are grouped by IP
 `-p` | Analyse page accesses | This will display a list with the most requested pages
 `-r` | Last requests | Displays the last requests

## Filter/Settings

By default no settings or filers are set. Filters and settings have to start with `--` and have to provide a value after `=`. For example: `--address=10.0.0.112` can be used this way:

```shell
dotnet NginxLogAnalyzer.dll --address=10.0.0.112
```

If your value contains whitespaces you have to escape them. This is different on every OS. On Windows you can use this `"--address=just a value"` format. On Mac you can escape Whitespaces with a `\` => `--address=just\ a\ value`

### List of Settings

Name | Format | Default | Description
--- | --- | --- | --
`--count` | Number or `ALL` | `25` | Changes the number of groups displayed (typicaly containing the address)
`--entrys` | Number or `ALL` | `5` | Changes the number of entrys displayed (subgroups of `--count`)
`--format` | String | `$remote_addr - $remote_user [$time_local] "$request" $status $body_bytes_sent "$http_referer" "$http_user_agent"` | Your Nginx log format string. You can copy and paste this from your config.

### List of Filters

Name | Format | Description
--- | --- | ---
`--address` | Any | Filters by address
`--accessTime` | `dd.MM.yyyy-hh:mm:ss` (i.e. `01.08.2021-15:30:00`) | Newer than this date

## Example output

```text
=================
Source

Using LogSFTPSource as source handler for sftp:***@***.***.***.***/var/log/nginx/
Using LogDirectorySource as source handler for /var/log/nginx

=================
Filter


=================
Settings

count: 50

=================
Switches

-A

=================
Format

$remote_addr - $remote_user [$time_local] "$request" $status $body_bytes_sent "$http_referer" "$http_user_agent"

=================
Reading

Reading source LogSFTPSource (sftp:***@***.***.***.***/var/log/nginx/)... finished in 1695.6ms
Reading source LogDirectorySource (/var/log/nginx)... finished in 81ms

Found 765 unique addresses with a total of 3886 (unfiltered 3886) requests.

=================
Most requested pages

931  (24%) => /
502  (13%) => /hlnugToLaMetric/
221  (06%) =>
106  (03%) => /robots.txt
93   (02%) => /boaform/admin/formLogin
68   (02%) => /.env
60   (02%) => /favicon.ico
38   (01%) => /wp-login.php
37   (01%) => /handler.php?get
34   (01%) => /vendor/phpunit/phpunit/src/Util/PHP/eval-stdin.php
34   (01%) => http://azenv.net/
30   (01%) => azenv.net:443
27   (01%) => /assets/js/util.js
26   (01%) => /assets/js/breakpoints.min.js
26   (01%) => /assets/js/jquery.min.js
26   (01%) => /assets/js/main.js
26   (01%) => /assets/js/jquery.poptrox.min.js
26   (01%) => /assets/js/browser.min.js
26   (01%) => /assets/css/fontawesome-all.min.css
25   (01%) => /assets/css/main.css
24   (01%) => /assets/css/images/close.svg
23   (01%) => /assets/css/images/spinner.svg
22   (01%) => /config/getuser?index=0
21   (01%) => /WebServerLogs/
18   (00%) => /ecp/Current/exporttool/microsoft.exchange.ediscovery.exportt...
18   (00%) => /assets/webfonts/fa-solid-900.woff2
18   (00%) => /assets/webfonts/fa-brands-400.woff2
18   (00%) => /assets/webfonts/fa-regular-400.woff2
17   (00%) => /contact.php?mail
15   (00%) => /_ignition/execute-solution
14   (00%) => /sql/myadmin/index.php?lang=en
13   (00%) => /owa/auth/logon.aspx
12   (00%) => mstshash=Administr
12   (00%) => /imgcache/17860802063386856.jpg
12   (00%) => /imgcache/17872631525190761.jpg
12   (00%) => /imgcache/17883519593148799.jpg
12   (00%) => /imgcache/17847968798350657.jpg
11   (00%) => /sitemap.xml
11   (00%) => /imgcache/17848115306452799.jpg
11   (00%) => /imgcache/17864365793014388.jpg
11   (00%) => /imgcache/17859806708054093.jpg
11   (00%) => /imgcache/17884361464633532.jpg
11   (00%) => /imgcache/17900609185757881.jpg
10   (00%) => /owa/auth/logon.aspx?url=https%3a%2f%2f1%2fecp%2f
10   (00%) => /actuator/health
10   (00%) => /imgcache/17911501492511879.jpg
10   (00%) => /imgcache/18154342684003617.jpg
10   (00%) => /imgcache/17852196464177984.jpg
9    (00%) => /index.html
9    (00%) => /api/jsonws/invoke

=================
Most requests by address

***.***.***.***       => 499
	499 => GET /hlnugToLaMetric/ HTTP/1.1
***.***.***.***    => 158
	2   => GET /wp-config-good HTTP/1.1
	2   => GET /wp-config.bak HTTP/1.1
	2   => GET /wp-config.php-bak HTTP/1.1
	2   => GET /wp-config.php.0 HTTP/1.1
	2   => GET /wp-config.php.1 HTTP/1.1
***.***.***.***        => 139
	15  => GET / HTTP/1.1
	9   => GET /handler.php?get HTTP/1.1
	9   => POST /WebServerLogs/index.php HTTP/1.1
	5   => GET /assets/css/main.css HTTP/1.1
	5   => GET /assets/js/main.js HTTP/1.1
***.***.***.***    => 122
	6   => GET /administrator/admin/index.php?lang=en HTTP/1.1
	4   => GET /pma2015/index.php?lang=en HTTP/1.1
	4   => GET /sql/myadmin/index.php?lang=en HTTP/1.1
	4   => GET /db/websql/index.php?lang=en HTTP/1.1
	3   => GET /phpMyAdmin_/index.php?lang=en HTTP/1.1
***.***.***.***   => 122
	4   => GET /sql/myadmin/index.php?lang=en HTTP/1.1
	4   => GET /_phpmyadmin/index.php?lang=en HTTP/1.1
	3   => GET /sql/phpMyAdmin2/index.php?lang=en HTTP/1.1
	3   => GET /phpmyadmin4/index.php?lang=en HTTP/1.1
	3   => GET /db/phpMyAdmin/index.php?lang=en HTTP/1.1
***.***.***.***     => 122
	4   => GET /mysql/index.php?lang=en HTTP/1.1
	3   => GET /PMA2015/index.php?lang=en HTTP/1.1
	3   => GET /mysql/db/index.php?lang=en HTTP/1.1
	3   => GET /phpmy-admin/index.php?lang=en HTTP/1.1
	3   => GET /sql/webdb/index.php?lang=en HTTP/1.1
***.***.***.***    => 122
	4   => GET /sql/myadmin/index.php?lang=en HTTP/1.1
	3   => GET /db/dbadmin/index.php?lang=en HTTP/1.1
	3   => GET /php-my-admin/index.php?lang=en HTTP/1.1
	3   => GET /sql/sqladmin/index.php?lang=en HTTP/1.1
	3   => GET /phpmyadmin2020/index.php?lang=en HTTP/1.1
***.***.***.***  => 104
	52  => POST /boaform/admin/formLogin HTTP/1.1
	52  =>
***.***.***.***   => 99
	18  => POST /vendor/phpunit/phpunit/src/Util/PHP/eval-stdin.php HTTP/1.1
	9   => POST /api/jsonws/invoke HTTP/1.1
	9   => GET /?XDEBUG_SESSION_START=phpstorm HTTP/1.1
	9   => GET /wp-content/plugins/wp-file-manager/readme.txt HTTP/1.1
	9   => GET /index.php?s=/Index/\x5Cthink\x5Capp/invokefunction&function=... HTTP/1.1
***.***.***.***    => 52
	6   => GET /index.php HTTP/1.1
	4   => GET / HTTP/1.1
	4   => GET /menu.html?images/ HTTP/1.1
	4   => GET /GponForm/diag_FORM?images/ HTTP/1.1
	4   => GET /public/index.php HTTP/1.1
***.***.***.***   => 46
	6   => GET / HTTP/1.1
	4   =>
	2   => GET //wp-includes/wlwmanifest.xml HTTP/1.1
	2   => GET //xmlrpc.php?rsd HTTP/1.1
	2   => GET //blog/wp-includes/wlwmanifest.xml HTTP/1.1
***.***.***.***      => 45
	22  => GET /robots.txt HTTP/1.1
	9   => GET / HTTP/1.1
	8   => GET /contact.php?mail HTTP/1.1
	6   => GET /index.html HTTP/1.1
***.***.***.***      => 38
	36  => GET / HTTP/1.1
	1   => GET /index.html/assets/js/jquery.poptrox.min.js HTTP/1.1
	1   => GET /assets/js/breakpoints.min.js HTTP/1.1
***.***.***.***     => 37
	3   => GET / HTTP/1.1
	3   => GET /handler.php?get HTTP/1.1
	2   => GET /assets/css/main.css HTTP/1.1
	2   => GET /assets/js/breakpoints.min.js HTTP/1.1
	2   => GET /assets/js/util.js HTTP/1.1
***.***.***.***        => 36
	5   => GET / HTTP/1.1
	5   => GET /handler.php?get HTTP/1.1
	1   => GET /assets/css/main.css HTTP/1.1
	1   => GET /assets/js/breakpoints.min.js HTTP/1.1
	1   => GET /assets/js/util.js HTTP/1.1
***.***.***.***   => 32
	4   => GET / HTTP/1.1
	2   => GET /handler.php?get HTTP/1.1
	1   => GET /assets/js/main.js HTTP/1.1
	1   => GET /assets/js/breakpoints.min.js HTTP/1.1
	1   => GET /assets/css/main.css HTTP/1.1
***.***.***.***   => 31
	17  => GET http://azenv.net/ HTTP/1.1
	14  => CONNECT azenv.net:443 HTTP/1.1
***.***.***.***   => 31
	2   => GET /favicon.ico HTTP/1.1
	2   => GET /handler.php?get HTTP/1.1
	1   => GET / HTTP/1.1
	1   => GET /assets/css/main.css HTTP/1.1
	1   => GET /assets/css/noscript.css HTTP/1.1
***.***.***.***    => 30
	28  => GET / HTTP/1.1
	2   => \x15\x0...
***.***.***.***     => 29
	2   => GET / HTTP/1.1
	1   => GET /assets/css/main.css HTTP/1.1
	1   => GET /assets/js/jquery.min.js HTTP/1.1
	1   => GET /assets/js/jquery.poptrox.min.js HTTP/1.1
	1   => GET /assets/js/browser.min.js HTTP/1.1
***.***.***.***        => 29
	3   => GET / HTTP/1.1
	1   => GET /assets/css/main.css HTTP/1.1
	1   => GET /assets/js/main.js HTTP/1.1
	1   => GET /assets/css/fontawesome-all.min.css HTTP/1.1
	1   => GET /assets/js/jquery.min.js HTTP/1.1
***.***.***.***     => 28
	11  => POST /boaform/admin/formLogin HTTP/1.1
	11  =>
	6   => GET / HTTP/1.1
***.***.***.***      => 28
	1   => GET / HTTP/1.1
	1   => GET /assets/css/main.css HTTP/1.1
	1   => GET /assets/js/breakpoints.min.js HTTP/1.1
	1   => GET /assets/js/util.js HTTP/1.1
	1   => GET /assets/js/jquery.min.js HTTP/1.1
***.***.***.***     => 28
	1   => GET / HTTP/1.1
	1   => GET /assets/css/main.css HTTP/1.1
	1   => GET /assets/js/breakpoints.min.js HTTP/1.1
	1   => GET /assets/js/util.js HTTP/1.1
	1   => GET /assets/js/jquery.min.js HTTP/1.1
***.***.***.***   => 27
	1   => GET / HTTP/1.1
	1   => GET /assets/css/main.css HTTP/1.1
	1   => GET /assets/js/breakpoints.min.js HTTP/1.1
	1   => GET /assets/js/jquery.min.js HTTP/1.1
	1   => GET /assets/js/jquery.poptrox.min.js HTTP/1.1
***.***.***.***    => 26
	4   => GET /system_api.php HTTP/1.1
	4   => GET /c/version.js HTTP/1.1
	4   => GET /streaming/clients_live.php HTTP/1.1
	4   => GET /stalker_portal/c/version.js HTTP/1.1
	4   => GET /stream/live.php HTTP/1.1
***.***.***.***  => 26
	4   => GET /system_api.php HTTP/1.1
	4   => GET /c/version.js HTTP/1.1
	4   => GET /streaming/clients_live.php HTTP/1.1
	4   => GET /stalker_portal/c/version.js HTTP/1.1
	4   => GET /stream/live.php HTTP/1.1
***.***.***.***       => 26
	17  => GET /WebServerLogs/ HTTP/1.1
	7   => GET / HTTP/1.1
	1   => GET /favicon.ico HTTP/1.1
	1   => GET /.env HTTP/1.1
***.***.***.***   => 23
	1   => GET / HTTP/1.1
	1   => GET /assets/css/main.css HTTP/1.1
	1   => GET /assets/js/browser.min.js HTTP/1.1
	1   => GET /assets/js/main.js HTTP/1.1
	1   => GET /assets/js/jquery.min.js HTTP/1.1
***.***.***.***   => 22
	3   => GET / HTTP/1.1
	2   =>
	1   => GET //wp-includes/wlwmanifest.xml HTTP/1.1
	1   => GET //xmlrpc.php?rsd HTTP/1.1
	1   => GET //blog/wp-includes/wlwmanifest.xml HTTP/1.1
***.***.***.***  => 22
	11  => POST /boaform/admin/formLogin HTTP/1.1
	11  =>
***.***.***.***     => 21
	3   => GET / HTTP/1.1
	2   => GET /robots.txt HTTP/1.1
	1   => GET /admin/ HTTP/1.1
	1   => GET /login HTTP/1.1
	1   => GET /admin.asp HTTP/1.1
***.***.***.***      => 21
	4   => GET / HTTP/1.1
	2   =>
	1   => GET //wp-includes/wlwmanifest.xml HTTP/1.1
	1   => GET //xmlrpc.php?rsd HTTP/1.1
	1   => GET //blog/wp-includes/wlwmanifest.xml HTTP/1.1
***.***.***.***    => 20
	18  => GET / HTTP/1.1
	1   => GET /assets/js/jquery.min.js HTTP/1.1
	1   => GET /assets/js/main.js HTTP/1.1
***.***.***.***   => 20
	10  =>
	3   => GET / HTTP/1.1
	2   => GET /sitemap.xml HTTP/1.1
	2   => GET /.well-known/security.txt HTTP/1.1
	2   => GET /favicon.ico HTTP/1.1
***.***.***.***    => 19
	3   => GET /assets/css/images/spinner.svg HTTP/1.1
	3   => GET /assets/css/images/close.svg HTTP/1.1
	2   => GET /assets/js/jquery.min.js HTTP/1.1
	2   => GET /assets/js/jquery.poptrox.min.js HTTP/1.1
	2   => GET /handler.php?get HTTP/1.1
***.***.***.***    => 18
	2   => GET / HTTP/1.1
	1   => \x16\x0... WM\x18\xEB)\x06\x88\xA2`\x1E\xEB\xBA\xF3z\xAEd\x9AE0\xC7y
	1   => GET /.DS_Store HTTP/1.1
	1   => GET /nginx.conf HTTP/1.1
	1   => GET /status HTTP/1.1
***.***.***.***   => 18
	2   => GET /invoker/readonly HTTP/1.1
	2   => POST /_ignition/execute-solution HTTP/1.1
	2   => POST /vendor/phpunit/phpunit/src/Util/PHP/eval-stdin.php HTTP/1.1
	2   => GET / HTTP/1.1
	2   => GET /login HTTP/1.1
***.***.***.***   => 18
	8   => GET http://fuwu.sogou.com/404/index.html HTTP/1.1
	4   => \x05\x0...
	4   => CONNECT fuwu.sogou.com:443 HTTP/1.1
	1   => \x16\x0... :\x8An\xCA\xA2\xA1\xC4\x14\xF6+\xF5c\xA4\xFBv\x0Ck\xF9\x08\xC...
	1   => \x16\x0... )m\xC2h\x94Y\xE2\xA3\x18\x05\xEF\xA0+\xE2\xE4X\xBA\x87\xEC\xA...
***.***.***.***    => 18
	2   => GET /invoker/readonly HTTP/1.1
	2   => POST /_ignition/execute-solution HTTP/1.1
	2   => POST /vendor/phpunit/phpunit/src/Util/PHP/eval-stdin.php HTTP/1.1
	2   => GET / HTTP/1.1
	2   => GET /login HTTP/1.1
***.***.***.***   => 17
	17  => GET /config/getuser?index=0 HTTP/1.1
***.***.***.***      => 17
	6   => \x03z\x...
	3   => GET /favicon.ico HTTP/1.1
	2   => GET / HTTP/1.1
	2   => GET /robots.txt HTTP/1.1
	2   => GET /sitemap.xml HTTP/1.1
***.***.***.***  => 16
	8   => POST /boaform/admin/formLogin HTTP/1.1
	8   =>
***.***.***.***    => 15
	1   => GET / HTTP/1.1
	1   => GET /assets/css/main.css HTTP/1.1
	1   => GET /assets/js/jquery.poptrox.min.js HTTP/1.1
	1   => GET /assets/js/breakpoints.min.js HTTP/1.1
	1   => GET /assets/js/browser.min.js HTTP/1.1
***.***.***.***     => 15
	6   => \x03\x5...
	3   => GET /favicon.ico HTTP/1.1
	2   => GET / HTTP/1.1
	2   => GET /.well-known/security.txt HTTP/1.1
	1   => GET /robots.txt HTTP/1.1
***.***.***.***    => 13
	2   => GET /assets/js/main.js HTTP/1.1
	2   => GET /assets/css/fontawesome-all.min.css HTTP/1.1
	2   => GET /assets/js/breakpoints.min.js HTTP/1.1
	1   => GET /robots.txt HTTP/1.1
	1   => GET /assets/js/util.js HTTP/1.1
***.***.***.***   => 12
	3   => GET / HTTP/1.0
	3   => GET /favicon.ico HTTP/1.1
	3   => GET /sitemap.xml HTTP/1.1
	2   => GET /robots.txt HTTP/1.1
	1   => \x16\x0...
***.***.***.***    => 12
	6   => GET /.env HTTP/1.1
	6   => POST / HTTP/1.1
***.***.***.***   => 12
	8   => GET / HTTP/1.1
	2   => GET /webfig/ HTTP/1.1
	2   => GET /Telerik.Web.UI.WebResource.axd?type=rau HTTP/1.1
***.***.***.***    => 12
	2   => GET /contact.php?mail HTTP/1.1
	1   => GET / HTTP/1.1
	1   => GET /assets/js/util.js HTTP/1.1
	1   => GET /assets/css/main.css HTTP/1.1
	1   => GET /assets/js/jquery.poptrox.min.js HTTP/1.1

=================
Accesses

***.***.***.***: 08/15/2021 20:30:48 => GET /WebServerLogs/index.php HTTP/1.1
***.***.***.***: 08/15/2021 20:30:43 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 20:29:20 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 20:29:19 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 20:29:15 => GET /robots.txt HTTP/1.1
***.***.***.***: 08/15/2021 20:29:15 => GET /robots.txt HTTP/1.1
***.***.***.***: 08/15/2021 20:28:33 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 20:28:30 => GET /robots.txt HTTP/1.1
***.***.***.***: 08/15/2021 20:25:35 => CONNECT azenv.net:443 HTTP/1.1
***.***.***.***: 08/15/2021 20:25:26 => GET http://azenv.net/ HTTP/1.1
***.***.***.***: 08/15/2021 20:23:11 => GET /manager/text/list HTTP/1.1
***.***.***.***: 08/15/2021 20:19:21 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 20:06:45 => GET /hlnugToLaMetric/ HTTP/1.1
***.***.***.***: 08/15/2021 20:02:13 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 19:45:38 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 19:45:32 => GET /robots.txt HTTP/1.1
***.***.***.***: 08/15/2021 19:45:31 => GET /robots.txt HTTP/1.1
***.***.***.***: 08/15/2021 19:36:44 => GET /hlnugToLaMetric/ HTTP/1.1
***.***.***.***: 08/15/2021 19:34:09 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 19:28:33 => GET /favicon.ico HTTP/1.1
***.***.***.***: 08/15/2021 19:21:55 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 19:06:44 => GET /hlnugToLaMetric/ HTTP/1.1
***.***.***.***: 08/15/2021 18:47:41 => GET /actuator/health HTTP/1.1
***.***.***.***: 08/15/2021 18:45:56 => \x16\x0...
***.***.***.***: 08/15/2021 18:41:35 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 18:36:43 => GET /hlnugToLaMetric/ HTTP/1.1
***.***.***.***: 08/15/2021 18:30:19 => CONNECT azenv.net:443 HTTP/1.1
***.***.***.***: 08/15/2021 18:30:10 => GET http://azenv.net/ HTTP/1.1
***.***.***.***: 08/15/2021 18:28:21 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 18:06:42 => GET /hlnugToLaMetric/ HTTP/1.1
***.***.***.***: 08/15/2021 17:38:57 => GET http://azenv.net/ HTTP/1.1
***.***.***.***: 08/15/2021 17:36:42 => GET /hlnugToLaMetric/ HTTP/1.1
***.***.***.***: 08/15/2021 17:30:34 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 17:20:33 => GET /owa/auth/logon.aspx HTTP/1.1
***.***.***.***: 08/15/2021 17:08:48 => GET /ecp/Current/exporttool/microsoft.exchange.ediscovery.exportt... HTTP/1.1
***.***.***.***: 08/15/2021 17:06:41 => GET /hlnugToLaMetric/ HTTP/1.1
***.***.***.***: 08/15/2021 17:03:52 => GET /owa/auth/x.js HTTP/1.1
***.***.***.***: 08/15/2021 16:50:53 => GET /remote/fgt_lang?lang=/../../../..//////////dev/cmdb/sslvpn_w... HTTP/1.1
***.***.***.***: 08/15/2021 16:37:08 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 16:36:40 => GET /hlnugToLaMetric/ HTTP/1.1
***.***.***.***: 08/15/2021 16:26:40 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 16:26:35 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 16:26:26 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 16:26:25 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 16:26:22 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 16:26:19 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 16:26:18 => GET / HTTP/1.1
***.***.***.***: 08/15/2021 16:18:30 => POST / HTTP/1.1
***.***.***.***: 08/15/2021 16:18:29 => GET /.env HTTP/1.1
***.***.***.***: 08/15/2021 16:16:27 => CONNECT azenv.net:443 HTTP/1.1

=================
Done
```
