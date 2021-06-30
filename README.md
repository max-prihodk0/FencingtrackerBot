# FencingtrackerBot
My first attempt at making a discord bot with the [Discord.Net API](https://github.com/discord-net/Discord.Net) for my current project, [fencingtracker.com](http://fencingtracker.com/).
This repository will be kept up to date and I will add some new features here and there.

### Commands
| Command | Usage                          | Description                                                                                                                                                                                                                           |
|---------|--------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| help    | *!help (optional command)*     | Returns all the commands available and their parameters.                                                                                                                                                                              |
| kick    | *!kick [user] [reason]*        | Kicks the specified user from the server with the given reason.                                                                                                                                                                       |
| mute    | *!mute [user] [time] [reason]* | Mutes the specified user so that they can not type or talk for an amount of time. The *time* parameter accepts values like **12m**, **3d**, **4h**, and **2w** *(the character representing the first letter of the time measurement)*. |
| purge   | *!purge [channel] [amount]*    | Deletes an amount of messages from the specified channel.                                                                                                                                                                             |
| verify  | *!verify*                      | Launches the [verification system](https://github.com/max-prihodk0/FencingtrackerBot/blob/main/README.md#verification-system) for the sender *(keep in mind this command can only be called in the verification channel and only by unverified members)*.                                                                          |

<br>

## Verification System
The fencingtracker bot has a simple but effective verification system that consists of three simple steps:

1. First, the user notifies the bot that they would like to get verified by joining the server. The bot will send the user a message containing a captcha.
2. Next, the user must complete the captia and send the correct code to the bot via a **direct message**.
3. Finally, if the user completed the captcha correctly, they will be granted access to the server.

<br>

## Bot Configuration
To configure the bot, you must first create a new discord application. Then (after building it) create a new JSON file, name it *config*, and place it in the same directory as the executable. Next paste this into the *config.json* file:

```json
{
    "discord": {
      "token": "",
      "id": "",
      "secret": "",
      "prefix":  "!",
      "server": "",
      "roles": {
          "verified": "",
          "muted": ""
      },
      "channels": {
          "welcome": "",
          "bot-commands": ""
      } 
    },
    "sql": {
      "server": "",
      "database": "",
      "user": "",
      "password": "",
      "port": ""
    }
  }
  ```
  
  Replace all the JSON fields (including the server, role, and channel ids) with your server and bot's properties. If you were wondering, yes, you do need to set up a MySQL server if you want this app to run as intended, you can read about the bot's database structure here. You can obviously change the prefix if you want.  
  
  Now in the same directory as the executable create a new folder and call it *logs*. This is where the bot will write it's log files, so that if it crashes unexpectedly you can review the logs and see what exactly triggered the crash.

<br>

## Database structure
The bot has a simple database strucure which consists of 2 tables, the ***Verification*** table and the ***Members*** table. The ***Verification*** table has 3 columns: *Id*, *Captcha*, and *Tries*. The ***Members*** table has four columns: *UserId*, *Warnings*, *MessagesSent*, and *VerificationId* (Which is a foreign key). The [Models.cs](https://github.com/max-prihodk0/FencingtrackerBot/blob/main/VerificationBot/References/SQL/Models.cs) file depicts it perfectly.

These are the SQL queries that you would have to run to replicate the database:

**Verfication Table**
```sql
CREATE TABLE Verification (
    Id INT PRIMARY KEY NOT NULL,
    Captcha VARCHAR(128) NOT NULL,
    Tries INT
);
```

**Members Table**
```sql
CREATE TABLE Members (
    UserId BIGINT UNSIGNED PRIMARY KEY NOT NULL,
    `Warnings` INT NOT NULL,
    MessagesSent INT NOT NULL,
    VerificationId INT,
    FOREIGN KEY (VerificationId) REFERENCES Verification(Id)
);
```

<br>

## Task List
Below is a simple list of features that I am hoping to complete in the near future.

- [x] Purge command that allows you to delete x amount of messages from a channel.
- [ ] Ban command: ***ban [user] (reason)***.
- [ ] Make all **reason** parameters for commands optional.
- [ ] Add a music player module similar to rythm.
- [ ] Add chat filter for offensive messages/words.

<br>

## Closing Remarks
I hope this repository helped you understand how the [Discord.Net API](https://github.com/discord-net/Discord.Net) works and maybe even taught you something new about C#!
If your interested in this project you can join our discord [here](https://discord.com/invite/NWFkyArGcp).
