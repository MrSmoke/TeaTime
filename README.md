# TeaTime ☕

![AppVeyor](https://img.shields.io/appveyor/ci/mrsmoke/teatime.svg)
![AppVeyor tests](https://img.shields.io/appveyor/tests/mrsmoke/teatime.svg)
[![Docker Pulls](https://img.shields.io/docker/pulls/dockdockcontainer/teatime.svg)](https://hub.docker.com/r/dockdockcontainer/teatime)
![GitHub](https://img.shields.io/github/license/mrsmoke/teatime.svg)

TeaTime is the Russian Roulette of tea making *(although not strictly limited to tea).*

## Overview

TeaTime allows your teams to start a round of tea, join with their choice of tea, and (on round end) randomly select someone to make the round of tea.

## Slack

### Commands

| Description | Command |
| ---- | ------- |
| Start a new round | `/teatime {group}` |
| Join a round | `/teatime join {option}` |
| End a round | `/teatime end` |
| Volunteer to make the round | `/teatime illmake` |

#### Groups

| Description | Command |
| ---- | ------- |
| Add a group | `/teatime groups add {name}` |
| Remove a group | `/teatime groups remove {name}` |

#### Options

| Description | Command |
| ---- | ------- |
| Add a new option for a group | `/teatime options add {group} {name}` |
| Remove an option from a group | `/teatime options remove {group} {name}` |

### Example Usage

1. Create a group called tea

    ```
    /teatime groups add tea
    ```

2. Add some options to your group

    ```
    /teatime options add tea "Earl Grey"
    /teatime options add tea "English Breakfast"
    ```

3. Start a new round of tea

    ```
    /teatime tea
    ```

    ![image](https://user-images.githubusercontent.com/709976/56972165-d810ba80-6bad-11e9-9b63-fdc50abeb068.png)

4. Wait for everyone to join...

    > (for those who link commands)
    ```
    /teatime join "Earl Grey"
    ```

    ![image](https://user-images.githubusercontent.com/709976/56972122-c4655400-6bad-11e9-8478-e1f15d4e9403.png)

5. _(optional)_ Be a good sport and volunteer to make the round

    ```
    /teatime illmake
    ```

    ![image](https://user-images.githubusercontent.com/709976/56972594-9cc2bb80-6bae-11e9-916b-2c6c6e40bae4.png)

6. End the round
    ```
    /teatime end
    ```

    ![image](https://user-images.githubusercontent.com/709976/56973263-d9db7d80-6baf-11e9-8b3c-e0ff1fe3c2f3.png)

7. Congratulations?

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
