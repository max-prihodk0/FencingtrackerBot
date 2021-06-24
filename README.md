# FencingtrackerBot
My first attempt at making a discord bot with the [Discord.Net API](https://github.com/discord-net/Discord.Net) for my current project, [fencingtracker.com](http://fencingtracker.com/).
This repository will be kept up to date and I will add some new features here and there.

### Commands
| Command | Usage                          | Description                                                                                                                                                                                                                           |
|---------|--------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| help    | *!help (optional command)*     | Returns all the commands available and their parameters.                                                                                                                                                                              |
| kick    | *!kick [user] [reason]*        | Kicks the specified user from the server with the given reason.                                                                                                                                                                       |
| mute    | *!mute [user] [time] [reason]* | Mutes the specified user so that they can not type or talk for an amount of time. The *time* parameter accepts values like **12m**, **3d**, **4h**, and **2w** (the character representing the first letter of the time measurement). |
| purge   | *!purge [channel] [amount]*    | Deletes an amount of messages from the specified channel.                                                                                                                                                                             |
| verify  | *!verify*                      | Launches the [verification system](https://github.com/max-prihodk0/FencingtrackerBot/blob/main/README.md#verification-system) for the sender *(keep in mind this command can only be called in the verification channel and only by unverified members)*.                                                                          |

<br>

## Verification System
The fencingtracker bot has a simple but effective verification system that consists of three simple steps:

1. First, the user notifies the bot that they would like to get verified by typing ***!verify*** in the official verification channel. The bot will send the user a message containing a captcha.
2. Next, the user must complete the captia and send the correct code to the bot via a **direct message**.
3. Finally, if the user completed the captcha correctly, they will be granted access to the server.

<br>

## Bot Configuration
