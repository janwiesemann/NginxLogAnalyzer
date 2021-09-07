<?php
    $config = array();
    //All settings will be stored in a file calles config.php. A php file is used to prevent access to the settings.

    function SetDefaultConfigValue(&$cfg, $key, $value) {
        if(!array_key_exists($key, $cfg) || (array_key_exists($key, $cfg) && empty($cfg[$key]))) {
            $cfg[$key] = $value;
        }
    }

    if(file_exists("config.php")) { //load config
        $cfgStr = file_get_contents("config.php");
        $i = strpos($cfgStr, "?>");
        if($i) {
            $cfgStr = substr($cfgStr, $i + 2);
        }

        $tmp = json_decode($cfgStr);
        foreach ($tmp as $key => $value) { //Convert from object to array
            $config[$key] = $value;
        }
    } else if(!isset($_GET["configure"])){ //redirect to config page
        header("Location: ".$_SERVER["PHP_SELF"]."?configure");
        exit();
    }

    //for dome reasone macOS can not find 'dotnet' as a command. This function will try to find a valid dotnet path
    function GetDotNetPath() {
        exec("dotnet", $out, $exitCode);
        if($exitCode == 0) { //dotnet was found as a command
            return "dotnet";
        }

        $paths = array(
            "/usr/local/share/dotnet/dotnet",
            "/usr/bin/dotnet",
            "/usr/share/dotnet"
        );
        foreach ($paths as $value) {
            if(file_exists($value)) {
                return $value;
            }
        }

        return "dotnet"; //fallback
    }

    //set default config values
    SetDefaultConfigValue($config, "sources", null);
    SetDefaultConfigValue($config, "loginKey", null);
    SetDefaultConfigValue($config, "count", 50);
    SetDefaultConfigValue($config, "entryCount", 10);
    SetDefaultConfigValue($config, "gitRefresh", false);
    SetDefaultConfigValue($config, "gitCommand", "git pull --ff-only");
    SetDefaultConfigValue($config, "dotnetCommand", GetDotNetPath()." run --project ./NginxLogAnalyzer/NginxLogAnalyzer.csproj -- ");

    $loginIsValid = false;
    if(!empty($config["loginKey"])) { //Login enabled    
        session_start();

        if(isset($_GET["logout"])) {
            session_destroy();
            header("Location: ".$_SERVER["PHP_SELF"]."?login");
            exit();
        }

        if(!isset($_SESSION["ok"]) || $_SESSION["ok"] !== true) { //Login not set
            if(!isset($_POST["loginKey"]) && !isset($_GET["login"])) { //redirect to login if no login key was entered
                header("Location: ".$_SERVER["PHP_SELF"]."?login");
                exit();
            }
    
            if(!empty($_POST["loginKey"])) {
                if($_POST["loginKey"] !== $config["loginKey"]) { //Invalid login key    
                    header("Location: ".$_SERVER["PHP_SELF"]."?login&wrong");
                    exit();
                } else { //Valid login
                    $_SESSION["ok"] = true;
    
                    header("Location: ".$_SERVER["PHP_SELF"]);
                    exit();
                }
            }
        }
        else if(isset($_GET["login"])){
            header("Location: ".$_SERVER["PHP_SELF"]);
            exit();
        } else {
            $loginIsValid = true;
        }
    } else {
        $loginIsValid = true;
    }

    if($loginIsValid && isset($_GET["setconfig"])) { //save config
        foreach ($_POST as $key => $value) {
            $config[$key] = $value;
        }

        $config["gitRefresh"] = isset($_POST["gitRefresh"]);

        $cfgStr = "<?php header('Location: ".$_SERVER["PHP_SELF"]."'); exit(); ?>".json_encode($config);
        file_put_contents("config.php", $cfgStr, FILE_USE_INCLUDE_PATH);  

        header("Location: ".$_SERVER["PHP_SELF"]."?configure");
        exit();
    }

    function ExecuteApp($command)
    {
        $proc = proc_open($command,[
            1 => ['pipe','w'],
            2 => ['pipe','w'],
        ],$pipes);

        $stdout = stream_get_contents($pipes[1]);
        fclose($pipes[1]);

        $stderr = stream_get_contents($pipes[2]);
        fclose($pipes[2]);

        if(!empty($stderr)) {
            echo "<pre class=\"out errorOut\">$stderr</pre>";
        }

        echo "<pre class=\"out\">$stdout</pre>";

        return proc_close($proc);
    }
?>
<html>
    <head>
        <title>NginxLogAnalyzer</title>
        <style>
            .out {

            }
            .errorOut {
                color: red;
            }
            input[name="args"] {
                width: 50%;
            }
        </style>
    </head>
    <body>
        <?php
            if(isset($_GET["login"])) { //Login
                if(isset($_GET["wrong"])) {
                   echo "Login invalid!<br>";
                }

                ?>
                    <form action="<?php echo $_SERVER["PHP_SELF"]."?login"; ?>" method="post">
                        Login: <input type="password" name="loginKey"/>
                        <input type="submit" value="Login">
                    </form>
                <?php
            } else if(isset($_GET["configure"])) { //Config
                ?>
                    <form action="<?php echo $_SERVER["PHP_SELF"]."?setconfig"; ?>" method="post">
                        Sources: <input type="text" name="sources" value="<?php echo $config["sources"]; ?>"/><br>
                        Login: <input type="password" name="loginKey" value="<?php echo $config["loginKey"]; ?>"/> (empty => no login)<br>
                        Default count: <input type="number" name="count" value="<?php echo $config["count"]; ?>"/><br>
                        Default entry count: <input type="number" name="entryCount" value="<?php echo $config["entryCount"]; ?>"/><br>
                        Pull repo before execution: <input type="checkbox" name="gitRefresh" <?php if($config["gitRefresh"] === true) { echo "checked"; } ?>/><br>
                        GitPull command: <input type="text" name="gitCommand" value="<?php echo $config["gitCommand"]; ?>"/><br>
                        dotnet command: <input type="text" name="dotnetCommand" value="<?php echo $config["dotnetCommand"]; ?>"/><br>
                        <input type="submit" value="Save"> <input type="button" value="Back" onclick="location.href='<?php echo $_SERVER["PHP_SELF"]; ?>'"/>
                    </form>
                <?php
            } else {
                ?>
                    <h1>Settings</h1>
                    <form action="<?php echo $_SERVER["PHP_SELF"]; ?>?configure" method="post">
                        <input type="submit" value="Edit"/>
                    </form>
                <?php

                if($config["gitRefresh"] === true) {
                    echo "<h1>git</h1>";
                    ExecuteApp($config["gitCommand"]);
                }

                echo "<h1>Analyzer</h1>";
                $args = isset($_POST["args"]) ? $_POST["args"] : "";
                if(strpos($args, "--count=") === false) {
                    $args = "--count=".$config["count"].$args;
                }

                ?>
                    <form action="<?php echo $_SERVER["PHP_SELF"]; ?>" method="post">
                        Arguments: <input type="text" name="args" value="<?php echo $args; ?>"/> <input type="submit" value="Reload">
                    </form>
                <?php

                if(getenv(""))

                ExecuteApp($config["dotnetCommand"]." ".$config["sources"]." ".$args);
            }
        ?>
    </body>
</html>