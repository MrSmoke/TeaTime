# TeaTime 🍵

![AppVeyor](https://img.shields.io/appveyor/ci/mrsmoke/teatime.svg)
![AppVeyor tests](https://img.shields.io/appveyor/tests/mrsmoke/teatime.svg)
[![Docker Pulls](https://img.shields.io/docker/pulls/dockdockcontainer/teatime.svg)](https://hub.docker.com/r/dockdockcontainer/teatime)
![GitHub](https://img.shields.io/github/license/mrsmoke/teatime.svg)

TeaTime is the Russian Roulette of tea making *(although not strictly limited to tea).*


## Overview

TeaTime allows your teams to 


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

4. Wait for everyone to join...

> (for those who link commands)
```
/teatime join "Earl Grey"
```

5. _(optional)_ Be a good sport and volunteer to make the round
```
/teatime illmake
```

6. End the round
```
/teatime end
```

7. Congratulations?


## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
