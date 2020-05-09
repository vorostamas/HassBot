# Home Assistant Discord Bot

[Home Assistant](https://www.home-assistant.io) uses [Discord](https://www.discordapp.com) for community support and real-time chat with other IoT ad smart home enthusiasts.
This discord channel is one of the largest growing in the Discord community with over 5000+ active users at any given point of time, and hence having a bot makes it easier to moderate and also offers self-help using simple commands.

This is **@HassBot** written by **[skalavala](https://github.com/skalavala)** for Home Assistant's Discord Channel (link above). Currently the bot is running in two different independent channels
1. https://discord.gg/c5DvZ4e (English Home-Assistant Discord Channel)
2. https://discord.gg/ggU7UBz (Swedish Smart Home Discord Channel)

* The @HassBot is highly configurable, frequently summoned by the moderators. Only members of the moderator group has access to edit the bot commands. 
* Anyone can invoke the bot to get the responses. 

The command prefixes are `~` and `.`. That means, the bot commands can be executed either by `~` or `.`. 
The following are the list of commands that it supports + custom/default command - which is anything!

```
~about          - Shows information about this bot.
~help           - Displays this message. Usage: ~help
~8ball          - Predicts an answer to a given question. Usage: ~8ball <question> <@optional user1> <@optional user2>...etc
~list           - Shows existing custom command list.
~command        - Create custom commands using: ~command <command name> <command description>
~command        - Run Custom Command. Usage: ~skalavala <@optional user1> <@optional user2>...etc
~lookup         - Provides links to the documentation from sitemap. Usage: ~lookup <search> <@optional user1> <@optional user2>...etc
~deepsearch     - Searches hard, sends you a direct message. Use with caution!
~format         - Shows how to format code. Usage: ~format <@optional user1> <@optional user2>...etc
~share          - Shows how to share code that is more than 10 -15 lines. Usage: ~share <@optional user1> <@optional user2>...etc
~lmgtfy         - Googles content for you. Usage: ~lmgtfy <@optional user1> <@optional user2> <search String>
~ping           - Reply with pong. Use this to check if the bot is alive or not. Usage: ~ping
~pong           - Reply with ping. Use this to check if the bot is alive or not. Usage: ~pong
~update         - Refreshes and updates the lookup/sitemap data. Usage: ~update
~yaml?          - Validates the given YAML code. Usage: ~yaml <yaml code> <@optional user1> <@optional user2>...etc
~welcome        - Shows welcome information. Usage: ~welcome <@optional user1> <@optional user2>...etc
~json2yaml      - Converts JSON code to YAML. Usage `json2yaml <json code>`
~yaml2json      - Converts YAML code to JSON. Usage: `~yaml2json <yaml code>`
~c2f            - Converts a given Celsius value to Fahrenheit
~f2c            - Converts a given Fahrenheit value to Celsius
~hex2dec        - Converts a given hex value to decimal
~dec2hex        - Converts a given decimal value to hex
~bin2dec        - Converts a given binary value to decimal
~dec2bin        - Converts a given decimal value to binary
~base64_encode  - Encodes a given string to base64 format
~base64_decode  - Decodes a given base64 encoded string

Tip: If you put the yaml/json code in the correct format [```yaml <code> ```], or [```json <code> ```], Hassbot will automatically validate the code, and responds using emojis :thumbsup:
```

## Default Command is "Lookup"
Apart from the commands listed above, one can also simply search by providing search string as the command. For ex: Even if there is no "pre-defined" command, called "`xyz`", you can call still run the command as `~xyz`. This will check for string `xyz` in the sitemap and if there are any matching entries, it will give you the links to those as a response. If not, an emoji reaction will be added to the original request indicating that there are no entries found with the search string `xyz`. In case if the command already exists, that takes precedence and automatically executes that command. 

## Mentioning Users
Almost all commands allow you to mention users. For ex: If you would like to refer to the output of a command to a user, you can simply pass user name as parameter.

```
~lookup docs @Tinkerer @Ludeeus
```

The above command looks up `docs` in the sitemap, and mentions that to both @Tinkerer and @Ludeeus in the response. You can also say,

```
~docs everyone should read.... especially @Tinkerer
```
This shares the docs url and mentions @Tinkerer


## The features include but not limited to:

### Welcoming new users
Every time a new user joins the channel, it sends a personal/direct message explaining the rules of the channel, and also some useful links. The message can be customized using `welcome.txt` in this repo.

### Code limit warnings
There is a limit of 10 lines of code when posting to prevent code walls. The Bot automatically moves the message to hastebin or other file sharing sites and provides a simple link in the channel to minimize the code and log walls.

### Automatic YAML code verification
People who come to the Home Assistant Discord channel tend to post their configuration and automation seeking for help. There is an automatic YAML verification in place, where everytime someone posts code, it automatically verifies the code, and responses in the form of emojis whether the code passed the test or it failed the test. Sort of like `yamllint`, except it is realtime.

For the automatic code verification to work, the code must use `~share` format. The share format is:

```
```yaml
code here```
```

### Lookup / Deepsearch in the sitemap document
When the lookup command is issued with a parameter, it searches in the Home Assistant's sitemap url (https://home-assistant.io/sitemap.xml) and points to the right articles and links.

### 8Ball Predictions
A fun command that randomly gives predictions to the questions. The answers are rarely and barely accurate.

### Ping - health check
Used to check the pulse of the Bot. When the `~ping` command is issued, the bot responses `Pong!`. Folks can also play ping-pong with each other, but do it in #botspam please!

### Welcome Command
When the command `~welcome` is issued, it reminds the user to follow welcome rules.

...more.

A big shout out to [@Tinkerer](https://github.com/DubhAd/Home-AssistantConfig/) and [@Ludeeus](https://github.com/ludeeus) for the requirements and testing :smile:

# Development Environment:

## Technology Stack used:
<table>
<tr><td>Programming Language</td><td>C#</td></tr>
<tr><td>Runtime Environment</td><td>.Net Core 2.1</td></tr>
<tr><td>Discord API</td><td>Search Google Discord.Net</td></tr>
<tr><td>Supported IDEs</td><td>VSCODE for *nix systems, or Visual Studio 2019 for Windows</td></tr>
</table>

The following packages are required and should be installed using Nuget or package manager console. 

This is the output of the command `Get-Package | select -Unique Id, Versions` in the Package Manager Console.

```
Id                                        Versions
--                                        --------
Microsoft.NETCore.App                     {2.1.0} 
Discord.Net.WebSocket                     {2.2.0} 
Microsoft.Extensions.DependencyInjection  {3.1.3} 
Discord.Net.Core                          {2.2.0} 
Discord.Net.Commands                      {2.2.0} 
Newtonsoft.Json                           {12.0.3}
System.Configuration.ConfigurationManager {4.7.0} 
log4net                                   {2.0.8} 
YamlDotNet.NetCore                        {1.0.0} 
```

**Please note that Discord.net api currently only supports .net core 2.1.** Hence the application is built to run on .Net Core 2.1 version. Make sure your local environment has .net core 2.1 installed. To check the version, run the following command:
```
dotnet --info
```

If you do not see .net core 2.1 in the output, that means you need to install the runtime and sdk before you run the application. For installation of .net runtime and SDK, please visit microsoft's web site.


## Running in Windows Environment:
Open the Solution file in Windows, Build the solution and you can publish HassBotAppCore project to the desired output folder, and run the following command:

```
dotnet HassBotAppCore.dll
```

Wherever you deploy the application, make sure you have required config files in the folder. Otherwise the program will not load and work. The required files are:

```
log4net.config
HassBotAppCore.dll.config
```

Before you run the bot, make sure you have correct file paths specified in the config files.

## Running in Ubuntu Environment:

The code requires .net core 2.1 runtime and SDK on your machine. To install .net core on ubuntu, follow the instructions:

### Installing pre-requisites for the runtime and SDK
```
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.asc.gpg
sudo mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/
wget -q https://packages.microsoft.com/config/ubuntu/18.04/prod.list 
sudo mv prod.list /etc/apt/sources.list.d/microsoft-prod.list
sudo chown root:root /etc/apt/trusted.gpg.d/microsoft.asc.gpg
sudo chown root:root /etc/apt/sources.list.d/microsoft-prod.list
```

### Install .net core 2.1 runtime

```
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install aspnetcore-runtime-2.1
```

### Install .net core 2.1 SDK:

```
sudo apt-get install dotnet-sdk-2.1
```

After successfully installing the runtime and SDK, you can continue with the steps below to compile, and deploy HassBot. There is no need to restart the server, although it is a good practice to do so.

### Download/Clone latest HassBot code to your local environment.
```
cd ~
git clone https://github.com/skalavala/HassBot.git
cd HassBot
dotnet build HassBotApp.sln
```

All the application settings are in App.Config.example file. You need to rename that file to App.Config and you need to enter the Token and other important information for the bot to run. After completing that step, copy those files over to the output folder and it is now ready to run.

copy log4net file and App.Config file to the output folder.

```
cp ~/HassBot/HassBotAppCore/log4net.config ~/HassBot/HassBotAppCore/bin/Debug/netcoreapp2.1/
cp ~/HassBot/HassBotAppCore/App.config ~/HassBot/HassBotAppCore/bin/Debug/netcoreapp2.1/HassBotAppCore.dll.config
```

After copying those files successfully, you can run the application in two different ways.

1. Go into the folder and run the app:

```
cd ~/HassBot/HassBotAppCore/bin/Debug/netcoreapp2.1/
dotnet HassBotAppCore.dll
```

2. Run from anywhere:

```
dotnet ~/HassBot/HassBotAppCore/bin/Debug/netcoreapp2.1/HassBotAppCore.dll
```

## Running in Container Environment
You can also run in docker/container environment. For that, follow the steps in the link below:
[Tutorial: Containerize a .NET Core app](https://docs.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=linux)