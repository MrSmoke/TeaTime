# TeaTime ☕

[![Docker Pulls](https://img.shields.io/docker/pulls/dockdockcontainer/teatime.svg)](https://hub.docker.com/r/dockdockcontainer/teatime)
![GitHub](https://img.shields.io/github/license/mrsmoke/teatime.svg)

TeaTime is the Russian Roulette of tea making *(although not strictly limited to tea).*

## Overview

TeaTime allows your teams to start a round of tea, join with their choice of tea, and (on round end) randomly select someone to make the round of tea.

## Slack

### Commands

| Description                       | Command                  |
|-----------------------------------|--------------------------|
| Show a list of available commands | `/teatime help`          |
| Start a new round of tea          | `/teatime start`         |
| Start a new round for a group     | `/teatime start {group}` |
| Join a round                      | `/teatime join {option}` |
| Leave the current round           | `/teatime leave`         |
| Volunteer to make the round       | `/teatime illmake`       |
| End a round                       | `/teatime end`           |

#### Groups

| Description    | Command                         |
|----------------|---------------------------------|
| Add a group    | `/teatime groups add {name}`    |
| Remove a group | `/teatime groups remove {name}` |

#### Options

| Description                     | Command                                       |
|---------------------------------|-----------------------------------------------|
| Add a new option for a group    | `/teatime options add {group} {name}`         |
| List a group's options          | `/teatime options list {group}`               |
| Remove an option from a group   | `/teatime options remove {group} {name}`      |
| Mark an option as available     | `/teatime options available {group} {name}`   |
| Mark an option as unavailable   | `/teatime options unavailable {group} {name}` |

### Example Usage

Each channel starts with a `tea` group containing *Earl Grey* and *English Breakfast*, so you can start a round straight away.

1. Start a new round of tea

    ```
    /teatime start
    ```

    ![image](https://user-images.githubusercontent.com/709976/56972165-d810ba80-6bad-11e9-9b63-fdc50abeb068.png)

2. Wait for everyone to join...

    Click an option on the round message to join, or click a different option to change your order.

    > (for those who like commands)

    ```
    /teatime join "Earl Grey"
    ```

    ![image](https://user-images.githubusercontent.com/709976/56972122-c4655400-6bad-11e9-8478-e1f15d4e9403.png)

3. _(optional)_ Be a good sport and volunteer to make the round

    ```
    /teatime illmake
    ```

    ![image](https://user-images.githubusercontent.com/709976/56972594-9cc2bb80-6bae-11e9-916b-2c6c6e40bae4.png)

4. End the round

    ```
    /teatime end
    ```

    ![image](https://user-images.githubusercontent.com/709976/56973263-d9db7d80-6baf-11e9-8b3c-e0ff1fe3c2f3.png)

5. Congratulations?

### Customising groups

Add your own groups and options, then start a round with the group name.

```
/teatime groups add coffee
/teatime options add coffee "Flat White"
/teatime options add coffee "Long Black"
/teatime start coffee
```

## Running

### Docker

The TeaTime docker image can be found on [Docker Hub](https://hub.docker.com/r/dockdockcontainer/teatime).

An example `docker-compose.yml`

```yml
services:
  web:
    image: dockdockcontainer/teatime:latest
    restart: always
    ports:
     - "80:8080"
    environment:
      SELFHOST: https://demo.example
      MYSQL__HOST: localhost
      MYSQL__USERNAME: username
      MYSQL__PASSWORD: password
      SLACK__OAUTH__ENABLED: true
      SLACK__OAUTH__CLIENTID: demoid
      SLACK__OAUTH__CLIENTSECRET: demosecret
      SLACK__REQUESTVERIFICATION__ENABLED: true
      SLACK__REQUESTVERIFICATION__SIGNINGSECRET: secret
```

#### Supported tags

| Tag                                    | Published                                   |
|----------------------------------------|---------------------------------------------|
| `latest`                               | Each release, excluding pre-releases        |
| `{major}.{minor}.{patch}` (eg `1.2.2`) | Each release                                |
| `{major}.{minor}` (eg `1.2`)           | Each release, tracking the latest patch     |
| `dev`                                  | Each commit to `dev`                        |
| `sha-{short}`                          | Each commit to `dev`, pinned to that commit |

#### Environment Variables

| Variable                                  | Required                    | Description                                                                                                                                               |
|-------------------------------------------|-----------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------|
| SELFHOST                                  | false                       | The host of the TeaTime application including the scheme, host and port. If not set, this will be generated from the request. Eg `https://runteatime.com` |
| MYSQL__HOST                               | false (default `localhost`) | The mysql server host.                                                                                                                                    |
| MYSQL__PORT                               | false (default `3306`)      | The mysql server port.                                                                                                                                    |
| MYSQL__USERNAME                           | false (default `root`)      | The mysql users username.                                                                                                                                 |
| MYSQL__PASSWORD                           | false (default `password`)  | The mysql users password.                                                                                                                                 |
| MYSQL__DATABASE                           | false (default `teatime`)   | The mysql database.                                                                                                                                       |
| SLACK__OAUTH__ENABLED                     | false (default `false`)     | Set to `true` to enable the "Add to Slack" button.                                                                                                        |
| SLACK__OAUTH__CLIENTID                    | **true** *if enabled*       | The "Client ID" for the Slack app.                                                                                                                        |
| SLACK__OAUTH__CLIENTSECRET                | **true** *if enabled*       | The "Client Secret" for the Slack app.                                                                                                                    |
| SLACK__REQUESTVERIFICATION__ENABLED       | false (default `false`)     | Set to `true` to enable Slack request verification                                                                                                        |
| SLACK__REQUESTVERIFICATION__SIGNINGSECRET | **true** *if enabled*       | The "Signing Secret" for the Slack app.                                                                                                                   |

### Slack app

Create a Slack app and point it at your TeaTime host (`SELFHOST` above).

| Slack app setting                  | Value                                                  |
|------------------------------------|--------------------------------------------------------|
| Slash Commands                     | `/teatime` with Request URL `{SELFHOST}/slack/slash`   |
| Interactivity & Shortcuts          | Request URL `{SELFHOST}/slack/interactions`            |
| OAuth & Permissions, Redirect URLs | `{SELFHOST}/slack/callback` (only if OAuth is enabled) |
| OAuth & Permissions, Bot scopes    | `commands`                                             |

Interactivity must be enabled for the join buttons to work.
Copy the app's Client ID, Client Secret and Signing Secret into the matching `SLACK__` variables.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
