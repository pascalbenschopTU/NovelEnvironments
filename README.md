# NovelEnvironments

## Parameter Implementation
All parameters are first defined in `EnvironmentConfiguration.cs`, the `ConfigType` defines the level of each parameter.
The list of EnvironmentConfigurations for each environment in an experiment are stored in `ExperimentMetaData.cs`.
When the player teleports from the DefaultScene to the EnvironmentScene the `StartingPositionGenerator.cs` script is called.

### Player initialization
In `StartingPositionGenerator.cs` the settings for the minimap, view distance and tasks are applied.

### World generation
Based on the chosen environment from the current EnvironmentConfiguration, the `EnvironmentGenerator.cs` script is called from `StartingPositionGenerator.cs` for the corresponding environment. The **Complexity** setting influences the generation of assets in the environment that are not landmarks. If <Insert Correct Config Name> is set to `ConfigType.High` then the list of complex assets is used to generate.


## File storage

All data is stored in folders that are in the folder defined by 'Application.dataPath'.

### Logging
The logging data is stored in the folder 'ExperimentLogs_{participant_id}'.

The movement data of the player is stored in the file 'movement.csv'
- The format is `{environment_id};{position.x};{position.y};{position.z};{rotation.w};{rotation.x};{rotation.y};{rotation.z}`
- This data is logged X times per second.

The task data is stored in the file 'tasks.csv'
- The format is `{environment_id};{position.x};{position.y};{position.z};{rotation.w};{rotation.x};{rotation.y};{rotation.z};{task}`
- When a picture is taken the location of the player is stored.
- When a Gatherable is gathered the location of the Gatherable is stored.
- When looking at a landmark the landmark position is stored.

### Picture storage
The pictures are stored in the folder 'Pictures'
- All pictures are stored as .png
