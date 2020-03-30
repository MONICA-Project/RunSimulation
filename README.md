# Monica RunSimulation
<!-- Short description of the project. -->

RunSimulation
The RunSimulation is intended for replaying collected OGCSensorthings API data.


The RunSimulation is based on the following technologies:
*	MQTT interface for sending messages
*	SQLite databases used for reading observations from


The COP professional API provides the following main functionalities:
*	Incident classification and management
*	Division of a geographical area into zones and subzones
*	Mapping of incidents, sensors, facilities and people/groups/crowds to zones
*	Fast retrieval of current status of the situational objects of interest



<!-- A teaser figure may be added here. It is best to keep the figure small (<500KB) and in the same repo -->

## Getting Started
The RunSimulation is implemented in ASP.NET Core 2.1.

The easiest way to build it is to clone the repository using Visual Studio 2017 or higher and then build the software.

Ready made docker images are available here [here](https://hub.docker.com/repository/docker/monicaproject/)

## Deployment
For deployment the COP.API relies on a Postgres database for internal use as well as a connection to a GOST database.

### Docker
To run the latest version of foobar:
```bash
docker run -p 8080:80 foobar
```

## Development
<!-- Developer instructions. -->

### Prerequisite
This projects depends on xyz. Installation instructions are available [here](https://xyz.com)

On Debian:
```bash
apt install xyz
```

### Test
Use tests.sh to run unit tests:
```bash
sh tests.sh
```

### Build

```bash
g++ -o app app.cpp
```

## Contributing
Contributions are welcome. 

Please fork, make your changes, and submit a pull request. For major changes, please open an issue first and discuss it with the other authors.

## Affiliation
![MONICA](https://github.com/MONICA-Project/template/raw/master/monica.png)  
This work is supported by the European Commission through the [MONICA H2020 PROJECT](https://www.monica-project.eu) under grant agreement No 732350.

